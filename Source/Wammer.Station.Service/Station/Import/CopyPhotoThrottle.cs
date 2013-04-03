using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Queue;

namespace Wammer.Station.Import
{
	class CopyPhotoThrottle : Throttle
	{
		public CopyPhotoThrottle(int concurrency)
			: base(concurrency)
		{
		}

		public override bool ShouldThrottle(ITask task)
		{
			return task is CopyPhotoTask;
		}
	}
}
