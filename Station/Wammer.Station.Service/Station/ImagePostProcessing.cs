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
					FileName = string.Format("{0}_{1}.dat", attchId, meta.ToString().ToLower()),
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
			:base(ImageFormat.Gif, "dat", "image/gif")
		{}
	}

	class PngImageSaveStrategy : CommonImageSaveStrategy
	{
		public PngImageSaveStrategy()
			:base(ImageFormat.Png, "dat", "image/png")
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
