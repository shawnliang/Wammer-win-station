using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
	public enum FeatureSet
	{
		None,
		StationAndClient,
		ClientOnly
	}


	[System.ComponentModel.ToolboxItem(false)]
	public partial class FeatureSelectionStep : ModernActionStep
	{
		public static FeatureSet SelectedFeature { get; set; }

		static FeatureSelectionStep()
		{
			SelectedFeature = FeatureSet.None;
		}

		public FeatureSelectionStep()
		{
			InitializeComponent();
		}

		private void FeatureSelectionStep_Entering(object sender, ChangeStepEventArgs e)
		{
			SelectedFeature = FeatureSet.StationAndClient;
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

				SelectedFeature = FeatureSet.StationAndClient;
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

				SelectedFeature = FeatureSet.ClientOnly;
			}
		}
	}
}
