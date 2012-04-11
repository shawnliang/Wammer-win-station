using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station
{
	interface ITaskStore
	{
		void Enqueue(WaitCallback cb, object state);
	}

	interface ITaskSource
	{
		ITask Dequeue();
	}

	class BodySyncTaskQueue: ITaskSource, ITaskStore
	{
		private Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private Queue<SimpleTask> highPriorityCallbacks = new Queue<SimpleTask>();
		private Queue<SimpleTask> lowPriorityCallbacks = new Queue<SimpleTask>();
		private HashSet<string> keys = new HashSet<string>();

		public event EventHandler Enqueued;


		public void Enqueue(WaitCallback cb, object state)
		{
			try
			{
				ResourceDownloadEventArgs arg = (ResourceDownloadEventArgs)state;

				string key = arg.attachment.object_id + arg.imagemeta;

				bool enqueued = false;

				if (arg.imagemeta == Model.ImageMeta.Small || arg.imagemeta == Model.ImageMeta.Medium)
				{
					lock (highPriorityCallbacks)
					{
						if (keys.Add(key))
						{
							highPriorityCallbacks.Enqueue(new SimpleTask(cb, state));
							enqueued = true;
						}
					}
				}
				else
				{
					lock (lowPriorityCallbacks)
					{
						if (keys.Add(key))
						{
							lowPriorityCallbacks.Enqueue(new SimpleTask(cb, state));
							enqueued = true;
						}
					}
				}

				if (enqueued)
				{
					hasItem.Release();
					OnEnqueued(EventArgs.Empty);
				}
			}
			catch (InvalidCastException e)
			{
				throw new ArgumentException("state is not a ResourceDownloadEventArgs", e);
			}
		}

		public ITask Dequeue()
		{
			hasItem.WaitOne();

			SimpleTask task;
			lock (highPriorityCallbacks)
			{
				if (highPriorityCallbacks.Count > 0)
				{
					task = highPriorityCallbacks.Dequeue();
				}
				else
				{
					lock (lowPriorityCallbacks)
					{
						task = lowPriorityCallbacks.Dequeue();
					}
				}
			}

			return task;
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
