using System;
using System.Collections.Generic;

namespace Waveface.Stream.WindowsClient
{
	public class DevicesEventArgs : EventArgs
	{
		public IEnumerable<Device> Devices { get; private set; }

		public DevicesEventArgs(IEnumerable<Device> devices)
		{
			this.Devices = devices;
		}
	}
}
