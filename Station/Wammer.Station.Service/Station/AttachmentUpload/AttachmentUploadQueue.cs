using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;

namespace Wammer.Station.AttachmentUpload
{
	class AttachmentUploadQueue : ITaskEnqueuable<ITask>, ITaskDequeuable<ITask>
	{
		public const string QNAME_HIGH = "attUpl_high";
		public const string QNAME_MED = "attUpl_med";
		public const string QNAME_LOW = "attUpl_low";

		object csLock = new object();
		Queue<DequeuedTask<ITask>> highQueue;
		Queue<DequeuedTask<ITask>> mediumQueue;
		Queue<DequeuedTask<ITask>> lowQueue;
		AttachmentUploadQueuePersistentStorage storage;


		static AttachmentUploadQueue instance;
		System.Threading.Semaphore hasItem;

		private AttachmentUploadQueue()
		{
			storage = new AttachmentUploadQueuePersistentStorage();
			highQueue = storage.Load(QNAME_HIGH);
			mediumQueue = storage.Load(QNAME_MED);
			lowQueue = storage.Load(QNAME_LOW);

			int initialTaskCount = highQueue.Count + mediumQueue.Count + lowQueue.Count;
			PerfMonitor.PerfCounter.GetCounter(PerfMonitor.PerfCounter.UP_REMAINED_COUNT, false).IncrementBy(initialTaskCount);
			hasItem = new System.Threading.Semaphore(initialTaskCount, int.MaxValue);
		}

		public static AttachmentUploadQueue Instance
		{
			get
			{
				if (instance == null)
					instance = new AttachmentUploadQueue();

				return instance;
			}
		}

		public void Enqueue(ITask task, TaskPriority priority)
		{
			lock (csLock)
			{
				var item = new DequeuedTask<ITask>(task, Guid.NewGuid());
				if (priority == TaskPriority.High)
					highQueue.Enqueue(item);
				else if (priority == TaskPriority.Medium)
					mediumQueue.Enqueue(item);
				else
					lowQueue.Enqueue(item);


				storage.Save(item, priority);
			}

			hasItem.Release();
		}

		public DequeuedTask<ITask> Dequeue()
		{
			hasItem.WaitOne();

			lock (csLock)
			{
				DequeuedTask<ITask> deqTask;

				if (highQueue.Count > 0)
					deqTask = highQueue.Dequeue();
				else if (mediumQueue.Count > 0)
					deqTask = mediumQueue.Dequeue();
				else
					deqTask = lowQueue.Dequeue();

				System.Diagnostics.Trace.Assert(deqTask != null);
				return deqTask;
			}
		}

		public void AckDequeue(DequeuedTask<ITask> task)
		{
			storage.Remove(task);
		}



		public void EnqueueDummyTask()
		{
			PerfMonitor.PerfCounter.GetCounter(PerfMonitor.PerfCounter.UP_REMAINED_COUNT, false).Increment();
			Enqueue(new NullTask(), TaskPriority.High);
		}
	}


	class AttachmentUploadQueuePersistentStorage
	{
		public void Save(DequeuedTask<ITask> task, TaskPriority pri)
		{
			string qname;
			if (pri == TaskPriority.High)
				qname = AttachmentUploadQueue.QNAME_HIGH;
			else if (pri == TaskPriority.Medium)
				qname = AttachmentUploadQueue.QNAME_MED;
			else
				qname = AttachmentUploadQueue.QNAME_LOW;

			Model.QueuedTaskCollection.Instance.Save(
				new Model.QueuedTask
				{
					id = (Guid)task.Key,
					Data = task.Task,
					queue = qname,
				});
		}

		public void Remove(DequeuedTask<ITask> task)
		{
			Model.QueuedTaskCollection.Instance.Remove(Query.EQ("_id", (Guid)task.Key));
		}

		public Queue<DequeuedTask<ITask>> Load(string qname)
		{
			var queue = new Queue<DequeuedTask<ITask>>();
			foreach (var item in Model.QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname)))
			{
				queue.Enqueue(new DequeuedTask<ITask>((ITask)item.Data, item.id));
			}

			return queue;
		}
	}
}
