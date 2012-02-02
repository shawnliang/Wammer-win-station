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
		private FeatureSelectionStep featureStep;

		public FinishStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
			this.featureStep = null;
		}

		public FinishStep(InstallationMode mode, FeatureSelectionStep featureStep)
		{
			InitializeComponent();
			this.mode = mode;
			this.featureStep = featureStep;
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
			else if (mode == InstallationMode.Install &&
				featureStep != null &&
				featureStep.SelectedFeature == FeatureSet.StationAndClient)
			{
				string installDir = MsiConnection.Instance.GetPath("INSTALLLOCATION");
				string stationUI = Path.Combine(installDir, "StationUI.exe");
				Process.Start(stationUI).Close();

				Wizard.Finish();
			}
			else if (mode == InstallationMode.Upgrade &&
				featureStep != null &&
				featureStep.SelectedFeature == FeatureSet.StationAndClient)
			{
				string installDir = MsiConnection.Instance.GetPath("INSTALLLOCATION");
				string stationUI = Path.Combine(installDir, "StationUI.exe");
				Process.Start(stationUI).Close();

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
			else if (mode == InstallationMode.Reinstall && Migration.HasFeaure("MainFeature"))
			{
				string installDir = MsiConnection.Instance.GetPath("INSTALLLOCATION");
				string stationUI = Path.Combine(installDir, "StationUI.exe");
				Process.Start(stationUI).Close();

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
	}
}
