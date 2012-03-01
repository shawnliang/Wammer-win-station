using System;
using System.IO;
using System.Diagnostics;
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class FinishStep : ModernInfoStep
	{
		private InstallationMode mode;

		public FinishStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
		}

		private void FinishStep_Entered(object sender, EventArgs e)
		{
			Wizard.BackButton.Enabled = false;
		}

		private void FinishStep_Finish(object sender, ChangeStepEventArgs e)
		{
		}

		private void FinishStep_Entering(object sender, ChangeStepEventArgs e)
		{
			if (mode == InstallationMode.Uninstall)
			{
				label_Hint.Hide();
			}
			else if (mode == InstallationMode.Install)
			{
				StartStationUIExe();

				Wizard.Finish();
			}
			else if (mode == InstallationMode.Upgrade ||
				mode == InstallationMode.Reinstall)
			{
				StartStationUIExe();

				try
				{
					if (!Migration.DriverRegistered())
					{
						Wizard.Finish();
					}
				}
				catch (Exception)
				{
					// skip mongodb connection force closed exception
				}
			}
		}

		private static void StartStationUIExe()
		{
			string installDir = MsiConnection.Instance.GetPath("INSTALLLOCATION");
			string stationUI = Path.Combine(installDir, "StationUI.exe");
			using (Process p = new Process())
			{
				p.StartInfo = new ProcessStartInfo(stationUI);
				// Installer is run as admistrator but we don't want stationUI inherits such 
				// privillege. So we specify "RunAsUser" below.
				p.StartInfo.Verb = "runasuser";
				p.Start();
			}
		}
	}
}
