#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using StationSetup;
using Wammer.Station.Management;
using Waveface.Component.MultiPage;
using Waveface.Localization;

#endregion

namespace Wammer.Station
{
	public partial class DropboxForm : Form
	{
		private const string HOST = "https://waveface.com/";
		private const string DEV_HOST = "http://develop.waveface.com:4343/";

		private bool m_canExit;
		private bool m_doAutoPost;
		private string m_email;
		private string m_password;
		private string m_dropboxOAuthUrl = string.Empty;
		private bool m_verifyOK;
		private bool m_verifying;
		private bool m_autoPostOK;
		
		public DropboxForm()
		{
			InitializeComponent();

			// The constructor is used when user configures dropbox from preference page 
			// after installation.

			// Page: welcome
			// If user clicks "skip" just close the form
			btn_Welcome_Skip.Click -= this.btn_Welcome_Skip_Click;
			btn_Welcome_Skip.Click += this.btn_Complete;

			// Page: connect to dropbox successfully
			// If user clicks "OK" just close the form
			btn_ConnectionSuccessfully_OpenWaveface.Text = I18n.L.T("Btn_OK");
			btn_ConnectionSuccessfully_OpenWaveface.Click -= this.btn_ConnectionSuccessfully_OpenWaveface_Click;
			btn_ConnectionSuccessfully_OpenWaveface.Click += this.btn_Complete;

			// Page: connect to dropbox unsuccessfully
			// If user clicks "skip" just close the form
			btn_ConnectionFailed_Skip.Click -= this.btn_ConnectionFailed_Skip_Click;
			btn_ConnectionFailed_Skip.Click += this.btn_Complete;

			// Page: install dropbox unsuccessfully
			btn_InstallationFailedSkip_Skip.Click -= this.btn_InstallationFailed_Skip_Click;
			btn_InstallationFailedSkip_Skip.Click += this.btn_Complete;

			// Just close the form if it needs to
			this.FormClosing -= this.DropboxForm_FormClosing;

			gotoPage(Page_Welcome);
		}

		public DropboxForm(string email, string password)
		{
			m_email = email;
			m_password = password;

			InitializeComponent();

			gotoPage(Page_Welcome); // 1-1
		}

		private void DropboxForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_autoPostOK)
				return;

			if (m_verifying)
			{
				e.Cancel = true;
				return;
			}

			if (m_doAutoPost)
			{
				e.Cancel = true;
				return;
			}

			if (m_canExit)
			{
				OpenWindowsClient();

				e.Cancel = true;
				return;
			}
			else
			{
				DialogResult _dr = MessageBox.Show(I18n.L.T("QuitDropboxSetupProcess"), "Waveface",
												   MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

				if (_dr == DialogResult.OK)
				{
					gotoPage(Page_SetupCompleted); // 1-2
				}

				e.Cancel = true;
			}
		}

		private void gotoPage(MultiPanelPage page)
		{
			multiPanel.SelectedPage = page;

			if (page == Page_SetupCompleted)
			{
				btn_SetupCompleted_OpenWaveface.Focus();
			}
			else if (page == Page_Welcome)
			{
				btn_Welcome_UseDropbox.Focus();
			}
			else if (page == Page_Linkage_1)
			{
				btn_Linkage_1_Connect.Focus();
			}
			else if (page == Page_Linkage_2)
			{
				btn_Linkage_2_Verify.Focus();
			}

			if ((page == Page_SetupCompleted) || (page == Page_ConnectionSuccessfully))
				m_canExit = true;
		}

		private void btn_Welcome_Skip_Click(object sender, EventArgs e)
		{
			gotoPage(Page_SetupCompleted); // 1-2
		}

		private void btn_Welcome_UseDropbox_Click(object sender, EventArgs e)
		{
			if (CheckDropboxInstalled())
			{
				gotoPage(Page_Linkage_1); // 3-1
			}
			else
			{
				gotoPage(Page_InstallDropbox_1); // 2-1
			}
		}

		private void btn_InstallDropbox_1_Install_Click(object sender, EventArgs e)
		{
			gotoPage(Page_InstallDropbox_2); // 2-2

			OpenDropboxInstallWeb();
		}

		private void btn_InstallDropbox_2_InstallAgain_Click(object sender, EventArgs e)
		{
			OpenDropboxInstallWeb();
		}

		private void btn_InstallDropbox_2_continue_Click(object sender, EventArgs e)
		{
			if (CheckDropboxInstalled())
			{
				gotoPage(Page_Linkage_1); // 3-1
			}
			else
			{
				gotoPage(Page_InstallationFailed); // 2-3
			}
		}

		private void btn_ConnectionFailed_Skip_Click(object sender, EventArgs e)
		{
			gotoPage(Page_SetupCompleted); //1-2
		}

		private void btn_ConnectionFailed_Retry_Click(object sender, EventArgs e)
		{
			try
			{
				OpenLinkageWebPage();
				gotoPage(Page_Linkage_2); // 3-1
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(I18n.L.T("ConnectCloudError"), "Waveface");
			}
		}

		private void btn_InstallationFailed_Skip_Click(object sender, EventArgs e)
		{
			gotoPage(Page_SetupCompleted); // 1-2
		}

		private void btn_InstallationFailed_Retry_Click(object sender, EventArgs e)
		{
			gotoPage(Page_InstallDropbox_1);
		}

		private void btn_Linkage_1_Connect_Click(object sender, EventArgs e)
		{
			try
			{
				OpenLinkageWebPage();

				gotoPage(Page_Linkage_2); //3-2
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(I18n.L.T("ConnectCloudError"), "Waveface");
			}
		}

		private void btn_Linkage_2_ConnectAgain_Click(object sender, EventArgs e)
		{
			try
			{
				OpenLinkageWebPage();
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(I18n.L.T("ConnectCloudError"), "Waveface");
			}
		}

		private void btn_Linkage_2_Verift_Click(object sender, EventArgs e)
		{
			gotoPage(Page_Verifying); //4-1

			Application.DoEvents();

			m_verifying = true;

			backgroundWorkerVerifying.RunWorkerAsync();
		}

		private void btn_Complete(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_ConnectionSuccessfully_OpenWaveface_Click(object sender, EventArgs e)
		{
			OpenWindowsClient();
		}
		  
		private void btn_SetupCompleted_OpenWaveface_Click(object sender, EventArgs e)
		{
			OpenWindowsClient();
		}

		private void btn_SetupCompleted_InstallAgain_Click(object sender, EventArgs e)
		{
			m_canExit = false;

			gotoPage(Page_Welcome); //1-1
		}

		#region Misc

		private bool CheckDropboxInstalled()
		{
			List<StorageStatus> _storageStatuses = StationController.DetectCloudStorage();

			if (_storageStatuses.Count == 0)
				return false;

			foreach (StorageStatus _status in _storageStatuses)
			{
				if (_status.type == "dropbox")
					return true;
			}

			return false;
		}

		private void OpenWindowsClient()
		{
			//gotoPage(Page_DefaultPosts);

			m_doAutoPost = true;

			backgroundWorkerDefaultPosts.RunWorkerAsync();
		}

		private void OpenDropboxInstallWeb()
		{
			string cloudURL = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "cloudBaseURL", "");
			if (cloudURL.Contains("develop"))
				Process.Start(DEV_HOST + "to?url=" + HttpUtility.UrlEncode("http://www.dropbox.com/"), null);
			else
				Process.Start(HOST + "to?url=" + HttpUtility.UrlEncode("http://www.dropbox.com/"), null);
		}

		private void OpenLinkageWebPage()
		{
			if (m_dropboxOAuthUrl == string.Empty)
			{
				m_dropboxOAuthUrl = StationController.GetDropboxOAuthUrl();
			}

			Process.Start(m_dropboxOAuthUrl, null);
		}

		#endregion

		#region BackgroundWorker

		private void backgroundWorkerDefaultPosts_DoWork(object sender, DoWorkEventArgs e)
		{
		}

		private void backgroundWorkerDefaultPosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			m_doAutoPost = false;

			SystemTrayHelper.StartSystemTrayProgram();

			m_autoPostOK = true;

			Close();
		}

		private void backgroundWorkerVerifying_DoWork(object sender, DoWorkEventArgs e)
		{
			m_verifyOK = true;

			try
			{
				StationController.ConnectDropbox(1024 * 1024 * 500); //500MB
			}
			catch (DropboxNoSyncFolderException)
			{
				m_verifyOK = false;
			}
			catch (DropboxWrongAccountException)
			{
				m_verifyOK = false;
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(I18n.L.T("ConnectCloudError"), "Waveface");
				m_verifyOK = false;
			}
			catch
			{
				m_verifyOK = false;
			}
		}

		private void backgroundWorkerVerifying_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!m_verifying)
				return;

			m_verifying = false;

			if (m_verifyOK)
			{
				gotoPage(Page_ConnectionSuccessfully); //5-1
			}
			else
			{
				gotoPage(Page_ConnectionFailed); //4-2
			}

			Application.DoEvents();
		}

		#endregion

		private void ChengeCulture_DoubleClick(object sender, EventArgs e)
		{
			if (CultureManager.ApplicationUICulture.Name == "en-US")
			{
				CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");
				return;
			}

			if (CultureManager.ApplicationUICulture.Name == "zh-TW")
			{
				CultureManager.ApplicationUICulture = new CultureInfo("en-US");
				return;
			}
		}

		private void lb_ConnectionFailed_2_Click(object sender, EventArgs e)
		{

		}

		private void lb_Welcome_2_Click(object sender, EventArgs e)
		{

		}

		private void lb_Welcome_1_Click(object sender, EventArgs e)
		{

		}

		private void lb_InstallationFailed_1_Click(object sender, EventArgs e)
		{

		}

		private void lb_InstallationFailed_2_Click(object sender, EventArgs e)
		{

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_canExit = false;

			gotoPage(Page_Welcome); //1-1
		}

		private void btnCancelVerifying_Click(object sender, EventArgs e)
		{
			m_verifying = false;

			backgroundWorkerVerifying.CancelAsync();
			gotoPage(Page_ConnectionFailed);
		}
	}
}