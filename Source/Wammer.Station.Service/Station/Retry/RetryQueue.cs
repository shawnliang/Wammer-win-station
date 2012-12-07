using System;
using System.Collections.Generic;
using System.Linq;

namespace Wammer.Station.Retry
{
	public class RetryQueue : AbstractTaskEnDequeueNotifier, IRetryQueue
	{
		private readonly SortedList<DateTime, IRetryTask> retryTasks = new SortedList<DateTime, IRetryTask>();
		private readonly IRetryQueuePersistentStorage storage;

		public RetryQueue(IRetryQueuePersistentStorage storage)
		{
			this.storage = storage;
		}

		public void LoadSavedTasks(Action<RetryQueueItem> itemLoadedCallback)
		{
			foreach (RetryQueueItem item in storage.LoadSavedTasks())
			{
				retryTasks.Add(item.NextRunTime, item.Task);

				if (itemLoadedCallback != null)
					itemLoadedCallback(item);
			}
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

				// sync NextRetryTime (which is key) to make sure this task can be removed from DB
				task.NextRetryTime = retryTime;

				retryTasks.Add(retryTime, task);
				storage.Add(retryTime, task);
				OnTaskEnqueued(task);
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

				foreach (var task in selected)
				{
					OnTaskDequeued(task);
				}

				return selected;
			}
		}

		public bool AckDequeue(DateTime key)
		{
			lock (retryTasks)
			{
				return storage.Remove(key);
			}
		}

		public void Clear()
		{
			lock (retryTasks)
			{
				retryTasks.Clear();
				storage.Clear();
			}
		}
	}
}