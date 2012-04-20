using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Retry
{
	public class RetryQueue : IRetryQueue
	{
		private static RetryQueue instance;
		SortedList<DateTime, IRetryTask> retryTasks = new SortedList<DateTime, IRetryTask>();


		private RetryQueue()
		{

		}
		public static RetryQueue Instance
		{
			get
			{
				if (instance == null)
					instance = new RetryQueue();

				return instance;
			}
		}


		public void Enqueue(IRetryTask task)
		{
			DateTime retryTime = task.NextRetryTime;

			lock (retryTasks)
			{
				while (retryTasks.ContainsKey(retryTime))
				{
					retryTime = retryTime.AddMilliseconds(1.0);
				}
				retryTasks.Add(retryTime, task);
			}
		}

		public ICollection<IRetryTask> Dequeue(DateTime now)
		{
			lock (retryTasks)
			{
				var selected = new List<IRetryTask>(retryTasks.Where(x => x.Key <= now).Select(x=>x.Value));
				var selectedKeys = new List<DateTime>(retryTasks.Where(x=>x.Key <= now).Select(x => x.Key));

				foreach (DateTime key in selectedKeys)
					retryTasks.Remove(key);

				return selected;
			}
		}


	}
}
