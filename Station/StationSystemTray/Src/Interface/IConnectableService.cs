using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IConnectableService
	{
		string Name { get; }
		bool Enable { get; set; }
		System.Drawing.Image Icon { get; }
	}
}
