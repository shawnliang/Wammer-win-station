using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using Wammer.Utility;

namespace Wammer.Station
{
	[Serializable]
	class UpdateDriverDBTask : Retry.DelayedRetryTask
	{
		private int retry_count;
		private UserLoginEventArgs args;
		private string station_id;

		public UpdateDriverDBTask(UserLoginEventArgs args, string station_id)
			:base(Retry.RetryQueue.Instance, TaskPriority.High)
		{
			this.args = args;
			this.station_id = station_id;
		}

		protected override void Run()
		{
			if (++retry_count >= 20)
			{
				this.LogErrorMsg("Failed to update drvier session token: " + args.email);
				return;
			}

			using (DefaultWebClient agent = new DefaultWebClient())
			{
				Cloud.StationApi api = new Cloud.StationApi(this.station_id, args.session_token);
				api.LogOn(agent, StatusChecker.GetDetail());

				DriverCollection.Instance.Update(
					Query.EQ("_id", args.user_id),
					Update.Set("session_token", api.Token));
			}
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}
