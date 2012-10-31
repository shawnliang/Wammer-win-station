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

			wizardControl.SetWizardPages(new StepPageControl[]
			{
				new IntroControl(),
				new SignUpControl(new StreamSignup()),
				new ServiceImportControl(new FacebookConnectableService()),
				new FileImportControl(m_photoSearch, SynchronizationContext.Current),
				new ImportFromPotableMediaControl(new PortableMediaService()),
				new CongratulationControl()
			});

			m_photoSearch.StartSearchAsync();
		}

	}
}
