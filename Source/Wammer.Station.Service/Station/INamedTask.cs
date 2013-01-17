using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public interface INamedTask : ITask
	{
		string Name { get; }
	}
}
