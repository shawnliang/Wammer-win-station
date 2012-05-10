using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Wammer.Station.Management;
using Wammer.Station;
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


		#region Private Static Property
		private static Mutex m_Mutex { get; set; }
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
			m_Mutex = new Mutex(true, "StationSystemTray", out isFirstCreated);

			ApplyInstalledCulture();

            if (!isFirstCreated)
            {
                var currentProcess = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

                foreach (var process in processes)
                {
                    if (process.Id == currentProcess.Id)
                        continue;

					IntPtr handle = Win32Helper.FindWindow(null, (new MainForm(true)).WindowsTitle);

                    if (handle == IntPtr.Zero)
                        return;

					Win32Helper.SendMessage(handle, 0x401, IntPtr.Zero, IntPtr.Zero);
                    return;
                }
                return;
            }
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool initMinimized = (args != null && args.Length > 0 && args[0] == "--minimized");

            Application.Run(new MainForm(initMinimized));
		}

		private static void ApplyInstalledCulture()
		{
			var culture = (string)StationRegistry.GetValue("Culture", null);

			if (culture != null)
			{
				var cultureInfo = new CultureInfo(culture);
				var currentThread = Thread.CurrentThread;

				currentThread.CurrentUICulture = cultureInfo;
				currentThread.CurrentCulture = cultureInfo;
			}
		}
	}
}
