using System;
using System.ComponentModel;

namespace Wammer.PerfMonitor
{
	internal class UploadDownloadMonitor
	{
		#region Property

#if DEBUG
		private int TotalNeedToDownload { get; set; }
		private int TotalDownloadCount { get; set; }
#endif

		#endregion


		private readonly IPerfCounter avgTime;
		private readonly IPerfCounter avgTimeBase;
		private readonly IPerfCounter DownstreamNumCounter;
		private readonly IPerfCounter DownstreamRateCounter;
		private readonly IPerfCounter UpstreamNumCounter;
		private readonly IPerfCounter UpstreamRateCounter;
		private readonly IPerfCounter attachmentUploadCounter;

		public void OnProcessSucceeded(object sender, Wammer.Station.HttpHandlerEventArgs evt)
		{
			try
			{
				avgTime.IncrementBy(evt.DurationInTicks);
				avgTimeBase.Increment();
				attachmentUploadCounter.Increment();
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to write performance data: " + PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD, e);
			}
		}

		public UploadDownloadMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE);

			DownstreamNumCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
			DownstreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE);

			UpstreamNumCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT);
			UpstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);

			attachmentUploadCounter = PerfCounter.GetCounter(PerfCounter.ATTACHMENT_UPLOAD_COUNT);
		}


		public void OnTaskEnqueued(object sender, Wammer.Station.TaskEventArgs arg)
		{
			if (arg.task is Wammer.Station.Timeline.ResourceDownloadTask)
				DownstreamNumCounter.Increment();
			else if (arg.task is Wammer.Station.AttachmentUpload.UpstreamTask)
				UpstreamNumCounter.Increment();
		}

		public void OnTaskDequeued(object sender, Wammer.Station.TaskEventArgs arg)
		{
			if (arg.task is Wammer.Station.Timeline.ResourceDownloadTask)
				DownstreamNumCounter.Decrement();
			else if (arg.task is Wammer.Station.AttachmentUpload.UpstreamTask)
				UpstreamNumCounter.Decrement();
		}

		public void OnDownstreamTaskInProgress(object sender, ProgressChangedEventArgs arg)
		{
			DownstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}