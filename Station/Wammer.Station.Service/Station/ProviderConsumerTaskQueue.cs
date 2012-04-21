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

	public class BodySyncTaskQueue: ITaskStore
	{
		private Semaphore hasItem = new Semaphore(0, int.MaxValue);
		private Queue<SimpleTask> highPriorityCallbacks = new Queue<SimpleTask>();
		private Queue<SimpleTask> mediumPriorityCallbacks = new Queue<SimpleTask>();
		private Queue<SimpleTask> lowPriorityCallbacks = new Queue<SimpleTask>();
		private HashSet<string> keys = new HashSet<string>();

		public event EventHandler Enqueued;


		public void Enqueue(WaitCallback cb, object state)
		{
			try
			{
				ResourceDownloadEventArgs arg = (ResourceDownloadEventArgs)state;

				string key = arg.attachment.object_id + arg.imagemeta;

				if (keys.Add(key))
				{
					if (arg.imagemeta == Model.ImageMeta.Small || arg.imagemeta == Model.ImageMeta.Medium)
					{
						lock (highPriorityCallbacks)
						{
							highPriorityCallbacks.Enqueue(new SimpleTask(cb, state));
							OnEnqueued(EventArgs.Empty);
							hasItem.Release();
						}
					}
					if (arg.imagemeta == Model.ImageMeta.Large || arg.imagemeta == Model.ImageMeta.Square)
					{
						lock (mediumPriorityCallbacks)
						{
							mediumPriorityCallbacks.Enqueue(new SimpleTask(cb, state));
							OnEnqueued(EventArgs.Empty);
							hasItem.Release();
						}
					}
					else
					{
						lock (lowPriorityCallbacks)
						{
							lowPriorityCallbacks.Enqueue(new SimpleTask(cb, state));
							OnEnqueued(EventArgs.Empty);
							hasItem.Release();
						}
					}
				}
			}
			catch (InvalidCastException e)
			{
				throw new ArgumentException("state is not a ResourceDownloadEventArgs", e);
			}
		}

		public SimpleTask Dequeue()
		{
			hasItem.WaitOne();

			lock (highPriorityCallbacks)
			{
				if (highPriorityCallbacks.Count > 0)
				{
					return highPriorityCallbacks.Dequeue();
				}
			}

			lock (mediumPriorityCallbacks)
			{
				if (mediumPriorityCallbacks.Count > 0)
				{
					return mediumPriorityCallbacks.Dequeue();
				}
			}

			lock (lowPriorityCallbacks)
			{
				return lowPriorityCallbacks.Dequeue();
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
