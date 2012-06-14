using System;

namespace Wammer.Station.Retry
{
	public interface IRetryTask : ITask
	{
		DateTime NextRetryTime { get; set; }
		TaskPriority Priority { get; }
		void ScheduleToRun();
	}
}