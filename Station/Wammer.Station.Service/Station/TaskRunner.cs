using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class TaskRunner : AbstrackTaskRunner
	{
		private ITaskDequeuable queue;

		public event EventHandler TaskExecuted;

		public TaskRunner(ITaskDequeuable queue)
		{
			this.queue = queue;
		}

		protected override void Do()
		{
			while (!exit)
			{
				try
				{
					DequeuedTask item = queue.Dequeue();
					item.Task.Execute();
					queue.AckDequeue(item);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Error while executing task.", e);
				}
				finally
				{
					OnTaskExecuted(EventArgs.Empty);
				}
			}
		}

		private void OnTaskExecuted(EventArgs arg)
		{
			EventHandler handler = TaskExecuted;
			if (handler != null)
				handler(this, arg);
		}
	}
}
