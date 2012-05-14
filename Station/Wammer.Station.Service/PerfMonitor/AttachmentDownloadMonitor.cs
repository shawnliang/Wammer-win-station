using System;
using System.ComponentModel;

namespace Wammer.PerfMonitor
{
	internal class AttachmentDownloadMonitor
	{
		private readonly IPerfCounter DownstreamNumCounter;
		private readonly IPerfCounter DownstreamRateCounter;

		public AttachmentDownloadMonitor()
		{
			DownstreamNumCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false);
			DownstreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false);
		}

		public void OnDownstreamTaskEnqueued(object sender, EventArgs arg)
		{
			DownstreamNumCounter.Increment();
		}

		public void OnDownstreamTaskDone(object sender, EventArgs arg)
		{
			DownstreamNumCounter.Decrement();
		}

		public void OnDownstreamTaskInProgress(object sender, ProgressChangedEventArgs arg)
		{
			DownstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}