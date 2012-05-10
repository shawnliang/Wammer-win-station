using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	class NullTask : ITask
	{
		public void Execute()
		{
			PerfMonitor.PerfCounter.GetCounter(PerfMonitor.PerfCounter.UP_REMAINED_COUNT, false).Decrement();
		}
	}
}
