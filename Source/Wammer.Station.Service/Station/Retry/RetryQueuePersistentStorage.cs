using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Model;

namespace Wammer.Station.Retry
{
	public class RetryQueuePersistentStorage : IRetryQueuePersistentStorage
	{
		#region IRetryQueuePersistentStorage Members

		public ICollection<RetryQueueItem> LoadSavedTasks()
		{
			MongoCursor<GenericData> savedItems = RetryQueueCollection.Instance.FindAll();

			return savedItems.Select(x =>
			{
				var item = new RetryQueueItem
				{
					Task = (IRetryTask)x.Data,
					NextRunTime = new DateTime(Convert.ToInt64(x.Id), DateTimeKind.Utc)
				};

				// Fix bugs of old versions:
				// Sync NextRetryTime to make sure this item can be removed from DB
				item.Task.NextRetryTime = item.NextRunTime;
				return item;
			}).ToList();
		}

		public void Add(DateTime key, IRetryTask task)
		{
			RetryQueueCollection.Instance.Save(
				new GenericData
				{
					Id = key.ToUniversalTime().Ticks.ToString(),
					Data = task
				});
		}

		public bool Remove(DateTime key)
		{
			var result = RetryQueueCollection.Instance.Remove(
				Query.EQ("_id", key.ToUniversalTime().Ticks.ToString()));

			return result.DocumentsAffected > 0;
		}

		public void Clear()
		{
			RetryQueueCollection.Instance.RemoveAll();
		}

		#endregion
	}
}