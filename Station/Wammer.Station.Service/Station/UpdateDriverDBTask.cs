using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	[Serializable]
	class UpdateDriverDBTask : Retry.DelayedRetryTask
	{
		private int retry_count;
		private UserLoginEventArgs args { get; set; }

		public UpdateDriverDBTask(UserLoginEventArgs args)
			:base(Retry.RetryQueue.Instance, TaskPriority.High)
		{
			this.args = args;
		}

		protected override void Run()
		{
			if (++retry_count >= 20)
			{
				this.LogErrorMsg("Failed to update drvier session token: " + args.email);
				return;
			}
			Cloud.User user = Cloud.User.LogIn(new System.Net.WebClient(), args.email, args.password);

			Driver userDoc = DriverCollection.Instance.FindOne(Query.EQ("_id", args.user_id));
			if (userDoc == null)
				return;

			userDoc.session_token = user.Token;
			DriverCollection.Instance.Save(userDoc);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}
