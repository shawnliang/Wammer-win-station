using System;
using Wammer.Queue;
using Wammer.Station.AttachmentUpload;

namespace Wammer.Station
{
	class ThumbnailThrottle : Throttle
	{
		Type type1 = typeof(MakeThumbnailAndUpstreamTask);
		Type type2 = typeof(MakeThumbnailTask);

		public ThumbnailThrottle(int maxConcurrency)
			: base(maxConcurrency)
		{
		}

		public override bool ShouldThrottle(ITask task)
		{
			var taskType = task.GetType();
			return taskType.Equals(type1) || task.Equals(type2);
		}
	}
}
