using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Wammer.Model;

namespace Wammer.Station.AttachmentUpload
{
	public class AttachmentUploadQueue : AbstractTaskEnDequeueNotifier, ITaskEnqueuable<INamedTask>, ITaskDequeuable<INamedTask>
	{
		public const string QNAME_HIGH = "attUpl_high";
		public const string QNAME_MED = "attUpl_med";
		public const string QNAME_LOW = "attUpl_low";

		private readonly object csLock = new object();
		bool isInited = false;
		private Semaphore hasItem;
		private Queue<DequeuedTask<INamedTask>> highQueue;
		private Queue<DequeuedTask<INamedTask>> lowQueue;
		private Queue<DequeuedTask<INamedTask>> mediumQueue;
		private readonly AttachmentUploadQueuePersistentStorage storage;

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


		#region ITaskDequeuable<INamedTask> Members

		public DequeuedTask<INamedTask> Dequeue()
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");


			hasItem.WaitOne();

			lock (csLock)
			{
				DequeuedTask<INamedTask> deqTask;

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

		public void AckDequeue(DequeuedTask<INamedTask> item)
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

			storage.Remove(item);
		}


		public void EnqueueDummyTask()
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

			Enqueue(new NullNamedTask(), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<ITask> Members

		public void Enqueue(INamedTask task, TaskPriority priority)
		{
			if (!isInited)
				throw new InvalidOperationException("Not inited");

			lock (csLock)
			{
				var item = new DequeuedTask<INamedTask>(task, task.Name);
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

		public object GetCount()
		{
			lock (csLock)
			{
				return (highQueue.Count + mediumQueue.Count + lowQueue.Count).ToString();
			}
		}
	}


	internal class AttachmentUploadQueuePersistentStorage
	{
		public void Save(DequeuedTask<INamedTask> task, TaskPriority pri)
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
						id = task.Key,
						Data = task.Task,
						queue = qname,
					});
		}

		public void Remove(DequeuedTask<INamedTask> task)
		{
			QueuedTaskCollection.Instance.Remove(Query.EQ("_id", task.Key));
		}

		public Queue<DequeuedTask<INamedTask>> Load(string qname)
		{
			var queue = new Queue<DequeuedTask<INamedTask>>();
			foreach (QueuedTask item in QueuedTaskCollection.Instance.Find(Query.EQ("queue", qname)))
			{
				queue.Enqueue(new DequeuedTask<INamedTask>((INamedTask)item.Data, item.id));
			}

			return queue;
		}
	}
}
