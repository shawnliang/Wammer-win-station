using System;

namespace Wammer.Station
{
	public class TaskRunner<T> : AbstrackTaskRunner where T : ITask
	{
#if DEBUG
		private static int TotalExecutedTaskCount { get; set; }
#endif

		private readonly ITaskDequeuable<T> queue;

		public TaskRunner(ITaskDequeuable<T> queue)
		{
			this.queue = queue;
		}

		public event EventHandler TaskExecuted;

		protected override void Do()
		{
			while (!exit)
			{
				try
				{
					var item = queue.Dequeue();
					item.Task.Execute();

					if (queue.IsPersistenceQueue)
						queue.AckDequeue(item);

					OnTaskExecuted(EventArgs.Empty);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Error while executing task.", e);
					if (!queue.IsPersistenceQueue)
						OnTaskExecuted(EventArgs.Empty);
				}
			}
		}

		public override void StopAsync()
		{
			exit = true;
			queue.EnqueueDummyTask(); // enqueue something to force task runner leave the blocking call of queue.Dequeue();
		}


		private void OnTaskExecuted(EventArgs arg)
		{
#if DEBUG
			++TotalExecutedTaskCount;
#endif

			EventHandler handler = TaskExecuted;
			if (handler != null)
				handler(this, arg);
		}
	}
}