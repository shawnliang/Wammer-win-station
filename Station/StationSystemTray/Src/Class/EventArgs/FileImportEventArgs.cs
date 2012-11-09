using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class FileImportEventArgs : EventArgs
	{
		public string FilePath { get; set; }
	}

	public class ImportDoneEventArgs : EventArgs
	{
		public string Error { get; set; }
	}
}
