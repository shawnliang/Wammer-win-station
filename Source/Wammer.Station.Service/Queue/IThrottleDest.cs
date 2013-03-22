using Wammer.Station;

namespace Wammer.Queue
{
	public interface IThrottleDest
	{
		void NoThrottleEnqueue(ThrottleTask task, TaskPriority pri);
	}
}
