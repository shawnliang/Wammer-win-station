using MongoDB.Driver.Builders;
using System;
using Wammer.Cloud;
using Wammer.Station.Retry;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	[Serializable]
	internal class UpdateDriverDBTask : DelayedRetryTask
	{
		public UserLoginEventArgs args { get; set; }
		public string station_id { get; set; }
		public int retry_count { get; set; }

		public UpdateDriverDBTask()
			: base(TaskPriority.High)
		{
		}

		public UpdateDriverDBTask(UserLoginEventArgs args, string station_id)
			: base(TaskPriority.High)
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

			StationSignUpResponse api = StationApi.SignUpBySession(args.session_token, station_id, StatusChecker.GetDetail());

			DriverCollection.Instance.Update(
				Query.EQ("_id", args.user_id),
				Update.Set("session_token", api.session_token));
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}