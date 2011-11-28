using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wammer.Utility
{
	public class DropboxHelper
	{
		private static string hostDb = @"Dropbox\host.db";
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DropboxHelper));

		public static bool IsInstalled()
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string hostDbPath = Path.Combine(appData, hostDb);

			logger.DebugFormat("Dropbox path for verification: {0}", hostDbPath);

			return File.Exists(hostDbPath);
		}

		public static string GetSyncFolder()
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string hostDbPath = Path.Combine(appData, hostDb);

			if (!File.Exists(hostDbPath))
				return string.Empty;

			using (FileStream fs = new FileStream(hostDbPath, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(fs))
				{
					var ignore = reader.ReadLine();
					string line = reader.ReadLine().Trim();
					byte[] data = Convert.FromBase64String(line);

					string syncFolderPath = Encoding.UTF8.GetString(data);
					logger.DebugFormat("Dropbox sync folder path = {0}", syncFolderPath);
					return syncFolderPath;
				}
			}
		}
	}
}
