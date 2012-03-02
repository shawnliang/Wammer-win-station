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
			if (mode == InstallationMode.Install ||
				mode == InstallationMode.Upgrade)
			{
				StartStationUIExe();
			}
		}

		private void FinishStep_Entering(object sender, ChangeStepEventArgs e)
		{
			if (mode == InstallationMode.Uninstall)
			{
				label_Hint.Hide();
			}
		}

		private static void StartStationUIExe()
		{
			string installDir = MsiConnection.Instance.GetPath("INSTALLLOCATION");
			string stationUI = Path.Combine(installDir, "stationUI.exe");

			UACHelper.CreateProcessAsStandardUser(stationUI, "");
		}
	}
}
