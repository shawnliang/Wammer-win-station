using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Timeline
{
	class BodySyncQueueStorage
	{
		private const string HIGH_QUEUE = "down_high";
		private const string MEDIUM_QUEUE = "down_midum";
		private const string LOW_QUEUE = "down_low";
		private const string VERY_LOW_QUEUE = "down_verylow";

		public void Save(IResourceDownloadTask task, TaskPriority pri)
		{
			QueuedTaskCollection.Instance.Save(new QueuedTask { id = "bodySync" + task.Name, queue = getQueueName(pri), Data = task });
		}

		public void Remove(IResourceDownloadTask task)
		{
			QueuedTaskCollection.Instance.Remove(Query.EQ("_id", "bodySync" + task.Name));
		}

		private string getQueueName(TaskPriority pri)
		{
			if (pri == TaskPriority.High)
				return HIGH_QUEUE;
			else if (pri == TaskPriority.Medium)
				return MEDIUM_QUEUE;
			else if (pri == TaskPriority.Low)
				return LOW_QUEUE;
			else
				return VERY_LOW_QUEUE;
		}

		internal IEnumerable<IResourceDownloadTask> Load(TaskPriority taskPriority)
		{
			var queueName = getQueueName(taskPriority);
			return QueuedTaskCollection.Instance.Find(Query.EQ("queue", queueName)).Select(x=>(IResourceDownloadTask)x.Data);
		}
	}
}
