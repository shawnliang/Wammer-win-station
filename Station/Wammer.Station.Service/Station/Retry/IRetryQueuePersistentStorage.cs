using System;
using System.Collections.Generic;

namespace Wammer.Station.Retry
{
	public class RetryQueueItem
	{
		public DateTime NextRunTime { get; set; }
		public IRetryTask Task { get; set; }
	}

	public class NullRetryQueuePersistentStorage : IRetryQueuePersistentStorage
	{
		#region IRetryQueuePersistentStorage Members

		public ICollection<RetryQueueItem> LoadSavedTasks()
		{
			return new List<RetryQueueItem>();
		}

		public void Add(DateTime key, IRetryTask task)
		{
		}

		public bool Remove(DateTime key)
		{
			return true;
		}

		public void Clear()
		{
		}
		#endregion
	}

	public interface IRetryQueuePersistentStorage
	{
		ICollection<RetryQueueItem> LoadSavedTasks();
		void Add(DateTime key, IRetryTask task);
		bool Remove(DateTime key);
		void Clear();
	}
}