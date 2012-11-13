using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using Wammer.Station;

namespace StationSystemTray
{
	internal static class Program
	{
		#region Private Static Property

		private static readonly ILog m_Logger = LogManager.GetLogger("Program");
		private static Mutex m_Mutex { get; set; }

		#endregion Private Static Property
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
//#if DEBUG
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
//            Application.Run(new FirstUseWizardDialog("123"));
//            return;
//#endif

            if (args.Length == 4)
            {
                var setting = GlobalSettings.Instance;
                setting.APPID = args[0];
                setting.MutexTrayName = args[1];
                setting.MessageReceiverName = args[2];
                setting.ClientMessageReceiverName = args[3];
            }

			Waveface.Common.TaskbarHelper.SetAppId(GlobalSettings.Instance.APPID);

			Environment.CurrentDirectory = Path.GetDirectoryName(
			Assembly.GetExecutingAssembly().Location);

			XmlConfigurator.Configure();

			bool isFirstCreated;

			//Create a new mutex using specific mutex name
			m_Mutex = new Mutex(true, GlobalSettings.Instance.MutexTrayName, out isFirstCreated);

			ApplyInstalledCulture();

			if (!isFirstCreated)
			{
				var currentProcess = Process.GetCurrentProcess();
				var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

				if (processes.Any(process => process.Id != currentProcess.Id))
				{
					var handle = Win32Helper.FindWindow(GlobalSettings.Instance.MessageReceiverName, null);

					if (handle == IntPtr.Zero)
						return;

					m_Logger.Info("Call old stream to open");
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
				Thread currentThread = Thread.CurrentThread;

				currentThread.CurrentUICulture = cultureInfo;
				currentThread.CurrentCulture = cultureInfo;
			}
		}
	}
}