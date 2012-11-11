using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station.Retry;
using Wammer.Cloud;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	class UploadMetadataTask : DelayedRetryTask
	{
		private string group_id;
		private string metadata;

		public UploadMetadataTask(string group_id, string metadata)
			:base(TaskPriority.High)
		{
			this.group_id = group_id;
			this.metadata = metadata;
		}

		protected override void Run()
		{
			var user = Model.DriverCollection.Instance.FindDriverByGroupId(group_id);

			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("Retry later because user session expired: " + user.email);

			AttachmentApi.UploadMetadata(user.session_token, CloudServer.APIKey, metadata);
		}

		public override void ScheduleToRun()
		{
			AttachmentUploadQueueHelper.Instance.Enqueue(this, Priority);
		}
	}
}
