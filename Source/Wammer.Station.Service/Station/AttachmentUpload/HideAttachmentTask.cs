using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station.Retry;
using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	class HideAttachmentTask : DelayedRetryTask, INamedTask
	{
		public string Name { get; set; }
		public List<string> object_ids { get; set; }
		public string user_id { get; set; }
		public int retry_count { get; set; }

		public HideAttachmentTask(List<string> attachments, string user_id)
			:base(TaskPriority.High)
		{
			Name = Guid.NewGuid().ToString();
			this.object_ids = attachments;
			this.user_id = user_id;
		}

		protected override void Run()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("user session expired");

			var result = AttachmentApi.Hide(user.session_token, CloudServer.APIKey, object_ids);

			foreach (var success_id in result.success_ids)
			{
				object_ids.Remove(success_id);
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
				this.LogWarnMsg("objects not hiden: " + string.Join(", ", object_ids.ToArray()));
				return;
			}

			AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(this, this.priority);
		}
	}
}
