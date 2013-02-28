using System;
using System.ComponentModel;
using Waveface.Stream.Model;

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
		private readonly IPerfCounter bytesToDownloadCounter;
		private readonly IPerfCounter bytesToUploadCounter;

		private static UploadDownloadMonitor instance;

		static UploadDownloadMonitor()
		{
			instance = new UploadDownloadMonitor();
		}

		private UploadDownloadMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE);

			DownstreamNumCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
			DownstreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE);

			UpstreamNumCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT);
			UpstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);

			attachmentUploadCounter = PerfCounter.GetCounter(PerfCounter.ATTACHMENT_UPLOAD_COUNT);

			bytesToDownloadCounter = PerfCounter.GetCounter(PerfCounter.BYTES_TO_DOWNLOAD);
			bytesToUploadCounter = PerfCounter.GetCounter(PerfCounter.BYTES_TO_UPLOAD);
		}

		public static UploadDownloadMonitor Instance
		{
			get { return instance; }
		}

		public void OnAttachmentProcessed(object sender, Wammer.Station.HttpHandlerEventArgs evt)
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

		public void OnTaskEnqueued(object sender, Wammer.Station.TaskEventArgs arg)
		{
			if (arg.task is Wammer.Station.Timeline.ResourceDownloadTask)
			{
				var bytesToDownload = getDownloadSize(arg);

				bytesToDownloadCounter.IncrementBy(bytesToDownload);
				DownstreamNumCounter.Increment();
			}
			else if (arg.task is Wammer.Station.AttachmentUpload.UpstreamTask)
			{
				UpstreamNumCounter.Increment();

				var uploadBytes = getUploadSize(arg);

				this.LogInfoMsg("[upload] " + ((Wammer.Station.AttachmentUpload.UpstreamTask)arg.task).object_id + "   " + uploadBytes.ToString());
				bytesToUploadCounter.IncrementBy(uploadBytes);
			}
		}

		public void OnTaskDequeued(object sender, Wammer.Station.TaskEventArgs arg)
		{
			if (arg.task is Wammer.Station.Timeline.ResourceDownloadTask)
			{
				var bytesDownload = getDownloadSize(arg);
				bytesToDownloadCounter.IncrementBy(-bytesDownload);
				DownstreamNumCounter.Decrement();
			}
			else if (arg.task is Wammer.Station.AttachmentUpload.UpstreamTask)
			{
				UpstreamNumCounter.Decrement();

				var uploadBytes = getUploadSize(arg);
				bytesToUploadCounter.IncrementBy(-uploadBytes);
			}
		}

		public void OnDownstreamTaskInProgress(object sender, ProgressChangedEventArgs arg)
		{
			DownstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}

		private static long getDownloadSize(Wammer.Station.TaskEventArgs arg)
		{
			var downloadTask = arg.task as Wammer.Station.Timeline.ResourceDownloadTask;

			var bytesToDownload = 0L;
			if (downloadTask.evtargs.IsOriginalAttachment() || downloadTask.evtargs.IsWebThumb())
				bytesToDownload = downloadTask.evtargs.attachment.file_size;
			else
				bytesToDownload = downloadTask.evtargs.attachment.GetThumbnail(downloadTask.evtargs.imagemeta).file_size;
			return bytesToDownload;
		}

		private long getUploadSize(Station.TaskEventArgs arg)
		{
			var task = (Wammer.Station.AttachmentUpload.UpstreamTask)arg.task;
			var att = AttachmentCollection.Instance.FindOneById(task.object_id);

			if (att == null || att.GetInfoByMeta(task.meta) == null)
				return 0L;

			return att.GetInfoByMeta(task.meta).file_size;
		}
	}
}