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
		private Queue<SimpleTask> callbacks = new Queue<SimpleTask>();
		private HashSet<string> keys = new HashSet<string>();

		public event EventHandler Enqueued;


		public void Enqueue(WaitCallback cb, object state)
		{
			try
			{
				ResourceDownloadEventArgs arg = (ResourceDownloadEventArgs)state;

				string key = arg.attachment.object_id + arg.imagemeta;

				bool enqueued = false;

				lock (callbacks)
				{
					if (keys.Add(key))
					{
						callbacks.Enqueue(new SimpleTask(cb, state));
						enqueued = true;
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

			lock (callbacks)
			{
				return callbacks.Dequeue();
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
