using System;
using System.Collections.Generic;
using System.Threading;

namespace Wammer.Station.Timeline
{
	public class BodySyncQueue : AbstractTaskEnDequeueNotifier, ITaskEnqueuable<IResourceDownloadTask>, ITaskDequeuable<IResourceDownloadTask>
	{
		#region Var
		private static BodySyncQueue _instance;
		#endregion

		#region Property
		public static BodySyncQueue Instance
		{
			get { return _instance ?? (_instance = new BodySyncQueue()); }
		}
		#endregion

#if DEBUG
		private int TotalTaskCount { get; set; }
		private int TotalDroppedTaskCount { get; set; }
#endif
		private readonly Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private readonly HashSet<string> keys = new HashSet<string>();
		private readonly Queue<IResourceDownloadTask> lowPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> mediumPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> highPriorityQueue = new Queue<IResourceDownloadTask>();


		//public event EventHandler TaskDropped;

		#region Constructor
		private BodySyncQueue()
		{
		}
		#endregion

		#region ITaskDequeuable<IResourceDownloadTask> Members

		public bool IsPersistenceQueue
		{
			get { return false; }
		}

		public DequeuedTask<IResourceDownloadTask> Dequeue()
		{
			hasItem.WaitOne();

			lock (keys)
			{
				IResourceDownloadTask dequeued = null;

				if (highPriorityQueue.Count > 0)
				{
					dequeued = highPriorityQueue.Dequeue();
				}
				else if (mediumPriorityQueue.Count > 0)
				{
					dequeued = mediumPriorityQueue.Dequeue();
				}
				else
					dequeued = lowPriorityQueue.Dequeue();


				if (dequeued == null)
					return null;
				keys.Remove(dequeued.Name);
				OnTaskDequeued(dequeued);
				return new DequeuedTask<IResourceDownloadTask>(dequeued, dequeued.Name);
			}
		}

		public void AckDequeue(DequeuedTask<IResourceDownloadTask> task)
		{
			// This is not a persistent queue so that 
			// we don't need to implement a this method
		}

		public void EnqueueDummyTask()
		{
			Enqueue(new DummyResourceDownloadTask(), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<IResourceDownloadTask> Members

		public void Enqueue(IResourceDownloadTask task, TaskPriority priority)
		{
			string taskName = task.Name;
			lock (keys)
			{
				if (keys.Add(taskName))
				{
					Queue<IResourceDownloadTask> queue = null;

					switch (priority)
					{
						case TaskPriority.High:
							queue = highPriorityQueue;
							break;
						case TaskPriority.Medium:
							queue = mediumPriorityQueue;
							break;
						default:
							queue = lowPriorityQueue;
							break;
					}

					queue.Enqueue(task);

					OnTaskEnqueued(task);
					hasItem.Release();
				}
			}
		}

		#endregion

		public void RemoveAllByUserId(string user_id)
		{
			lock (keys)
			{
				RemoveUserTasksFromQueue(user_id, highPriorityQueue);
				RemoveUserTasksFromQueue(user_id, mediumPriorityQueue);
				RemoveUserTasksFromQueue(user_id, lowPriorityQueue);
			}
		}

		private void RemoveUserTasksFromQueue(string user_id, Queue<IResourceDownloadTask> oldQueue)
		{
			var newQueue = new Queue<IResourceDownloadTask>();
			foreach (var task in oldQueue)
			{
				if (!task.UserId.Equals(user_id))
					newQueue.Enqueue(task);
				else
				{
					keys.Remove(task.Name);
					hasItem.WaitOne();
					OnTaskDequeued(task);
				}
			}

			if (oldQueue.Count != newQueue.Count)
			{
				oldQueue.Clear();
				foreach (var task in newQueue)
					oldQueue.Enqueue(task);
			}
		}

	}


	internal class DummyResourceDownloadTask : IResourceDownloadTask
	{
		private readonly string name = Guid.NewGuid().ToString();

		public string Name
		{
			get { return name; }
		}

		public string UserId
		{
			get { return string.Empty; }
		}

		public void Execute()
		{
		}
	}
}
