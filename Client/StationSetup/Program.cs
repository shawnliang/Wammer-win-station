using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Wammer.Station;

namespace StationSetup
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


			if (WammerZHelper.IsAlreadyResistered())
			{
				WammerZHelper.StartWammerZ();
				return;
			}

			Application.Run(new SignInForm());
		}
	}

	public class WammerZHelper
	{
		public static void SetRegistered()
		{
		}

		public static bool IsAlreadyResistered()
		{
			return Wammer.Station.Management.StationController.GetOwner() != null;
		}

		public static void StartWammerZ()
		{
			string ProgramDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string WammerZPath = Path.Combine(ProgramDir, "WammerZ.exe");
			try
			{
				Process.Start(WammerZPath, null);
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to start " + WammerZPath, "Program error");
			}
		}
	}
}
