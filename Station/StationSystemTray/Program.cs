using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Wammer.Station.Management;

namespace StationSystemTray
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			FileStream fileLock = null;

			try
			{
				using (fileLock = AcquireLock())
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new MainForm());
				}
			}
			catch (FileLoadException)
			{
				// is already running
			}
		}

		private const string PID_FILE = "WavefaceSysTray.lock";

		static FileStream AcquireLock()
		{
			try
			{
				string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string lockFile = Path.Combine(appDataDir, PID_FILE);

				return File.Open(lockFile, FileMode.Create, FileAccess.Write, FileShare.None);
			}
			catch (Exception)
			{
				throw new FileLoadException();
			}
		}
	}


	class FileLockException: System.Exception
	{
	}
}
