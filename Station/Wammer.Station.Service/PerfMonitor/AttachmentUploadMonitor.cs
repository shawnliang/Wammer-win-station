using System;
using Wammer.Station;

namespace Wammer.PerfMonitor
{
	internal class AttachmentUploadMonitor
	{
		private readonly IPerfCounter avgTime;
		private readonly IPerfCounter avgTimeBase;

		public AttachmentUploadMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE);
		}

		public void OnProcessSucceeded(object sender, HttpHandlerEventArgs evt)
		{
			try
			{
				avgTime.IncrementBy(evt.DurationInTicks);
				avgTimeBase.Increment();
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to write performance data: " + PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD, e);
			}
		}
	}
}