using System;
using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public interface IPortableMediaService
	{
		IEnumerable<PortableDevice> GetPortableDevices();

		string ImportAsync(string drive_path);

		ImportTaskStaus QueryTaskStatus(string taskId);

		bool GetAlwaysAutoImport(string driveName);
		void SetAlwaysAutoImport(string driveName, bool autoImport);
	}
}
