using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;

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
			if (evt.Attachment.Kind != AttachmentType.image)
				return;

			try
			{
				using (MemoryStream m = new MemoryStream(evt.Attachment.RawData))
				using (Bitmap origin = new Bitmap(m))
				using (MemoryStream output = new MemoryStream())
				{
					Bitmap thumbnail = ImageHelper.Scale(origin, (int)ImageMeta.Small);

					thumbnail.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);


					output.Position = 0;
					string thumbnailId = Guid.NewGuid().ToString();
					Cloud.Attachment.UploadImage(output.ToArray(), evt.Attachment.ObjectId,
											thumbnailId + ".jpeg", "image/jpeg", ImageMeta.Small);

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
	}
}
