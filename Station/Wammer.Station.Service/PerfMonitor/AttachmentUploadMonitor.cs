using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.PerfMonitor
{
	class AttachmentUploadMonitor
	{
		private IPerfCounter avgTime;
		private IPerfCounter avgTimeBase;
		private static log4net.ILog Logger = log4net.LogManager.GetLogger("PerfCounter");
		public AttachmentUploadMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE);
		}

		public void OnProcessSucceeded(object sender, Wammer.Station.HttpHandlerEventArgs evt)
		{
			try
			{
				avgTime.IncrementBy(evt.DurationInTicks);
				avgTimeBase.Increment();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to write performance data", e);
			}
		}
	}
}
