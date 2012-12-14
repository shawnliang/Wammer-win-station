using System;
using Wammer.Cloud;

namespace Wammer.Station
{
	[Serializable]
	internal class NotifyCloudOfBodySyncedTask : Retry.DelayedRetryTask
	{
		public string object_id { get; set; }
		public string session_token { get; set; }
		public int retry_count { get; set; }

		public NotifyCloudOfBodySyncedTask()
			: base(TaskPriority.Low)
		{
		}

		public NotifyCloudOfBodySyncedTask(string object_id, string session_token)
			: this()
		{
			if (object_id == null || session_token == null)
				throw new ArgumentNullException();

			this.object_id = object_id;
			this.session_token = session_token;
		}

		#region DelayedRetryTask Members
		protected override void Run()
		{
			if (++retry_count >= 100)
			{
				this.LogWarnMsg("Abort reporting body synced to cloud. Failure too many times.");
				return;
			}

			AttachmentApi.SetSync(object_id, session_token);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority, true);
		}
		#endregion

	}
}