using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station.Timeline
{
	public class BodySyncQueue : ITaskEnqueuable<ResourceDownloadTask>, ITaskDequeuable<ResourceDownloadTask>
	{
		private readonly Semaphore hasItem = new Semaphore(0, int.MaxValue);
		
		private readonly HashSet<string> keys = new HashSet<string>();
		private readonly Queue<ResourceDownloadTask> lowPriorityQueue = new Queue<ResourceDownloadTask>();
		private readonly Queue<ResourceDownloadTask> mediumPriorityQueue = new Queue<ResourceDownloadTask>();
		private readonly Queue<ResourceDownloadTask> highPriorityQueue = new Queue<ResourceDownloadTask>();

		public event EventHandler TaskDropped;

		#region ITaskDequeuable<INamedTask> Members

		public DequeuedTask<ResourceDownloadTask> Dequeue()
		{
			hasItem.WaitOne();

			lock (keys)
			{
				ResourceDownloadTask dequeued = null;

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
				return new DequeuedTask<ResourceDownloadTask>(dequeued, dequeued.Name);
			}
		}

		public void AckDequeue(DequeuedTask<ResourceDownloadTask> task)
		{
			// This is not a persistent queue so that 
			// we don't need to implement a this method
		}

		public void EnqueueDummyTask()
		{
			Enqueue(new ResourceDownloadTask(null, null, TaskPriority.High), TaskPriority.High);
		}

		#endregion

		#region ITaskEnqueuable<INamedTask> Members

		public void Enqueue(ResourceDownloadTask task, TaskPriority priority)
		{
			string taskName = task.Name;
			lock (keys)
			{
				if (keys.Add(taskName))
				{
					Queue<ResourceDownloadTask> queue = null;

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
					OnEnqueued(EventArgs.Empty);
					hasItem.Release();
				}
			}
		}

		#endregion

		public event EventHandler Enqueued;

		private void OnEnqueued(EventArgs arg)
		{
			EventHandler handler = Enqueued;
			if (handler != null)
			{
				handler(this, arg);
			}
		}

		public void RemoveAllByUserId(string user_id)
		{
			lock (highPriorityQueue)
			{
				RemoveUserTasksFromQueue(user_id, highPriorityQueue);
			}

			lock (mediumPriorityQueue)
			{
				RemoveUserTasksFromQueue(user_id, mediumPriorityQueue);
			}

			lock (lowPriorityQueue)
			{
				RemoveUserTasksFromQueue(user_id, lowPriorityQueue);
			}
		}

		private void RemoveUserTasksFromQueue(string user_id, Queue<ResourceDownloadTask> oldQueue)
		{
			Queue<ResourceDownloadTask> newQueue = new Queue<ResourceDownloadTask>();
			foreach (var task in oldQueue)
			{
				if (!task.UserId.Equals(user_id))
					newQueue.Enqueue(task);
				else
				{
					keys.Remove(task.Name);
					hasItem.WaitOne();
					OnTaskDropped();
				}
			}

			if (oldQueue.Count != newQueue.Count)
			{
				oldQueue.Clear();
				foreach (var task in newQueue)
					oldQueue.Enqueue(task);
			}
		}

		private void OnTaskDropped()
		{
			EventHandler handler = TaskDropped;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
