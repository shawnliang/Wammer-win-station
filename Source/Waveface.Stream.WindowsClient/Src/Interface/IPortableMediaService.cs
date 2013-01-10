using System;
using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public interface IPortableMediaService
	{
		IEnumerable<PortableDevice> GetPortableDevices();

		/// <summary>
		/// Requests station to import a drive
		/// </summary>
		/// <param name="drive_path"></param>
		/// <param name="user_id"></param>
		/// <param name="session_token"></param>
		/// <param name="apikey"></param>
		/// <returns>Task Id</returns>
		string ImportAsync(string drive_path, string user_id, string session_token, string apikey);

		ImportTaskStaus QueryTaskStatus(string taskId);

		bool GetAlwaysAutoImport(string driveName);
		void SetAlwaysAutoImport(string driveName, bool autoImport);
	}
}
