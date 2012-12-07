
namespace Wammer.PerfMonitor
{
	class NullPerfCounter : IPerfCounter
	{
		public void Increment()
		{
		}

		public void IncrementBy(long value)
		{
		}

		public void Decrement()
		{
		}

		public float NextValue()
		{
			return 0f;
		}
	}
}
