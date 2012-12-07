
namespace Wammer.Station.Retry
{
	public class RetryQueueHelper
	{
		private static RetryQueue queue;

		static RetryQueueHelper()
		{
			queue = new RetryQueue(new RetryQueuePersistentStorage());
		}

		public static RetryQueue Instance
		{
			get { return queue; }
		}
	}
}
