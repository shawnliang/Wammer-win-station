using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;

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
		private readonly ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue;

		public ResourceDownloader(ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
		{
			this.bodySyncQueue = bodySyncQueue;
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
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				if (user.isPrimaryStation)
				{
					var queued_items = AttachmentApi.GetQueue(user.session_token, int.MaxValue);
					foreach (var object_id in queued_items.objects)
					{
						var attachmentInfo = AttachmentApi.GetInfo(object_id, user.session_token);
						EnqueueDownstreamTask(attachmentInfo, user, ImageMeta.Origin);
					}
				}
			}


			var posts = PostCollection.Instance.Find(
				Query.And(
					Query.Exists("attachments", true),
					Query.EQ("hidden", "false")));

			foreach (var post in posts)
			{
				foreach (var attachment in post.attachments)
				{
					var savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachment.object_id));
					var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", attachment.creator_id));

					// driver might be removed before download tasks completed
					if (driver == null)
						break;

					var imageMeta = attachment.image_meta;
					if (imageMeta == null)
						break;

					var savedImageMeta = (savedDoc == null) ? null : savedDoc.image_meta;

					// small
					if (imageMeta.small != null &&
						(savedImageMeta == null || savedImageMeta.small == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Small);
					}

					// medium
					if (imageMeta.medium != null &&
						(savedImageMeta == null || savedImageMeta.medium == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Medium);
					}

					// large
					if (imageMeta.large != null &&
						(savedImageMeta == null || savedImageMeta.large == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
					}

					// square
					if (imageMeta.square != null &&
					    (savedImageMeta == null || savedImageMeta.square == null))
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
				var info = AttachmentApi.GetInfo(object_id, user.session_token);
				return !string.IsNullOrEmpty(info.url);
			}
			catch
			{
				// Not able to get attachment info - just assume "YES"
				return true;
			}
		}
	}
}
