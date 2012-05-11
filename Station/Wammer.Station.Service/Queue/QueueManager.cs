using System;
using System.Collections.Generic;

namespace Wammer.Queue
{

	public interface IPersistentStore
	{
		WMSQueue TryLoadQueue(string qname);
		void Save(WMSMessage msg);
		void Remove(WMSMessage msg);
	}

	public class NullPersistentStore : IPersistentStore
	{
		public WMSQueue TryLoadQueue(string qname)
		{
			return new WMSQueue(qname, this);
		}

		public void Save(WMSMessage msg)
		{
		}

		public void Remove(WMSMessage msg)
		{
		}
	}

	public class WMSBroker
	{
		private readonly Dictionary<String, WMSQueue> queues = new Dictionary<string, WMSQueue>();
		private readonly IPersistentStore persistentStore;

		public WMSBroker()
		{
			this.persistentStore = new NullPersistentStore();
		}

		public WMSBroker(IPersistentStore persistentStore)
		{
			if (persistentStore == null)
				throw new ArgumentNullException("persistentStore");

			this.persistentStore = persistentStore;
		}

		public WMSQueue GetQueue(string name)
		{
			if (queues.ContainsKey(name))
				return queues[name];

			queues[name] = persistentStore.TryLoadQueue(name);
			return queues[name];
		}

		public WMSSession CreateSession()
		{
			return new WMSSession();
		}
	}

	public class WMSSession : IDisposable
	{
		private readonly HashSet<WMSQueue> popQueues = new HashSet<WMSQueue>();
		public Guid Id { get; private set; }
		
		
		public WMSSession()
		{
			Id = Guid.NewGuid();
		}

		public WMSMessage Pop(WMSQueue queue)
		{
			lock (popQueues)
			{
				popQueues.Add(queue);
			}
			return queue.Pop(this);
		}

		public void Push(WMSQueue queue, object data)
		{
			var msg = new WMSMessage() { Data = data };
			queue.Push(msg, false);
		}

		public void Push(WMSQueue queue, object data, bool persistent)
		{
			var msg = new WMSMessage() { Data = data };
			queue.Push(msg, persistent);
		}

		public void Close()
		{
			foreach (WMSQueue q in popQueues)
			{
				q.Restock(this);
			}
		}

		public void Dispose()
		{
			Close();
		}
	}


	public class WMSMessage
	{
		public object Data { get; set; }
		public Guid Id { get; private set; }
		public WMSQueue Queue { get; set; }
		public bool IsPersistent { get; set; }

		public WMSMessage()
		{
			Id = Guid.NewGuid();
		}
		
		public WMSMessage(object data)
		{
			Id = Guid.NewGuid();
			this.Data = data;
		}

		public WMSMessage(Guid id, object data)
		{
			this.Id = id;
			this.Data = data;
		}

		public void Acknowledge()
		{
			if (Queue == null)
				throw new InvalidOperationException("This message is not popped from a Queue.");

			Queue.ConfirmPop(this);
		}
	}
}
