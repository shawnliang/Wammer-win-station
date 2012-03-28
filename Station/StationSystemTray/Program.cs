using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Wammer.Station.Management;
using Wammer.Station;
using Waveface.Localization;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace StationSystemTray
{
	static class Program
	{
        #region Const
        const string CLIENT_TITLE = "Waveface ";        
        #endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();

            bool isFirstCreated;

            //Create a new mutex using specific mutex name
            Mutex m = new Mutex(true, "StationSystemTray", out isFirstCreated);

            if (!isFirstCreated)
            {
                var currentProcess = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

                foreach (var process in processes)
                {
                    if (process.Id == currentProcess.Id)
                        continue;

                    IntPtr handle = Win32Helper.FindWindow(null, CLIENT_TITLE);
                    if (handle == IntPtr.Zero)
                    {
                        handle = Win32Helper.FindWindow(null, "Log In - Waveface");
                    }

                    if (handle == IntPtr.Zero)
                        return;

                    Win32Helper.SetForegroundWindow(handle);
                    Win32Helper.ShowWindow(handle, 5);
                    return;
                }
                return;
            }

            ApplyInstalledCulture();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool initMinimized = (args != null && args.Length > 0 && args[0] == "--minimized");

            Application.Run(new MainForm(initMinimized));
		}

		private static void ApplyInstalledCulture()
		{
			string _culture = (string)StationRegistry.GetValue("Culture", null);

			if (_culture == null)
				CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
			else
				CultureManager.ApplicationUICulture = new CultureInfo(_culture);
		}
	}
}
