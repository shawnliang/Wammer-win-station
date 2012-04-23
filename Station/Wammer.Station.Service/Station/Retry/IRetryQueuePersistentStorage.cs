using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Retry
{
	public class RetryQueueItem
	{
		public DateTime NextRunTime { get; set; }
		public IRetryTask Task { get; set; }
	}

	public class NullRetryQueuePersistentStorage: IRetryQueuePersistentStorage
	{

		public ICollection<RetryQueueItem> LoadSavedTasks()
		{
			return new List<RetryQueueItem>();
		}

		public void Add(DateTime key, IRetryTask task)
		{
		}

		public void Remove(DateTime key)
		{
		}
	}

	public interface IRetryQueuePersistentStorage
	{
		ICollection<RetryQueueItem> LoadSavedTasks();
		void Add(DateTime key, IRetryTask task);
		void Remove(DateTime key);
	}
}
