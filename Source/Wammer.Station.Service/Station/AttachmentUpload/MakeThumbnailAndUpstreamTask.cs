using System;
using Wammer.Model;
using Wammer.Station.Retry;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class MakeThumbnailAndUpstreamTask : DelayedRetryTask
	{
		public ImageMeta meta { get; set; }
		public string object_id { get; set; }
		
		public MakeThumbnailAndUpstreamTask()
			:base(TaskPriority.Medium)
		{
		}

		public MakeThumbnailAndUpstreamTask(string object_id, ImageMeta meta, TaskPriority pri)
			: base(pri)
		{
			this.object_id = object_id;
			this.meta = meta;
		}

		protected override void Run()
		{
			var makeThumbnail = new MakeThumbnailTask(object_id, meta, this.priority);
			makeThumbnail.MakeThumbnail();

			new AttachmentUtility().UpstreamAttachmentAsync(object_id, meta, TaskPriority.VeryLow);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}