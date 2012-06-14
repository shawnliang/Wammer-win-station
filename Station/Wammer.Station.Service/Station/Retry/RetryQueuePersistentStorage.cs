using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Station.Retry
{
	public class RetryQueuePersistentStorage : IRetryQueuePersistentStorage
	{
		#region IRetryQueuePersistentStorage Members

		public ICollection<RetryQueueItem> LoadSavedTasks()
		{
			MongoCursor<GenericData> savedItems = RetryQueueCollection.Instance.FindAll();

			var formatter = new BinaryFormatter();


			return savedItems.Select(x =>
										{
											using (var m = new MemoryStream(x.Data))
											{
												var task = formatter.Deserialize(m) as IRetryTask;
												var item = new RetryQueueItem
														{
															Task = task,
															NextRunTime = new DateTime(Convert.ToInt64(x.Id), DateTimeKind.Utc)
														};

												// Fix bugs of old versions:
												// Sync NextRetryTime to make sure this item can be removed from DB
												item.Task.NextRetryTime = item.NextRunTime;
												return item;
											}
										}).ToList();
		}

		public void Add(DateTime key, IRetryTask task)
		{
			if (!task.GetType().IsSerializable)
				throw new ArgumentException(task +
				                            " is not serializable. Not adding [Serializable] attribute in front of the class?");

			var formatter = new BinaryFormatter();
			using (var m = new MemoryStream())
			{
				formatter.Serialize(m, task);
				RetryQueueCollection.Instance.Save(new GenericData
				                                   	{
				                                   		Id = key.ToUniversalTime().Ticks.ToString(),
				                                   		Data = m.ToArray()
				                                   	});
			}
		}

		public bool Remove(DateTime key)
		{
			var result = RetryQueueCollection.Instance.Remove(
				Query.EQ("_id", key.ToUniversalTime().Ticks.ToString()));

			return result.DocumentsAffected > 0;
		}

		#endregion
	}
}