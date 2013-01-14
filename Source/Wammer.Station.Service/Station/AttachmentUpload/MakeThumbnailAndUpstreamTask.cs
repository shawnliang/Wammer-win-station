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
		public Guid? importTaskId { get; set; }

		public MakeThumbnailAndUpstreamTask()
			:base(TaskPriority.Medium)
		{
		}

		public MakeThumbnailAndUpstreamTask(string object_id, ImageMeta meta, TaskPriority pri, Guid? importTaskId = null)
			: base(pri)
		{
			this.object_id = object_id;
			this.meta = meta;
			this.importTaskId = importTaskId;
		}

		protected override void Run()
		{
			var makeThumbnail = new MakeThumbnailTask(object_id, meta, this.priority, importTaskId);
			makeThumbnail.MakeThumbnail();

			new AttachmentUtility(importTaskId).UpstreamAttachmentAsync(object_id, meta, TaskPriority.VeryLow);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}