using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Wammer.Station.Management;

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
			//StationController.StationOnline();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
