using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	class MakeThumbnailAndUpstreamTask: Retry.DelayedRetryTask
	{
		private readonly string object_id;
		private readonly ImageMeta meta;
		private readonly TaskPriority pri;
		private readonly IAttachmentUtil util;

		public MakeThumbnailAndUpstreamTask(string object_id, ImageMeta meta, TaskPriority pri, IAttachmentUtil util)
			: base(Retry.RetryQueue.Instance, pri)
		{
			this.object_id = object_id;
			this.meta = meta;
			this.pri = pri;
			this.util = util;
		}

		protected override void Run()
		{
			var makeThumbnail = new MakeThumbnailTask(object_id, meta, pri);
			makeThumbnail.MakeThumbnail();

			util.UpstreamAttachmentAsync(object_id, meta, pri);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, this.priority);
		}
	}
}
