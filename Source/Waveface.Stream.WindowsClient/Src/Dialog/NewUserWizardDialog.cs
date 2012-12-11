using System.Drawing;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class NewUserWizardDialog : StepByStepWizardDialog
	{
		#region Var
		private PhotoSearch m_photoSearch = new PhotoSearch();
		#endregion

		public NewUserWizardDialog()
		{
			InitializeComponent();

			Image[] tutorial = new Image[] { Resources.P1, Resources.P2, Resources.P3 };
			var intro = new IntroControl(tutorial);
			intro.CustomLabelForNextStep = "Sign Up";


			wizardControl.SetWizardPages(new StepPageControl[]
			{
				intro,
				new SignUpControl(new StreamSignup()),
				new ServiceImportControl(),
				new ChoosePlanControl(),
				new ImportFolderAndMediaControl(m_photoSearch),
				new DocImportControl(),
				new PersonalCloudStatusControl2(new PersonalCloudStatusService())
			});

			m_photoSearch.StartSearchAsync();
		}

	}
}
