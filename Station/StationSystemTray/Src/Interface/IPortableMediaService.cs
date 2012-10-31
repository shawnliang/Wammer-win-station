using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IPortableMediaService
	{
		IEnumerable<PortableDevice> GetPortableDevices();
		IEnumerable<string> GetFileList(string path);
		void Import(string file);
	}
}
