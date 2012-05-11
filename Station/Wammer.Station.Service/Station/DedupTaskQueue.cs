using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station
{

	public interface INamedTask : ITask
	{
		string Name { get; }
	}

	class NamedTask: SimpleTask, INamedTask
	{
		public NamedTask(WaitCallback cb, object state, string name)
			:base(cb, state)
		{
			if (cb == null)
				throw new ArgumentNullException("cb");

			if (name == null)
				throw new ArgumentNullException("name");

			this.Name = name;
		}

		public string Name { get; set; }
	}

	public class DequeuedTask<T> where T : ITask
	{
		public DequeuedTask(T t, object key)
		{
			this.Task = t;
			this.Key = key;
		}

		public T Task { get; private set; }
		public object Key { get; private set; }
	}


	public interface ITaskEnqueuable<in T> where T : ITask
	{
		void Enqueue(T task, TaskPriority priority);
	}

	public interface ITaskDequeuable<T> where T :ITask
	{
		DequeuedTask<T> Dequeue();
		void AckDequeue(DequeuedTask<T> task);
		void EnqueueDummyTask();
	}

	public class DedupTaskQueue : ITaskEnqueuable<INamedTask>, ITaskDequeuable<INamedTask>
	{
		private readonly Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private readonly Queue<INamedTask> highPriorityCallbacks = new Queue<INamedTask>();
		private readonly Queue<INamedTask> mediumPriorityCallbacks = new Queue<INamedTask>();
		private readonly Queue<INamedTask> lowPriorityCallbacks = new Queue<INamedTask>();
		private readonly HashSet<string> keys = new HashSet<string>();

		public event EventHandler Enqueued;

		public void Enqueue(INamedTask task, TaskPriority priority)
		{
			string taskName = task.Name;
			lock (keys)
			{
				if (keys.Add(taskName))
				{
					Queue<INamedTask> queue = null;

					switch (priority)
					{
						case TaskPriority.High:
							queue = highPriorityCallbacks;
							break;
						case TaskPriority.Medium:
							queue = mediumPriorityCallbacks;
							break;
						default:
							queue = lowPriorityCallbacks;
							break;
					}

					queue.Enqueue(task);
					OnEnqueued(EventArgs.Empty);
					hasItem.Release();
				}
			}				
		}

		public DequeuedTask<INamedTask> Dequeue()
		{
			hasItem.WaitOne();

			lock (keys)
			{
				INamedTask dequeued = null;

				if (highPriorityCallbacks.Count > 0)
				{
					dequeued = highPriorityCallbacks.Dequeue();
				}
				else if (mediumPriorityCallbacks.Count > 0)
				{
					dequeued = mediumPriorityCallbacks.Dequeue();
				}
				else
					dequeued = lowPriorityCallbacks.Dequeue();

				if (dequeued == null)
					return null;
				else
				{
					keys.Remove(dequeued.Name);
					return new DequeuedTask<INamedTask>(dequeued, dequeued.Name);
				}
			}
		}

		public void AckDequeue(DequeuedTask<INamedTask> task)
		{
			// This is not a persistent queue so that 
			// we don't need to implement a this method
		}

		private void OnEnqueued(EventArgs arg)
		{
			EventHandler handler = Enqueued;
			if (handler != null)
			{
				handler(this, arg);
			}
		}

		public void EnqueueDummyTask()
		{
			Enqueue(new NullNamedTask(), TaskPriority.High);
		}


	}
}
