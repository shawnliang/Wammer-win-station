using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.PerfMonitor;

namespace Wammer.Station.AttachmentUpload
{
	internal class AttachmentUploadQueue : ITaskEnqueuable<ITask>, ITaskDequeuable<ITask>
	{
		public const string QNAME_HIGH = "attUpl_high";
		public const string QNAME_MED = "attUpl_med";
		public const string QNAME_LOW = "attUpl_low";
		private static AttachmentUploadQueue instance;

		private readonly object csLock = new object();
		private readonly Semaphore hasItem;
		private readonly Queue<DequeuedTask<ITask>> highQueue;
		private readonly Queue<DequeuedTask<ITask>> lowQueue;
		private readonly Queue<DequeuedTask<ITask>> mediumQueue;
		private readonly AttachmentUploadQueuePersistentStorage storage;


		public bool IsPersistenceQueue
		{
			get { return true; }
		}

		private AttachmentUploadQueue()
		{
			storage = new AttachmentUploadQueuePersistentStorage();
			highQueue = storage.Load(QNAME_HIGH);
			mediumQueue = storage.Load(QNAME_MED);
			lowQueue = storage.Load(QNAME_LOW);

			int initialTaskCount = highQueue.Count + mediumQueue.Count + lowQueue.Count;
			PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).IncrementBy(initialTaskCount);
			hasItem = new Semaphore(initialTaskCount, int.MaxValue);
		}

		public static AttachmentUploadQueue Instance
		{
			get { return instance ?? (instance = new AttachmentUploadQueue()); }
		}

		#region ITaskDequeuable<ITask> Members

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

				Trace.Assert(deqTask != null);
				return deqTask;
			}
		}

		public void AckDequeue(DequeuedTask<ITask> task)
		{
			storage.Remove(task);
		}


		public void EnqueueDummyTask()
		{
			PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).Increment();
			Enqueue(new NullTask(), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<ITask> Members

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

		#endregion
	}


	internal class AttachmentUploadQueuePersistentStorage
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

			QueuedTaskCollection.Instance.Save(
				new QueuedTask
					{
						id = (Guid) task.Key,
						Data = task.Task,
						queue = qname,
					});
		}

		public void Remove(DequeuedTask<ITask> task)
		{
			QueuedTaskCollection.Instance.Remove(Query.EQ("_id", (Guid) task.Key));
		}

		public Queue<DequeuedTask<ITask>> Load(string qname)
		{
			var queue = new Queue<DequeuedTask<ITask>>();
			foreach (QueuedTask item in QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname)))
			{
				queue.Enqueue(new DequeuedTask<ITask>((ITask) item.Data, item.id));
			}

			return queue;
		}
	}
}