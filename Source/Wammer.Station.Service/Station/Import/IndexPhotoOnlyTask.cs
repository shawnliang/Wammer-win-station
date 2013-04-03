using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;
using System.IO;
using Wammer.Utility;
using Wammer.Station.AttachmentUpload;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Import
{
	class IndexPhotoOnlyTask : ITask
	{
		public FileMetadata file;
		public string user_id;
		public Guid task_id;

		public IndexPhotoOnlyTask(FileMetadata fileMeta, string user_id, Guid task_id)
		{
			this.file = fileMeta;
			this.user_id = user_id;
			this.task_id = task_id;
		}


		public void Execute()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				return;

			try
			{
				saveToAttachmentDB(user);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Error import photo (index only): " + file.file_path, e);
				
				var delete = new AttachmentDeleteTask(new List<string> { file.object_id }, user_id, true);
				AttachmentUploadQueueHelper.Instance.Enqueue(delete, TaskPriority.High);

				return;
			}

			updateImportTaskStatus();

			if (user.isPaidUser)
			{
				var backupTask = new UpstreamTask(file.object_id, ImageMeta.Origin, TaskPriority.VeryLow, task_id);
				AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(backupTask, TaskPriority.VeryLow);

				var medium = new MakeThumbnailTask(file.object_id, ImageMeta.Medium, TaskPriority.Medium, task_id);
				TaskQueue.Enqueue(medium, medium.Priority, true);
			}
			else
			{
				var medium = new MakeThumbnailAndUpstreamTask(file.object_id, ImageMeta.Medium, TaskPriority.Medium, task_id);
				TaskQueue.Enqueue(medium, medium.Priority, true);
			}


			var small = new MakeThumbnailTask(file.object_id, ImageMeta.Small, TaskPriority.Medium, task_id);
			TaskQueue.Enqueue(small, small.Priority, true);
		}

		private void saveToAttachmentDB(Driver user)
		{
			using (var imageStream = File.OpenRead(file.file_path))
			{
				var imageSize = ImageHelper.GetImageSize(imageStream);
				imageStream.Position = 0;

				var attDoc = new Attachment
				{
					creator_id = user_id,
					device_id = StationRegistry.StationId,
					event_time = file.EventTime,
					file_create_time = file.file_create_time,
					file_modify_time = File.GetLastWriteTime(file.file_path),
					file_name = file.file_name,
					file_path = file.file_path,
					file_size = new FileInfo(file.file_path).Length,
					fromLocal = true,
					group_id = user.groups[0].group_id,
					image_meta = new ImageProperty { exif = file.exif },
					mime_type = MimeTypeHelper.GetMIMEType(file.file_name),
					MD5 = MD5Helper.ComputeMD5(imageStream),
					modify_time = DateTime.Now,
					object_id = file.object_id,
					timezone = file.timezone,
					type = AttachmentType.image,
					IndexOnly = true
				};
				AttachmentCollection.Instance.Save(attDoc);
			}
		}

		public void updateImportTaskStatus()
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", task_id), Update.Inc("Copied", 1));
		}
	}
}
