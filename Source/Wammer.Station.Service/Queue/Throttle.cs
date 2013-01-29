using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;

namespace Wammer.Queue
{
	public abstract class Throttle
	{
		private Queue<ITask> _tasks;
		private int maxValue;
		private int running = 0;
		private object cs = new object();

		public IThrottleDest Dest { get; set; }

		public abstract bool ShouldThrottle(ITask task);

		private Queue<ITask> m_Tasks
		{
			get { return _tasks ?? (_tasks = new Queue<ITask>()); }
		}

		protected Throttle(int maxValue)
		{
			this.maxValue = maxValue;
		}

		public void Eat(ITask task, TaskPriority pri)
		{
			lock (cs)
			{
				if (running < maxValue)
				{
					Dest.NoThrottleEnqueue(new ThrottleTask(task, this), pri);
					running++;
				}
				else
				{
					m_Tasks.Enqueue(task);
				}
			}
		}

		public void AdvanceThrottle()
		{
			ITask task = null;

			lock (cs)
			{
				running--;

				if (running < maxValue && m_Tasks.Any())
				{
					task = new ThrottleTask(m_Tasks.Dequeue(), this);
					running++;
				}
			}

			if (task != null)
				Dest.NoThrottleEnqueue(task, TaskPriority.Medium);
		}
	}


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
