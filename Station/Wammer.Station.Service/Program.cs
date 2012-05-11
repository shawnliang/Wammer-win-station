using System;
using System.Linq;
using System.ServiceProcess;

namespace Wammer.Station.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] argv)
		{
			if (argv.Any<string>((item)=>{ return item.Equals("-i", StringComparison.CurrentCultureIgnoreCase);}))
			{
				Console.WriteLine("Install Waveface station performance counters");
				Wammer.PerfMonitor.PerfCounterInstaller.Install();
			}

			if (argv.Any<string>((item) => { return item.Equals("-c", StringComparison.CurrentCultureIgnoreCase); }))
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
