using System;
using System.Collections.Generic;
using System.Threading;
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
		private static log4net.ILog Logger = log4net.LogManager.GetLogger("TaskQueue");

		private static IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);

		private static WMSBroker mqBroker;
		private static WMSSession mqSession;
		private static WMSQueue mqHighPriority;
		private static WMSQueue mqMediumPriority;
		private static WMSQueue mqLowPriority;


		public static int MaxCurrentTaskCount { get; set; }
		private static int runningTaskCount;
		private static int totalTaskCount;
		private static object lockObj = new object();

		static TaskQueue()
		{
			mqBroker = new WMSBroker(new SQLitePersistentStorage("taskQueue.db"));
			mqSession = mqBroker.CreateSession();
			mqHighPriority = mqBroker.GetQueue("high");
			mqMediumPriority = mqBroker.GetQueue("medium");
			mqLowPriority = mqBroker.GetQueue("low");

			MaxCurrentTaskCount = 6;
			runningTaskCount = 0;
			totalTaskCount = 0;
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

		/// <summary>
		/// Enqueues a non persistent task to low priority queue
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		public static void EnqueueLow(WaitCallback cb, object state)
		{
			Enqueue(new SimpleTask(cb, state), TaskPriority.Low, false);
		}

		/// <summary>
		/// Enqueues a non persistent task to medium priority queue
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		public static void EnqueueMedium(WaitCallback cb, object state)
		{
			Enqueue(new SimpleTask(cb, state), TaskPriority.Medium, false);
		}

		/// <summary>
		/// Enqueues a non persistent task to high priority queue
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		public static void EnqueueHigh(WaitCallback cb, object state)
		{
			Enqueue(new SimpleTask(cb, state), TaskPriority.High, false);
		}

		private static void Enqueue(WMSQueue queue, ITask task, bool persistent)
		{
			itemsInQueue.Increment();

			mqSession.Push(queue, task, persistent);

			lock (lockObj)
			{
				++totalTaskCount;
				if (runningTaskCount < MaxCurrentTaskCount)
				{
					ThreadPool.QueueUserWorkItem(RunPriorityQueue);
					++runningTaskCount;
				}
			}
		}

		private static WMSMessage Dequeue()
		{

			WMSMessage item = null;

			try
			{
				item = mqSession.Pop(mqHighPriority);
				if (item != null)
					return item;

				item = mqSession.Pop(mqMediumPriority);
				if (item != null)
					return item;

				item = mqSession.Pop(mqLowPriority);
				if (item != null)
					return item;
			}
			finally
			{
				if (item != null)
					itemsInQueue.Decrement();
			}

			throw new InvalidOperationException("No items in TaskQueue");
		}

		public static void RunPriorityQueue(object nil)
		{
			WMSMessage queueItem = null;
			
			try
			{
				itemsInProgress.Increment();
				queueItem = Dequeue();
				((ITask)queueItem.Data).Execute();
			}
			catch (Exception e)
			{
				Logger.Warn("Error while task execution", e);
			}
			finally
			{
				itemsInProgress.Decrement();
				queueItem.Acknowledge();

				lock (lockObj)
				{
					--runningTaskCount;
					--totalTaskCount;

					if (totalTaskCount > runningTaskCount &&
						runningTaskCount < MaxCurrentTaskCount)
					{
						System.Threading.ThreadPool.QueueUserWorkItem(RunPriorityQueue);
						++runningTaskCount;
					}
				}
			}
		}
	}

	public interface ITask
	{
		void Execute();
	}

	public class SimpleTask : ITask
	{
		private WaitCallback cb;
		private object state;

		public SimpleTask(WaitCallback cb, object state)
		{
			this.cb = cb;
			this.state = state;
		}

		public void Execute()
		{
			this.cb(this.state);
		}
	}
}
