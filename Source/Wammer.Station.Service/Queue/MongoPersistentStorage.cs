using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Linq;
using Wammer.Model;

namespace Wammer.Queue
{
	public class MongoPersistentStorage : IPersistentStore
	{
		#region IPersistentStore Members

		public WMSQueue TryLoadQueue(string qname)
		{
			MongoCursor<QueuedTask> tasks = QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname));
			List<WMSMessage> msgs = tasks.Select(task => new WMSMessage(task.id, task.Data) { IsPersistent = true }).ToList();

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

		#endregion
	}
}