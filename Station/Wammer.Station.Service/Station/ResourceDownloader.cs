using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

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
			Debug.Assert(!string.IsNullOrEmpty(driver.user_id));
			Debug.Assert(!string.IsNullOrEmpty(attachment.object_id));

			var fs = new FileStorage(driver);
			var evtargs = new ResourceDownloadEventArgs
							{
								user_id = driver.user_id,
								attachment = attachment,
								imagemeta = meta,
								filepath = Path.Combine(fs.basePath, GetSavedFile(attachment.object_id, attachment.file_name, meta) + @".tmp") //FileStorage.GetTempFile(driver)
							};

			EnqueueDownstreamTask(meta, evtargs);
		}

		private static string GetSavedFile(string objectID, string uri, ImageMeta meta)
		{
			var fileName = objectID;

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				var metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;
				fileName += "_" + metaStr;
			}

			if (uri.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
				uri = new Uri(uri).AbsolutePath;

			var extension = Path.GetExtension(uri);

			if (meta == ImageMeta.Small || meta == ImageMeta.Medium || meta == ImageMeta.Large || meta == ImageMeta.Square)
				fileName += ".dat";
			else if (!string.IsNullOrEmpty(extension))
				fileName += extension;

			return fileName;
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
			Debug.Assert(driver != null);

			// driver might be removed before running download tasks
			if (driver == null)
				return;

			foreach (PostInfo post in posts)
			{
				if (string.Compare(post.hidden, "true", true) == 0)
					continue;

				foreach (AttachmentInfo attachment in post.attachments)
				{
					Debug.Assert(!string.IsNullOrEmpty(attachment.object_id));

					if (string.IsNullOrEmpty(attachment.object_id))
						continue;

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
			DateTime beginTime = DateTime.Now;

			try
			{
				DownloadOriginalAttachmentsFromCloud();

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

						// temp skil large thumbnails because no client uses this at this moment.
						//if (imageMeta.large != null &&
						//    (savedImageMeta == null || savedImageMeta.large == null))
						//{
						//    EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
						//}

					// square
					if (imageMeta.square != null &&
					    (savedImageMeta == null || savedImageMeta.square == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Square);
					}
				}
			}
		}
			catch (Exception e)
			{
				this.LogWarnMsg("Resume unfinished downstream tasks not success: " + e.ToString());
			}
			finally
			{
				TimeSpan duration = DateTime.Now - beginTime;
				this.LogDebugMsg("Resume unfinished downstream tasks done. Totoal seconds spent: " + duration.TotalSeconds.ToString());
			}
		}

		private void DownloadOriginalAttachmentsFromCloud()
		{
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				if (user.isPrimaryStation)
				{
					var queued_items = AttachmentApi.GetQueue(user.session_token, int.MaxValue);
					foreach (var object_id in queued_items.objects)
					{
						try
						{
							var attachmentInfo = AttachmentApi.GetInfo(object_id, user.session_token);
							EnqueueDownstreamTask(attachmentInfo, user, ImageMeta.Origin);
						}
						catch (Exception e)
						{
							this.LogWarnMsg(string.Format("Unable to download origin attachment {0} : {1}", object_id, e.ToString()));
						}
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
