using System;
using System.Collections.Generic;

namespace Wammer.Queue
{
	public class WMSQueue
	{
		private readonly LinkedList<WMSMessage> items = new LinkedList<WMSMessage>();
		private readonly IPersistentStore persistentStorage;
		private readonly Dictionary<Guid, UnackedMsg> popMsgs = new Dictionary<Guid, UnackedMsg>();

		public WMSQueue(string name, IPersistentStore persistentStorage)
		{
			this.persistentStorage = persistentStorage;
			Name = name;
		}

		public WMSQueue(string name, IPersistentStore persistentStorage, IEnumerable<WMSMessage> initMessages)
		{
			this.persistentStorage = persistentStorage;
			Name = name;

			foreach (WMSMessage msg in initMessages)
			{
				msg.Queue = this;
				items.AddLast(msg);
			}
		}

		public string Name { get; private set; }

		public int Count
		{
			get
			{
				lock (items)
				{
					return items.Count;
				}
			}
		}

		public void Push(WMSMessage msg, bool persistent)
		{
			if (msg == null)
				throw new ArgumentNullException();

			lock (items)
			{
				msg.Queue = this;
				msg.IsPersistent = persistent;

				items.AddLast(msg);

				if (persistent)
					persistentStorage.Save(msg);
			}
		}

		public WMSMessage Pop(WMSSession s)
		{
			lock (items)
			{
				if (items.Count == 0)
					return null;

				WMSMessage msg = items.First.Value;
				items.RemoveFirst();
				popMsgs.Add(msg.Id,
							new UnackedMsg { Msg = msg, SessionId = s.Id });
				return msg;
			}
		}

		public void ConfirmPop(WMSMessage msg)
		{
			lock (items)
			{
				if (!popMsgs.Remove(msg.Id))
					throw new KeyNotFoundException("Msg " + msg.Id.ToString() + " is never popped before");

				if (msg.IsPersistent)
					persistentStorage.Remove(msg);
			}
		}

		public void Restock(WMSSession session)
		{
			lock (items)
			{
				var KeysToRemove = new List<Guid>();

				foreach (var pair in popMsgs)
				{
					if (pair.Value.SessionId == session.Id)
					{
						items.AddLast(pair.Value.Msg);
						KeysToRemove.Add(pair.Key);
					}
				}

				foreach (Guid key in KeysToRemove)
					popMsgs.Remove(key);
			}
		}
	}


	internal class UnackedMsg
	{
		public WMSMessage Msg { get; set; }
		public Guid SessionId { get; set; }
	}
}