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
		public FeatureSet SelectedFeature { get; private set; }

		public FeatureSelectionStep()
		{
			InitializeComponent();
			SelectedFeature = FeatureSet.None;
		}

		private void FeatureSelectionStep_Entering(object sender, ChangeStepEventArgs e)
		{
			SelectedFeature = FeatureSet.StationAndClient;
		}

		private void radioClientAndStation_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioClientAndStation.Checked)
			{
				SelectedFeature = FeatureSet.StationAndClient;
			}
		}

		private void radioClientOnly_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioClientOnly.Checked)
			{
				SelectedFeature = FeatureSet.ClientOnly;
			}
		}
	}
}
