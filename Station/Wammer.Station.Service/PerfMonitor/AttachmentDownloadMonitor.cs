using System;
using System.ComponentModel;

namespace Wammer.PerfMonitor
{
	internal class AttachmentDownloadMonitor
	{
		#region Property

#if DEBUG
		private int TotalNeedToDownload { get; set; }
		private int TotalDownloadCount { get; set; }
#endif

		#endregion

		private readonly IPerfCounter DownstreamNumCounter;
		private readonly IPerfCounter DownstreamRateCounter;

		public AttachmentDownloadMonitor()
		{
			DownstreamNumCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
			DownstreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE);
		}

		public void OnDownstreamTaskEnqueued(object sender, EventArgs arg)
		{
#if DEBUG
			TotalNeedToDownload++;
#endif
			
			DownstreamNumCounter.Increment();
		}

		public void OnDownstreamTaskDone(object sender, EventArgs arg)
		{
#if DEBUG
			TotalDownloadCount++;
#endif
			
			DownstreamNumCounter.Decrement();
		}

		public void OnDownstreamTaskInProgress(object sender, ProgressChangedEventArgs arg)
		{
			DownstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}