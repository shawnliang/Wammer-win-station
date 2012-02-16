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

		private static LinkedList<ITask> HighPriorityQ = new LinkedList<ITask>();
		private static LinkedList<ITask> MediumPriorityQ = new LinkedList<ITask>();
		private static LinkedList<ITask> LowPriorityQ = new LinkedList<ITask>();

		private static log4net.ILog Logger = log4net.LogManager.GetLogger("TaskQueue");

		private static IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);

		public static void EnqueueHigh(ITask task)
		{
			Enqueue(HighPriorityQ, task);
		}

		public static void EnqueueMedium(ITask task)
		{
			Enqueue(MediumPriorityQ, task);
		}

		public static void EnqueueLow(ITask task)
		{
			Enqueue(LowPriorityQ, task);
		}

		private static void Enqueue(LinkedList<ITask> queue, ITask task)
		{
			itemsInQueue.Increment();
			lock (lockAllQueue)
			{
				queue.AddLast(task);
			}

			System.Threading.ThreadPool.QueueUserWorkItem(RunPriorityQueue);
		}

		private static ITask Dequeue()
		{
			lock (lockAllQueue)
			{
				LinkedList<ITask> queue;
				if (HighPriorityQ.Count > 0)
					queue = HighPriorityQ;
				else if (MediumPriorityQ.Count > 0)
					queue = MediumPriorityQ;
				else if (LowPriorityQ.Count > 0)
					queue = LowPriorityQ;
				else
					throw new InvalidOperationException("No items in TaskQueue");

				ITask task = queue.First.Value;
				queue.RemoveFirst();

				itemsInQueue.Decrement();
				return task;
			}
		}

		public static void RunPriorityQueue(object nil)
		{
			try
			{
				ITask task = Dequeue();
				itemsInProgress.Increment();
				task.Execute();
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

	interface ITask
	{
		void Execute();
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
