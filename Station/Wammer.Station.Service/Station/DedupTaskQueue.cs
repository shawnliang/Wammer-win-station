using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station
{

	interface INamedTask : ITask
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

	public class DequeuedTask
	{
		public DequeuedTask(ITask t, object key)
		{
			this.Task = t;
			this.Key = key;
		}

		public ITask Task { get; private set; }
		public object Key { get; private set; }
	}


	public interface ITaskEnqueuable
	{
		void Enqueue(ITask task, TaskPriority priority);
	}

	public interface ITaskDequeuable
	{
		DequeuedTask Dequeue();
		void AckDequeue(DequeuedTask task);
	}

	public class DedupTaskQueue: ITaskEnqueuable, ITaskDequeuable
	{
		private Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private Queue<INamedTask> highPriorityCallbacks = new Queue<INamedTask>();
		private Queue<INamedTask> mediumPriorityCallbacks = new Queue<INamedTask>();
		private Queue<INamedTask> lowPriorityCallbacks = new Queue<INamedTask>();
		private HashSet<string> keys = new HashSet<string>();

		public event EventHandler Enqueued;

		public void Enqueue(ITask task, TaskPriority priority)
		{
			try
			{
				string taskName = ((INamedTask)task).Name;
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

						queue.Enqueue((INamedTask)task);
						OnEnqueued(EventArgs.Empty);
						hasItem.Release();
					}
				}
			}
			catch (InvalidCastException e)
			{
				throw new ArgumentException("task is not an INamedTask", e);
			}
		}

		public DequeuedTask Dequeue()
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
					return new DequeuedTask(dequeued, dequeued.Name);
			}
		}

		public void AckDequeue(DequeuedTask task)
		{
			lock (keys)
			{
				keys.Remove((string)task.Key);
			}
		}

		private void OnEnqueued(EventArgs arg)
		{
			EventHandler handler = Enqueued;
			if (handler != null)
			{
				handler(this, arg);
			}
		}
	}
}
