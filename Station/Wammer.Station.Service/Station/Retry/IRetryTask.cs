using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Retry
{
	public interface IRetryTask : ITask
	{
		DateTime NextRetryTime { get; }
		TaskPriority Priority { get; }
	}
}
