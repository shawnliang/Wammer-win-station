using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Wammer.Station.AttachmentUpload;
using System.IO;
using Wammer.Utility;
using Waveface.Stream.Model;
using Waveface.Stream.Core;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Import
{
	class CopyPhotoTask : ITask
	{
		public FileMetadata file { get; set; }
		public string user_id { get; set; }
		public Guid task_id { get; set; }

		public CopyPhotoTask(FileMetadata file, string user_id, Guid task_id)
		{
			this.file = file;
			this.user_id = user_id;
			this.task_id = task_id;
		}

		public void Execute()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null || string.IsNullOrEmpty(user.session_token))
				return;

			long begin = Stopwatch.GetTimestamp();

			copyPhoto(user);

			long duration = getProcessDuration(begin);

			Wammer.PerfMonitor.UploadDownloadMonitor.Instance.OnAttachmentProcessed(this, new HttpHandlerEventArgs(duration));

			SystemEventSubscriber.Instance.TriggerAttachmentArrivedEvent(file.object_id);

			updateImportTaskStatus();
		}

		private static long getProcessDuration(long begin)
		{
			long end = Stopwatch.GetTimestamp();
			long duration = end - begin;
			if (duration < 0)
				duration += long.MaxValue;
			return duration;
		}

		private void copyPhoto(Driver user)
		{
			try
			{
				var imp = new AttachmentUploadHandlerImp(
							 new AttachmentUploadHandlerDB(),
							 new AttachmentUploadStorage(new AttachmentUploadStorageDB()));

				var postProcess = new AttachmentProcessedHandler(new AttachmentUtility(task_id));

				imp.AttachmentProcessed += postProcess.OnProcessed;


				var uploadData = new UploadData()
				{
					object_id = file.object_id,
					raw_data = new ArraySegment<byte>(File.ReadAllBytes(file.file_path)),
					file_name = Path.GetFileName(file.file_path),
					mime_type = MimeTypeHelper.GetMIMEType(file.file_path),
					group_id = user.groups[0].group_id,
					api_key = CloudServer.APIKey,
					session_token = user.session_token,
					file_path = file.file_path,
					import_time = DateTime.Now,
					file_create_time = file.file_create_time,
					type = AttachmentType.image,
					imageMeta = ImageMeta.Origin,
					fromLocal = true,
					timezone = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes,
					exif = file.exif.ToFastJSON()
				};
				imp.Process(uploadData);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to import photo: " + file.file_path, e);

				var delete = new AttachmentDeleteTask(new List<string> { file.object_id }, user_id, true);
				AttachmentUploadQueueHelper.Instance.Enqueue(delete, TaskPriority.High);
			}
		}

		public void updateImportTaskStatus()
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", task_id), Update.Inc("Copied", 1));
		}
	}
}
