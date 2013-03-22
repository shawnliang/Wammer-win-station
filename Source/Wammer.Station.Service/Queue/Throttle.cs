using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Station;

namespace Wammer.Queue
{
	public abstract class Throttle
	{
		private Queue<WMSMessage> _msgs;

		private int maxValue;
		private int running;
		private object cs = new object();

		public IThrottleDest Dest { get; set; }

		public abstract bool ShouldThrottle(ITask task);

		private Queue<WMSMessage> m_Msgs
		{
			get { return _msgs ?? (_msgs = new Queue<WMSMessage>()); }
		}

		protected Throttle(int maxValue)
		{
			this.maxValue = maxValue;
		}

		public void Eat(WMSMessage msg, TaskPriority pri)
		{
			lock (cs)
			{
				if (running < maxValue)
				{
					msg.Data = new ThrottleTask((ITask)msg.Data, this);
					msg.IsPersistent = false;
					Dest.NoThrottleEnqueue(msg, pri);
					running++;
				}
				else
				{
					m_Msgs.Enqueue(msg);
				}
			}
		}

		public void AdvanceThrottle()
		{
			WMSMessage msg = null;

			lock (cs)
			{
				running--;

				if (running < maxValue && m_Msgs.Any())
				{
					msg = m_Msgs.Dequeue();
					running++;
				}
			}

			if (msg != null)
			{
				msg.Data = new ThrottleTask((ITask)msg.Data, this);
				msg.IsPersistent = false;
				Dest.NoThrottleEnqueue(msg, TaskPriority.Medium);
			}
		}
	}

	[Serializable]
	internal class ThrottleTask : ITask
	{
		private ITask task;
		private Throttle throttle;

		public ThrottleTask(ITask task, Throttle throttle)
		{
			this.task = task;
			this.throttle = throttle;
		}

		public void Execute()
		{
			try
			{
				task.Execute();
			}
			finally
			{
				throttle.AdvanceThrottle();
			}
		}
	}
}
