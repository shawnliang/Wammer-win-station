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
		private IPerfCounter thumbnailUpstreamRate;

		private static log4net.ILog Logger = log4net.LogManager.GetLogger("PerfCounter");

		public AttachmentUploadMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE);
			thumbnailUpstreamRate = PerfCounter.GetCounter(PerfCounter.THUMBNAIL_UPSTREAM_RATE);
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
				Logger.Warn("Unable to write performance data: " + PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD, e);
			}
		}

		public void OnThumbnailUpstreamed(object sender, Wammer.Station.ThumbnailUpstreamedEventArgs evt)
		{
			try
			{
				thumbnailUpstreamRate.IncrementBy(evt.BytesUpstreamed);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to write performance data: " + PerfCounter.THUMBNAIL_UPSTREAM_RATE, e);
			}
		}

	}
}
