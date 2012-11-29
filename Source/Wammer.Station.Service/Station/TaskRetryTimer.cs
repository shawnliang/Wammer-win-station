using System;
using System.Collections.Generic;
using Wammer.Station.Retry;

namespace Wammer.Station
{
	internal class TaskRetryTimer : NonReentrantTimer
	{
		public TaskRetryTimer()
			: base(10*1000)
		{
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			ICollection<IRetryTask> tasks = RetryQueueHelper.Instance.Dequeue(DateTime.Now);

			foreach (IRetryTask task in tasks)
			{
				// In case NextRetryTime is changed during task.ScheduleToRun()
				// save this value for later use.
				var key = task.NextRetryTime;

				task.ScheduleToRun();

				var removed = RetryQueueHelper.Instance.AckDequeue(key);

				if (!removed)
					this.LogWarnMsg("Data is not removed from retry_queue collection: " + task.ToString());
			}
		}
	}
}