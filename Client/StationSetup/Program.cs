using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Wammer.Station;
using Waveface.Localization;

namespace StationSetup
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
			//CultureManager.ApplicationUICulture = new CultureInfo("en-US");
			//CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (WavefaceWindowsClientHelper.IsAlreadyResistered())
			{
				WavefaceWindowsClientHelper.StartWavefaceWindowsClient("", "");
				return;
			}

			SignInForm form = new SignInForm();
    
            Application.Run(form);
		}
	}

	public class WavefaceWindowsClientHelper
	{
		public static bool IsAlreadyResistered()
		{
			return Wammer.Station.Management.StationController.GetOwner() != null;
		}

		public static void StartWavefaceWindowsClient(string email, string password)
		{
			string ProgramDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string WavefaceWindowsClientPath = Path.Combine(ProgramDir, "WavefaceWindowsClient.exe");

			try
			{
				if ((email == string.Empty) || (password == string.Empty))
				{
					Process.Start(WavefaceWindowsClientPath, null);
				}
				else
				{
					Process.Start(WavefaceWindowsClientPath, email + " " + password);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to start Waveface Windows Client: " + e.Message, "Waveface",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
