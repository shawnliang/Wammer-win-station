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
		static void Main(string[] args)
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

					bool initMinimized = (args != null && args.Length > 0 && args[0] == "--minimized");

					Application.Run(new MainForm(initMinimized));
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
