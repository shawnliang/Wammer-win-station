
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
				//new ImportFolderAndMediaControl(m_photoSearch),
				new FileImportControl(),
				new DocImportControl(),
				new PersonalCloudStatusControl2(new PersonalCloudStatusService())
			});

			m_photoSearch.StartSearchAsync();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OldUserWizardDialog));
			this.SuspendLayout();
			// 
			// wizardControl
			// 
			resources.ApplyResources(this.wizardControl, "wizardControl");
			// 
			// nextButton
			// 
			resources.ApplyResources(this.nextButton, "nextButton");
			// 
			// prevButton
			// 
			resources.ApplyResources(this.prevButton, "prevButton");
			// 
			// OldUserWizardDialog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Name = "OldUserWizardDialog";
			this.ResumeLayout(false);

		}
	}
}
