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

		public void HandleImageAttachmentSaved(object sender, ImageAttachmentEventArgs evt)
		{
			System.Diagnostics.Debug.Assert(evt.Attachment.Kind == AttachmentType.image);
			if (evt.Meta != ImageMeta.Origin)
				return;

			try
			{
				Thumbnail thumbnail = MakeThumbnail(evt.Attachment, ImageMeta.Small);

				ThreadPool.QueueUserWorkItem(this.UpstreamThumbnail,
											new UpstreamArgs(thumbnail, evt.Attachment.ObjectId));
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
				Thumbnail thumbnail = MakeThumbnail(attachment, meta);
				UpstreamThumbnail(thumbnail, attachment.ObjectId);
			}
			catch (Exception e)
			{
				logger.Warn("Image attachment post processing unsuccess", e);
			}
		}

		private Thumbnail MakeThumbnail(Station.Attachment attachment, ImageMeta meta)
		{
			using (MemoryStream m = new MemoryStream(attachment.RawData))
			using (Bitmap origin = new Bitmap(m))
			{
				Bitmap thumbnail = null;

				if (meta == ImageMeta.Square)
					thumbnail = MakeSquareThumbnail(origin);
				else
					thumbnail = ImageHelper.Scale(origin, (int)meta);

				string thumbnailId = Guid.NewGuid().ToString();
				Thumbnail output = new Thumbnail(thumbnail, meta, thumbnailId);
				fileStorage.Save(thumbnailId + ".jpeg", output.ToArray());

				return output;
			}
		}

		private void UpstreamThumbnail(object state)
		{
			UpstreamArgs args = (UpstreamArgs)state;

			try
			{
				UpstreamThumbnail(args.Thumbnail, args.FullImageId);
			}
			catch (Exception e)
			{
				logger.Warn("Unable to upstream " + args.Thumbnail.Meta + 
												" thumbnail of orig image " + args.FullImageId, e);
			}
		}

		private void UpstreamThumbnail(Thumbnail thumbnail, string fullImgId)
		{
			using (MemoryStream output = new MemoryStream())
			{
				thumbnail.Image.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);
				Cloud.Attachment.UploadImage(output.ToArray(), fullImgId, thumbnail.Id +
														".jpeg", "image/jpeg", thumbnail.Meta);

				logger.DebugFormat("Thumbnail {0}.jpeg is uploaded to Cloud", thumbnail.Id);
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

	class UpstreamArgs
	{
		public Thumbnail Thumbnail { get; private set; }
		public string FullImageId { get; private set; }

		public UpstreamArgs(Thumbnail tb, string fullImgId)
		{
			Thumbnail = tb;
			FullImageId = fullImgId;
		}
	}

	class Thumbnail
	{
		public Bitmap Image { get; set; }
		public string Id { get; set; }
		public ImageMeta Meta { get; set; }

		public Thumbnail(Bitmap image, ImageMeta meta, string id)
		{
			Image = image;
			Meta = meta;
			Id = id;
		}

		public byte[] ToArray()
		{
			using (MemoryStream m = new MemoryStream())
			{
				Image.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
				return m.ToArray();
			}
		}
	}


}
