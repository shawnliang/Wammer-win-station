using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.PerfMonitor
{
	class AttachmentDownloadMonitor
	{
		private IPerfCounter DownstreamNumCounter;
		private IPerfCounter DownstreamRateCounter;

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

		public void OnDownstreamTaskInProgress(object sender, System.ComponentModel.ProgressChangedEventArgs arg)
		{
			DownstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}
