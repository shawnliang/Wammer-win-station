using System;
using Wammer.Cloud;
using Wammer.Station.Retry;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	public class UploadMetadataTask : DelayedRetryTask
	{
		public string group_id { get; set; }
		public string metadata { get; set; }
		public int metaCount { get; set; }

		[field: NonSerialized]
		public event EventHandler<MetadataUploadEventArgs> Uploaded;

		public UploadMetadataTask(string group_id, string metadata, int metaCount)
			: base(TaskPriority.High)
		{
			this.group_id = group_id;
			this.metadata = metadata;
			this.metaCount = metaCount;
		}

		protected override void Run()
		{
			var user = Model.DriverCollection.Instance.FindDriverByGroupId(group_id);

			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("Retry later because user session expired: " + user.email);

			AttachmentApi.UploadMetadata(user.session_token, CloudServer.APIKey, metadata);

			OnUploaded();
		}

		private void OnUploaded()
		{
			var handler = Uploaded;
			if (handler != null)
				handler(this, new MetadataUploadEventArgs(metaCount));
		}

		public override void ScheduleToRun()
		{
			AttachmentUploadQueueHelper.Instance.Enqueue(this, Priority);
		}
	}
}
