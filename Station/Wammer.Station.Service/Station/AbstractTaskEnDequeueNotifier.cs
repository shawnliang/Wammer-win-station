using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public abstract class AbstractTaskEnDequeueNotifier
	{
		public event EventHandler<TaskEventArgs> TaskEnqueued;
		public event EventHandler<TaskEventArgs> TaskDequeued;

		protected AbstractTaskEnDequeueNotifier()
		{
		}

		protected void OnTaskEnqueued(ITask task)
		{
			EventHandler<TaskEventArgs> handler = TaskEnqueued;
			if (handler != null)
				handler(this, new TaskEventArgs(task));
		}

		protected void OnTaskDequeued(ITask task)
		{
			EventHandler<TaskEventArgs> handler = TaskDequeued;
			if (handler != null)
				handler(this, new TaskEventArgs(task));
		}
	}
}
