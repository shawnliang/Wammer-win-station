using System;
using System.Collections.Generic;
using System.Net;
using log4net;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;
using Wammer.Utility;

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
	}

	

	internal class ResourceDownloader
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof (ResourceDownloader));
		private readonly ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue;
		private readonly string stationId;

		public ResourceDownloader(ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue, string stationId)
		{
			this.bodySyncQueue = bodySyncQueue;
			this.stationId = stationId;
		}

		public void PostRetrieved(object sender, TimelineSyncEventArgs e)
		{
			DownloadMissedResource(e.Driver, e.Posts);
		}

		public void EnqueueDownstreamTask(AttachmentInfo attachment, Driver driver, ImageMeta meta)
		{
			var evtargs = new ResourceDownloadEventArgs
			              	{
			              		user_id = driver.user_id,
			              		attachment = attachment,
			              		imagemeta = meta,
			              		filepath = FileStorage.GetTempFile(driver)
			              	};

			EnqueueDownstreamTask(meta, evtargs);
		}

		private void EnqueueDownstreamTask(ImageMeta meta, ResourceDownloadEventArgs evtargs)
		{
			TaskPriority pri;
			if (meta == ImageMeta.Medium || meta == ImageMeta.Small)
				pri = TaskPriority.High;
			else if (meta == ImageMeta.Large || meta == ImageMeta.Square)
				pri = TaskPriority.Medium;
			else
				pri = TaskPriority.Low;

			bodySyncQueue.Enqueue(new ResourceDownloadTask(evtargs, pri), pri);
		}

		private void DownloadMissedResource(Driver driver, IEnumerable<PostInfo> posts)
		{
			foreach (PostInfo post in posts)
			{
				if (string.Compare(post.hidden, "true", true) == 0)
					return;

				foreach (AttachmentInfo attachment in post.attachments)
				{
					// driver might be removed before running download tasks
					if (driver == null)
						break;

					// original
					if (!string.IsNullOrEmpty(attachment.url) && driver.isPrimaryStation)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Origin);
					}

					if (attachment.image_meta == null)
						break;

					// small
					if (attachment.image_meta.small != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Small);
					}

					// medium
					if (attachment.image_meta.medium != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Medium);
					}

					// large
					if (attachment.image_meta.large != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
					}

					// square
					if (attachment.image_meta.square != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Square);
					}
				}
			}
		}

		public void ResumeUnfinishedDownstreamTasks()
		{
			MongoCursor<PostInfo> posts = PostCollection.Instance.Find(
				Query.And(
					Query.Exists("attachments", true),
					Query.EQ("hidden", "false")));

			foreach (PostInfo post in posts)
			{
				foreach (AttachmentInfo attachment in post.attachments)
				{
					Attachment savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachment.object_id));
					Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", attachment.creator_id));

					// driver might be removed before download tasks completed
					if (driver == null)
						break;

					// origin
					if (driver.isPrimaryStation &&
					    (savedDoc == null || savedDoc.saved_file_name == null))
					{
						if (CloudHasOriginAttachment(attachment.object_id, driver))
						{
							//logger.DebugFormat("Attachement {0} found in cloud... Go download it", attachment.object_id);
							EnqueueDownstreamTask(attachment, driver, ImageMeta.Origin);
						}
						else
						{
							//logger.DebugFormat("Attachement {0} NOT found in cloud...", attachment.object_id);
						}
					}

					if (attachment.image_meta == null)
						break;

					// small
					if (attachment.image_meta.small != null &&
					    (savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.small == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Small);
					}

					// medium
					if (attachment.image_meta.medium != null &&
					    (savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.medium == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Medium);
					}

					// large
					if (attachment.image_meta.large != null &&
					    (savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.large == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
					}

					// square
					if (attachment.image_meta.square != null &&
					    (savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.square == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Square);
					}
				}
			}
		}

		private bool CloudHasOriginAttachment(string object_id, Driver user)
		{
			try
			{
				using (WebClient agent = new DefaultWebClient())
				{
					AttachmentInfo info = AttachmentApi.GetInfo(agent, object_id, user.session_token);
					return !string.IsNullOrEmpty(info.url);
				}
			}
			catch
			{
				// Not able to get attachment info - just assume "YES"
				return true;
			}
		}
	}
}