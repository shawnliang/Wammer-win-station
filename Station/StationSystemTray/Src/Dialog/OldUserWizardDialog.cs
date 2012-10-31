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
			wizardControl.SetWizardPages(new StepPageControl[]
			{
				new LoginControl(new StreamLogin()),
				new FileImportControl(m_photoSearch, SynchronizationContext.Current),				
				new ImportFromPotableMediaControl(new PortableMediaService()),
				new CongratulationControl()
			});

			m_photoSearch.StartSearchAsync();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			this.wizardControl.Size = new System.Drawing.Size(589, 350);
			// 
			// nextButton
			// 
			this.nextButton.Location = new System.Drawing.Point(498, 358);
			// 
			// prevButton
			// 
			this.prevButton.Location = new System.Drawing.Point(417, 358);
			// 
			// OldUserWizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(585, 393);
			this.Name = "OldUserWizardDialog";
			this.Text = "Stream (0 of 0)";
			this.ResumeLayout(false);

		}
	}
}
