using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class BodySyncTaskRunner : TaskRunner
	{
		private BodySyncTaskQueue queue;

		public event EventHandler TaskExecuted;

		public BodySyncTaskRunner(BodySyncTaskQueue queue)
		{
			this.queue = queue;
		}

		protected override void Do()
		{
			while (!exit)
			{
				try
				{
					SimpleTask task = queue.Dequeue();
					task.Execute();
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
