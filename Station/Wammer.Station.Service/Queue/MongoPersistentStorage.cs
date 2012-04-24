using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using Wammer.Model;
using MongoDB.Driver;

namespace Wammer.Queue
{
	public class MongoPersistentStorage : IPersistentStore
	{
		public WMSQueue TryLoadQueue(string qname)
		{
			List<WMSMessage> msgs = new List<WMSMessage>();

			MongoCursor<QueuedTask> tasks = QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname));
			foreach (var task in tasks)
			{
				msgs.Add(new WMSMessage(task.id, task.Data) { IsPersistent = true });
			}

			return new WMSQueue(qname, this, msgs);
		}

		public void Save(WMSMessage msg)
		{
			QueuedTaskCollection.Instance.Save(
				new QueuedTask { id = msg.Id, queue = msg.Queue.Name, Data = msg.Data });
		}

		public void Remove(WMSMessage msg)
		{
			QueuedTaskCollection.Instance.Remove(Query.EQ("_id", msg.Id));
		}
	}
}
