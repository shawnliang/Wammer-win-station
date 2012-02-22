using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Queue
{
	public class WMSBroker
	{
		private Dictionary<String, WMSQueue> queues;

		public WMSBroker()
		{
			queues = new Dictionary<string, WMSQueue>();
		}

		public WMSQueue GetQueue(string name)
		{
			if (!queues.ContainsKey(name))
				queues[name] = new WMSQueue();

			return queues[name];
		}

		public WMSSession CreateSession()
		{
			return new WMSSession();
		}
	}

	public class WMSSession : IDisposable
	{
		private HashSet<WMSQueue> popQueues = new HashSet<WMSQueue>();
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
			WMSMessage msg = new WMSMessage() { Data = data };
			queue.Push(msg);
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


		public WMSMessage()
		{
			Id = Guid.NewGuid();
		}

		public void Acknowledge()
		{
			if (Queue == null)
				throw new InvalidOperationException("This message is not popped from a Queue.");

			Queue.ConfirmPop(this);
		}
	}
}
