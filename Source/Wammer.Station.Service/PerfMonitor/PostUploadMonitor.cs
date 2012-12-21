using Waveface.Stream.Model;
namespace Wammer.PerfMonitor
{
	internal class PostUploadMonitor
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