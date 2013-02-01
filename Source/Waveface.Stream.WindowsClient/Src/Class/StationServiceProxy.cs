using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net;

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
			var stationProcess = Process.GetProcessesByName("Station.Service");
			Array.ForEach(stationProcess, (proc) => proc.Kill());
		}

		public void StartService()
		{
			stationProc = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo("Station.Service.exe", "-c")
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
			var begin = DateTime.Now;
			stationProc.Exited -= stationProc_Exited;

			if (stationProc != null)
			{
				while (!stationProc.HasExited)
					killProcess(stationProc);
			}

			var dur = (DateTime.Now - begin).TotalMilliseconds;
			LogManager.GetLogger(typeof(StationServiceProxy)).InfoFormat("Close station process successfully. {0} ms.", dur);
		}

		private void stationProc_Exited(object sender, EventArgs e)
		{
			//TODO: pass this event to other handler? or ??
			LogManager.GetLogger(typeof(StationServiceProxy)).ErrorFormat("Station process is closed. Exit code : {0}", stationProc.ExitCode);
		}
		
		

		private bool killProcess(Process proc)
		{
			try
			{
				proc.Kill();
				proc.WaitForExit(1000);
			}
			catch (Exception e)
			{
				if (!proc.HasExited)
					LogManager.GetLogger(typeof(StationServiceProxy)).Warn("Kill process exception: " + proc.ToString(), e);
			}

			return proc.HasExited;
		}
	}
}
