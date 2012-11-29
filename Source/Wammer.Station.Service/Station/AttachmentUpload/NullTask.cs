using System;
using Wammer.PerfMonitor;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class NullTask : ITask
	{
		#region ITask Members

		public void Execute()
		{
		}

		#endregion
	}
}