
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
				new FileImportControl(),
				new DocImportControl()
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
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "OldUserWizardDialog";
			this.ResumeLayout(false);

		}
	}
}
