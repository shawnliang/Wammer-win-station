using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station.Retry;

namespace Wammer.Station
{
	class TaskRetryTimer : NonReentrantTimer
	{
		public TaskRetryTimer()
			:base(10*1000)
		{
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			var tasks = Retry.RetryQueue.Instance.Dequeue(DateTime.Now);

			foreach (IRetryTask task in tasks)
			{
				TaskQueue.Enqueue(task, task.Priority);
				Retry.RetryQueue.Instance.AckDequeue(task.NextRetryTime);
			}
		}
	}
}
