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
			get { return _instance; }
		}
		#endregion

		private readonly Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private readonly HashSet<string> keys = new HashSet<string>();
		private readonly Queue<IResourceDownloadTask> lowPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> mediumPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> highPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly BodySyncQueueStorage storage = new BodySyncQueueStorage();

		//public event EventHandler TaskDropped;

		#region Constructor
		private BodySyncQueue()
		{
		}

		static BodySyncQueue()
		{
			_instance = new BodySyncQueue();
		}
		#endregion

		#region ITaskDequeuable<IResourceDownloadTask> Members

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
				OnTaskDequeued(dequeued);
				return new DequeuedTask<IResourceDownloadTask>(dequeued, dequeued.Name);
			}
		}

		public void AckDequeue(DequeuedTask<IResourceDownloadTask> task)
		{
			storage.Remove(task.Task);
			keys.Remove(task.Task.Name);
		}

		public void EnqueueDummyTask()
		{
			Enqueue(new DummyResourceDownloadTask(), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<IResourceDownloadTask> Members

		public void Enqueue(IResourceDownloadTask task, TaskPriority priority)
		{
			enqueue(task, priority, false);
		}

		public void EnqueueAlways(ResourceDownloadTask task, TaskPriority priority)
		{
			enqueue(task, priority, true);
		}

		private void enqueue(IResourceDownloadTask task, TaskPriority priority, bool noCheckDuplicate)
		{
			string taskName = task.Name;
			lock (keys)
			{
				if (keys.Add(taskName) || noCheckDuplicate)
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
					storage.Save(task, priority);

					OnTaskEnqueued(task);
					hasItem.Release();
				}
			}
		}
		#endregion

		public void Init()
		{
			foreach (var task in storage.Load(TaskPriority.High))
			{
				Enqueue(task, TaskPriority.High);
			}

			foreach (var task in storage.Load(TaskPriority.Medium))
			{
				Enqueue(task, TaskPriority.High);
			}

			foreach (var task in storage.Load(TaskPriority.Low))
			{
				Enqueue(task, TaskPriority.Medium);
			}

			foreach (var task in storage.Load(TaskPriority.VeryLow))
			{
				Enqueue(task, TaskPriority.Low);
			}
		}

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
					storage.Remove(task);
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
		private string name = Guid.NewGuid().ToString();

		public string Name
		{
			get { return name; }
			set { name = value; }
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
