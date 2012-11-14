using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InstallationLocationStep : ModernActionStep
	{
		public InstallationLocationStep()
		{
			InitializeComponent();
		}

		private void InstallationLocationStep_Entering(object sender, ChangeStepEventArgs e)
		{
			//if (!Wizard.GetVariable<bool>("CustomInstallation"))
			//    Wizard.ContinueMove();
		}
	}
}
