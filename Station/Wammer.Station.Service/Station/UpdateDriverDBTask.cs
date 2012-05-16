using System;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Retry;
using Wammer.Utility;

namespace Wammer.Station
{
	[Serializable]
	internal class UpdateDriverDBTask : DelayedRetryTask
	{
		private readonly UserLoginEventArgs args;
		private readonly string station_id;
		private int retry_count;

		public UpdateDriverDBTask(UserLoginEventArgs args, string station_id)
			: base(RetryQueue.Instance, TaskPriority.High)
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

			using (var agent = new DefaultWebClient())
			{
				StationSignUpResponse api = StationApi.SignUpBySession(agent, args.session_token, station_id, StatusChecker.GetDetail());

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