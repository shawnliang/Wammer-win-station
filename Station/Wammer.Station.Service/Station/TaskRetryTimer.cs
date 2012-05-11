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
			ICollection<IRetryTask> tasks = RetryQueue.Instance.Dequeue(DateTime.Now);

			foreach (IRetryTask task in tasks)
			{
				task.ScheduleToRun();
				RetryQueue.Instance.AckDequeue(task.NextRetryTime);
			}
		}
	}
}