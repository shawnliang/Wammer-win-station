using System;
using System.IO;
using Wammer.Station.Timeline;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	[Serializable]
	public class ResourceDownloadEventArgs
	{
		public ResourceDownloadEventArgs()
		{
			failureCount = 0;
		}

		public string user_id { get; set; }
		public AttachmentInfo attachment { get; set; }
		public ImageMeta imagemeta { get; set; }
		public string filepath { get; set; }
		public int failureCount { get; set; }

		public bool IsOriginalAttachment()
		{
			return imagemeta == ImageMeta.Origin || imagemeta == ImageMeta.None;
		}

		public bool IsWebThumb()
		{
			return attachment.type.Equals("webthumb");
		}
	}



	internal static class ResourceDownloadTaskFactory
	{
		public static ResourceDownloadTask createDownloadTask(Driver driver, ImageMeta meta, AttachmentInfo attachment)
		{
			string tmpFolder;

			if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				tmpFolder = new FileStorage(driver).basePath;
			else
			{
				tmpFolder = FileStorage.GetCachePath(driver.user_id);
				if (!Directory.Exists(tmpFolder))
					Directory.CreateDirectory(tmpFolder);
			}

			var evtargs = new ResourceDownloadEventArgs
			{
				user_id = driver.user_id,
				attachment = attachment,
				imagemeta = meta,
				filepath = Path.Combine(FileStorage.GetCachePath(driver.user_id), Guid.NewGuid().ToString())
			};

			TaskPriority pri;
			if (meta == ImageMeta.Medium || meta == ImageMeta.Small)
				pri = TaskPriority.High;
			else if (meta == ImageMeta.Large || meta == ImageMeta.Square)
				pri = TaskPriority.Medium;
			else
				pri = TaskPriority.Low;

			return new ResourceDownloadTask(evtargs, pri);
		}


		public static WebThumbDownloadTask createWebThumbDownloadTask(Driver user, string object_id, long webthumb_id)
		{
			return new WebThumbDownloadTask(user.user_id, object_id, webthumb_id);
		}
	}
}
