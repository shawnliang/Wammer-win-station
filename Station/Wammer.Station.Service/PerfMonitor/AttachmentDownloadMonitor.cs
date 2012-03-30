using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.PerfMonitor
{
	class AttachmentDownloadMonitor
	{
		private IPerfCounter DownstreamNumCounter;

		public AttachmentDownloadMonitor()
		{
			DownstreamNumCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
		}

		public void OnDownstreamTaskEnqueued(object sender, EventArgs arg)
		{
			DownstreamNumCounter.Increment();
		}

		public void OnDownstreamTaskDone(object sender, EventArgs arg)
		{
			DownstreamNumCounter.Decrement();
		}
	}
}
