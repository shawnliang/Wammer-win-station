using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;

namespace Wammer.Station.Retry
{
	[Serializable]
	public abstract class AbstrackRetryTask : IRetryTask
	{
		// see Execute() for why this field is not serializable
		[NonSerialized]
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
				// failQueue should not not be serialized because 
				// this queue (RetryQueue) in fact is a single instance. 
				// If multiple instances of failQueue is created because of deserialization,
				// failed task will be added into those instance of failQueue but not one will
				// add the tasks in those failQueues back to TaskQueue.
				//
				// In case the failQueue is null after deserializartion, point failQueue to 
				// RetryQueue.Instance.
				if (failQueue == null)
					failQueue = RetryQueue.Instance;

				failQueue.Enqueue(this);
			}
		}

		protected abstract void Do();
		public abstract DateTime NextRetryTime { get; }
		public TaskPriority Priority { get { return priority; } }
	}

	[Serializable]
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
			catch(Exception e)
			{
				nextRetryTime = DateTime.Now.AddSeconds(backoff.NextValue());

				this.LogWarnMsg(this.GetType().ToString() + " failed and will be rescheduled at " + NextRetryTime.ToString("u"), e);
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
	
	[Serializable]
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
