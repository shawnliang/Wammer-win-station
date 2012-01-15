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

		public string LblLocalStorageUsageText
		{
			get { return lblLocalStorageUsage.Text; }
			set { lblLocalStorageUsage.Text = value; }
		}

		public string LblDeviceNameText
		{
			get { return lblDeviceName.Text; }
			set { lblDeviceName.Text = value; }
		}

		public Button BtnDropboxAction
		{
			get { return btnDropboxAction; }
		}

		public string LblDropboxAccountText
		{
			get { return label_dropboxAccount.Text; }
			set { label_dropboxAccount.Text = value; }
		}

		public bool FormEnabled
		{
			get { return this.Enabled; }
			set { this.Enabled = value; }
		}

		public string LblMonthlyLimitValueText
		{
			get { return label_MonthlyLimitValue.Text; }
			set { label_MonthlyLimitValue.Text = value; }
		}

		public string LblDaysLeftValueText
		{
			get { return label_DaysLeftValue.Text; }
			set { label_DaysLeftValue.Text = value; }
		}

		public string LblUsedCountValueText
		{
			get { return label_UsedCountValue.Text; }
			set { label_UsedCountValue.Text = value; }
		}

		public int BarCloudUsageValue
		{
			get { return barCloudUsage.Value; }
			set { barCloudUsage.Value = value; }
		}

		public bool BtnTestConnectionEnabled
		{
			get { return btnTestConnection.Enabled; }
			set { btnTestConnection.Enabled = value; }
		}

		public string LblConnectionStatusText
		{
			get { return labelConnectionStatus.Text; }
			set { labelConnectionStatus.Text = value; }
		}

		public string WebURL
		{
			get {
				if (CloudServer.BaseUrl == "https://api.waveface.com")
					return "https://waveface.com";
				else
					return "http://develop.waveface.com:4343";
			}
		}

		private string m_stationToken;
		private Driver m_driver;

		private Messenger messenger;

		private GetStationStatusUIController uictrlGetStationStatus;
		private LoadDropboxUIController uictrlLoadDropbox;
		private LoadStorageUsageUIController uictrlLoadStorageUsage;
		private ConnectDropboxUIController uictrlConnectDropbox;
		private UnlinkDropboxUIController uictrlUnlinkDropbox;
		private TestConnectionUIController uictrlTestConnection;

		public PreferenceForm()
		{
			m_stationToken = StationCollection.Instance.FindOne().SessionToken;
			m_driver = DriverCollection.Instance.FindOne();

			messenger = new Messenger(this);

			uictrlGetStationStatus = new GetStationStatusUIController(this, messenger);
			uictrlLoadDropbox = new LoadDropboxUIController(this, messenger);
			uictrlLoadStorageUsage = new LoadStorageUsageUIController(this, messenger);
			uictrlConnectDropbox = new ConnectDropboxUIController(this, messenger);
			uictrlUnlinkDropbox = new UnlinkDropboxUIController(this, messenger);
			uictrlTestConnection = new TestConnectionUIController(this, messenger);

			InitializeComponent();
		}

		private void PreferenceForm_Load(object sender, EventArgs e)
		{

			lblUserName.Text = m_driver.email;
			string _execPath = Assembly.GetExecutingAssembly().Location;
			FileVersionInfo version = FileVersionInfo.GetVersionInfo(_execPath);
			lblVersion.Text = version.FileVersion;

			uictrlGetStationStatus.PerformAction();
			uictrlLoadDropbox.PerformAction();
			uictrlLoadStorageUsage.PerformAction(m_driver.session_token);

			LoadAutoStartCheckbox();
		}

		public void AddButtonClickEventHandler(Button btn, EventHandler handler)
		{
			btn.Click += handler;
		}

		public void RemoveButtonClickEventHandler(Button btn, EventHandler handler)
		{
			btn.Click -= handler;
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
			uictrlUnlinkDropbox.PerformAction();
		}

		public void btnConnectDropbox_Click(object sender, EventArgs e)
		{
			uictrlConnectDropbox.PerformAction();
		}

		private void label_switchAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			DialogResult _confirm = MessageBox.Show(I18n.L.T("Main.ChangeOwnerWarning", lblUserName.Text), "Waveface",
												   MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (_confirm == DialogResult.No)
				return;

			Cursor.Current = Cursors.WaitCursor;

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
				Cursor.Current = Cursors.Default;
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
	}

	#endregion

	#region StationStatusUIController
	public class GetStationStatusUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private Messenger messenger;

		public GetStationStatusUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
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

		protected override void SetFormControls(object obj)
		{
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
		}

		protected override void UpdateUI(object obj)
		{
			pform.LblLocalStorageUsageText = "";
			pform.LblDeviceNameText = "";
		}

		protected override void UpdateUIInCallback(object obj)
		{
			GetStatusResponse stationStatus = (GetStatusResponse)obj;

			long _usedSize = 0;

			foreach (DiskUsage _du in stationStatus.station_status.diskusage)
			{
				_usedSize += _du.used;
			}

			pform.LblLocalStorageUsageText = string.Format("{0:0.0} MB", _usedSize / (1024 * 1024));
			pform.LblDeviceNameText = stationStatus.station_status.computer_name;
		}

		protected override void UpdateUIInError(Exception ex)
		{
			pform.LblLocalStorageUsageText = I18n.L.T("NoData");
			pform.LblDeviceNameText = I18n.L.T("NoData");
		}
	}
	#endregion

	#region LoadDropboxUIController
	public class LoadDropboxUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private Messenger messenger;

		public LoadDropboxUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
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

		protected override void SetFormControls(object obj)
		{
			pform.BtnDropboxAction.Enabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			ListCloudStorageResponse cloudStorage = (ListCloudStorageResponse)obj;

			if (cloudStorage.cloudstorages.Count > 0 && cloudStorage.cloudstorages[0].connected)
			{
				pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnConnectDropbox_Click);
				pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnUnlinkDropbox_Click);
				pform.AddButtonClickEventHandler(pform.BtnDropboxAction, pform.btnUnlinkDropbox_Click);
			}
			else
			{
				pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnUnlinkDropbox_Click);
				pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnConnectDropbox_Click);
				pform.AddButtonClickEventHandler(pform.BtnDropboxAction, pform.btnConnectDropbox_Click);
			}

			pform.BtnDropboxAction.Enabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				messenger.ShowLoginDialog();
				return;
			}

			pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnUnlinkDropbox_Click);
			pform.RemoveButtonClickEventHandler(pform.BtnDropboxAction, pform.btnConnectDropbox_Click);
			pform.AddButtonClickEventHandler(pform.BtnDropboxAction, pform.btnConnectDropbox_Click);

			pform.BtnDropboxAction.Enabled = true;
		}

		protected override void UpdateUI(object obj)
		{
			pform.BtnDropboxAction.Text = "";
			pform.LblDropboxAccountText = "";
		}

		protected override void UpdateUIInCallback(object obj)
		{
			ListCloudStorageResponse cloudStorage = (ListCloudStorageResponse)obj;

			if (cloudStorage.cloudstorages.Count > 0 && cloudStorage.cloudstorages[0].connected)
			{
				pform.BtnDropboxAction.Text = I18n.L.T("DropboxUI_Disconnect");
				pform.LblDropboxAccountText = cloudStorage.cloudstorages[0].account;
			}
			else
			{
				pform.BtnDropboxAction.Text = I18n.L.T("DropboxUI_ConnectNow");
				pform.LblDropboxAccountText = I18n.L.T("MonthlyUsage_NotConnectedYet");
			}
		}

		protected override void UpdateUIInError(Exception ex)
		{
			pform.BtnDropboxAction.Text = I18n.L.T("DropboxUI_ConnectNow");
			pform.LblDropboxAccountText = I18n.L.T("MonthlyUsage_NotConnectedYet");
		}
	}
	#endregion

	#region ConnectDropboxUIController
	public class ConnectDropboxUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private LoadDropboxUIController uictrlLoadDropbox;
		private Messenger messenger;

		public Process procDropboxSetup;

		public ConnectDropboxUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
			this.uictrlLoadDropbox = new LoadDropboxUIController(pform, messenger);
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

		protected override void SetFormControls(object obj)
		{
			pform.BtnDropboxAction.Enabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			messenger.ShowMessage(I18n.L.T("ConnectCloudStorageFail"));

			pform.BtnDropboxAction.Enabled = true;
		}

		protected override void UpdateUI(object obj)
		{
		}

		protected override void UpdateUIInCallback(object obj)
		{
		}

		protected override void UpdateUIInError(Exception ex)
		{
		}
	}
	#endregion

	#region UnlinkDropboxUIController
	public class UnlinkDropboxUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private LoadDropboxUIController uictrlLoadDropbox;
		private Messenger messenger;

		public UnlinkDropboxUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
			this.uictrlLoadDropbox = new LoadDropboxUIController(pform, messenger);
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

		protected override void SetFormControls(object obj)
		{
			pform.BtnDropboxAction.Enabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				messenger.ShowLoginDialog();
				return;
			}

			messenger.ShowMessage(I18n.L.T("UnlinkCloudStorageFail"));

			pform.BtnDropboxAction.Enabled = true;
		}

		protected override void UpdateUI(object obj)
		{
		}

		protected override void UpdateUIInCallback(object obj)
		{
		}

		protected override void UpdateUIInError(Exception ex)
		{
		}
	}
	#endregion

	#region LoadStorageUsageUIController
	public class LoadStorageUsageUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private Messenger messenger;

		public LoadStorageUsageUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
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

		protected override void SetFormControls(object obj)
		{
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				messenger.ShowLoginDialog();
				return;
			}
		}

		protected override void UpdateUI(object obj)
		{
			pform.LblMonthlyLimitValueText = "";
			pform.LblDaysLeftValueText = "";
			pform.LblUsedCountValueText = "";

			pform.BarCloudUsageValue = 0;
		}

		protected override void UpdateUIInCallback(object obj)
		{
			StorageUsageResponse storageUsage = (StorageUsageResponse)obj;

			long quota = storageUsage.storages.waveface.quota.month_total_objects;
			long usage = storageUsage.storages.waveface.usage.month_total_objects;
			int daysLeft = storageUsage.storages.waveface.interval.quota_interval_left_days;

			if (quota < 0)
				pform.LblMonthlyLimitValueText = I18n.L.T("MonthlyUsage_Unlimited");
			else
				pform.LblMonthlyLimitValueText = quota.ToString();

			pform.LblDaysLeftValueText = daysLeft.ToString();
			pform.LblUsedCountValueText = usage.ToString();

			pform.BarCloudUsageValue = (int)(usage * 100 / quota);
		}

		protected override void UpdateUIInError(Exception ex)
		{
			pform.LblMonthlyLimitValueText = I18n.L.T("NoData");
			pform.LblDaysLeftValueText = I18n.L.T("NoData");
			pform.LblUsedCountValueText = I18n.L.T("NoData");
		}
	}
	#endregion

	#region TestConnectionUIController
	public class TestConnectionUIController : SimpleUIController
	{
		private PreferenceForm pform;
		private Messenger messenger;

		public TestConnectionUIController(PreferenceForm pform, Messenger messenger)
			: base(pform)
		{
			this.pform = pform;
			this.messenger = messenger;
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
			while (DateTime.Now - startTime > TimeSpan.FromSeconds(10));

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

		protected override void SetFormControls(object obj)
		{
			pform.BtnTestConnectionEnabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			pform.BtnTestConnectionEnabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				messenger.ShowLoginDialog();
				return;
			}

			pform.BtnTestConnectionEnabled = true;
		}

		protected override void UpdateUI(object obj)
		{
			pform.LblConnectionStatusText = I18n.L.T("Testing");
		}

		protected override void UpdateUIInCallback(object obj)
		{
			pform.LblConnectionStatusText = I18n.L.T("Connected");
		}

		protected override void UpdateUIInError(Exception ex)
		{
			pform.LblConnectionStatusText = I18n.L.T("NotConnected");
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