using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wammer.Cloud;
using Wammer.Station.Retry;
using Waveface.Stream.Model;
using Microsoft.VisualBasic.FileIO;

namespace Wammer.Station.AttachmentUpload
{
	class AttachmentDeleteTask : DelayedRetryTask, INamedTask
	{
		public string Name { get; set; }
		public List<string> object_ids { get; set; }
		public string user_id { get; set; }
		public int retry_count { get; set; }
		public Boolean m_NeedNotifyCloud { get; set; }

		public AttachmentDeleteTask(List<string> attachments, string user_id, Boolean needNotifyCloud)
			: base(TaskPriority.High)
		{
			Name = Guid.NewGuid().ToString();
			this.object_ids = attachments;
			this.user_id = user_id;
			m_NeedNotifyCloud = needNotifyCloud;
		}

		protected override void Run()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("user session expired");

			var attachmentIDs = default(IEnumerable<string>);

			if (m_NeedNotifyCloud)
			{
				var result = AttachmentApi.Delete(user.session_token, CloudServer.APIKey, object_ids);
				attachmentIDs = result.success_ids;
			}
			else
				attachmentIDs = this.object_ids.ToList();

			foreach (var attachmentID in attachmentIDs)
			{
				var attachment = AttachmentCollection.Instance.FindOneById(attachmentID);

				if (attachment != null)
				{
					var saveFileName = attachment.saved_file_name;

					AttachmentCollection.Instance.Remove(Query.EQ("_id", attachmentID));

					var fs = new FileStorage(user);
					var originFile = fs.GetResourceFilePath(saveFileName);

					if (File.Exists(originFile))
					{
						try
						{
							FileSystem.DeleteFile(originFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
						}
						catch (Exception)
						{
						}
					}
				}

				object_ids.Remove(attachmentID);
			}

			if (object_ids.Count > 0)
			{
				retry_count++;
				ScheduleToRun();
			}
		}

		public override void ScheduleToRun()
		{
			if (retry_count > 100)
			{
				this.LogWarnMsg("This task failed for too many times... abort retry");
				this.LogWarnMsg("objects not delete: " + string.Join(", ", object_ids.ToArray()));
				return;
			}

			AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(this, this.priority);
		}
	}
}
