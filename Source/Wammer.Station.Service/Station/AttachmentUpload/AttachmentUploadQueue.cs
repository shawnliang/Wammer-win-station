using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Wammer.Model;

namespace Wammer.Station.AttachmentUpload
{
	internal class AttachmentUploadQueue : AbstractTaskEnDequeueNotifier, ITaskEnqueuable<ITask>, ITaskDequeuable<ITask>
	{
		public const string QNAME_HIGH = "attUpl_high";
		public const string QNAME_MED = "attUpl_med";
		public const string QNAME_LOW = "attUpl_low";

		private readonly object csLock = new object();
		bool isInited = false;
		private Semaphore hasItem;
		private Queue<DequeuedTask<ITask>> highQueue;
		private Queue<DequeuedTask<ITask>> lowQueue;
		private Queue<DequeuedTask<ITask>> mediumQueue;
		private readonly AttachmentUploadQueuePersistentStorage storage;

		public bool IsPersistenceQueue
		{
			get { return true; }
		}

		public AttachmentUploadQueue()
		{
			storage = new AttachmentUploadQueuePersistentStorage();
		}


		public void Init()
		{
			lock (csLock)
			{
				highQueue = storage.Load(QNAME_HIGH);
				mediumQueue = storage.Load(QNAME_MED);
				lowQueue = storage.Load(QNAME_LOW);

				foreach (var t in highQueue)
				{
					OnTaskEnqueued(t.Task);
				}
				foreach (var t in mediumQueue)
				{
					OnTaskEnqueued(t.Task);
				}
				foreach (var t in lowQueue)
				{
					OnTaskEnqueued(t.Task);
				}

				hasItem = new Semaphore(highQueue.Count + mediumQueue.Count + lowQueue.Count, int.MaxValue);
				isInited = true;
			}
		}


		#region ITaskDequeuable<ITask> Members

		public DequeuedTask<ITask> Dequeue()
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");


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
				OnTaskDequeued(deqTask.Task);
				return deqTask;
			}
		}

		public void AckDequeue(DequeuedTask<ITask> item)
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

			storage.Remove(item);
		}


		public void EnqueueDummyTask()
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

			Enqueue(new NullTask(), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<ITask> Members

		public void Enqueue(ITask task, TaskPriority priority)
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

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
			OnTaskEnqueued(task);
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
						id = (Guid)task.Key,
						Data = task.Task,
						queue = qname,
					});
		}

		public void Remove(DequeuedTask<ITask> task)
		{
			QueuedTaskCollection.Instance.Remove(Query.EQ("_id", (Guid)task.Key));
		}

		public Queue<DequeuedTask<ITask>> Load(string qname)
		{
			var queue = new Queue<DequeuedTask<ITask>>();
			foreach (QueuedTask item in QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname)))
			{
				queue.Enqueue(new DequeuedTask<ITask>(item.Data, item.id));
			}

			return queue;
		}
	}
}