
namespace Waveface.Stream.WindowsClient
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
				//new PersonalCloudStatusControl(new PersonalCloudStatusService())
				new PersonalCloudStatusControl2(new PersonalCloudStatusService())
			});

			m_photoSearch.StartSearchAsync();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// prevButton
			// 
			this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			// 
			// OldUserWizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(554, 453);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Name = "OldUserWizardDialog";
			this.Text = "Stream (0 of 0)";
			this.ResumeLayout(false);

		}
	}
}
