using fastJSON;
using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;
using Waveface.Stream.WindowsClient.Src.Dialog;

namespace Waveface.Stream.WindowsClient
{
	public partial class PreferencesDialog : Form
	{
		class RemoveParam
		{
			public string user_id { get; set; }
			public string email { get; set; }
			public bool removeData { get; set; }
		}

		#region Const
		const string EMAIL_VALIDATE_MATCH_PATTERN = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";
		#endregion

		#region Static Var
		private static PreferencesDialog _instance;
		#endregion


		#region Var
		private ProcessingDialog _processingDialog;
		private AutoUpdate _updator;
		private AutoResetEvent _startRemoveEvt = new AutoResetEvent(false);
		#endregion


		#region Public Static Property
		public static PreferencesDialog Instance
		{
			get
			{
				return _instance ?? (_instance = new PreferencesDialog());
			}
		}
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ processing dialog.
		/// </summary>
		/// <value>The m_ processing dialog.</value>
		private ProcessingDialog m_ProcessingDialog
		{
			get
			{
				return _processingDialog ?? (_processingDialog = new ProcessingDialog());
			}
			set
			{
				if (_processingDialog == value)
					return;

				if (_processingDialog != null)
					_processingDialog.Dispose();

				_processingDialog = value;
			}
		}

		/// <summary>
		/// Gets the m_ updator.
		/// </summary>
		/// <value>The m_ updator.</value>
		private AutoUpdate m_Updator
		{
			get
			{
				return _updator ?? (_updator = new AutoUpdate(false));
			}
		}
		#endregion

		#region Constructor
		private PreferencesDialog()
		{
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			this.Disposed += PreferencesDialog_Disposed;

			//pbxDevice.Hide();
			devNameCtl.Hide();
			lblSyncStatus.Text = string.Empty;
			lblSyncTransferStatus.Text = string.Empty;
			lblDeviceConnectStatus.Text = string.Empty;
			//lblDeviceName.Text = string.Empty;
			label2.Text = string.Empty;

			ConnectionStatus.Instance.DeviceAdded += Instance_DeviceAdded;
			ConnectionStatus.Instance.DeviceRemoved += Instance_DeviceRemoved;

		}
		#endregion


		#region Private Method
		private void UpdateAccountInfo()
		{
			try
			{
				var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;
				tbxEmail.Text = userInfo.Email;
				tbxName.Text = userInfo.NickName;
			}
			catch (Exception)
			{
			}
		}


		private void RemoveAccount(string userID, string email, Boolean removeAllDatas)
		{
			BackgroundWorker removeAccountBgWorker = new BackgroundWorker();
			removeAccountBgWorker.DoWork += removeAccountBgWorker_DoWork;
			removeAccountBgWorker.RunWorkerCompleted += removeAccountBgWorker_RunWorkerCompleted;
			removeAccountBgWorker.RunWorkerAsync(new RemoveParam { user_id = userID, email = email, removeData = removeAllDatas });

			m_ProcessingDialog.ProcessMessage = Resources.REMOVE_ACCOUNT_MESSAGE;
			m_ProcessingDialog.ProgressStyle = ProgressBarStyle.Marquee;
			m_ProcessingDialog.StartPosition = FormStartPosition.CenterParent;

			_startRemoveEvt.Set();
			m_ProcessingDialog.ShowDialog(this);
		}


		private void GetSizeAndUnit(float value, ref float size, ref string unit)
		{
			var units = new string[] { "B", "KB", "MB", "GB" };
			var index = Array.IndexOf(units, unit);

			if (index == -1)
				index = 0;

			if (value > 1024)
			{
				value = value / 1024;
				size = value;
				unit = units[index + 1];
				GetSizeAndUnit(value, ref size, ref unit);
				return;
			}

			size = value;
			unit = units[index];
		}

		private void UpdateCloudUsage()
		{
			var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;

			var quota = userInfo.TotalQuota;
			var size = 0.0f;
			var unit = string.Empty;
			GetSizeAndUnit(quota, ref size, ref unit);

			var usagePercent = (int)(((double)userInfo.TotalUsage) / userInfo.TotalQuota * 100);
			usageDetailControl1.CloudTotalUsage = string.Format("{0}% of {1}{2}", usagePercent, size, unit);
		}


		private void UpdateUsageDetail()
		{
			var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;

			UpdateCloudUsage();

			var localPhotos = AttachmentCollection.Instance.Find(Query.And(Query.EQ("group_id", StreamClient.Instance.LoginedUser.GroupID), Query.EQ("type", AttachmentType.image), Query.Exists("saved_file_name")));
			usageDetailControl1.LocalPhoto = localPhotos.Count().ToString("N0");

			var localDocs = AttachmentCollection.Instance.Find(Query.And(Query.EQ("group_id", StreamClient.Instance.LoginedUser.GroupID), Query.EQ("type", AttachmentType.doc), Query.Exists("saved_file_name")));
			usageDetailControl1.LocalDocument = localDocs.Count().ToString("N0");

			usageDetailControl1.TotalPhoto = userInfo.PhotoMetaCount;
			usageDetailControl1.TotalWeb = userInfo.WebMetaCount;
			usageDetailControl1.TotalDocument = userInfo.DocumentMetaCount;
		}

		private void UpdateResourceFolder()
		{
			try
			{
				usageDetailControl1.ResourcePath = ResourceFolder.GetResFolder(StreamClient.Instance.LoginedUser.UserID);
			}
			catch
			{
			}
		}

		private bool IsWinVistaOrLater()
		{
			bool isWinVistaOrLater;

			var os = Environment.OSVersion;
			if (os.Platform == PlatformID.Win32NT && os.Version.Major == 6)
				isWinVistaOrLater = true;
			else
				isWinVistaOrLater = false;
			return isWinVistaOrLater;
		}

		private static DialogResult ShowFileImportDialog()
		{
			try
			{
				var dialog = FileImportDialog.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog(MainForm.Instance);
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}


		private void normalizeTo32bit(long max64, long cur64, out int max32, out int cur32)
		{
			while (max64 > Int32.MaxValue)
			{
				max64 = max64 >> 1;
				cur64 = cur64 >> 1;
			}

			max32 = (int)max64;
			cur32 = (int)cur64;
		}


		private void UpdateLeftPanel()
		{
			UpdateSyncStatus();
			UpdateDeviceConnectCount();
		}

		private void UpdateAccountPage()
		{
			UpdateAccountInfo();
		}

		private void UpdateGeneralPage()
		{
			UpdateResourceFolder();
			UpdateUsageDetail();
		}


		private void BindingSelectedDevice()
		{
			var device = cmbDevice.SelectedItem as Device;
			device.RemainingBackUpCountChanged += device_RemainingBackUpCountChanged;

			UpdateDeviceSyncStatus();
		}

		private void UpdateDeviceSyncStatus()
		{
			//pbxDevice.Show();

			var device = cmbDevice.SelectedItem as Device;
			//lblDeviceName.Text = device.Name;
			devNameCtl.Show();
			devNameCtl.DeviceName = device.Name;

			if (device.RemainingBackUpCount > 0)
				label2.Text = string.Format(Resources.RECEVING_FILES_PATTERN, device.RemainingBackUpCount.ToString());
			else
				label2.Text = Resources.SYNC_CONNECTED_LOCALLY;
		}

		private Boolean IsValidEmailFormat()
		{
			return Regex.IsMatch(tbxEmail.Text, EMAIL_VALIDATE_MATCH_PATTERN, RegexOptions.IgnoreCase);
		}

		private Boolean CheckEmailFormat()
		{
			if (!IsValidEmailFormat())
			{
				errorProvider1.SetError(tbxEmail, Resources.INVALID_EMAIL);
				return false;
			}
			errorProvider1.SetError(tbxEmail, string.Empty);
			return true;
		}

		private Boolean IsValidNickNameFormat()
		{
			return tbxName.Text.Trim().Length > 0;
		}

		private Boolean CheckNickNameFormat()
		{
			if (!IsValidNickNameFormat())
			{
				errorProvider1.SetError(tbxName, Resources.INVALID_NICKNAME);
				return false;
			}
			errorProvider1.SetError(tbxName, string.Empty);
			return true;
		}

		private Boolean UpdateEmailToCloud()
		{
			try
			{
				if (tbxEmail.Text.Length == 0)
				{
					var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;
					tbxEmail.Text = userInfo.Email;
				}

				var user = StreamClient.Instance.LoginedUser;

				if (tbxEmail.Text.Equals(user.EMail, StringComparison.CurrentCultureIgnoreCase))
				{
					errorProvider1.SetError(tbxEmail, string.Empty);
					return true;
				}

				if (!CheckEmailFormat())
					return false;


				StationAPI.UpdateUser(user.SessionToken, user.UserID, email: tbxEmail.Text);
				errorProvider1.SetError(tbxEmail, string.Empty);

				return true;
			}
			catch (WebException ex)
			{
				using (var sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					var cloudResponse = JSON.Instance.ToObject<CloudResponse>(sr.ReadToEnd());
					if (cloudResponse.status == 401)
					{
						StreamClient.Instance.Logout();
						return false;
					}

					errorProvider1.SetError(tbxEmail, cloudResponse.api_ret_message);
				}
				return false;
			}
		}

		private Boolean UpdateNickNameToCloud()
		{
			try
			{
				var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;
				if (tbxName.Text.Length == 0)
				{
					tbxName.Text = userInfo.NickName;
				}

				if (tbxName.Text.Equals(userInfo.NickName, StringComparison.CurrentCultureIgnoreCase))
				{
					errorProvider1.SetError(tbxName, string.Empty);
					return true;
				}

				if (!CheckNickNameFormat())
					return false;

				var user = StreamClient.Instance.LoginedUser;

				StationAPI.UpdateUser(user.SessionToken, user.UserID, nickName: tbxName.Text);
				errorProvider1.SetError(tbxName, string.Empty);

				return true;
			}
			catch (WebException ex)
			{
				using (var sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					var cloudResponse = JSON.Instance.ToObject<CloudResponse>(sr.ReadToEnd());
					if (cloudResponse.status == 401)
					{
						StreamClient.Instance.Logout();
						return false;
					}

					errorProvider1.SetError(tbxEmail, cloudResponse.api_ret_message);
					return false;
				}
			}
		}
		#endregion


		#region Event Process
		void PreferencesDialog_Disposed(object sender, EventArgs e)
		{
			_instance = null;
		}

		private void btnUnLink_Click(object sender, EventArgs e)
		{
			var user = StreamClient.Instance.LoginedUser;
			using (var dialog = new CleanResourceDialog())
			{
				dialog.TopMost = this.TopMost;
				dialog.BackColor = this.BackColor;
				dialog.ShowInTaskbar = false;
				if (dialog.ShowDialog() == DialogResult.Yes)
					RemoveAccount(user.UserID, user.EMail, dialog.RemoveAllDatas);
			}
		}

		void removeAccountBgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var param = e.Argument as RemoveParam;
			e.Result = param;


			_startRemoveEvt.WaitOne();
			var response = StationAPI.RemoveUser(param.user_id, param.removeData);
		}


		void removeAccountBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.GetDisplayDescription(), Resources.UNKNOW_REMOVEACCOUNT_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				StreamClient.Instance.Logout();
				m_ProcessingDialog = null;
				return;
			}

			var param = e.Result as RemoveParam;
			if (param != null && !string.IsNullOrEmpty(param.email))
			{
				if (StreamClient.Instance.IsLogined && param.email.Equals(StreamClient.Instance.LoginedUser.EMail, StringComparison.CurrentCultureIgnoreCase))
				{
					StreamClient.Instance.Logout();
				}
			}

			m_ProcessingDialog = null;
		}

		private void refreshStatusTimer_Tick(object sender, EventArgs e)
		{
			UpdateLeftPanel();
		}

		private void UpdateSyncStatus()
		{
			var summary = ImportStatus.Lookup(StreamClient.Instance.LoginedUser.UserID);

			lblSyncStatus.Text = string.IsNullOrEmpty(summary.Description) ? SyncStatus.GetSyncStatus() : summary.Description;
			lblSyncTransferStatus.Text = string.IsNullOrEmpty(summary.Description) ? SyncStatus.GetSyncTransferStatus() : string.Empty;
		}


		private void UpdateDeviceConnectCount()
		{
			lblDeviceConnectStatus.Text = string.Format(Resources.DEVICE_CONNECTED_PATTERN, cmbDevice.Items.Count.ToString());
		}

		private void UpdateDeviceComboBox()
		{
			cmbDevice.Enabled = ConnectionStatus.Instance.Devices.Any();


			cmbDevice.BeginUpdate();
			cmbDevice.DisplayMember = "Name";
			cmbDevice.ValueMember = "ID";
			cmbDevice.DataSource = ConnectionStatus.Instance.Devices.ToList();
			cmbDevice.EndUpdate();
		}

		private void PreferencesDialog_Load(object sender, EventArgs e)
		{
			if (this.IsDesignMode())
				return;

			pbxLogo.Image = new Icon(this.Icon, 64, 64).ToBitmap();

			tabControl1.SelectedTab = tabGeneral;

			errorProvider1.SetError(tbxEmail, string.Empty);
			errorProvider1.SetError(tbxName, string.Empty);


			checkBox1.Checked = UsbImportController.Instance.Enabled;
			checkBox2.Checked = RecentDocumentWatcher.Instance.Enabled;


			UpdateGeneralPage();
			UpdateLeftPanel();
			UpdateDeviceComboBox();

			refreshStatusTimer.Start();

			this.tabConnections.Controls.Clear();
			this.tabConnections.Controls.Add(new ServiceImportControl() { Dock = DockStyle.Fill });

			this.tabDevices.Controls.Clear();
			this.tabDevices.Controls.Add(new PersonalCloudStatusControl2() { Dock = DockStyle.Fill });

			Waveface.Stream.ClientFramework.UserInfo.Instance.UserInfoUpdated += Instance_UserInfoUpdated;
			Waveface.Stream.ClientFramework.UserInfo.Instance.UserInfoUpdateFail += Instance_UserInfoUpdateFail;

			tabControl1.SelectedIndexChanged -= InitAccountPage;
			tabControl1.SelectedIndexChanged += InitAccountPage;

			(tabDevices.Controls[0] as PersonalCloudStatusControl2).RefreshInterval = refreshStatusTimer.Interval * 3;


			refreshUserInfoAsync();
		}

		private static void refreshUserInfoAsync()
		{
			// force update user's info to refresh all information
			var refreshBgWorker = new BackgroundWorker();
			refreshBgWorker.DoWork += (sendr, arg) =>
			{
				Waveface.Stream.ClientFramework.UserInfo.Instance.Update();
			};

			refreshBgWorker.RunWorkerAsync();
		}

		void Instance_UserInfoUpdated(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(() => { Instance_UserInfoUpdated(sender, e); }));
				return;
			}

			if (tabControl1.SelectedTab == tabGeneral)
				UpdateGeneralPage();
			else
			{
				tabControl1.SelectedIndexChanged -= InitGeneralPage;
				tabControl1.SelectedIndexChanged += InitGeneralPage;
			}
		}

		private void InitAccountPage(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabAccount)
			{
				UpdateAccountPage();

				tabControl1.SelectedIndexChanged -= InitAccountPage;
			}
		}

		private void InitGeneralPage(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabGeneral)
			{
				UpdateGeneralPage();


				tabControl1.SelectedIndexChanged -= InitGeneralPage;
			}
		}


		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			var personalCloudControl = tabDevices.Controls[0] as PersonalCloudStatusControl2;

			personalCloudControl.EnableAutoRefreshStatus = (tabControl1.SelectedTab == tabDevices);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var dialog = new PlanChangeDialog();
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(Resources.DELETE_ACCOUNT_MESSAGE, Resources.DELETE_ACCOUNT_TITLE, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{
				StationAPI.DeleteAccount(StreamClient.Instance.LoginedUser.SessionToken);
			}
		}

		static void Instance_UserInfoUpdateFail(object sender, ExceptionEventArgs e)
		{
			if (e.Exception is WebException)
			{
				using (var sr = new StreamReader((e.Exception as WebException).Response.GetResponseStream()))
				{
					var cloudResponse = JSON.Instance.ToObject<CloudResponse>(sr.ReadToEnd());
					if (cloudResponse.status == 401)
					{
						StreamClient.Instance.Logout();
					}
				}
			}
		}

		private void usageDetailControl1_ChangeResourcePathButtonClick(object sender, EventArgs e)
		{
			ResourceFolder.Change(
				StreamClient.Instance.LoginedUser.UserID,
				StreamClient.Instance.LoginedUser.SessionToken,
				(newFolder) => { usageDetailControl1.ResourcePath = newFolder; }
			);
		}

		private void cmbDevice_TextChanged(object sender, EventArgs e)
		{
			BindingSelectedDevice();
		}

		void Instance_DeviceRemoved(object sender, DevicesEventArgs e)
		{
			UpdateDeviceComboBox();

			if (cmbDevice.Items.Count == 0 || cmbDevice.SelectedItem == null)
			{
				//pbxDevice.Hide();
				//lblDeviceName.Text = string.Empty;
				devNameCtl.Hide();
				lblDeviceConnectStatus.Text = string.Empty;
				label2.Text = string.Empty;
				return;
			}
		}

		void Instance_DeviceAdded(object sender, DevicesEventArgs e)
		{
			UpdateDeviceComboBox();
		}

		void device_RemainingBackUpCountChanged(object sender, EventArgs e)
		{
			UpdateDeviceSyncStatus();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!UpdateEmailToCloud() || !UpdateNickNameToCloud())
			{
				tabControl1.SelectedTab = tabAccount;
				return;
			}

			Settings.Default.DetectMediaInsert = UsbImportController.Instance.Enabled = checkBox1.Checked;
			Settings.Default.ImportOpenedDoc = RecentDocumentWatcher.Instance.Enabled = checkBox2.Checked;
			Settings.Default.Save();

			Waveface.Stream.ClientFramework.UserInfo.Instance.Clear();

			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void tbxEmail_Leave(object sender, EventArgs e)
		{
			CheckEmailFormat();
		}

		private void tbxName_Leave(object sender, EventArgs e)
		{
			CheckNickNameFormat();
		}
		#endregion
	}
}
