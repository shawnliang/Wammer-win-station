using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StationSystemTray
{
	class PortableMediaService : IPortableMediaService
	{
		public IEnumerable<PortableDevice> GetPortableDevices()
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				if ((drive.DriveType == DriveType.CDRom || drive.DriveType == DriveType.Removable) &&
					drive.IsReady)
				{
					yield return new PortableDevice
					{
						DrivePath = drive.Name,
						Name = string.Format("({0}) {1}", drive.Name, drive.VolumeLabel)
					};
				}
			}
			
		}

		public IEnumerable<string> GetFileList(string path)
		{
			return Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);			
		}

		public void Import(string file)
		{
			System.Threading.Thread.Sleep(100);
		}
	}
}
