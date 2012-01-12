using System;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;
using System.IO;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InstallationStep : ModernActionStep
	{
		InstallationMode mode;
		FeatureSelectionStep featureStep;

		public InstallationStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
		}

		public InstallationStep(InstallationMode mode, FeatureSelectionStep featureStep)
		{
			InitializeComponent();
			this.mode = mode;
			this.featureStep = featureStep;
		}

		private void InstallationStep_Entered(object sender, EventArgs e)
		{
			ipProgress.StartListening();
			try
			{
				if (mode == InstallationMode.Uninstall)
				{
					MsiConnection.Instance.Uninstall();
					/*
					try
					{
						MsiConnection.Instance.Open(new Guid("{da97b6b8-4989-4dd0-964f-ac25a7d36a36}"));
						MsiConnection.Instance.Uninstall();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Uninstall");
					}
					*/
					if (File.Exists(Resources.MainMsiFile))
						MsiConnection.Instance.Open(Resources.MainMsiFile, true);
				}
				else if (mode == InstallationMode.Install)
				{
					/*
					MsiConnection.Instance.SaveAs("MainInstall");
					MsiConnection.Instance.Open("other.msi", false);
					MsiConnection.Instance.Install("");
					MsiConnection.Instance.OpenSaved("MainInstall");
					*/

					ApplyFeatureSet(featureStep.SelectedFeature);

					MsiConnection.Instance.Install();
				}
				else
					MessageBox.Show("Unknown mode");
			}
			catch (MsiException mex)
			{
				if (mex.ErrorCode != (uint)InstallError.UserExit)
					MessageBox.Show("Installation failed: " + mex.Message);
				Wizard.Finish();
			}
			ipProgress.StopListening();
			Wizard.NextStep();
		}

		public override bool CanClose()
		{
			return false;
		}

		private void ApplyFeatureSet(FeatureSet featureSet)
		{
			foreach (Feature feature in MsiConnection.Instance.Features)
			{
				if (feature.Id == "MainFeature")
				{
					feature.State = (featureSet == FeatureSet.StationAndClient) ?
												FeatureState.Installed : FeatureState.NotInstalled;
				}
			}
		}
	}
}
