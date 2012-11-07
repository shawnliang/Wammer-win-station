using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StationSystemTray.Src.Dialog
{
	class OldUserWizardDialog : StepByStepWizardDialog
	{
		private PhotoSearch m_photoSearch = new PhotoSearch();

		public OldUserWizardDialog()
		{
			InitializeComponent();

			wizardControl.SetWizardPages(new StepPageControl[]
			{
				new LoginControl(new StreamLogin()),
				new ChoosePlanControl(),
				new ImportFolderAndMediaControl(m_photoSearch),
				new PersonalCloudStatusControl(new PersonalCloudStatusService())
			});

			m_photoSearch.StartSearchAsync();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			this.wizardControl.Size = new System.Drawing.Size(555, 400);
			// 
			// nextButton
			// 
			this.nextButton.Location = new System.Drawing.Point(432, 409);
			this.nextButton.Size = new System.Drawing.Size(110, 32);
			// 
			// prevButton
			// 
			this.prevButton.Location = new System.Drawing.Point(10, 409);
			this.prevButton.Size = new System.Drawing.Size(110, 32);
			// 
			// OldUserWizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(554, 453);
			this.Name = "OldUserWizardDialog";
			this.Text = "Stream (0 of 0)";
			this.ResumeLayout(false);

		}
	}
}
