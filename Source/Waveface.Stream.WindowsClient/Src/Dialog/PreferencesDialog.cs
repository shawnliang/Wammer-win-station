﻿using fastJSON;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
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

			lblSyncStatus.Text = string.Empty;
			lblSyncTransferStatus.Text = string.Empty;
			lblDeviceConnectStatus.Text = string.Empty;
			lblDeviceName.Text = string.Empty;
			label2.Text = string.Empty;
		}
		#endregion


		#region Private Method
		private void UpdateAccountInfo()
		{
			try
			{
				var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;
				lblEmail.Text = userInfo.Email;
				lblName.Text = userInfo.NickName;

				AdjustEditFunction();
			}
			catch (Exception)
			{
			}
		}

		private void AdjustEditFunction()
		{
			var left = lblEmail.Left + Math.Max(lblEmail.Width, lblName.Width) + lblEmail.Padding.Right + lnklblEditEmail.Padding.Left;
			lnklblEditEmail.Left = left;
			lnklblEditName.Left = left;
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

			var localPhotos = AttachmentCollection.Instance.Find(Query.And(Query.EQ("group_id", StreamClient.Instance.LoginedUser.GroupID), Query.EQ("type", AttachmentType.image)));
			usageDetailControl1.LocalPhoto = localPhotos.Count().ToString();

			var localDocs = AttachmentCollection.Instance.Find(Query.And(Query.EQ("group_id", StreamClient.Instance.LoginedUser.GroupID), Query.EQ("type", AttachmentType.doc)));
			usageDetailControl1.LocalDocument = localDocs.Count().ToString();

			usageDetailControl1.TotalPhoto = userInfo.PhotoMetaCount;
			usageDetailControl1.TotalWeb = userInfo.WebMetaCount;
			usageDetailControl1.TotalDocument = userInfo.DocumentMetaCount;
		}

		private void UpdateResourceFolder()
		{
			try
			{
				var user = DriverCollection.Instance.FindOneById(StreamClient.Instance.LoginedUser.UserID);
				usageDetailControl1.ResourcePath = user.folder;
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
			UpdateDeviceComboBox();
			UpdateDeviceSyncStatus();
			UpdateDeviceConnectCount();
		}

		private void UpdateAccountPage()
		{
			UpdateAccountInfo();
		}

		private void SwitchToEmailEditMode()
		{
			tbxEmail.Text = lblEmail.Text;
			lblEmail.Visible = false;
			lnklblEditEmail.Visible = false;
			tbxEmail.Visible = true;
			lnklblSaveEmail.Visible = true;
			lnklblCancelEmail.Visible = true;

			tbxEmail.Focus();
		}

		private void SwitchToNameEditMode()
		{
			tbxName.Text = lblName.Text;
			lblName.Visible = false;
			lnklblEditName.Visible = false;
			tbxName.Visible = true;
			lnklblSaveName.Visible = true;
			lnklblCancelName.Visible = true;

			tbxName.Focus();
		}

		private void SwitchToEmailDisplayMode()
		{
			lblEmail.Visible = true;
			lnklblEditEmail.Visible = true;
			tbxEmail.Visible = false;
			lnklblSaveEmail.Visible = false;
			lnklblCancelEmail.Visible = false;
		}

		private void SwitchToNameDisplayMode()
		{
			lblName.Visible = true;
			lnklblEditName.Visible = true;
			tbxName.Visible = false;
			lnklblSaveName.Visible = false;
			lnklblCancelName.Visible = false;
		}


		private void UpdateGeneralPage()
		{
			UpdateResourceFolder();
			UpdateUsageDetail();
		}

		private void UpdateDeviceSyncStatus()
		{
			if (cmbDevice.Items.Count == 0 || cmbDevice.SelectedValue == null)
			{
				lblDeviceName.Text = string.Empty;
				lblDeviceConnectStatus.Text = string.Empty;
				label2.Text = string.Empty;
				return;
			}

			var deviceID = cmbDevice.SelectedValue.ToString();
			lblDeviceName.Text = cmbDevice.Text;

			var connection = ConnectionCollection.Instance.FindOne(
						Query.EQ("device.device_id", deviceID));

			if (connection == null)
				return;


			if (connection.files_to_backup != null && connection.files_to_backup > 0)
				label2.Text = string.Format("receiving {0} files", connection.files_to_backup.ToString());
			else
				label2.Text = "Connected Locally";
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
			using (var dialog = new CleanResourceDialog(user.EMail))
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
				if (e.Error is AuthenticationException)
				{
					MessageBox.Show(Resources.AUTH_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else if (e.Error is StationServiceDownException)
				{
					MessageBox.Show(Resources.STATION_SERVICE_DOWN, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else if (e.Error is ConnectToCloudException)
				{
					MessageBox.Show(Resources.CONNECT_CLOUD_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					MessageBox.Show(Resources.UNKNOW_REMOVEACCOUNT_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
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
			lblDeviceConnectStatus.Text = string.Format("{0} devices connected", cmbDevice.Items.Count.ToString());
		}

		private void UpdateDeviceComboBox()
		{
			var newDatas = ConnectionCollection.Instance.FindAll().Select(item => new 
			{
				DeviceID = item.device.device_id,
				DeviceName = item.device.device_name
			}).ToList();


			cmbDevice.DisplayMember = "DeviceName";
			cmbDevice.ValueMember = "DeviceID";
			cmbDevice.DataSource = newDatas;
		}

		private void PreferencesDialog_Load(object sender, EventArgs e)
		{
			if (this.IsDesignMode())
				return;

			pbxLogo.Image = new Icon(this.Icon, 64, 64).ToBitmap();

			checkBox1.Checked = UsbImportController.Instance.Enabled;

			SwitchToEmailDisplayMode();
			SwitchToNameDisplayMode();

			UpdateGeneralPage();
			UpdateLeftPanel();

			refreshStatusTimer.Start();

			this.tabConnections.Controls.Clear();
			this.tabConnections.Controls.Add(new ServiceImportControl() { Dock = DockStyle.Fill });

			this.tabDevices.Controls.Clear();
			this.tabDevices.Controls.Add(new PersonalCloudStatusControl2() { Dock = DockStyle.Fill });

			Waveface.Stream.ClientFramework.UserInfo.Instance.UserInfoUpdated += Instance_UserInfoUpdated;
			Waveface.Stream.ClientFramework.UserInfo.Instance.UserInfoUpdateFail += Instance_UserInfoUpdateFail;

			tabControl1.SelectedIndexChanged -= InitAccountPage;
			tabControl1.SelectedIndexChanged += InitAccountPage;
		}

		void Instance_UserInfoUpdated(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabAccount)
				UpdateAccountPage();
			else if (tabControl1.SelectedTab == tabGeneral)
				UpdateGeneralPage();
			else
			{
				tabControl1.SelectedIndexChanged -= InitAccountPage;
				tabControl1.SelectedIndexChanged += InitAccountPage;

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
			(tabDevices.Controls[0] as PersonalCloudStatusControl2).EnableAutoRefreshStatus = (tabControl1.SelectedTab == tabDevices);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			UsbImportController.Instance.Enabled = checkBox1.Checked;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var dialog = new PlanChangeDialog();
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			StationAPI.DeleteAccount(StreamClient.Instance.LoginedUser.SessionToken);
			MessageBox.Show("Delete email sended...");
		}

		private void lnklblEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			SwitchToEmailEditMode();
		}

		private void lnklblName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			SwitchToNameEditMode();
		}

		private void lnklblCancelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			SwitchToEmailDisplayMode();
		}


		private void lnklblSaveEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				var user = StreamClient.Instance.LoginedUser;
				StationAPI.UpdateUser(user.SessionToken, user.UserID, email: tbxEmail.Text);

				lblEmail.Text = tbxEmail.Text;

				AdjustEditFunction();
				SwitchToEmailDisplayMode();

				Waveface.Stream.ClientFramework.UserInfo.Instance.Reset();
			}
			catch (WebException ex)
			{
				using (var sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					var cloudResponse = JSON.Instance.ToObject<CloudResponse>(sr.ReadToEnd());
					if (cloudResponse.status == 401)
					{
						StreamClient.Instance.Logout();
						return;
					}
					MessageBox.Show(cloudResponse.api_ret_message);
				}
			}
		}

		private void lnklblSaveName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				var user = StreamClient.Instance.LoginedUser;
				StationAPI.UpdateUser(user.SessionToken, user.UserID, nickName: tbxName.Text);

				lblName.Text = tbxName.Text;

				AdjustEditFunction();
				SwitchToNameDisplayMode();

				Waveface.Stream.ClientFramework.UserInfo.Instance.Reset();
			}
			catch (WebException ex)
			{
				using (var sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					var cloudResponse = JSON.Instance.ToObject<CloudResponse>(sr.ReadToEnd());
					if (cloudResponse.status == 401)
					{
						StreamClient.Instance.Logout();
						return;
					}
					MessageBox.Show(cloudResponse.api_ret_message);
				}
			}
		}



		private void lnklblCancelName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			SwitchToNameDisplayMode();
		}

		static void Instance_UserInfoUpdateFail(object sender, ExceptionEventArgs e)
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

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			RecentDocumentWatcher.Instance.Enabled = checkBox2.Checked;
		}

		private void usageDetailControl1_ChangeResourcePathButtonClick(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) // cancelled
					return;

				if (dialog.SelectedPath.Equals(usageDetailControl1.ResourcePath)) // not changed
					return;

				DialogResult confirm = MessageBox.Show("Stream is going to move the resource folder to " + dialog.SelectedPath + ". Are you sure?",
					"Are you sure?", MessageBoxButtons.OKCancel);

				if (confirm != System.Windows.Forms.DialogResult.OK)	// cancelled
					return;

				usageDetailControl1.ResourcePath = dialog.SelectedPath;

				BackgroundWorker bgWorker = new BackgroundWorker();
				bgWorker.DoWork += MoveResourceFolder_DoWork;
				bgWorker.RunWorkerCompleted += MoveResourceFolder_WorkCompleted;
				bgWorker.RunWorkerAsync(bgWorker);

				Cursor.Current = Cursors.WaitCursor;
				//SetUIEnabled(false);
				//_isMovingFolder = true;

				m_ProcessingDialog.ProcessMessage = Resources.MovingResourceFolder;
				m_ProcessingDialog.ProgressStyle = ProgressBarStyle.Marquee;
				m_ProcessingDialog.StartPosition = FormStartPosition.CenterParent;
				m_ProcessingDialog.ShowDialog(this);
			}
		}

		private void MoveResourceFolder_DoWork(object sender, DoWorkEventArgs args)
		{
			string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "move_folder.out");

			Process p = new Process();
			p.StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Station.Management.exe"),
				Arguments = string.Format("--moveFolder \"{0}\" --output \"{1}\"", usageDetailControl1.ResourcePath, outputFilename),
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
			};

			if (IsWinVistaOrLater())
				p.StartInfo.Verb = "runas";

			p.Start();
			p.WaitForExit();

			if (p.ExitCode != 0)
			{
				if (File.Exists(outputFilename))
				{
					using (StreamReader reader = File.OpenText(outputFilename))
					{
						throw new Exception(reader.ReadToEnd());
					}
				}
				else
				{
					throw new Exception("Unknown error");
				}
			}
		}

		private void MoveResourceFolder_WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
		{
			try
			{
				if (args.Cancelled)
					return;

				if (args.Error != null)
				{
					MessageBox.Show(args.Error.Message, Resources.MoveFolderUnsuccess);
					return;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			finally
			{
				//RefreshCurrentResourceFolder();

				Cursor.Current = Cursors.Default;

				//SetUIEnabled(true);
				//_isMovingFolder = false;
				m_ProcessingDialog = null;
			}
		}

		private void cmbDevice_TextChanged(object sender, EventArgs e)
		{
			UpdateDeviceSyncStatus();
		}


		#endregion

	}
}