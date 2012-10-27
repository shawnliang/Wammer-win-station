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

namespace StationSystemTray
{
	public partial class FirstUseWizardDialog : StepByStepWizardDialog
	{
		#region Var
		private InstallAppMonitor m_installAppMonitor;
		private PhotoSearch m_photoSearch;
		#endregion


		#region Private Property
		private string m_SessionToken { get; set; }
		#endregion


		public FirstUseWizardDialog(string user_id, string sessionToken)
			: base()
		{
			InitializeComponent();
			m_SessionToken = sessionToken;
			m_installAppMonitor = new InstallAppMonitor(user_id);
			m_photoSearch = new PhotoSearch(m_SessionToken);

			var buildPersonalCloud = new BuildPersonalCloudUserControl();
			buildPersonalCloud.OnAppInstall += m_installAppMonitor.OnAppInstall;
			buildPersonalCloud.OnAppInstallCanceled += m_installAppMonitor.OnAppInstallCanceled;

			wizardControl.SetWizardPages(new AbstractStepPageControl[]
			{
				buildPersonalCloud,
				new FileImportControl(m_photoSearch, SynchronizationContext.Current),
				new ServiceImportControl(new FacebookConnectableService(user_id,sessionToken,StationAPI.API_KEY)),
				new CongratulationControl()
			});

			m_photoSearch.StartSearchAsync();
		}
	}
}
