﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Retry
{
	public class RetryQueue : IRetryQueue
	{
		private static RetryQueue instance;
		SortedList<DateTime, IRetryTask> retryTasks = new SortedList<DateTime, IRetryTask>();
		IRetryQueuePersistentStorage storage;

		private RetryQueue()
		{
			storage = new RetryQueuePersistentStorage();

			foreach (var item in storage.LoadSavedTasks())
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

			foreach (var item in storage.LoadSavedTasks())
			{
				retryTasks.Add(item.NextRunTime, item.Task);
			}
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
				storage.Add(retryTime, task);
			}
		}

		public ICollection<IRetryTask> Dequeue(DateTime now)
		{
			lock (retryTasks)
			{
				var selected = new List<IRetryTask>(retryTasks.Where(x => x.Key <= now).Select(x=>x.Value));
				var selectedKeys = new List<DateTime>(retryTasks.Where(x=>x.Key <= now).Select(x => x.Key));

				foreach (DateTime key in selectedKeys)
				{
					retryTasks.Remove(key);
					storage.Remove(key);
				}

				return selected;
			}
		}


	}
}