using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Wammer.PerfMonitor;
using Wammer.Queue;

namespace Wammer.Station
{

	enum TaskPriority
	{
		Low,
		Medium,
		High
	}

	static class TaskQueue
	{
		private static object lockAllQueue = new object();

		//private static LinkedList<ITask> HighPriorityQ = new LinkedList<ITask>();
		//private static LinkedList<ITask> MediumPriorityQ = new LinkedList<ITask>();
		//private static LinkedList<ITask> LowPriorityQ = new LinkedList<ITask>();

		

		private static log4net.ILog Logger = log4net.LogManager.GetLogger("TaskQueue");

		private static IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);


		private static WMSBroker mqBroker;
		private static WMSSession mqSession;
		private static WMSQueue mqHighPriority;
		private static WMSQueue mqMediumPriority;
		private static WMSQueue mqLowPriority;

		public static TaskQueue()
		{
			mqBroker = new WMSBroker(new SQLitePersistentStorage("taskQueue.db"));
			mqSession = mqBroker.CreateSession();
			mqHighPriority = mqBroker.GetQueue("high");
			mqMediumPriority = mqBroker.GetQueue("medium");
			mqLowPriority = mqBroker.GetQueue("low");
		}

		/// <summary>
		/// Enqueues a non-persistent task
		/// </summary>
		/// <param name="task"></param>
		/// <param name="priority"></param>
		public static void Enqueue(ITask task, TaskPriority priority)
		{
			Enqueue(task, priority, false);
		}

		/// <summary>
		/// Enqueues a task
		/// </summary>
		/// <param name="task"></param>
		/// <param name="priority"></param>
		/// <param name="persistent"></param>
		public static void Enqueue(ITask task, TaskPriority priority, bool persistent)
		{
			WMSQueue queue = null;

			if (priority == TaskPriority.High)
				queue = mqHighPriority;
			else if (priority == TaskPriority.Medium)
				queue = mqMediumPriority;
			else if (priority == TaskPriority.Low)
				queue = mqLowPriority;
			else
				throw new ArgumentOutOfRangeException("unknown priority: " + priority.ToString());

			Enqueue(queue, task, persistent);
		}

		private static void Enqueue(WMSQueue queue, ITask task, bool persistent)
		{
			itemsInQueue.Increment();

			mqSession.Push(queue, task, persistent);

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
}
