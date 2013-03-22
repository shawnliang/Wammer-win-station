using log4net;
using System;
using System.Collections.Generic;
using Wammer.Station;
using Waveface.Stream.Model;

namespace Wammer.Queue
{
	public class TaskQueueImp : IThrottleDest
	{
		private MongoPersistentStorage storage;
		private WMSBroker mqBroker;
		private WMSSession mqSession;
		private WMSQueue mqHighPriority;
		private WMSQueue mqMediumPriority;
		private WMSQueue mqLowPriority;
		private WMSQueue mqVeryLowPriority;
		private IThreadPool threadPool;

		private int maxConcurrentTaskCount = 6;
		private int runningTaskCount;
		private int totalTaskCount;
		private int waitingHighTaskCount;
		private int maxRunningNonHighTaskCount;
		private int runningNonHighTaskCount;

		private List<Throttle> throttles = new List<Throttle>();

		private readonly object lockObj = new object();

		private static readonly IPerfCounter itemsInQueue = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_QUEUE);
		private static readonly IPerfCounter itemsInProgress = PerfCounter.GetCounter(PerfCounter.ITEMS_IN_PROGRESS);
		private static readonly ILog Logger = LogManager.GetLogger("TaskQueue");


		public TaskQueueImp(IThreadPool threadPool, int maxConcurrency = int.MaxValue)
		{
			this.threadPool = threadPool;

			storage = new MongoPersistentStorage();
			mqBroker = new WMSBroker(storage);
			mqSession = mqBroker.CreateSession();
			mqHighPriority = mqBroker.GetQueue("high");
			mqMediumPriority = mqBroker.GetQueue("medium");
			mqLowPriority = mqBroker.GetQueue("low");
			mqVeryLowPriority = mqBroker.GetQueue("verylow");

			this.MaxConcurrentTaskCount = maxConcurrency;
		}

		public void Enqueue(Station.ITask task, TaskPriority priority = TaskPriority.Medium, bool persistent = false)
		{
			WMSQueue queue = getQueue(priority);
			enqueue(priority, queue, task, persistent);
		}

		private WMSQueue getQueue(TaskPriority priority)
		{
			WMSQueue queue;
			switch (priority)
			{
				case TaskPriority.High:
					queue = mqHighPriority;
					break;
				case TaskPriority.Medium:
					queue = mqMediumPriority;
					break;
				case TaskPriority.Low:
					queue = mqLowPriority;
					break;
				case TaskPriority.VeryLow:
					queue = mqVeryLowPriority;
					break;
				default:
					throw new ArgumentOutOfRangeException("unknown priority: " + priority.ToString());
			}
			return queue;
		}

		public void Init()
		{
			totalTaskCount = mqHighPriority.Count + mqMediumPriority.Count + mqLowPriority.Count + mqVeryLowPriority.Count;

			itemsInQueue.IncrementBy(totalTaskCount);
			waitingHighTaskCount = mqHighPriority.Count;

			for (int i = 0; i < Math.Min(MaxConcurrentTaskCount, totalTaskCount); i++)
			{
				lock (lockObj)
				{
					_scheduleNextTaskToRun();
				}
			}
		}

		public int MaxConcurrentTaskCount
		{
			get { return maxConcurrentTaskCount; }

			set
			{
				maxConcurrentTaskCount = value;
				maxRunningNonHighTaskCount = maxConcurrentTaskCount / 2;

				if (maxRunningNonHighTaskCount == 0)
					maxRunningNonHighTaskCount = 1;
			}
		}

		public void RunPriorityQueue(object nil)
		{
			DequeuedItem dequeuedItem = null;

			try
			{
				itemsInProgress.Increment();
				dequeuedItem = dequeue();
				((ITask)dequeuedItem.Item.Data).Execute();
			}
			catch (Exception e)
			{
				Logger.Warn("Error while task execution", e);
			}
			finally
			{
				itemsInProgress.Decrement();

				//Debug.Assert(dequeuedItem != null, "dequeuedItem != null");
				//Debug.Assert(dequeuedItem.Item != null, "dequeuedItem.Item != null");

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

		public void AddThrottle(Throttle throttle)
		{
			lock (this.lockObj)
			{
				throttle.Dest = this;
				throttles.Add(throttle);
			}
		}

		public void NoThrottleEnqueue(ThrottleTask task, TaskPriority priority)
		{
			var que = getQueue(priority);
			noThrottleEnqueue(priority, que, task, false);
		}

		private void enqueue(TaskPriority priority, WMSQueue queue, ITask task, bool persistent)
		{
			itemsInQueue.Increment();

			foreach (var throttle in throttles)
			{
				if (throttle.ShouldThrottle(task))
				{
					var saveItem = new WMSMessage(task) { IsPersistent = persistent, Queue = queue };

					if (persistent)
						storage.Save(saveItem);

					throttle.Eat(saveItem, priority);
					return;
				}
			}


			noThrottleEnqueue(priority, queue, task, persistent);
		}

		private void noThrottleEnqueue(TaskPriority priority, WMSQueue queue, WMSMessage msg)
		{
			queue.Push(msg, msg.IsPersistent);

			lock (lockObj)
			{
				++totalTaskCount;
				if (priority == TaskPriority.High)
					++waitingHighTaskCount;

				_scheduleNextTaskToRun();
			}
		}

		private void noThrottleEnqueue(TaskPriority priority, WMSQueue queue, ITask task, bool persistent)
		{
			WMSMessage msg = new WMSMessage(task) { IsPersistent = persistent, Queue = queue };
			noThrottleEnqueue(priority, queue, msg);
		}

		private void _scheduleNextTaskToRun()
		{
			if (isATaskWaiting() && !reachConcurrentTaskLimit())
			{
				if (isAHighPriorityTaskWaiting())
				{
					++runningTaskCount;
					--waitingHighTaskCount;
					threadPool.QueueWorkItem(RunPriorityQueue);
				}
				else if (!reachNonHighPriorityTaskLimit())
				{
					++runningTaskCount;
					++runningNonHighTaskCount;
					threadPool.QueueWorkItem(RunPriorityQueue);
				}
			}
		}

		private bool isATaskWaiting()
		{
			return totalTaskCount > runningTaskCount;
		}

		private bool reachNonHighPriorityTaskLimit()
		{
			return runningNonHighTaskCount >= maxRunningNonHighTaskCount;
		}

		private bool isAHighPriorityTaskWaiting()
		{
			return waitingHighTaskCount > 0;
		}

		private bool reachConcurrentTaskLimit()
		{
			return runningTaskCount >= MaxConcurrentTaskCount;
		}

		private DequeuedItem dequeue()
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
	}
}
