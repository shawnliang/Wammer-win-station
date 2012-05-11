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
		private readonly UserLoginEventArgs args;
		private readonly string station_id;

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
				Cloud.StationSignUpResponse api = Cloud.StationApi.SignUpBySession(agent, this.args.session_token, station_id);

				DriverCollection.Instance.Update(
					Query.EQ("_id", args.user_id),
					Update.Set("session_token", api.session_token));
			}
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}
