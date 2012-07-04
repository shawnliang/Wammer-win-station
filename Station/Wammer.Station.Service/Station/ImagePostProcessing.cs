using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Wammer.Model;
using Wammer.Utility;
using log4net;
using System.Diagnostics;

namespace Wammer.Station
{
	public class ImagePostProcessing
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof (ImagePostProcessing));

		public static ThumbnailInfo MakeThumbnail(Bitmap origin, ImageMeta meta, ExifOrientations orientation,
		                                          string attachmentId, Driver driver, string origFileName)
		{
#if DEBUG
			var sw = Stopwatch.StartNew();
#endif
			try
			{
				Image thumbnail = meta == ImageMeta.Square
						? MakeSquareThumbnail(origin)
						: ImageHelper.ScaleBasedOnLongSide(origin, (int)meta);

				try
				{
					ImageHelper.CorrectOrientation(
						orientation != ExifOrientations.Unknown ? orientation : ImageHelper.ImageOrientation(origin), thumbnail);
				}
				catch (ExternalException ex)
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
			finally
			{
#if DEBUG
				logger.DebugFormat("Make {0} {1} thumbnail take {2} ms", origFileName, meta.ToString(), sw.ElapsedMilliseconds.ToString());
#endif
			}
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

		private static Image MakeSquareThumbnail(Bitmap origin)
		{
			Image tmpImage = ImageHelper.ScaleBasedOnShortSide(origin, 128);

			if (tmpImage.Width == tmpImage.Height)
				return tmpImage;

			int height = (tmpImage.Height <= tmpImage.Width)? 0 : (int)(tmpImage.Height * 0.08);
			int shortSize =  ImageHelper.ShortSizeLength(tmpImage);
			tmpImage = ImageHelper.Crop(tmpImage, 0, height, shortSize);
			return tmpImage;
		}
	}

	internal class SavedResult
	{
		public string FileName { get; set; }
		public string MimeType { get; set; }
		public byte[] SavedRawData { get; set; }
	}

	internal interface ImageSaveStrategy
	{
		SavedResult Save(Image img, string attchId, ImageMeta meta, Driver driver);
	}

	internal class JpegImageSaveStrategy : ImageSaveStrategy
	{
		#region ImageSaveStrategy Members

		public SavedResult Save(Image img, string attchId, ImageMeta meta, Driver driver)
		{
			using (var m = new MemoryStream())
			{
				var encodeParams = new EncoderParameters(1);
				encodeParams.Param[0] = new EncoderParameter(Encoder.Quality, (long) 85);
				img.Save(m, ImageHelper.JpegCodec, encodeParams);

				var savedResult = new SavedResult
				                  	{
				                  		SavedRawData = m.ToArray(),
				                  		FileName = string.Format("{0}_{1}.dat", attchId, meta.ToString().ToLower()),
				                  		MimeType = "image/jpeg"
				                  	};

				new FileStorage(driver).SaveFile(savedResult.FileName, new ArraySegment<byte>(savedResult.SavedRawData));

				return savedResult;
			}
		}

		#endregion
	}

	internal abstract class CommonImageSaveStrategy : ImageSaveStrategy
	{
		private readonly ImageFormat format;
		private readonly string mimeType;
		private readonly string suffix;

		protected CommonImageSaveStrategy(ImageFormat format, string suffix, string mimeType)
		{
			this.format = format;
			this.suffix = suffix;
			this.mimeType = mimeType;
		}

		#region ImageSaveStrategy Members

		public SavedResult Save(Image img, string attchId, ImageMeta meta, Driver driver)
		{
			using (var m = new MemoryStream())
			{
				img.Save(m, format);

				var savedResult = new SavedResult
				                  	{
				                  		SavedRawData = m.ToArray(),
				                  		FileName = string.Format("{0}_{1}.{2}", attchId, meta.ToString().ToLower(), suffix),
				                  		MimeType = mimeType
				                  	};

				new FileStorage(driver).SaveFile(savedResult.FileName, new ArraySegment<byte>(savedResult.SavedRawData));

				return savedResult;
			}
		}

		#endregion
	}

	internal class GifImageSaveStrategy : CommonImageSaveStrategy
	{
		public GifImageSaveStrategy()
			: base(ImageFormat.Gif, "dat", "image/gif")
		{
		}
	}

	internal class PngImageSaveStrategy : CommonImageSaveStrategy
	{
		public PngImageSaveStrategy()
			: base(ImageFormat.Png, "dat", "image/png")
		{
		}
	}

	public class ThumbnailUpstreamedEventArgs : EventArgs
	{
		public ThumbnailUpstreamedEventArgs(long bytes)
		{
			BytesUpstreamed = bytes;
		}

		public long BytesUpstreamed { get; private set; }
	}
}