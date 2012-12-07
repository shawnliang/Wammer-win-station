
namespace Waveface.Stream.Model
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
