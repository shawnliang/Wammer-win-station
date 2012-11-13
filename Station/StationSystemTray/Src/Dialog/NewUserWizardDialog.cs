using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Src.Class;
using StationSystemTray.Src.Control;
using System.Threading;
using StationSystemTray.Properties;

namespace StationSystemTray.Src.Dialog
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

			wizardControl.SetWizardPages(new StepPageControl[]
			{
				new IntroControl(tutorial),
				new ChoosePlanControl() { CustomLabelForNextStep = "Sign Up" },
				new SignUpControl(new StreamSignup()),
				new ServiceImportControl(new FacebookConnectableService()),
				new ImportFolderAndMediaControl(m_photoSearch),
				new PersonalCloudStatusControl(new PersonalCloudStatusService())
			});

			m_photoSearch.StartSearchAsync();
		}

	}
}
