using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;

namespace Wammer.Station.Retry
{
	public abstract class AbstrackRetryTask : IRetryTask
	{
		protected IRetryQueue failQueue;
		protected TaskPriority priority;

		protected AbstrackRetryTask(IRetryQueue failQueue, TaskPriority priority)
		{
			this.failQueue = failQueue;
			this.priority = priority;
		}

		public void Execute()
		{
			try
			{
				this.Do();
			}
			catch (Exception e)
			{
				failQueue.Enqueue(this);
			}
		}

		protected abstract void Do();
		public abstract DateTime NextRetryTime { get; }
		public TaskPriority Priority { get { return priority; } }
	}

	public abstract class DelayedRetryTask : AbstrackRetryTask
	{
		private BackOff backoff = new BackOff(10, 20, 30, 50, 80, 130, 210, 340, 550, 890, 1440, 2330);
		private DateTime nextRetryTime;

		protected DelayedRetryTask(IRetryQueue failQueue, TaskPriority priority)
			:base(failQueue, priority)
		{
		}

		protected override void Do()
		{
			try
			{
				Run();
			}
			catch
			{
				nextRetryTime = DateTime.Now.AddSeconds(backoff.NextValue());
				backoff.IncreaseLevel();
				throw;
			}
		}
		
		public override DateTime NextRetryTime
		{
			get
			{
				return nextRetryTime;
			}
		}

		protected abstract void Run();
	}

	public class PostponedTask: IRetryTask
	{
		private ITask taskToRun;
		public TaskPriority Priority { get; private set; }
		public DateTime NextRetryTime { get; private set; }

		public PostponedTask(DateTime nextRetryTime, TaskPriority priority, ITask taskToRun)
		{
			this.taskToRun = taskToRun;
			this.Priority = priority;
			this.NextRetryTime = nextRetryTime;
		}

		public void Execute()
		{
			taskToRun.Execute();
		}
	}

}
