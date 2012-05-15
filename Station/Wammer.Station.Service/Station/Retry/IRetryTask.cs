using System;

namespace Wammer.Station.Retry
{
	public interface IRetryTask : ITask
	{
		DateTime NextRetryTime { get; }
		TaskPriority Priority { get; }
		void ScheduleToRun();
	}
}