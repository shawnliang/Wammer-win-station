using Wammer.Station;

namespace Wammer.Queue
{
	public interface IThrottleDest
	{
		void NoThrottleEnqueue(WMSMessage msg, TaskPriority pri);
	}
}
