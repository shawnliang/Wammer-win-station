using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Wammer.PerfMonitor;


namespace Wammer.Station
{

	static class TaskQueue
	{
		private static object lockAllQueue = new object();

		private static LinkedList<Task> HighPriorityQueue = new LinkedList<Task>();
		private static LinkedList<Task> MediumPriorityQueue = new LinkedList<Task>();
		private static LinkedList<Task> LowPriorityQueue = new LinkedList<Task>();
		private static log4net.ILog Logger = log4net.LogManager.GetLogger("TaskQueue");

		private static IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);

		public static void EnqueueHigh(WaitCallback callback, object state)
		{
			Enqueue(HighPriorityQueue, callback, state);
		}

		public static void EnqueueHigh(WaitCallback callback)
		{
			Enqueue(HighPriorityQueue, callback, null); 
		}

		public static void EnqueueMedium(WaitCallback callback, object state)
		{
			Enqueue(MediumPriorityQueue, callback, state);
		}

		public static void EnqueueMedium(WaitCallback callback)
		{
			Enqueue(MediumPriorityQueue, callback, null);
		}

		public static void EnqueueLow(WaitCallback callback, object state)
		{
			Enqueue(LowPriorityQueue, callback, state);
		}

		public static void EnqueueLow(WaitCallback callback)
		{
			Enqueue(LowPriorityQueue, callback, null);
		}

		private static void Enqueue(LinkedList<Task> Queue, WaitCallback callback, object state)
		{
			itemsInQueue.Increment();
			lock (lockAllQueue)
			{
				Queue.AddLast(new Task(callback, state));
			}

			System.Threading.ThreadPool.QueueUserWorkItem(RunPriorityQueue);
		}

		private static Task Dequeue()
		{
			lock (lockAllQueue)
			{
				LinkedList<Task> queue;
				if (HighPriorityQueue.Count > 0)
					queue = HighPriorityQueue;
				else if (MediumPriorityQueue.Count > 0)
					queue = MediumPriorityQueue;
				else if (LowPriorityQueue.Count > 0)
					queue = LowPriorityQueue;
				else
					throw new InvalidOperationException("No items in TaskQueue");

				Task task = queue.First.Value;
				queue.RemoveFirst();

				itemsInQueue.Decrement();
				return task;
			}
		}

		public static void RunPriorityQueue(object nil)
		{
			try
			{
				Task task = Dequeue();
				itemsInProgress.Increment();
				task.Execute(null);
			}
			catch (Exception e)
			{
				Logger.Warn("Error while task execution", e);
			}
			finally
			{
				itemsInProgress.Decrement();
			}
		}
	}

	class Task
	{
		private WaitCallback callback;
		private object state;

		public Task(WaitCallback callback)
		{
			this.callback = callback;
		}

		public Task(WaitCallback callback, object state)
		{
			this.callback = callback;
			this.state = state;
		}

		public void Execute(object nil)
		{
			this.callback(this.state);
		}
	}
}
