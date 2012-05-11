using System;

namespace Wammer.Station
{
	public class TaskRunner<T> : AbstrackTaskRunner where T : ITask
	{
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
					DequeuedTask<T> item = queue.Dequeue();
					item.Task.Execute();
					queue.AckDequeue(item);
					OnTaskExecuted(EventArgs.Empty);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Error while executing task.", e);
				}
			}
		}

		public override void Stop()
		{
			exit = true;
			queue.EnqueueDummyTask(); // enqueue something to force task runner leave the blocking call of queue.Dequeue();
			base.Stop();
		}


		private void OnTaskExecuted(EventArgs arg)
		{
			EventHandler handler = TaskExecuted;
			if (handler != null)
				handler(this, arg);
		}
	}
}