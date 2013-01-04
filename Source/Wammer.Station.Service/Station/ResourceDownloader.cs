using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Wammer.Cloud;
using Wammer.Model;
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
	}



	internal class ResourceDownloader
	{
		private readonly ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue;

		public ResourceDownloader(ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
		{
			this.bodySyncQueue = bodySyncQueue;
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

		public static ResourceDownloadTask createDownloadTask(Driver driver, ImageMeta meta, AttachmentInfo attachment)
		{
			string tmpFolder;

			if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				tmpFolder = new FileStorage(driver).basePath;
			else
			{
				tmpFolder = Path.Combine("cache", driver.user_id);
				if (!Directory.Exists(tmpFolder))
					Directory.CreateDirectory(tmpFolder);
			}

			var evtargs = new ResourceDownloadEventArgs
			{
				user_id = driver.user_id,
				attachment = attachment,
				imagemeta = meta,
				filepath = Path.Combine(tmpFolder, GetSavedFile(attachment.object_id, attachment.file_name, meta) + @".tmp") //FileStorage.GetTempFile(driver)
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

	}

	[Serializable]
	internal class QueryIfDownstreamNeededTask : Retry.DelayedRetryTask
	{
		public string object_id { get; set; }
		public string user_id { get; set; }
		public AttachmentInfo cloudDoc { get; set; }

		public QueryIfDownstreamNeededTask()
			: base(TaskPriority.Low)
		{
		}

		public QueryIfDownstreamNeededTask(string user_id, string object_id)
			: this()
		{
			this.object_id = object_id;
			this.user_id = user_id;
		}

		protected override void Run()
		{
			try
			{
				var user = DriverCollection.Instance.FindOneById(user_id);
				if (user == null)
					return;

				var localDoc = AttachmentCollection.Instance.FindOneById(object_id);

				if (localDoc != null && localDoc.type == AttachmentType.image && localDoc.fromLocal)
					return;

				if (localHasNoMedium(localDoc) && cloudHasMedium(getCloudDoc(user)))
				{
					var task = ResourceDownloader.createDownloadTask(user, ImageMeta.Medium, cloudDoc);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}

				if (localHasNoSmall(localDoc) && cloudHasSmall(getCloudDoc(user)))
				{
					var task = ResourceDownloader.createDownloadTask(user, ImageMeta.Small, cloudDoc);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}

				bool scheduledToDownloadOrigDoc = false;
				if (localHasNoOrigin(localDoc) && cloudHasOrigin(getCloudDoc(user)) && !user.ReachOriginSizeLimit())
				{
					var task = ResourceDownloader.createDownloadTask(user, ImageMeta.Origin, cloudDoc);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);

					scheduledToDownloadOrigDoc = cloudDoc.type.Equals("doc", StringComparison.InvariantCultureIgnoreCase);
				}

				if (isDocTypeAndLocalHasNoPreview(user, localDoc) && cloudHasPreview(user) && !scheduledToDownloadOrigDoc)
				{
					var task = new DownloadDocPreviewsTask(object_id, user_id);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == (int)AttachmentApiError.AttachmentNotExist)
					return;
				else
					throw;
			}
		}

		private bool cloudHasPreview(Driver user)
		{
			return getCloudDoc(user).doc_meta != null && getCloudDoc(user).doc_meta.preview_pages > 0;
		}

		private bool isDocType(Driver user, Attachment localDoc)
		{
			var isDocType = localDoc != null && localDoc.type == AttachmentType.doc || getCloudDoc(user).type.Equals("doc", StringComparison.InvariantCultureIgnoreCase);
			return isDocType;
		}

		private bool isDocTypeAndLocalHasNoPreview(Driver user, Attachment localDoc)
		{

			return isDocType(user, localDoc) &&
				(localDoc == null ||
				 localDoc.doc_meta == null ||
				 localDoc.doc_meta.preview_files == null ||
				 localDoc.doc_meta.preview_files.Count == 0);
		}

		private AttachmentInfo getCloudDoc(Driver user)
		{
			if (cloudDoc == null)
			{
				cloudDoc = AttachmentApi.GetInfo(object_id, user.session_token);
			}

			return cloudDoc;
		}

		private static bool localHasNoMedium(Attachment localDoc)
		{
			return localDoc == null || localDoc.image_meta == null || localDoc.image_meta.medium == null ||
					string.IsNullOrEmpty(localDoc.image_meta.medium.saved_file_name);
		}

		private static bool localHasNoSmall(Attachment localDoc)
		{
			return localDoc == null || localDoc.image_meta == null || localDoc.image_meta.small == null ||
					string.IsNullOrEmpty(localDoc.image_meta.small.saved_file_name);
		}

		private static bool localHasNoOrigin(Attachment localDoc)
		{
			return localDoc == null || string.IsNullOrEmpty(localDoc.saved_file_name);
		}

		private static bool cloudHasOrigin(AttachmentInfo attachment)
		{
			return !string.IsNullOrEmpty(attachment.url);
		}

		private static bool cloudHasSmall(AttachmentInfo attachment)
		{
			return attachment.image_meta != null && attachment.image_meta.small != null &&
								!string.IsNullOrEmpty(attachment.image_meta.small.url);
		}

		private static bool cloudHasMedium(AttachmentInfo attachment)
		{
			return attachment.image_meta != null && attachment.image_meta.medium != null &&
								!string.IsNullOrEmpty(attachment.image_meta.medium.url);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, this.Priority);
		}
	}

}
