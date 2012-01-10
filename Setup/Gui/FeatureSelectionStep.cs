using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class FeatureSelectionStep : ModernActionStep
	{
		public FeatureSelectionStep()
		{
			InitializeComponent();
		}

		private void ftMain_AfterSelect(object sender, TreeViewEventArgs e)
		{
		}

		private void FeatureSelectionStep_Entering(object sender, ChangeStepEventArgs e)
		{
			//if (!Wizard.GetVariable<bool>("CustomInstallation"))
			//	Wizard.ContinueMove();

		}

		private void radioClientAndStation_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioClientAndStation.Checked)
			{
				foreach (Feature feature in MsiConnection.Instance.Features)
				{
					if (feature.Id == "MainFeature")
						feature.State = FeatureState.Installed;
				}
			}
		}

		private void radioClientOnly_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioClientOnly.Checked)
			{
				foreach (Feature feature in MsiConnection.Instance.Features)
				{
					if (feature.Id == "MainFeature")
						feature.State = FeatureState.NotInstalled;
				}
			}
		}
	}
}
