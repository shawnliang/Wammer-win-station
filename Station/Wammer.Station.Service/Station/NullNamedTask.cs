using System;

namespace Wammer.Station
{
	[Serializable]
	class NullNamedTask: INamedTask
	{
		public string Name
		{
			get { return "no-name"; }
		}

		public void Execute()
		{
		}
	}
}
