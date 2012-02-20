using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

using Wammer.Utility;
using Wammer.Cloud;
using Wammer.Model;
using log4net;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class ImagePostProcessing
	{
		private static ILog logger = LogManager.GetLogger(typeof(ImagePostProcessing));

		public event EventHandler<ThumbnailUpstreamedEventArgs> ThumbnailUpstreamed;

		public void HandleImageAttachmentSaved(object sender, ImageAttachmentEventArgs evt)
		{
			System.Diagnostics.Debug.Assert(evt.Attachment.type == AttachmentType.image);
			if (evt.Meta != ImageMeta.Origin)
				return;

			if (evt.Attachment.image_meta != null && evt.Attachment.image_meta.medium != null)
				return;

			try
			{
				ThumbnailInfo medium;
				Attachment update;

				using (Bitmap origImage = BuildBitmap(evt.Attachment.RawData))
				{
					// release raw data immediately
					evt.Attachment.RawData = new ArraySegment<byte>();
					medium = MakeThumbnail(origImage, ImageMeta.Medium, 
						evt.Attachment.Orientation, 
						evt.Attachment.object_id, 
						evt.Driver,
						evt.Attachment.file_name);

					update = new Attachment
					{
						object_id = evt.Attachment.object_id,
						image_meta = new ImageProperty
						{
							width = origImage.Width,
							height = origImage.Height,
							medium = medium
						}
					};
				}

				BsonDocument exist = AttachmentCollection.Instance.FindOneAs<BsonDocument>(
													Query.EQ("_id", evt.Attachment.object_id));
				exist.DeepMerge(update.ToBsonDocument());
				AttachmentCollection.Instance.Save(exist);

				TaskQueue.Enqueue(
					new UpstreamMediumThumbnailTask(
						new UpstreamArgs
						{
							FullImageId = evt.Attachment.object_id,
							GroupId = evt.Attachment.group_id,
							ImageMeta = ImageMeta.Medium,
							Thumbnail = medium,
							UserApiKey = evt.UserApiKey,
							UserSessionToken = evt.UserSessionToken
						},
						ThumbnailUpstreamed
					),
					TaskPriority.Medium
				);
				
			}
			catch (Exception e)
			{
				logger.Warn("Unabel to make thumbnail and upstream", e);
			}
		}

		public void HandleImageAttachmentCompleted(object sender, ImageAttachmentEventArgs evt)
		{
			if (evt.Attachment.type != AttachmentType.image || evt.Meta != ImageMeta.Origin)
				return;

			TaskQueue.Enqueue(new UpstreamThumbnailsTask(evt, this.ThumbnailUpstreamed), TaskPriority.Medium);
		}

		public static Bitmap BuildBitmap(ArraySegment<byte> imageData)
		{
			using (MemoryStream s = new MemoryStream(imageData.Array, imageData.Offset, imageData.Count))
			{
				return new Bitmap(s);
			}
		}

		public static ThumbnailInfo MakeThumbnail(Bitmap origin, ImageMeta meta, ExifOrientations orientation,
			string attachmentId, Driver driver, string origFileName)
		{
			Bitmap thumbnail = null;

			if (meta == ImageMeta.Square)
				thumbnail = MakeSquareThumbnail(origin);
			else
				thumbnail = ImageHelper.ScaleBasedOnLongSide(origin, (int)meta);

			try
			{
				if (orientation != ExifOrientations.Unknown)
					ImageHelper.CorrectOrientation(orientation, thumbnail);
				else
					ImageHelper.CorrectOrientation(ImageHelper.ImageOrientation(origin), thumbnail);
			}
			catch (System.Runtime.InteropServices.ExternalException ex)
			{
				logger.ErrorFormat("Unable to correct orientation of image {0}", origFileName);
				logger.Error("External exception", ex);
			}

			ImageSaveStrategy imageStrategy = GetImageSaveStrategy(Path.GetExtension(origFileName));
			SavedResult savedThumbnail = imageStrategy.Save(thumbnail, attachmentId, meta, driver);

			return new ThumbnailInfo
			{
				saved_file_name = savedThumbnail.FileName,
				file_name = origFileName,
				width = thumbnail.Width,
				height = thumbnail.Height,
				file_size = savedThumbnail.SavedRawData.Length,
				mime_type = savedThumbnail.MimeType,
				modify_time = DateTime.UtcNow,
				url = "/v2/attachments/view/?object_id=" + attachmentId +
													"&image_meta=" + meta.ToString().ToLower(),
				RawData = savedThumbnail.SavedRawData
			};

		}

		private static ImageSaveStrategy GetImageSaveStrategy(string fileExtension)
		{
			if (fileExtension == null)
				return new JpegImageSaveStrategy();

			switch (fileExtension.ToLower())
			{
				case ".png":
					return new PngImageSaveStrategy();
				case ".gif":
				case ".giff":
					return new GifImageSaveStrategy();
				default:
					return new JpegImageSaveStrategy();
			}
		}

		private static Bitmap MakeSquareThumbnail(Bitmap origin)
		{
			Bitmap tmpImage = ImageHelper.ScaleBasedOnShortSide(origin, 128);

			tmpImage = ImageHelper.Crop(tmpImage,
												ImageHelper.ShortSizeLength(tmpImage),
												ImageHelper.ShortSizeLength(tmpImage));
			return tmpImage;
		}

		protected void OnThumbnailUpstreamed(ThumbnailUpstreamedEventArgs evt)
		{
			EventHandler<ThumbnailUpstreamedEventArgs> handler = this.ThumbnailUpstreamed;

			if (handler != null)
			{
				handler(this, evt);
			}
		}
	}

	public abstract class UpstreamTask: ITask
	{
		private EventHandler<ThumbnailUpstreamedEventArgs> upstreamedHandler;
		protected static ILog logger = LogManager.GetLogger("MakeThumbnail");

		protected UpstreamTask(EventHandler<ThumbnailUpstreamedEventArgs> upstreamedHandler)
		{
			this.upstreamedHandler = upstreamedHandler;
		}

		protected void UpstreamThumbnail(ThumbnailInfo thumbnail, string groupId, string fullImgId,
														ImageMeta meta, string apiKey, string token)
		{
			Attachment.UploadImage(new ArraySegment<byte>(thumbnail.RawData), groupId, fullImgId,
				thumbnail.file_name, "image/jpeg", meta, apiKey, token);

			OnThumbnailUpstreamed(thumbnail);
			logger.DebugFormat("Thumbnail {0} is uploaded to Cloud", thumbnail.file_name);
		}

		protected void OnThumbnailUpstreamed(ThumbnailInfo thumbnail)
		{
			if (upstreamedHandler != null)
				upstreamedHandler(this, new ThumbnailUpstreamedEventArgs(thumbnail.RawData.Length));
		}

		public abstract void Execute();
	}


	public class UpstreamThumbnailsTask : UpstreamTask
	{
		private static long g_counter = 0;
		private ImageAttachmentEventArgs evt;

		public UpstreamThumbnailsTask(ImageAttachmentEventArgs evt, 
			EventHandler<ThumbnailUpstreamedEventArgs> upstreamedHandler)
			:base(upstreamedHandler)
		{
			this.evt = evt;
		}

		public override void Execute()
		{
			if (evt.Attachment.type != AttachmentType.image || evt.Meta != ImageMeta.Origin)
				return;

			try
			{
				ThumbnailInfo small;
				ThumbnailInfo large;
				ThumbnailInfo square;
				string origImgObjectId = evt.Attachment.object_id;

				using (Bitmap origImage = ImagePostProcessing.BuildBitmap(evt.Attachment.RawData))
				{
					// release raw data immediately
					evt.Attachment.RawData = new ArraySegment<byte>();

					small = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Small, evt.Attachment.Orientation,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);
					large = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Large, evt.Attachment.Orientation,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);
					square = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Square, evt.Attachment.Orientation,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);
				}

				Attachment update = new Attachment
				{
					object_id = evt.Attachment.object_id,
					image_meta = new ImageProperty
					{
						small = small,
						large = large,
						square = square
					}
				};

				BsonDocument doc = AttachmentCollection.Instance.FindOneAs<BsonDocument>(
																Query.EQ("_id", origImgObjectId));
				doc.DeepMerge(update.ToBsonDocument());
				AttachmentCollection.Instance.Save(doc);
				doc.Clear();
				doc = null;

				UpstreamThumbnail(small, evt.Attachment.group_id, evt.Attachment.object_id,
					ImageMeta.Small, evt.UserApiKey, evt.UserSessionToken);
				UpstreamThumbnail(large, evt.Attachment.group_id, evt.Attachment.object_id,
					ImageMeta.Large, evt.UserApiKey, evt.UserSessionToken);
				UpstreamThumbnail(square, evt.Attachment.group_id, evt.Attachment.object_id,
					ImageMeta.Square, evt.UserApiKey, evt.UserSessionToken);


				long newValue = Interlocked.Add(ref g_counter, 1L);
				if (newValue % 5 == 0)
					GC.Collect();

			}
			catch (Exception e)
			{
				logger.Warn("Image attachment post processing unsuccess", e);
			}
		}
	}

	class UpstreamMediumThumbnailTask : UpstreamTask
	{
		private UpstreamArgs args;

		public UpstreamMediumThumbnailTask(UpstreamArgs args, EventHandler<ThumbnailUpstreamedEventArgs> upstreamedHandler)
			:base(upstreamedHandler)
		{
			this.args = args;
		}

		public override void Execute()
		{
			try
			{
				UpstreamThumbnail(args.Thumbnail, args.GroupId, args.FullImageId, args.ImageMeta,
					args.UserApiKey, args.UserSessionToken);
			}
			catch (WebException e)
			{
				WammerCloudException ex = new WammerCloudException(
					"Unable to upstream " + args.Thumbnail.file_name +
					" thumbnail of orig image " + args.FullImageId, e);

				logger.Warn(ex.ToString());
			}
			catch (Exception e)
			{
				logger.Warn("Unable to upstream " + args.Thumbnail.file_name +
					" thumbnail of orig image " + args.FullImageId, e);
			}
		}
	}

	class UpstreamArgs
	{
		public ThumbnailInfo Thumbnail { get; set; }
		public string FullImageId { get; set; }
		public ImageMeta ImageMeta { get; set; }
		public string GroupId { get; set; }
		public string UserApiKey { get; set; }
		public string UserSessionToken { get; set; }

		public UpstreamArgs()
		{

		}
	}

	class SavedResult
	{
		public string FileName { get; set; }
		public string MimeType { get; set; }
		public byte[] SavedRawData { get; set; }
	}

	interface ImageSaveStrategy
	{
		SavedResult Save(Bitmap img, string attchId, ImageMeta meta, Driver driver);
	}

	class JpegImageSaveStrategy : ImageSaveStrategy
	{
		public SavedResult Save(Bitmap img, string attchId, ImageMeta meta, Driver driver)
		{
			using (MemoryStream m = new MemoryStream())
			{
				EncoderParameters encodeParams = new EncoderParameters(1);
				encodeParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)85);
				img.Save(m, ImageHelper.JpegCodec, encodeParams);	

				SavedResult savedResult = new SavedResult 
				{ 
					SavedRawData = m.ToArray(),
					FileName = string.Format("{0}_{1}.jpeg", attchId, meta.ToString().ToLower()),
					MimeType = "image/jpeg"
				};

				new FileStorage(driver).SaveFile(savedResult.FileName, new ArraySegment<byte>(savedResult.SavedRawData));

				return savedResult;
			}
		}
	}

	abstract class CommonImageSaveStrategy : ImageSaveStrategy
	{
		private ImageFormat format;
		private string suffix;
		private string mimeType;

		protected CommonImageSaveStrategy(ImageFormat format, string suffix, string mimeType)
		{
			this.format = format;
			this.suffix = suffix;
			this.mimeType = mimeType;
		}

		public SavedResult Save(Bitmap img, string attchId, ImageMeta meta, Driver driver)
		{
			using (MemoryStream m = new MemoryStream())
			{
				img.Save(m, format);

				SavedResult savedResult = new SavedResult
				{
					SavedRawData = m.ToArray(),
					FileName = string.Format("{0}_{1}.{2}", attchId, meta.ToString().ToLower(), suffix),
					MimeType = mimeType
				};

				new FileStorage(driver).SaveFile(savedResult.FileName, new ArraySegment<byte>(savedResult.SavedRawData));

				return savedResult;
			}
		}
	}

	class GifImageSaveStrategy : CommonImageSaveStrategy
	{
		public GifImageSaveStrategy()
			:base(ImageFormat.Gif, "gif", "image/gif")
		{}
	}

	class PngImageSaveStrategy : CommonImageSaveStrategy
	{
		public PngImageSaveStrategy()
			:base(ImageFormat.Png, "png", "image/png")
		{}
	}

	public class ThumbnailUpstreamedEventArgs : EventArgs
	{
		public long BytesUpstreamed { get; private set; }

		public ThumbnailUpstreamedEventArgs(long bytes)
		{
			this.BytesUpstreamed = bytes;
		}
	}
}
