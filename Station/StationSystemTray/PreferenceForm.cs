using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections.Generic;

using Wammer.Station.Management;
using Wammer.Model;
using Wammer.Station;
using Wammer.Cloud;

namespace StationSystemTray
{
	#region PreferenceForm

	public partial class PreferenceForm : Form
	{
		public static log4net.ILog logger = log4net.LogManager.GetLogger("PreferenceForm");
		private const string AUTO_RUN_VALUE_NAME = @"WavefaceStation";

		public string WebURL
		{
			get {
				if (CloudServer.BaseUrl.Contains("api.waveface.com"))
					return "https://waveface.com";
				else if (CloudServer.BaseUrl.Contains("develop.waveface.com"))
					return "http://develop.waveface.com:4343";
				else
					return "https://waveface.com";
			}
		}

		private string m_stationToken;
		private Driver m_driver;

		private int loadingActions;

		private Messenger messenger;

		private GetStationStatusUIController uictrlGetStationStatus;
		private LoadDropboxUIController uictrlLoadDropbox;
		private LoadStorageUsageUIController uictrlLoadStorageUsage;
		private ConnectDropboxUIController uictrlConnectDropbox;
		private UnlinkDropboxUIController uictrlUnlinkDropbox;
		private TestConnectionUIController uictrlTestConnection;

		MainForm mainform;

		public PreferenceForm(MainForm mainform)
		{
			this.mainform = mainform;
			this.loadingActions = 0;

			m_stationToken = StationCollection.Instance.FindOne().SessionToken;
			m_driver = DriverCollection.Instance.FindOne();

			messenger = new Messenger(this);

			uictrlGetStationStatus = new GetStationStatusUIController(this);
			uictrlGetStationStatus.UICallback += this.GetStationStatusUICallback;
			uictrlGetStationStatus.UIError += this.GetStationStatusUIError;

			uictrlLoadDropbox = new LoadDropboxUIController(this);
			uictrlLoadDropbox.UICallback += this.LoadDropboxUICallback;
			uictrlLoadDropbox.UIError += this.LoadDropboxUIError;

			uictrlLoadStorageUsage = new LoadStorageUsageUIController(this);
			uictrlLoadStorageUsage.UICallback += this.LoadStorageUsageUICallback;
			uictrlLoadStorageUsage.UIError += this.LoadStorageUsageUIError;

			uictrlConnectDropbox = new ConnectDropboxUIController(this, uictrlLoadDropbox);
			uictrlConnectDropbox.UICallback += this.ConnectDropboxUICallback;
			uictrlConnectDropbox.UIError += this.ConnectDropboxUIError;

			uictrlUnlinkDropbox = new UnlinkDropboxUIController(this, uictrlLoadDropbox);
			uictrlUnlinkDropbox.UICallback += this.UnlinkDropboxUICallback;
			uictrlUnlinkDropbox.UIError += this.UnlinkDropboxUIError;

			uictrlTestConnection = new TestConnectionUIController(this);
			uictrlTestConnection.UICallback += this.TestConnectionUICallback;
			uictrlTestConnection.UIError += this.TestConnectionUIError;

			InitializeComponent();
		}

		private void PreferenceForm_Load(object sender, EventArgs e)
		{
			StartWaitCursor(3);

			lblUserName.Text = m_driver.email;
			string _execPath = Assembly.GetExecutingAssembly().Location;
			FileVersionInfo version = FileVersionInfo.GetVersionInfo(_execPath);
			lblCopyRight.Text = version.LegalCopyright + " All Rights Reserved.";
			lblVersion.Text = version.FileVersion;

			lblLocalStorageUsage.Text = "";
			lblDeviceName.Text = "";
			uictrlGetStationStatus.PerformAction();

			btnDropboxAction.Enabled = false;
			btnDropboxAction.Text = "";
			label_dropboxAccount.Text = "";
			uictrlLoadDropbox.PerformAction();

			label_MonthlyLimitValue.Text = "";
			label_DaysLeftValue.Text = "";
			label_UsedCountValue.Text = "";
			barCloudUsage.Value = 0;
			uictrlLoadStorageUsage.PerformAction(m_driver.session_token);

			LoadAutoStartCheckbox();
		}

		private void GetStationStatusUICallback(object sender, SimpleEventArgs evt)
		{
			GetStatusResponse stationStatus = (GetStatusResponse)evt.param;

			long _usedSize = 0;

			foreach (DiskUsage _du in stationStatus.station_status.diskusage)
			{
				_usedSize += _du.used;
			}

			lblLocalStorageUsage.Text = string.Format("{0:0.0} MB", _usedSize / (1024 * 1024));
			lblDeviceName.Text = stationStatus.station_status.computer_name;

			RestoreCursor();
		}

		private void GetStationStatusUIError(object sender, SimpleEventArgs evt)
		{
			lblLocalStorageUsage.Text = I18n.L.T("NoData");
			lblDeviceName.Text = I18n.L.T("NoData");

			Exception ex = (Exception)evt.param;
			if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}

			RestoreCursor();
		}

		private void LoadDropboxUICallback(object sender, SimpleEventArgs evt)
		{
			ListCloudStorageResponse cloudStorage = (ListCloudStorageResponse)evt.param;

			if (cloudStorage.cloudstorages.Count > 0 && cloudStorage.cloudstorages[0].connected)
			{
				btnDropboxAction.Text = I18n.L.T("DropboxUI_Disconnect");
				label_dropboxAccount.Text = cloudStorage.cloudstorages[0].account;
				btnDropboxAction.Click -= btnConnectDropbox_Click;
				btnDropboxAction.Click -= btnUnlinkDropbox_Click;
				btnDropboxAction.Click += btnUnlinkDropbox_Click;
			}
			else
			{
				btnDropboxAction.Text = I18n.L.T("DropboxUI_ConnectNow");
				label_dropboxAccount.Text = I18n.L.T("MonthlyUsage_NotConnectedYet");
				btnDropboxAction.Click -= btnUnlinkDropbox_Click;
				btnDropboxAction.Click -= btnConnectDropbox_Click;
				btnDropboxAction.Click += btnConnectDropbox_Click;
			}

			btnDropboxAction.Enabled = true;

			RestoreCursor();
		}

		private void LoadDropboxUIError(object sender, SimpleEventArgs evt)
		{
			btnDropboxAction.Text = I18n.L.T("DropboxUI_ConnectNow");
			label_dropboxAccount.Text = I18n.L.T("MonthlyUsage_NotConnectedYet");

			btnDropboxAction.Click -= btnUnlinkDropbox_Click;
			btnDropboxAction.Click -= btnConnectDropbox_Click;
			btnDropboxAction.Click += btnConnectDropbox_Click;

			btnDropboxAction.Enabled = true;

			Exception ex = (Exception)evt.param;
			if (ex is AuthenticationException)
			{
				mainform.CurrentState.SessionExpired();
			}
			else if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}

			RestoreCursor();
		}

		private void LoadStorageUsageUICallback(object sender, SimpleEventArgs evt)
		{
			StorageUsageResponse storageUsage = (StorageUsageResponse)evt.param;

			long quota = storageUsage.storages.waveface.quota.month_total_objects;
			long usage = storageUsage.storages.waveface.usage.month_total_objects;
			int daysLeft = storageUsage.storages.waveface.interval.quota_interval_left_days;

			if (quota < 0)
				label_MonthlyLimitValue.Text = I18n.L.T("MonthlyUsage_Unlimited");
			else
				label_MonthlyLimitValue.Text = quota.ToString();

			label_DaysLeftValue.Text = daysLeft.ToString();
			label_UsedCountValue.Text = usage.ToString();

			if (quota < 0)
				barCloudUsage.Value = (int)(usage * 100 / int.MaxValue);
			else
				barCloudUsage.Value = (int)(usage * 100 / quota);

			RestoreCursor();
		}

		private void LoadStorageUsageUIError(object sender, SimpleEventArgs evt)
		{
			label_MonthlyLimitValue.Text = I18n.L.T("NoData");
			label_DaysLeftValue.Text = I18n.L.T("NoData");
			label_UsedCountValue.Text = I18n.L.T("NoData");

			Exception ex = (Exception)evt.param;
			if (ex is AuthenticationException)
			{
				mainform.CurrentState.SessionExpired();
			}
			else if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}

			RestoreCursor();
		}

		private void ConnectDropboxUICallback(object sender, SimpleEventArgs evt)
		{
			btnDropboxAction.Enabled = true;
		}

		private void ConnectDropboxUIError(object sender, SimpleEventArgs evt)
		{
			messenger.ShowMessage(I18n.L.T("ConnectCloudStorageFail"));
			btnDropboxAction.Enabled = true;

			Exception ex = (Exception)evt.param;
			if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}
		}

		private void UnlinkDropboxUICallback(object sender, SimpleEventArgs evt)
		{
			btnDropboxAction.Enabled = true;
		}

		private void UnlinkDropboxUIError(object sender, SimpleEventArgs evt)
		{
			messenger.ShowMessage(I18n.L.T("UnlinkCloudStorageFail"));
			btnDropboxAction.Enabled = true;

			Exception ex = (Exception)evt.param;
			if (ex is AuthenticationException)
			{
				mainform.CurrentState.SessionExpired();
			}
			else if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}
		}

		private void TestConnectionUICallback(object sender, SimpleEventArgs evt)
		{
			labelConnectionStatus.Text = I18n.L.T("Connected");
			btnTestConnection.Enabled = true;
		}

		private void TestConnectionUIError(object sender, SimpleEventArgs evt)
		{
			labelConnectionStatus.Text = I18n.L.T("NotConnected");
			btnTestConnection.Enabled = true;

			Exception ex = (Exception)evt.param;
			if (ex is AuthenticationException)
			{
				mainform.CurrentState.SessionExpired();
			}
			else if (ex is ConnectToCloudException)
			{
				mainform.CurrentState.Offlined();
			}
		}

		private void LoadAutoStartCheckbox()
		{
			checkBox_autoStartWaveface.Checked = Wammer.Utility.AutoRun.Exists(AUTO_RUN_VALUE_NAME);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		public void btnUnlinkDropbox_Click(object sender, EventArgs e)
		{
			StartWaitCursor(1);

			btnDropboxAction.Enabled = false;
			uictrlUnlinkDropbox.PerformAction();
		}

		public void btnConnectDropbox_Click(object sender, EventArgs e)
		{
			StartWaitCursor(1);

			btnDropboxAction.Enabled = false;
			uictrlConnectDropbox.PerformAction();
		}

		private void label_switchAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			DialogResult _confirm = MessageBox.Show(I18n.L.T("Main.ChangeOwnerWarning", lblUserName.Text), "Waveface",
												   MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (_confirm == DialogResult.No)
				return;

			Cursor = Cursors.WaitCursor;

			try
			{
				StationController.RemoveOwner(m_stationToken);

				MessageBox.Show(I18n.L.T("Main.ChangeOwnerSuccess", m_driver.email), "waveface");

				string statioinUI = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "StationUI.exe");
				Process.Start(statioinUI);

				Application.Exit();
			}
			catch (Exception _e)
			{
				MessageBox.Show(I18n.L.T("ChangeOwnerError") + " : " + _e, "waveface");
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void checkBox_autoStartWaveface_Click(object sender, EventArgs e)
		{
			try
			{
				if (checkBox_autoStartWaveface.Checked)
				{
					string _installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					string _stationSetupPath = Path.Combine(_installDir, "StationUI.exe");

					Wammer.Utility.AutoRun.Add(AUTO_RUN_VALUE_NAME, _stationSetupPath);
				}
				else
				{
					Wammer.Utility.AutoRun.Remove(AUTO_RUN_VALUE_NAME);
				}
			}
			catch (Exception _e)
			{
				MessageBox.Show(_e.Message);
			}
		}

		private void btnEditAccount_Click(object sender, EventArgs e)
		{
			string _userProfileUrl = WebURL + "/user/profile";
			Process.Start(WebURL + "/login?cont=" + HttpUtility.UrlEncode(_userProfileUrl), null);
		}

		private void btnTestConnection_Click(object sender, EventArgs e)
		{
			btnTestConnection.Enabled = false;
			labelConnectionStatus.Text = I18n.L.T("Testing");
			uictrlTestConnection.PerformAction(m_driver);
		}

		private void linkLegalNotice_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(WebURL + "/page/privacy", null);
		}

		private void PreferenceForm_Activated(object sender, EventArgs e)
		{
			if (this.OwnedForms.Length > 0)
			{
				this.OwnedForms[0].Activate();
			}
			else if (uictrlConnectDropbox.procDropboxSetup != null)
			{
				Win32Helper.SetForegroundWindow(uictrlConnectDropbox.procDropboxSetup.MainWindowHandle);
			}
		}

		private void RestoreCursor()
		{
			int newLoadingActions = Interlocked.Decrement(ref loadingActions);
			if (newLoadingActions == 0)
			{
				Cursor = Cursors.Default;
			}
		}

		private void StartWaitCursor(int actions)
		{
			for (int i = 0; i < actions; i++)
				Interlocked.Increment(ref loadingActions);

			if (loadingActions > 0)
				Cursor = Cursors.WaitCursor;
		}
	}

	#endregion

	#region StationStatusUIController
	public class GetStationStatusUIController : SimpleUIController
	{
		public GetStationStatusUIController(PreferenceForm pform)
			: base(pform)
		{
		}

		protected override object Action(object obj)
		{
			return StationController.GetStationStatus();
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			PreferenceForm.logger.Error("Unable to get station status", ex);
		}
	}
	#endregion

	#region LoadDropboxUIController
	public class LoadDropboxUIController : SimpleUIController
	{
		public LoadDropboxUIController(PreferenceForm pform)
			: base(pform)
		{
		}

		protected override object Action(object obj)
		{
			return StationController.ListCloudStorage();
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			PreferenceForm.logger.Error("Unable to list cloud storage", ex);
		}
	}
	#endregion

	#region ConnectDropboxUIController
	public class ConnectDropboxUIController : SimpleUIController
	{
		private LoadDropboxUIController uictrlLoadDropbox;

		public Process procDropboxSetup;

		public ConnectDropboxUIController(PreferenceForm pform, 
										  LoadDropboxUIController uictrlLoadDropbox)
			: base(pform)
		{
			this.uictrlLoadDropbox = uictrlLoadDropbox;
			this.procDropboxSetup = null;
		}

		protected override object Action(object obj)
		{
			string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										   "StationUI.exe");
			procDropboxSetup = Process.Start(_execPath, "--dropbox");
			procDropboxSetup.WaitForExit();

			return null;
		}

		protected override void ActionCallback(object obj)
		{
			procDropboxSetup = null;
			uictrlLoadDropbox.PerformAction();
		}

		protected override void ActionError(Exception ex)
		{
			procDropboxSetup = null;
			PreferenceForm.logger.Error("Unable to connect cloud storage", ex);
		}
	}
	#endregion

	#region UnlinkDropboxUIController
	public class UnlinkDropboxUIController : SimpleUIController
	{
		private LoadDropboxUIController uictrlLoadDropbox;

		public UnlinkDropboxUIController(PreferenceForm pform,
										 LoadDropboxUIController uictrlLoadDropbox)
			: base(pform)
		{
			this.uictrlLoadDropbox = uictrlLoadDropbox;
		}

		protected override object Action(object obj)
		{
			StationController.DisconnectDropbox();

			return null;
		}

		protected override void ActionCallback(object obj)
		{
			uictrlLoadDropbox.PerformAction();
		}

		protected override void ActionError(Exception ex)
		{
			PreferenceForm.logger.Error("Unable to unlink Dropbox", ex);
		}
	}
	#endregion

	#region LoadStorageUsageUIController
	public class LoadStorageUsageUIController : SimpleUIController
	{
		public LoadStorageUsageUIController(PreferenceForm pform)
			: base(pform)
		{
		}

		protected override object Action(object obj)
		{
			string sessionToken = (string)obj;
			return StationController.StorageUsage(sessionToken);
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			PreferenceForm.logger.Error("Unable to get storage usage information", ex);
		}
	}
	#endregion

	#region TestConnectionUIController
	public class TestConnectionUIController : SimpleUIController
	{
		public TestConnectionUIController(PreferenceForm pform)
			: base(pform)
		{
		}

		protected override object Action(object obj)
		{
			Driver driver = (Driver)obj;
			StationController.PingMyStation(driver.session_token);

			DateTime startTime = DateTime.Now;
			do
			{
				Thread.Sleep(2000);
				GetUserResponse user = StationController.GetUser(driver.session_token, driver.user_id);

				if (user.stations.Count > 0 &&
					user.stations[0].accessible != null &&
					user.stations[0].accessible == "available")
				{
					return null;
				}

			}
			while (DateTime.Now - startTime < TimeSpan.FromSeconds(10.0));

			Thread.CurrentThread.Abort();

			return null;
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			PreferenceForm.logger.Error("Unable to test station accessibility", ex);
		}
	}
	#endregion

	#region StorageUsage

	public class StorageUsage
	{
		public long quota { get; set; }
		public long usage { get; set; }
		public int daysLeft { get; set; }
	}

	#endregion

}