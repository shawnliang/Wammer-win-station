using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station.Timeline
{
	public class BodySyncQueue : ITaskEnqueuable<IResourceDownloadTask>, ITaskDequeuable<IResourceDownloadTask>
	{
		private readonly Semaphore hasItem = new Semaphore(0, int.MaxValue);
		
		private readonly HashSet<string> keys = new HashSet<string>();
		private readonly Queue<IResourceDownloadTask> lowPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> mediumPriorityQueue = new Queue<IResourceDownloadTask>();
		private readonly Queue<IResourceDownloadTask> highPriorityQueue = new Queue<IResourceDownloadTask>();

		public event EventHandler TaskDropped;

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
				keys.Remove(dequeued.Name);
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

		private void RemoveUserTasksFromQueue(string user_id, Queue<IResourceDownloadTask> oldQueue)
		{
			Queue<IResourceDownloadTask> newQueue = new Queue<IResourceDownloadTask>();
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


	internal class DummyResourceDownloadTask: IResourceDownloadTask
	{

		public string Name
		{
			get { return string.Empty; }
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
