using System;
using System.Diagnostics;

namespace Wammer.Station
{
	public class TaskRunner<T> : AbstrackTaskRunner where T : ITask
	{
#if DEBUG
		private static object lockObj = new Object();
		private static int TotalExecutedTaskCount { get; set; }
#endif

		private readonly ITaskDequeuable<T> queue;

		public TaskRunner(ITaskDequeuable<T> queue)
		{
			this.queue = queue;
		}

		public TaskRunner(ITaskDequeuable<T> queue, Exit exit)
		{
			this.exit = exit;
			this.queue = queue;
		}

		public event EventHandler TaskExecuted;

		protected override void Do()
		{
			while (!exit.GoExit)
			{
				DequeuedTask<T> item = null;
				try
				{
					item = queue.Dequeue();

					Debug.Assert(item != null);

					if (item == null)
					{
						this.LogDebugMsg("Dequeue an empty item!");
						continue;
					}

					item.Task.Execute();

					if (queue.IsPersistenceQueue)
						queue.AckDequeue(item);

					OnTaskExecuted(EventArgs.Empty);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Error while executing task.", e);
					if (!queue.IsPersistenceQueue && item != null)
						OnTaskExecuted(EventArgs.Empty);
				}
			}
		}

		public override void StopAsync()
		{
			exit.GoExit = true;
			queue.EnqueueDummyTask(); // enqueue something to force task runner leave the blocking call of queue.Dequeue();
		}


		private void OnTaskExecuted(EventArgs arg)
		{
#if DEBUG
			lock (lockObj)
			{
				++TotalExecutedTaskCount;
			}
#endif

			EventHandler handler = TaskExecuted;
			if (handler != null)
				handler(this, arg);
		}
	}
}