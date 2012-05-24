using System;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	[Serializable]
	internal class NotifyCloudOfBodySyncedTask : Retry.DelayedRetryTask
	{
		private readonly string object_id;
		private readonly string session_token;
		private int retry_count;

		public NotifyCloudOfBodySyncedTask(string object_id, string session_token)
			:base(Retry.RetryQueue.Instance, TaskPriority.Low)
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