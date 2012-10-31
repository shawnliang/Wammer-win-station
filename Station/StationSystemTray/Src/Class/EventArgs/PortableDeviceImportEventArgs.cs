using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class PortableDeviceImportEventArgs : EventArgs
	{
		public string CurrentFile { get; private set; }
		public bool Cancel { get; set; }

		public PortableDeviceImportEventArgs(string file)
		{
			CurrentFile = file;
			Cancel = false;
		}
	}
}
