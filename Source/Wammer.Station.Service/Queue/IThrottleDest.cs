using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;

namespace Wammer.Queue
{
	public interface IThrottleDest
	{
		void NoThrottleEnqueue(ITask task, TaskPriority pri);
	}
}
