using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wammer.Station
{

	class ProviderConsumerTaskQueue
	{
		private Semaphore hasItem = new Semaphore(0, int.MaxValue);
		Queue<SimpleTask> callbacks = new Queue<SimpleTask>();

		public ProviderConsumerTaskQueue()
		{
		}

		public void Enqueue(WaitCallback cb, object state)
		{
			lock (callbacks)
			{
				callbacks.Enqueue(new SimpleTask(cb, state));
			}

			hasItem.Release();
		}

		public SimpleTask Dequeue()
		{
			hasItem.WaitOne();

			lock (callbacks)
			{
				return callbacks.Dequeue();
			}
		}
	}
}
