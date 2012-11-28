using System;
using System.Diagnostics;

namespace Wammer.Station
{
	public class TaskRunner<T> : AbstrackTaskRunner where T : ITask
	{
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
					queue.AckDequeue(item);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Error while executing task.", e);
				}
			}
		}

		public override void StopAsync()
		{
			exit.GoExit = true;
			queue.EnqueueDummyTask(); // enqueue something to force task runner leave the blocking call of queue.Dequeue();
		}
	}
}