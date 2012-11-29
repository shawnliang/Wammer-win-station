using System;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class LicenseStep : ModernActionStep
	{
		public LicenseStep()
		{
			InitializeComponent();
		}

		private void LicenseStep_Load(object sender, EventArgs e)
		{
			rtbLicense.Rtf = Gui.Properties.Resources.LicenseStepRtf;
		}

		private void cbAccept_CheckedChanged(object sender, EventArgs e)
		{
			Wizard.NextButton.Enabled = cbAccept.Checked;
		}
	}
}
