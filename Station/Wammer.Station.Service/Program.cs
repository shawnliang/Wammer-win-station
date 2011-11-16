using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace Wammer.Station.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] argv)
		{
			if (argv.Length == 1 && argv[0].Equals("-c"))
			{
				Console.WriteLine("Wammer Station in Console Mode:");
				StationService svc = new StationService();
				svc.Run();
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new StationService() 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}
