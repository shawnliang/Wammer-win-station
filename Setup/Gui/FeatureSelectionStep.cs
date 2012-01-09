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
			lblDescription.Text = (e.Node.Tag as Feature).Description;
		}

		private void FeatureSelectionStep_Entering(object sender, ChangeStepEventArgs e)
		{
			//if (!Wizard.GetVariable<bool>("CustomInstallation"))
			//	Wizard.ContinueMove();

			SharpSetup.Base.MsiConnection.Instance.Features.Add(new Feature(""))
		}
	}
}
