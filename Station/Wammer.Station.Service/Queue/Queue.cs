using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Queue
{
	public class WMSQueue
	{
		private LinkedList<WMSMessage> items = new LinkedList<WMSMessage>();
		private Dictionary<Guid, UnackedMsg> popMsgs = new Dictionary<Guid, UnackedMsg>();

		public void Push(WMSMessage msg)
		{
			if (msg == null)
				throw new ArgumentNullException();

			lock (items)
			{
				msg.Queue = this;
				items.AddLast(msg);
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
					new UnackedMsg() { Msg = msg, SessionId = s.Id });
				return msg;
			}
		}

		public void ConfirmPop(WMSMessage msg)
		{
			lock (items)
			{
				if (!popMsgs.Remove(msg.Id))
					throw new KeyNotFoundException("Msg " + msg.Id.ToString() + " is never popped before");
			}
		}

		public void Restock(WMSSession session)
		{
			lock (items)
			{

				List<Guid> KeysToRemove = new List<Guid>();

				foreach (KeyValuePair<Guid, UnackedMsg> pair in popMsgs)
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


	class UnackedMsg
	{
		public WMSMessage Msg { get; set; }
		public Guid SessionId { get; set; }
	}
}
