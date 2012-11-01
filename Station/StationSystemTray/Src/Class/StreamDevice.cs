using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class StreamDevice
	{
		public string Name { get; private set; }
		public bool Online { get; private set; }
		public string Type { get; private set; }

		public StreamDevice(string name, bool online, string type)
		{
			Name = name;
			Online = online;
			Type = type;
		}
	}
}
