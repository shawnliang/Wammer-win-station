using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	class StationServiceProxy
	{
		private static StationServiceProxy instance;

		private Process stationProc;

		static StationServiceProxy()
		{
			instance = new StationServiceProxy();
		}

		public static StationServiceProxy Instance
		{
			get { return instance; }
		}

		private StationServiceProxy()
		{
		}

		public void StartService()
		{
			StopService();

			var installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var stationPath = Path.Combine(installDir, "station.service.exe");

			stationProc = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo(stationPath, "-c")
				{
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				}
			};

			stationProc.Exited += stationProc_Exited;
			stationProc.Start();
		}


		public void StopService()
		{
			var stationProcess = Process.GetProcessesByName("Station.Service");
			Array.ForEach(stationProcess, (proc) =>
				{
					var processOwner = proc.GetProcessOwner();

					if (processOwner == null || !processOwner.Equals(string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName), StringComparison.CurrentCultureIgnoreCase))
						throw new Exception(Resources.SERVICE_ALREADY_USED);

					proc.SafeClose(500);
				});
		}

		private void stationProc_Exited(object sender, EventArgs e)
		{
			stationProc.Exited -= stationProc_Exited;
			//TODO: pass this event to other handler? or ??
			LogManager.GetLogger(typeof(StationServiceProxy)).ErrorFormat("Station process is closed. Exit code : {0}", stationProc.ExitCode.ToString());

			stationProc = null;
		}
	}
}
