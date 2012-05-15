using System;
using System.Collections.Generic;
using System.Linq;

namespace Wammer.Station.Retry
{
	public class RetryQueue : IRetryQueue
	{
		private static RetryQueue instance;
		private readonly SortedList<DateTime, IRetryTask> retryTasks = new SortedList<DateTime, IRetryTask>();
		private readonly IRetryQueuePersistentStorage storage;

		private RetryQueue()
		{
			storage = new RetryQueuePersistentStorage();

			foreach (RetryQueueItem item in storage.LoadSavedTasks())
			{
				retryTasks.Add(item.NextRunTime, item.Task);
			}
		}

		/// <summary>
		/// For unit test only.
		/// </summary>
		/// <param name="storage"></param>
		public RetryQueue(IRetryQueuePersistentStorage storage)
		{
			this.storage = storage;

			foreach (RetryQueueItem item in storage.LoadSavedTasks())
			{
				retryTasks.Add(item.NextRunTime, item.Task);
			}
		}

		public static RetryQueue Instance
		{
			get { return instance ?? (instance = new RetryQueue()); }
		}

		#region IRetryQueue Members

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
				storage.Add(retryTime, task);
			}
		}

		#endregion

		public ICollection<IRetryTask> Dequeue(DateTime now)
		{
			lock (retryTasks)
			{
				var selected = new List<IRetryTask>(retryTasks.Where(x => x.Key <= now).Select(x => x.Value));
				var selectedKeys = new List<DateTime>(retryTasks.Where(x => x.Key <= now).Select(x => x.Key));

				foreach (DateTime key in selectedKeys)
				{
					retryTasks.Remove(key);
				}

				return selected;
			}
		}

		public void AckDequeue(DateTime key)
		{
			storage.Remove(key);
		}
	}
}