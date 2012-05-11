using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Wammer.PerfMonitor;
using Wammer.Queue;

namespace Wammer.Station
{
	[Serializable]
	public enum TaskPriority
	{
		VeryLow,
		Low,
		Medium,
		High
	}

	static class TaskQueue
	{
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger("TaskQueue");

		private static readonly IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static readonly IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);

		private static readonly WMSBroker mqBroker;
		private static readonly WMSSession mqSession;
		private static readonly WMSQueue mqHighPriority;
		private static readonly WMSQueue mqMediumPriority;
		private static readonly WMSQueue mqLowPriority;
		private static readonly WMSQueue mqVeryLowPriority;


		public static int MaxConcurrentTaskCount
		{
			get
			{
				return maxConcurrentTaskCount;
			}

			set
			{
				maxConcurrentTaskCount = value;
				maxRunningNonHighTaskCount = maxConcurrentTaskCount / 2;

				if (maxRunningNonHighTaskCount == 0)
					maxRunningNonHighTaskCount = 1;
			}
		}

		private static int maxConcurrentTaskCount;
		private static int runningTaskCount;
		private static int totalTaskCount;
		private static int waitingHighTaskCount;
		private static int maxRunningNonHighTaskCount;
		private static int runningNonHighTaskCount;

		private static readonly object lockObj = new object();

		static TaskQueue()
		{
			mqBroker = new WMSBroker(new MongoPersistentStorage());
			mqSession = mqBroker.CreateSession();
			mqHighPriority = mqBroker.GetQueue("high");
			mqMediumPriority = mqBroker.GetQueue("medium");
			mqLowPriority = mqBroker.GetQueue("low");
			mqVeryLowPriority = mqBroker.GetQueue("verylow");

			MaxConcurrentTaskCount = 6;

			totalTaskCount = mqHighPriority.Count + mqMediumPriority.Count + mqLowPriority.Count + mqVeryLowPriority.Count;

			PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE).IncrementBy(totalTaskCount);
			waitingHighTaskCount = mqHighPriority.Count;

			for (int i = 0; i < MaxConcurrentTaskCount; i++)
			{
				lock (lockObj)
				{
					_scheduleNextTaskToRun();
				}
			}
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
			else if (priority == TaskPriority.VeryLow)
				queue = mqVeryLowPriority;
			else
				throw new ArgumentOutOfRangeException("unknown priority: " + priority.ToString());

			Enqueue(priority, queue, task, persistent);
		}

		private static void Enqueue(TaskPriority priority, WMSQueue queue, ITask task, bool persistent)
		{
			itemsInQueue.Increment();

			mqSession.Push(queue, task, persistent);

			lock (lockObj)
			{
				++totalTaskCount;
				if (priority == TaskPriority.High)
					++waitingHighTaskCount;

				_scheduleNextTaskToRun();
			}
		}

		private static void _scheduleNextTaskToRun()
		{
			if (isATaskWaiting() && !reachConcurrentTaskLimit())
			{
				if (isAHighPriorityTaskWaiting())
				{
					++runningTaskCount;
					--waitingHighTaskCount;
					ThreadPool.QueueUserWorkItem(RunPriorityQueue);
				}
				else if (!reachNonHighPriorityTaskLimit())
				{
					++runningTaskCount;
					++runningNonHighTaskCount;
					ThreadPool.QueueUserWorkItem(RunPriorityQueue);
				}
			}
		}

		private static bool isATaskWaiting()
		{
			return totalTaskCount > runningTaskCount;
		}

		private static bool reachNonHighPriorityTaskLimit()
		{
			return runningNonHighTaskCount >= maxRunningNonHighTaskCount;
		}

		private static bool isAHighPriorityTaskWaiting()
		{
			return waitingHighTaskCount > 0;
		}

		private static bool reachConcurrentTaskLimit()
		{
			return runningTaskCount >= MaxConcurrentTaskCount;
		}

		private static DequeuedItem Dequeue()
		{

			WMSMessage item = null;

			try
			{
				item = mqSession.Pop(mqHighPriority);
				if (item != null)
					return new DequeuedItem(item, TaskPriority.High);

				item = mqSession.Pop(mqMediumPriority);
				if (item != null)
					return new DequeuedItem(item, TaskPriority.Medium);

				item = mqSession.Pop(mqLowPriority);
				if (item != null)
					return new DequeuedItem(item, TaskPriority.Low);

				item = mqSession.Pop(mqVeryLowPriority);
				if (item != null)
					return new DequeuedItem(item, TaskPriority.VeryLow);
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
			DequeuedItem dequeuedItem = null;
			
			try
			{
				itemsInProgress.Increment();
				dequeuedItem = Dequeue();
				((ITask)dequeuedItem.Item.Data).Execute();
			}
			catch (Exception e)
			{
				Logger.Warn("Error while task execution", e);
			}
			finally
			{
				itemsInProgress.Decrement();

				Debug.Assert(dequeuedItem != null, "dequeuedItem != null");
				Debug.Assert(dequeuedItem.Item != null, "dequeuedItem.Item != null");

				dequeuedItem.Item.Acknowledge();

				lock (lockObj)
				{
					--runningTaskCount;
					--totalTaskCount;

					if (dequeuedItem.Priority != TaskPriority.High)
						--runningNonHighTaskCount;

					_scheduleNextTaskToRun();
				}
			}
		}
	}

	public class DequeuedItem
	{
		public WMSMessage Item { get; private set; }
		public TaskPriority Priority { get; private set; }

		public DequeuedItem(WMSMessage item, TaskPriority priority)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			this.Item = item;
			this.Priority = priority;
		}
	}

	public interface ITask
	{
		void Execute();
	}

	public class SimpleTask : ITask
	{
		private readonly WaitCallback cb;
		private readonly object state;

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
