using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public interface IPortableMediaService
	{
		event EventHandler<FileImportedEventArgs> FileImported;
		event EventHandler<ImportDoneEventArgs> ImportDone;

		IEnumerable<PortableDevice> GetPortableDevices();
		void ImportAsync(IEnumerable<string> files, string user_id, string session_token, string apikey);
	}
}
