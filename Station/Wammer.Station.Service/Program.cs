using System;
using System.Linq;
using System.ServiceProcess;
using Wammer.PerfMonitor;

namespace Wammer.Station.Service
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] argv)
		{
			if (argv.Any(item => item.Equals("-i", StringComparison.CurrentCultureIgnoreCase)))
			{
				Console.WriteLine("Install Waveface station performance counters");
				PerfCounterInstaller.Install();
			}

			if (argv.Any(item => item.Equals("-c", StringComparison.CurrentCultureIgnoreCase)))
			{
				Console.WriteLine("Wammer Station in Console Mode:");
				var svc = new StationService();
				svc.Run();
			}
			else
			{
				var ServicesToRun = new ServiceBase[]
				                    	{
				                    		new StationService()
				                    	};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}