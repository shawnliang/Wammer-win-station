using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;

using Wammer.Utility;
using Wammer.Cloud;
using log4net;

namespace Wammer.Station
{
	public class ImagePostProcessing
	{
		private readonly FileStorage fileStorage;
		private static ILog logger = LogManager.GetLogger(typeof(ImagePostProcessing));

		public ImagePostProcessing(FileStorage fileStorage)
		{
			this.fileStorage = fileStorage;
		}

		public void HandleAttachmentSaved(object sender, AttachmentUploadEventArgs evt)
		{
			if (evt.Attachment.Kind == AttachmentType.image)
				MakeThumbnailAndUpstream(evt.Attachment, ImageMeta.Small);
		}

		public void HandleRequestCompleted(object sender, AttachmentUploadEventArgs evt)
		{
			if (evt.Attachment.Kind == AttachmentType.image)
			{
				ThreadPool.QueueUserWorkItem(this.MakeThumbnailAndUpstream,
					new ThumbnailArgs(evt.Attachment, ImageMeta.Medium));

				ThreadPool.QueueUserWorkItem(this.MakeThumbnailAndUpstream,
					new ThumbnailArgs(evt.Attachment, ImageMeta.Large));

				ThreadPool.QueueUserWorkItem(this.MakeThumbnailAndUpstream,
					new ThumbnailArgs(evt.Attachment, ImageMeta.Square));
			}
		}

		private void MakeThumbnailAndUpstream(Station.Attachment attachment, ImageMeta meta)
		{
			try
			{
				using (MemoryStream m = new MemoryStream(attachment.RawData))
				using (Bitmap origin = new Bitmap(m))
				using (MemoryStream output = new MemoryStream())
				{
					Bitmap thumbnail = null;

					if (meta == ImageMeta.Square)
						thumbnail = MakeSquareThumbnail(origin);
					else
						thumbnail = ImageHelper.Scale(origin, (int)meta);

					thumbnail.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);

					output.Position = 0;
					string thumbnailId = Guid.NewGuid().ToString();
					Cloud.Attachment.UploadImage(output.ToArray(), attachment.ObjectId,
											thumbnailId + ".jpeg", "image/jpeg", meta);

					fileStorage.Save(thumbnailId + ".jpeg", output.ToArray());
				}
			}
			catch (Exception e)
			{
				logger.Warn("Image attachment post processing unsuccess", e);
				System.Diagnostics.Debug.Fail("Image attachment post processing unsuccess: " +
																					e.ToString());
			}
		}

		private static Bitmap MakeSquareThumbnail(Bitmap origin)
		{
			Bitmap tmpImage;
			int longSide = ImageHelper.LongSizeLength(origin);
			int shortSide = ImageHelper.ShortSizeLength(origin);
			int newSize = longSide * 128 / shortSide;

			tmpImage = ImageHelper.Scale(origin, newSize);
			tmpImage = ImageHelper.Crop(tmpImage,
												ImageHelper.ShortSizeLength(tmpImage),
												ImageHelper.ShortSizeLength(tmpImage));
			return tmpImage;
		}

		private void MakeThumbnailAndUpstream(object state)
		{
			ThumbnailArgs args = (ThumbnailArgs)state;
			MakeThumbnailAndUpstream(args.Attachment, args.ImageMeta);
		}
	}

	class ThumbnailArgs
	{
		public ImageMeta ImageMeta { get; private set; }
		public Attachment Attachment { get; private set; }

		public ThumbnailArgs(Attachment attachment, ImageMeta meta)
		{
			this.ImageMeta = meta;
			this.Attachment = attachment;
		}
	}
}
