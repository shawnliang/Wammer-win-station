using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Wammer.Station.Management;
using Wammer.Station;
using Waveface.Localization;
using System.Globalization;

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
			log4net.Config.XmlConfigurator.Configure();
			FileStream fileLock = null;

			try
			{
				using (fileLock = AcquireLock())
				{
					ApplyInstalledCulture();

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

		private static void ApplyInstalledCulture()
		{
			string _culture = (string)StationRegistry.GetValue("Culture", null);

			if (_culture == null)
				CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
			else
				CultureManager.ApplicationUICulture = new CultureInfo(_culture);
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
