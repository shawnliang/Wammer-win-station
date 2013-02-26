using System.Collections.Generic;
using System.Threading;

namespace Wammer.Station.Import
{
	class ImportTaskQueue : ITaskEnqueuable<ImportTask>, ITaskDequeuable<ImportTask>
	{
		private Queue<ImportTask> queue = new Queue<ImportTask>();
		private object cs = new object();
		private Semaphore sem = new Semaphore(0, int.MaxValue);


		private static ImportTaskQueue instance;


		static ImportTaskQueue()
		{
			instance = new ImportTaskQueue();
		}

		private ImportTaskQueue()
		{
		}

		public static ImportTaskQueue Instance
		{
			get { return instance; }
		}

		public void Enqueue(ImportTask task, TaskPriority priority)
		{
			lock (cs)
			{
				queue.Enqueue(task);
			}

			sem.Release();
		}

		public DequeuedTask<ImportTask> Dequeue()
		{
			sem.WaitOne();

			lock (cs)
			{
				var task = queue.Dequeue();
				return new DequeuedTask<ImportTask>(task, task.TaskId.ToString());
			}
		}

		public void AckDequeue(DequeuedTask<ImportTask> task)
		{
		}

		public void EnqueueDummyTask()
		{
			Enqueue(new ImportTask("", "", "", new List<string>(), true), TaskPriority.VeryLow);
		}
	}
}
