using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IPortableMediaService
	{
		event EventHandler<Wammer.Station.FileImportedEventArgs> FileImported;
		event EventHandler<Wammer.Station.ImportDoneEventArgs> ImportDone;

		IEnumerable<PortableDevice> GetPortableDevices();
		IEnumerable<string> GetFileList(string path);
		void ImportAsync(IEnumerable<string> files, string user_id, string session_token, string apikey);
	}
}
