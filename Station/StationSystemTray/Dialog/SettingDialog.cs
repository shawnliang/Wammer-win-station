using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.Management;
using StationSystemTray.Properties;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace StationSystemTray
{
	public partial class SettingDialog : Form
	{
		private string m_CurrentUserSession { get; set; }
		private bool isMovingFolder = false;
		private MethodInvoker closeClientProgram;

		public event EventHandler<AccountEventArgs> AccountRemoving;
		public event EventHandler<AccountEventArgs> AccountRemoved;

		protected void OnAccountRemoving(AccountEventArgs e)
		{
			if (AccountRemoving == null)
				return;

			AccountRemoving(this, e);
		}

		protected void OnAccountRemoved(AccountEventArgs e)
		{
			if (AccountRemoved == null)
				return;

			AccountRemoved(this, e);
		}

		public SettingDialog(string currentUserSession, MethodInvoker closeClientProgram)
		{
			InitializeComponent();

			m_CurrentUserSession = currentUserSession;
			this.closeClientProgram = closeClientProgram;
		}

		private void AdjustRemoveButton()
		{
			btnUnlink.Enabled = !string.IsNullOrEmpty(cmbStations.Text);
		}

		private void SetStorageUsage()
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
			{
				lblStorageUsageValue.Text = "0 MB";
				return;
			}

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));

			if (driver == null)
			{
				lblStorageUsageValue.Text = "0 MB";
				return;
			}

			var fs = new FileStorage(driver);
			lblStorageUsageValue.Text = (fs.GetUsedSize() / 1024 / 1024).ToString() + " MB";
		}


		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			RefreshAccountList();
			RefreshCurrentResourceFolder();
		}

		private void RefreshAccountList()
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", m_CurrentUserSession));
			var users = (from item in DriverCollection.Instance.FindAll()
			             where item != null && item.user != null
			             select new
			                    	{
			                    		User = item.user,
			                    		EMail = item.user.email,
			                    		NickName = item.user.nickname,
			                    		DisplayName =
			             	(loginedUser != null && item.user.email == loginedUser.user.email)
								? item.user.email + " " + Resources.CURRENT_ACCOUT
								: item.user.email
			                    	}).ToList();

			lblStorageUsageValue.Text = "0 MB";

			cmbStations.DisplayMember = "DisplayName";
			cmbStations.ValueMember = "User";
			cmbStations.DataSource = users;

			if (loginedUser != null)
				cmbStations.Text = loginedUser.user.nickname + " " + Resources.CURRENT_ACCOUT;
			AdjustRemoveButton();
		}

		private void RefreshCurrentResourceFolder()
		{
			txtLocation.Text = StationRegistry.GetValue("ResourceFolder", "") as string;
		}

		private void btnUnlink_Click(object sender, EventArgs e)
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
				return;

			using (var dialog = new CleanResourceForm(user.email))
			{
				dialog.TopMost = this.TopMost;
				dialog.BackColor = this.BackColor;
				dialog.ShowInTaskbar = false;
				if (dialog.ShowDialog() == DialogResult.Yes)
					RemoveCurrentAccount(dialog.RemoveAllDatas);
			}
		}

		private void RemoveCurrentAccount(Boolean removeAllDatas)
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
				return;

			OnAccountRemoving(new AccountEventArgs(user.email));

			try
			{
				StationController.RemoveOwner(user.user_id, removeAllDatas);
				OnAccountRemoved(new AccountEventArgs(user.email));
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UNKNOW_REMOVEACCOUNT_ERROR, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			RefreshAccountList();
		}
		
		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmbStations_TextChanged(object sender, EventArgs e)
		{
			AdjustRemoveButton();
			SetStorageUsage();
		}

		private void btnMove_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) // cancelled
					return;

				if (dialog.SelectedPath.Equals(txtLocation.Text)) // not changed
					return;

				DialogResult confirm = MessageBox.Show("Stream is going to move the resource folder to " + dialog.SelectedPath + ". Are you sure?",
					"Are you sure?", MessageBoxButtons.OKCancel);

				if (confirm != System.Windows.Forms.DialogResult.OK)	// cancelled
					return;

				closeClientProgram();

				txtLocation.Text = dialog.SelectedPath;

				BackgroundWorker bgWorker = new BackgroundWorker();
				bgWorker.DoWork += MoveResourceFolder_DoWork;
				bgWorker.RunWorkerCompleted += MoveResourceFolder_WorkCompleted;
				bgWorker.RunWorkerAsync(bgWorker);

				Cursor.Current = Cursors.WaitCursor;
				btnMove.Enabled = false;
				button1.Enabled = false;
				isMovingFolder = true;
			}
		}

		private void MoveResourceFolder_DoWork(object sender, DoWorkEventArgs args)
		{
			string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "move_folder.out");

			Process p = new Process();
			p.StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Station.Management.exe"),
				Arguments = string.Format("--moveFolder \"{0}\" --output \"{1}\"", txtLocation.Text, outputFilename),
				Verb = "runas",
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
			};

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
					MessageBox.Show(args.Error.Message);
					return;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			finally
			{
				RefreshCurrentResourceFolder();

				Cursor.Current = Cursors.Default;

				btnMove.Enabled = true;
				button1.Enabled = true;
				isMovingFolder = false;
			}
		}

		private void SettingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isMovingFolder)
				e.Cancel = true;
		}
	}
}
