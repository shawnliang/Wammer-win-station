
namespace Wammer.PerfMonitor
{
	class PostUploadMonitor
	{
		private readonly IPerfCounter UploadNumCounter;

		public PostUploadMonitor()
		{
			UploadNumCounter = PerfCounter.GetCounter(PerfCounter.POST_IN_QUEUE);
		}

		public void PostUploadTaskEnqueued()
		{
			UploadNumCounter.Increment();
		}

		public void PostUploadTaskDone()
		{
			UploadNumCounter.Decrement();
		}
	}
}
