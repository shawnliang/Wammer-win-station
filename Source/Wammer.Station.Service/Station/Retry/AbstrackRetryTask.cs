using System;
using System.Diagnostics;
using Wammer.Utility;

namespace Wammer.Station.Retry
{
	[Serializable]
	public abstract class AbstrackRetryTask : IRetryTask
	{
		protected TaskPriority priority;

		protected AbstrackRetryTask(TaskPriority priority)
		{
			this.priority = priority;
		}

		#region IRetryTask Members

		public void Execute()
		{
			try
			{
				Do();
			}
			catch
			{
				RetryQueueHelper.Instance.Enqueue(this);
				throw;
			}
		}

		public abstract void ScheduleToRun();


		public abstract DateTime NextRetryTime { get; set; }

		public TaskPriority Priority
		{
			get { return priority; }
		}

		#endregion

		protected abstract void Do();
	}

	[Serializable]
	public abstract class DelayedRetryTask : AbstrackRetryTask
	{
		private readonly BackOff backoff = new BackOff(10, 20, 30, 50, 80, 130, 210, 340, 550, 890, 1440, 2330);
		private DateTime nextRetryTime;

		protected DelayedRetryTask(TaskPriority priority)
			: base(priority)
		{
		}

		public override DateTime NextRetryTime
		{
			get { return nextRetryTime; }
			set { nextRetryTime = value; }
		}

		protected override void Do()
		{
			try
			{
				Run();
			}
			catch (Exception e)
			{
				nextRetryTime = DateTime.Now.AddSeconds(backoff.NextValue());

				this.LogDebugMsg(GetType() + " failed and will be rescheduled at " + NextRetryTime.ToString("u"), e);
				backoff.IncreaseLevel();

				throw;
			}
		}

		protected abstract void Run();
	}

	[Serializable]
	public class PostponedTask : IRetryTask
	{
		private readonly ITask taskToRun;

		public PostponedTask(DateTime nextRetryTime, TaskPriority priority, ITask taskToRun)
		{
			this.taskToRun = taskToRun;
			Priority = priority;
			NextRetryTime = nextRetryTime;
		}

		#region IRetryTask Members

		public TaskPriority Priority { get; private set; }
		public DateTime NextRetryTime { get; set; }

		public void ScheduleToRun()
		{
			TaskQueue.Enqueue(taskToRun, Priority);
		}

		public void Execute()
		{
			// Becuase PostponedTask is a placeholder for another task, 
			// its Execute() is not expected to be executed.
			Trace.Fail("This method should not be called.");
		}

		#endregion
	}
}