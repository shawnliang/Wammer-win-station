using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.PerfMonitor;

namespace Wammer.Station
{
	public static class ThreadPool
	{
		private static IPerfCounter ItemsInQueueCounter;
		private static IPerfCounter ItemsInProgrssCounter;

		static ThreadPool()
		{
			ItemsInProgrssCounter = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);
			ItemsInQueueCounter = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		}

		public static void Enqueue(System.Threading.WaitCallback callback)
		{
			ItemsInQueueCounter.Increment();
			System.Threading.ThreadPool.QueueUserWorkItem(new CallbackWrapper(callback, ItemsInQueueCounter, ItemsInProgrssCounter).Callback);
		}

		public static void Enqueue(System.Threading.WaitCallback callback, object state)
		{
			ItemsInQueueCounter.Increment();
			System.Threading.ThreadPool.QueueUserWorkItem(new CallbackWrapper(callback, ItemsInQueueCounter, ItemsInProgrssCounter).Callback, state);
		}
	}

	class CallbackWrapper
	{
		private System.Threading.WaitCallback callback;
		private IPerfCounter itemsInQueue;
		private IPerfCounter itemsInProgress;

		public CallbackWrapper(System.Threading.WaitCallback callback, IPerfCounter itemsInQueue, IPerfCounter itemsInProgress)
		{
			this.callback = callback;
			this.itemsInQueue = itemsInQueue;
			this.itemsInProgress = itemsInProgress;
		}

		public void Callback(object state)
		{
			this.itemsInQueue.Decrement();
			this.itemsInProgress.Increment();
			this.callback(state);
			this.itemsInProgress.Decrement();
		}
	}
}
