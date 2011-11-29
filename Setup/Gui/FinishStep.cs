using System;
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
			if (cbRunNow.Checked)
				Process.Start(string.Format(Gui.Properties.Resources.FinishStepCommand, MsiConnection.Instance.GetPath("INSTALLLOCATION")));
		}

		private void FinishStep_Entering(object sender, ChangeStepEventArgs e)
		{
			cbRunNow.Visible = (this.mode == InstallationMode.Install);
		}
	}
}
