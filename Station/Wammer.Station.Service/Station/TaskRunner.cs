using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Wammer.Station
{
	class TaskRunner
	{
		private static ILog logger = LogManager.GetLogger("TaskRunner");
		private Thread thread;
		private BodySyncTaskQueue queue;
		private volatile bool exit;

		public event EventHandler TaskExecuted;

		public TaskRunner(BodySyncTaskQueue queue)
		{
			this.queue = queue;
			this.thread = new Thread(Do);
			this.exit = false;
		}

		public void Start()
		{
			thread.Start();
		}

		public void Stop()
		{
			exit = true;
			if (!thread.Join(5000))
				thread.Abort();
		}

		private void Do()
		{
			while (!exit)
			{
				try
				{
					ITask task = queue.Dequeue();
					task.Execute();
				}
				catch (Exception e)
				{
					logger.Warn(e);
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
