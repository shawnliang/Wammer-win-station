using System;
using System.IO;
using System.Linq;
using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.Station.Timeline
{
	[Serializable]
	class QueryIfDownstreamNeededTask : Retry.DelayedRetryTask
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
					var task = ResourceDownloadTaskFactory.createDownloadTask(user, ImageMeta.Medium, cloudDoc);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}

				if (localHasNoSmall(localDoc) && cloudHasSmall(getCloudDoc(user)))
				{
					var task = ResourceDownloadTaskFactory.createDownloadTask(user, ImageMeta.Small, cloudDoc);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}

				var recoveredOriginFile = findExistingOriginalFilePath(user);

				if (localHasNoOrigin(localDoc) && !string.IsNullOrEmpty(recoveredOriginFile))
				{
					ResourceDownloadTask.SaveToAttachmentDB(ImageMeta.Origin,
						recoveredOriginFile.Substring(user.folder.Length + 1),
						getCloudDoc(user),
						new FileInfo(recoveredOriginFile).Length);
				}


				bool scheduledToDownloadOrigDoc = false;
				if (localHasNoOrigin(localDoc) && cloudHasOrigin(getCloudDoc(user)) && !user.ReachOriginSizeLimit())
				{
					if (!user.ReachOriginSizeLimit())
					{
						var task = ResourceDownloadTaskFactory.createDownloadTask(user, ImageMeta.Origin, cloudDoc);
						BodySyncQueue.Instance.Enqueue(task, task.Priority);

						scheduledToDownloadOrigDoc = cloudDoc.type.Equals("doc", StringComparison.InvariantCultureIgnoreCase);
					}
				}

				if (isDocTypeAndLocalHasNoPreview(user, localDoc) && cloudHasPreview(user) && !scheduledToDownloadOrigDoc)
				{
					var task = new DownloadDocPreviewsTask(object_id, user_id);
					BodySyncQueue.Instance.Enqueue(task, task.Priority);
				}

				if (isWebThumbType(user, localDoc))
				{
					var cloudApiResponse = getCloudDoc(user);
					var localDbDoc = AutoMapper.Mapper.Map<AttachmentInfo, Attachment>(cloudApiResponse);
					AttachmentCollection.Instance.Save(localDbDoc);

					if (cloudHasAnyWebThumb(user))
					{
						foreach (var cloudThumb in getCloudDoc(user).web_meta.thumbs.Where(x => !x.broken_thumb))
						{
							if (!localHasThisWebThumb(localDoc, cloudThumb))
							{
								var task = ResourceDownloadTaskFactory.createWebThumbDownloadTask(user, object_id, cloudThumb.id);
								BodySyncQueue.Instance.Enqueue(task, TaskPriority.Medium);
							}
						}
					}
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

		private string findExistingOriginalFilePath(Driver user)
		{
			var attInfo = getCloudDoc(user);

			if (string.IsNullOrEmpty(attInfo.file_name))
				return null;

			var dir = AttachmentUpload.AttachmentUploadStorage.GetAttachmentRelativeFolder(attInfo.event_time.ToUTCISO8601ShortString(), TimeHelper.ISO8601ToDateTime(attInfo.file_create_time));

			if (string.IsNullOrEmpty(dir))
				return null;

			var fullDir = Path.Combine(user.folder, dir);
			var filePathWithId = Path.Combine(fullDir, Path.GetFileNameWithoutExtension(attInfo.file_name)) + "." + attInfo.object_id + Path.GetExtension(attInfo.file_name);
			var filePathWithoutId = Path.Combine(fullDir, attInfo.file_name);

			if (File.Exists(filePathWithId))
				return filePathWithId;
			else if (File.Exists(filePathWithoutId))
				return filePathWithoutId;
			else
				return null;
		}

		private bool cloudHasPreview(Driver user)
		{
			return getCloudDoc(user).doc_meta != null && getCloudDoc(user).doc_meta.preview_pages > 0;
		}

		private bool isDocType(Driver user, Attachment localDoc)
		{
			if (localDoc != null)
				return localDoc.type == AttachmentType.doc;
			else
				return getCloudDoc(user).type.Equals("doc", StringComparison.InvariantCultureIgnoreCase);
		}

		private bool isWebThumbType(Driver user, Attachment localDoc)
		{
			if (localDoc != null)
				return localDoc.type == AttachmentType.webthumb;
			else
				return getCloudDoc(user).type.Equals("webthumb", StringComparison.InvariantCultureIgnoreCase);
		}

		private bool localHasThisWebThumb(Attachment localDoc, WebThumb cloudThumb)
		{
			return
				localDoc != null &&
				localDoc.web_meta != null &&
				localDoc.web_meta.thumbs != null &&
				localDoc.web_meta.thumbs.Any(x => x.id == cloudThumb.id && !string.IsNullOrEmpty(x.saved_file_name));
		}

		private bool cloudHasAnyWebThumb(Driver user)
		{
			return getCloudDoc(user).web_meta != null && getCloudDoc(user).web_meta.thumbs != null && getCloudDoc(user).web_meta.thumbs.Any();
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
			TaskQueue.Enqueue(this, this.Priority, true);
		}
	}
}
