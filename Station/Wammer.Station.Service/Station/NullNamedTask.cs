using System;

namespace Wammer.Station
{
	[Serializable]
	internal class NullNamedTask : INamedTask
	{
		#region INamedTask Members

		public string Name
		{
			get { return "no-name"; }
		}

		public void Execute()
		{
		}

		#endregion
	}
}