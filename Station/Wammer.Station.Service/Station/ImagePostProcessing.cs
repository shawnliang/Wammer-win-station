using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using System.Threading;
using System.Diagnostics;

namespace Wammer.Station
{
	public class ImagePostProcessing
	{
		private static ILog logger = LogManager.GetLogger(typeof(ImagePostProcessing));
		private IUpstreamThumbnailTaskFactory upstreamTaskFactory;

		public ImagePostProcessing(IUpstreamThumbnailTaskFactory factory)
		{
			this.upstreamTaskFactory = factory;
		}

		public void HandleImageAttachmentSaved(object sender, ImageAttachmentEventArgs evt)
		{
			if (evt.Meta != ImageMeta.Origin)
				return;

			Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", evt.AttachmentId));

			// Don't regenerate a medium thumbnail if its modify time is later than orig attachment
			if (attachment == null || 
				(attachment.HasThumbnail(ImageMeta.Medium) && 
				 attachment.modify_time <= attachment.image_meta.medium.modify_time))
				return;

			try
			{
				ThumbnailInfo medium;
				Attachment update;

				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", evt.UserId));
				if (user == null)
					return;

				FileStorage storage = new FileStorage(user);
				
				using (FileStream f = storage.Load(attachment.saved_file_name))
				using (Bitmap origImage = new Bitmap(f))
				{
					medium = MakeThumbnail(origImage, ImageMeta.Medium, 
						ExifOrientations.Unknown,
						evt.AttachmentId,
						user,
						attachment.file_name);

					update = new Attachment
					{
						object_id = evt.AttachmentId,
						image_meta = new ImageProperty
						{
							width = origImage.Width,
							height = origImage.Height,
							medium = medium
						}
					};
				}

				AttachmentCollection.Instance.Update(Query.EQ("_id", evt.AttachmentId),
					Update.Set("image_meta.width", update.image_meta.width).
							Set("image_meta.height", update.image_meta.height).
							Set("image_meta.medium", medium.ToBsonDocument()) );


				TaskQueue.Enqueue(
					upstreamTaskFactory.CreateTask(evt, ImageMeta.Medium), TaskPriority.Medium, true);
				
			}
			catch (Exception e)
			{
				logger.Warn("Unabel to make thumbnail and upstream", e);
			}
		}

		public void HandleImageAttachmentCompleted(object sender, ImageAttachmentEventArgs evt)
		{
			if (evt.Meta != ImageMeta.Origin)
				return;

			TaskQueue.Enqueue(new MakeAllThumbnailsAndUpstreamTask(evt, upstreamTaskFactory), TaskPriority.Medium, true);

			Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", evt.UserId));
			if (!user.isPrimaryStation && evt.Meta == ImageMeta.Origin)
				TaskQueue.Enqueue(new UpstreamThumbnailTask(evt, ImageMeta.Origin), TaskPriority.Low, true);
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
	}

	public class MakeAllThumbnailsAndUpstreamTask : ITask
	{
		protected static ILog logger = LogManager.GetLogger("MakeThumbnail");
		private ImageAttachmentEventArgs evt;
		private IUpstreamThumbnailTaskFactory upstreamTaskfactory;

		public MakeAllThumbnailsAndUpstreamTask(ImageAttachmentEventArgs evt, IUpstreamThumbnailTaskFactory factory)
		{
			this.evt = evt;
			this.upstreamTaskfactory = factory;
		}

		public void Execute()
		{
			Debug.Assert(evt.Meta == ImageMeta.Origin);

			try
			{
				Attachment file = AttachmentCollection.Instance.FindOne(Query.EQ("_id", evt.AttachmentId));
				if (file == null)
					return;

				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", evt.UserId));
				if (user == null)
					return;

				ThumbnailInfo small;
				ThumbnailInfo large;
				ThumbnailInfo square;

				string origImgObjectId = evt.AttachmentId;
				FileStorage storage = new FileStorage(user);
				
				using (FileStream f = storage.Load(file.saved_file_name))
				using (Bitmap origImage = new Bitmap(f))
				{
					small = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Small, ExifOrientations.Unknown,
										origImgObjectId, user, file.file_name);
					large = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Large, ExifOrientations.Unknown,
										origImgObjectId, user, file.file_name);
					square = ImagePostProcessing.MakeThumbnail(origImage, ImageMeta.Square, ExifOrientations.Unknown,
										origImgObjectId, user, file.file_name);
				}

				AttachmentCollection.Instance.Update(
					Query.EQ("_id", evt.AttachmentId),
					Update.Set("image_meta.small", small.ToBsonDocument()).
							Set("image_meta.large", large.ToBsonDocument()).
							Set("image_meta.square", square.ToBsonDocument()));


				// Secondary station does not need to upload thumbnails because 
				// it uploads orig image to cloud for body sync and cloud will then 
				// generates thumbnails. 
				if (user.isPrimaryStation)
				{
					TaskQueue.Enqueue(upstreamTaskfactory.CreateTask(evt, ImageMeta.Small), TaskPriority.Medium, true);
					TaskQueue.Enqueue(upstreamTaskfactory.CreateTask(evt, ImageMeta.Large), TaskPriority.Medium, true);
					TaskQueue.Enqueue(upstreamTaskfactory.CreateTask(evt, ImageMeta.Square), TaskPriority.Medium, true);
				}
			}
			catch (Exception e)
			{
				logger.Warn("Image attachment post processing unsuccess", e);
			}
		}
	}

	
	public interface IUpstreamThumbnailTaskFactory
	{
		ITask CreateTask(AttachmentEventArgs args, ImageMeta meta);
	}

	class UpstreamThumbnailTaskFactory: IUpstreamThumbnailTaskFactory
	{
		public ITask CreateTask(AttachmentEventArgs args, ImageMeta meta)
		{
			return new UpstreamThumbnailTask(args, meta);
		}
	}


	class UpstreamThumbnailTask: ITask
	{
		protected static ILog logger = LogManager.GetLogger("MakeThumbnail");

		private AttachmentEventArgs args;
		private ImageMeta meta;

		public static event EventHandler<ThumbnailUpstreamedEventArgs> ThumbnailUpstreamed;


		public UpstreamThumbnailTask(AttachmentEventArgs args, ImageMeta meta)
		{
			this.args = args;
			this.meta = meta;
		}

		public void Execute()
		{
			try
			{
				Attachment file = AttachmentCollection.Instance.FindOne(Query.EQ("_id", args.AttachmentId));
				if (file == null)
					return;

				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", args.UserId));
				if (user == null)
					return;

				IImageAttachmentInfo info = file.GetImgInfo(meta);
				if (info == null)
					return;

				FileStorage storage = new FileStorage(user);

				using (FileStream f = storage.Load(info.saved_file_name))
				{
					Attachment.Upload(f, file.group_id, file.object_id,
						file.file_name, info.mime_type, meta, file.type, args.UserApiKey, args.UserSessionToken, 1024, (sender, e) =>
						{
							OnThumbnailUpstreamed(this, new ThumbnailUpstreamedEventArgs(long.Parse(e.UserState.ToString())));
						});			
				}
			}
			catch (WebException e)
			{
				WammerCloudException ex = new WammerCloudException(
					"Unable to upstream " + meta +
					" thumbnail of image " + args.AttachmentId, e);

				logger.Warn(ex.ToString());
			}
			catch (Exception e)
			{
				logger.Warn("Unable to upstream " + meta +
					" thumbnail of image " + args.AttachmentId, e);
			}
		}

		private static void OnThumbnailUpstreamed(object sender, ThumbnailUpstreamedEventArgs arg)
		{
			EventHandler<ThumbnailUpstreamedEventArgs> handler = ThumbnailUpstreamed;

			if (handler != null)
				handler(sender, arg);
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
					FileName = string.Format("{0}_{1}.jpg", attchId, meta.ToString().ToLower()),
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
