using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
