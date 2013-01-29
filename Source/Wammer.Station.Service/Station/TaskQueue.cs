using log4net;
using System;
using System.Diagnostics;
using System.Threading;
using Wammer.PerfMonitor;
using Wammer.Queue;
using Waveface.Stream.Model;

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

	internal static class TaskQueue
	{
		static TaskQueueImp queue;

		static TaskQueue()
		{
			queue = new TaskQueueImp(new ThreadPoolAdapter(), 20);
		}

		public static void Init()
		{
			var concurrency = Environment.ProcessorCount / 2;
			if (concurrency == 0)
				concurrency = 1;

			queue.AddThrottle(new ThumbnailThrottle(concurrency));
			queue.Init();
		}

		public static int MaxConcurrentTaskCount
		{
			set { queue.MaxConcurrentTaskCount = value; }
			get { return queue.MaxConcurrentTaskCount; }
		}

		public static void Enqueue(ITask task, TaskPriority priority, bool persistent = false)
		{
			queue.Enqueue(task, priority, persistent);
		}

		class ThreadPoolAdapter : IThreadPool
		{

			public void QueueWorkItem(WaitCallback callback)
			{
				ThreadPool.QueueUserWorkItem(callback);
			}
		}
	}

	public class DequeuedItem
	{
		public DequeuedItem(WMSMessage item, TaskPriority priority)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			Item = item;
			Priority = priority;
		}

		public WMSMessage Item { get; private set; }
		public TaskPriority Priority { get; private set; }
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

		#region ITask Members

		public void Execute()
		{
			cb(state);
		}

		#endregion
	}
}
