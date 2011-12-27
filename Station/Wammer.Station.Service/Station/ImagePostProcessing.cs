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

		public void HandleImageAttachmentSaved(object sender, ImageAttachmentEventArgs evt)
		{
			System.Diagnostics.Debug.Assert(evt.Attachment.type == AttachmentType.image);
			if (evt.Meta != ImageMeta.Origin)
				return;

			if (evt.Attachment.image_meta != null && evt.Attachment.image_meta.medium != null)
				return;

			try
			{
				using (Bitmap origImage = BuildBitmap(evt.Attachment.RawData))
				{
					ThumbnailInfo medium = MakeThumbnail(origImage, ImageMeta.Medium,
														evt.Attachment.object_id, evt.Driver,
														evt.Attachment.file_name);

					Attachment update = new Attachment
					{
						object_id = evt.Attachment.object_id,
						image_meta = new ImageProperty
						{
							width = origImage.Width,
							height = origImage.Height,
							medium = medium
						}
					};

					BsonDocument exist = AttachmentCollection.Instance.FindOneAs<BsonDocument>(
														Query.EQ("_id", evt.Attachment.object_id));
					exist.DeepMerge(update.ToBsonDocument());
					AttachmentCollection.Instance.Save(exist);

					ThreadPool.QueueUserWorkItem(this.UpstreamThumbnail,
						new UpstreamArgs
						{
							 FullImageId = evt.Attachment.object_id,
							 GroupId = evt.Attachment.group_id,
							 ImageMeta = ImageMeta.Medium,
							 Thumbnail = medium,
							 UserApiKey = evt.UserApiKey,
							 UserSessionToken = evt.UserSessionToken
						});
				}
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


			ThreadPool.QueueUserWorkItem(this.HandleImageAttachmentCompletedSync, evt);
		}

		public void HandleImageAttachmentCompletedSync(object args)
		{
			ImageAttachmentEventArgs evt = (ImageAttachmentEventArgs)args;

			if (evt.Attachment.type != AttachmentType.image || evt.Meta != ImageMeta.Origin)
				return;

			try
			{
				Bitmap origImage = evt.Attachment.Bitmap;
				if (origImage==null)
					origImage = BuildBitmap(evt.Attachment.RawData);
				
				using (origImage)
				{
					string origImgObjectId = evt.Attachment.object_id;
					ThumbnailInfo small = MakeThumbnail(origImage, ImageMeta.Small,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);
					ThumbnailInfo large = MakeThumbnail(origImage, ImageMeta.Large,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);
					ThumbnailInfo square = MakeThumbnail(origImage, ImageMeta.Square,
										origImgObjectId, evt.Driver, evt.Attachment.file_name);

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

					UpstreamThumbnail(small, evt.Attachment.group_id, evt.Attachment.object_id,
						ImageMeta.Small, evt.UserApiKey, evt.UserSessionToken);
					UpstreamThumbnail(large, evt.Attachment.group_id, evt.Attachment.object_id,
						ImageMeta.Large, evt.UserApiKey, evt.UserSessionToken);
					UpstreamThumbnail(square, evt.Attachment.group_id, evt.Attachment.object_id,
						ImageMeta.Square, evt.UserApiKey, evt.UserSessionToken);
				}
			}
			catch (Exception e)
			{
				logger.Warn("Image attachment post processing unsuccess", e);
			}
		}

		private static Bitmap BuildBitmap(byte[] imageData)
		{
			using (MemoryStream s = new MemoryStream(imageData))
			{
				return new Bitmap(s);
			}
		}

		public static ThumbnailInfo MakeThumbnail(Bitmap origin, ImageMeta meta, string attachmentId, 
			Driver driver, string origFileName)
		{
			Bitmap thumbnail = null;

			if (meta == ImageMeta.Square)
				thumbnail = MakeSquareThumbnail(origin);
			else
				thumbnail = ImageHelper.ScaleBasedOnLongSide(origin, (int)meta);

			ImageHelper.CorrectOrientation(ImageHelper.ImageOrientation(origin), thumbnail);

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

		private void UpstreamThumbnail(object state)
		{
			UpstreamArgs args = (UpstreamArgs)state;

			try
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
			}
			catch (Exception e)
			{
				logger.Warn("Unable to upstream " + args.Thumbnail.file_name +
												" thumbnail of orig image " + args.FullImageId, e);
			}
		}

		private void UpstreamThumbnail(ThumbnailInfo thumbnail, string groupId, string fullImgId, 
														ImageMeta meta, string apiKey, string token)
		{
			using (MemoryStream output = new MemoryStream())
			{
				Attachment.UploadImage(thumbnail.RawData, groupId, fullImgId, 
												thumbnail.file_name, "image/jpeg", meta, apiKey, token);

				logger.DebugFormat("Thumbnail {0} is uploaded to Cloud", thumbnail.file_name);
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

				new FileStorage(driver).SaveFile(savedResult.FileName, savedResult.SavedRawData);

				return savedResult;
			}
		}
	}

	class GifImageSaveStrategy : ImageSaveStrategy
	{
		public SavedResult Save(Bitmap img, string attchId, ImageMeta meta, Driver driver)
		{
			using (MemoryStream m = new MemoryStream())
			{
				img.Save(m, ImageFormat.Gif);

				SavedResult savedResult = new SavedResult
				{
					SavedRawData = m.ToArray(),
					FileName = string.Format("{0}_{1}.gif", attchId, meta.ToString().ToLower()),
					MimeType = "image/gif"
				};

				new FileStorage(driver).SaveFile(savedResult.FileName, savedResult.SavedRawData);

				return savedResult;
			}
		}
	}

	class PngImageSaveStrategy : ImageSaveStrategy
	{
		public SavedResult Save(Bitmap img, string attchId, ImageMeta meta, Driver driver)
		{
			using (MemoryStream m = new MemoryStream())
			{
				img.Save(m, ImageFormat.Png);

				SavedResult savedResult = new SavedResult
				{
					SavedRawData = m.ToArray(),
					FileName = string.Format("{0}_{1}.png", attchId, meta.ToString().ToLower()),
					MimeType = "image/png"
				};

				new FileStorage(driver).SaveFile(savedResult.FileName, savedResult.SavedRawData);

				return savedResult;
			}
		}
	}
}
