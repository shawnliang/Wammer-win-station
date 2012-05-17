using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	[Serializable]
	class NotifyCloudOfMultiBodySyncedTask : Retry.DelayedRetryTask
	{
		private List<String> object_ids;
		private string user_id;
		private int retry_count;

		public NotifyCloudOfMultiBodySyncedTask(List<string> object_ids, string user_id)
			:base(Retry.RetryQueue.Instance, TaskPriority.Low)
		{
			if (object_ids == null || object_ids.Count == 0 || string.IsNullOrEmpty(user_id))
				throw new ArgumentNullException();

			this.object_ids = object_ids;
			this.user_id = user_id;
		}

		protected override void Run()
		{
			if (++retry_count > 50)
			{
				this.LogWarnMsg("Abort notifying cloud of body synced. Object count : " + object_ids.Count);
				return;
			}

			Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));
			if (user == null)
			{
				this.LogDebugMsg("Abort task because user " + user_id + " is removed.");
				return;
			}

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("User session is not valid. Try it latter.");

			using (System.Net.WebClient agent = new DefaultWebClient())
			{
				Cloud.AttachmentApi.SetSync(agent, object_ids, user.session_token);
			}
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, TaskPriority.Low, true);
		}
	}
}
