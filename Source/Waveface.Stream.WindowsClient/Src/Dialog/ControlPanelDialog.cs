﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class ControlPanelDialog : Form
	{
		class RemoveParam
		{
			public string user_id { get; set; }
			public string email { get; set; }
			public bool removeData { get; set; }
		}

		#region Static Var
		private static ControlPanelDialog _instance;
		#endregion


		#region Var
		private ProcessingDialog _processingDialog;
		private BackgroundWorker _updateBackgroundWorker;
		private AutoUpdate _updator;
		private AutoResetEvent _startRemoveEvt = new AutoResetEvent(false);
		#endregion


		#region Public Static Property
		public static ControlPanelDialog Instance
		{ 
			get
			{
				return _instance ?? (_instance = new ControlPanelDialog());
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
		/// Gets the m_ update background worker.
		/// </summary>
		/// <value>The m_ update background worker.</value>
		private BackgroundWorker m_UpdateBackgroundWorker
		{
			get
			{
				if (_updateBackgroundWorker == null)
				{
					_updateBackgroundWorker = new BackgroundWorker();
					_updateBackgroundWorker.DoWork += bgworkerUpdate_DoWork;
					_updateBackgroundWorker.RunWorkerCompleted += bgworkerUpdate_RunWorkerCompleted;
				}
				return _updateBackgroundWorker;
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
		private ControlPanelDialog()
		{
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			lblSyncStatus.Text = string.Empty;
			lblSyncTransferStatus.Text = string.Empty;
			lblLocalProcessStatus.Text = string.Empty;

			UpdateAccountInfo();
			UpdateUserPackage();
			UpdateUsageStatus();
			UpdateResourceFolder();
			UpdateSoftwareInfo();
			UpdateImportStatus();

			refreshStatusTimer.Enabled = true;
			refreshStatusTimer.Start();
		}
		#endregion


		#region Private Method
		private void UpdateAccountInfo()
		{
			var userInfo = UserInfo.Instance;
			lblEmail.Text = userInfo.Email;
			lblName.Text = userInfo.NickName;
			chkSubscribed.Checked = userInfo.Subscribed;
		}

		private void RemoveAccount(string userID, string email, Boolean removeAllDatas)
		{
			BackgroundWorker removeAccountBgWorker = new BackgroundWorker();
			removeAccountBgWorker.DoWork += new DoWorkEventHandler(removeAccountBgWorker_DoWork);
			removeAccountBgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(removeAccountBgWorker_RunWorkerCompleted);
			removeAccountBgWorker.RunWorkerAsync(new RemoveParam { user_id = userID, email = email, removeData = removeAllDatas });

			m_ProcessingDialog.ProcessMessage = Resources.REMOVE_ACCOUNT_MESSAGE;
			m_ProcessingDialog.ProgressStyle = ProgressBarStyle.Marquee;
			m_ProcessingDialog.StartPosition = FormStartPosition.CenterParent;

			_startRemoveEvt.Set();
			m_ProcessingDialog.ShowDialog(this);
		}

		private void UpdateUserPackage()
		{
			var user = DriverCollection.Instance.FindOneById(StreamClient.Instance.LoginedUser.UserID);

			lblPackage.Text = (user.isPaidUser)? "VIP": "Free";
		}

		private void GetSizeAndUnit(float value, ref float size, ref string unit)
		{
			var units = new string[] { "B", "KB", "MB" };
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

		private void UpdateUsageStatus()
		{
			var userInfo = UserInfo.Instance;

			var quota = userInfo.Quota;
			var size = 0.0f;
			var unit = string.Empty;
			GetSizeAndUnit(quota,ref size,ref unit);

			var usagePercent = (int)(((double)userInfo.Usage) / userInfo.Quota * 100);
			lblUsageStatus.Text = string.Format("{0}% of {1}{2}", usagePercent, size, unit);
			usageBar1.Maximum = (int)size;
			usageBar1.Unit = unit;
			usageBar1.Value = (int)(size * usagePercent / 100);
		}

		private void UpdateResourceFolder()
		{
			lblResorcePath.Text = StationRegistry.GetValue("ResourceFolder", string.Empty) as string;
		}

		private void UpdateSoftwareInfo()
		{
			lblVersion.Text = ProductVersion;
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

		private void UpdateImportStatus()
		{
			try
			{
				var summary = ImportStatus.Lookup(StreamClient.Instance.LoginedUser.UserID);

				if (string.IsNullOrEmpty(summary.Description))
				{
					progressBar1.Visible = lblLocalProcessStatus.Visible = false;
					return;
				}
				else
				{
					lblLocalProcessStatus.Text = summary.Description;
					lblLocalProcessStatus.Visible = true;

					int max;
					int cur;
					if (summary.GetProgress(out max, out cur))
					{
						progressBar1.Maximum = max;
						progressBar1.Value = cur;
						progressBar1.Visible = true;
					}
				}
			}
			catch
			{

			}
		}
		#endregion

		#region Event Process
		private void ControlPanelDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			var userInfo = UserInfo.Instance;
			if (userInfo.Subscribed == chkSubscribed.Checked)
				return;

			var user = StreamClient.Instance.LoginedUser;
			StationAPI.UpdateUser(user.SessionToken, user.UserID, chkSubscribed.Checked);

			userInfo.Update();
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
					StreamClient.Instance.Logout();
			}

			m_ProcessingDialog = null;
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			m_UpdateBackgroundWorker.RunWorkerAsync();
			btnUpdate.Text = Resources.CHECKING_UPDATE;
			btnUpdate.Enabled = false;
		}

		private void bgworkerUpdate_DoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = m_Updator.IsUpdateRequired();
		}

		private void bgworkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (e.Error != null)
				{
					MessageBox.Show(e.Error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool isUpdateRequired = (bool)e.Result;

				if (isUpdateRequired)
				{
					m_Updator.ShowUpdateNeededUI();
				}
				else
				{
					MessageBox.Show(Properties.Resources.ALREAD_UPDATED, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			finally
			{
				btnUpdate.Enabled = true;
				btnUpdate.Text = Properties.Resources.CHECK_FOR_UPDATE;
			}
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			ShowFileImportDialog();
		}

		private void refreshStatusTimer_Tick(object sender, EventArgs e)
		{
			UpdateImportStatus();

			lblSyncStatus.Text = SyncStatus.GetSyncStatus();
			lblSyncTransferStatus.Text = SyncStatus.GetSyncTransferStatus();
		}

		#endregion

		
	}
}