using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver.Builders;
using StationSystemTray.Properties;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.Management;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using AppLimit.NetSparkle;
using System.Drawing;

namespace StationSystemTray
{
	public partial class SettingDialog : Form
	{
		public const string DEF_BASE_URL = "https://develop.waveface.com/v2/"; // https://api.waveface.com/v2/

		private Sparkle m_autoUpdator;
		private string m_CurrentUserSession { get; set; }
		private bool isMovingFolder = false;
		private MethodInvoker closeClientProgram;

		public event EventHandler<AccountEventArgs> AccountRemoving;
		public event EventHandler<AccountEventArgs> AccountRemoved;

		public static string CloudBaseURL
		{
			get { return (string)StationRegistry.GetValue("cloudBaseURL", DEF_BASE_URL); }
		}

		public static string WebURL
		{
			get
			{
				if (CloudBaseURL.Contains("api.waveface.com"))
					return "https://waveface.com";
				else if (CloudBaseURL.Contains("develop.waveface.com"))
					return "https://devweb.waveface.com";
				else if (CloudBaseURL.Contains("staging.waveface.com"))
					return "http://staging.waveface.com";
				else
					return "https://waveface.com";
			}
		}

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

		//private void AdjustRemoveButton()
		//{
		//    btnUnlink.Enabled = !string.IsNullOrEmpty(cmbStations.Text);
		//}

		private long GetStorageUsage(string userID)
		{
			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));

			if (driver == null)
				return 0;

			var fs = new FileStorage(driver);
			return fs.GetUsedSize();
		}


		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			m_autoUpdator = new Sparkle(WebURL + "/extensions/windowsUpdate/versioninfo.xml");
			m_autoUpdator.ApplicationIcon = Resources.software_update_available;
			m_autoUpdator.ApplicationWindowIcon = Resources.UpdateAvailable;

			btnUpdate.Text = Properties.Resources.CHECK_FOR_UPDATE;

			dgvAccountList.DefaultCellStyle.SelectionBackColor = dgvAccountList.DefaultCellStyle.BackColor;
			dgvAccountList.DefaultCellStyle.SelectionForeColor = dgvAccountList.DefaultCellStyle.ForeColor;

			string _execPath = Assembly.GetExecutingAssembly().Location;
			FileVersionInfo _version = FileVersionInfo.GetVersionInfo(_execPath);
			lblVersion.Text = _version.FileVersion;

			RefreshAccountList();
			RefreshCurrentResourceFolder();
		}

		private void RefreshAccountList()
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", m_CurrentUserSession));
			var users = from item in DriverCollection.Instance.FindAll()
						where item != null && item.user != null
						select new { ID = item.user_id, EMail = item.user.email };

			dgvAccountList.Rows.Clear();
			foreach (var user in users)
			{
				var rowIndex = dgvAccountList.Rows.Add(new object[] { user.EMail, (GetStorageUsage(user.ID) / 1024 / 1024).ToString() + " MB", Resources.REMOVE_ACCOUNT_BUTTON_TITLE });

				dgvAccountList.Rows[rowIndex].Tag = user.ID;		
			}
		}

		private void RefreshCurrentResourceFolder()
		{
			lblResorcePath.Text = StationRegistry.GetValue("ResourceFolder", "") as string;
		}


		private void RemoveAccount(string userID, string email, Boolean removeAllDatas)
		{
			OnAccountRemoving(new AccountEventArgs(email));

			try
			{
				StationController.RemoveOwner(userID, removeAllDatas);
				OnAccountRemoved(new AccountEventArgs(email));
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

		private void btnMove_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) // cancelled
					return;

				if (dialog.SelectedPath.Equals(lblResorcePath.Text)) // not changed
					return;

				DialogResult confirm = MessageBox.Show("Stream is going to move the resource folder to " + dialog.SelectedPath + ". Are you sure?",
					"Are you sure?", MessageBoxButtons.OKCancel);

				if (confirm != System.Windows.Forms.DialogResult.OK)	// cancelled
					return;

				closeClientProgram();

				lblResorcePath.Text = dialog.SelectedPath;

				BackgroundWorker bgWorker = new BackgroundWorker();
				bgWorker.DoWork += MoveResourceFolder_DoWork;
				bgWorker.RunWorkerCompleted += MoveResourceFolder_WorkCompleted;
				bgWorker.RunWorkerAsync(bgWorker);

				Cursor.Current = Cursors.WaitCursor;
				this.Enabled = false;
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
				Arguments = string.Format("--moveFolder \"{0}\" --output \"{1}\"", lblResorcePath.Text, outputFilename),
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

				this.Enabled = true;
				isMovingFolder = false;
			}
		}

		private void SettingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isMovingFolder)
				e.Cancel = true;
		}

		private void dgvAccountList_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;

			if (e.ColumnIndex == 2)
			{
				using (var dialog = new CleanResourceForm(dgvAccountList.Rows[e.RowIndex].Cells[0].Value.ToString()))
				{
					dialog.TopMost = this.TopMost;
					dialog.BackColor = this.BackColor;
					dialog.ShowInTaskbar = false;
					if (dialog.ShowDialog() == DialogResult.Yes)
						RemoveAccount(dgvAccountList.Rows[e.RowIndex].Tag.ToString(), dgvAccountList.Rows[e.RowIndex].Cells[0].Value.ToString(), dialog.RemoveAllDatas);
				}
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bgworkerUpdate.RunWorkerAsync();
			btnUpdate.Text = Properties.Resources.CHECKING_UPDATE;
			btnUpdate.Enabled = false;
		}

		private void bgworkerUpdate_DoWork(object sender, DoWorkEventArgs e)
		{
			NetSparkleAppCastItem _lastVersion;

			if (m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(), out _lastVersion))
				e.Result = _lastVersion;
			else
				e.Result = null;
		}

		private void bgworkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			NetSparkleAppCastItem _lastVersion = e.Result as NetSparkleAppCastItem;

			if (_lastVersion != null)
			{
				m_autoUpdator.ShowUpdateNeededUI(_lastVersion);
			}
			else
			{
				MessageBox.Show(Properties.Resources.ALREAD_UPDATED, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			btnUpdate.Enabled = true;
			btnUpdate.Text = Properties.Resources.CHECK_FOR_UPDATE;
		}

		private void dgvAccountList_Paint(object sender, PaintEventArgs e)
		{
			var columnOffset = 0;
			foreach (DataGridViewColumn column in dgvAccountList.Columns)
			{
				columnOffset += column.Width;
				e.Graphics.DrawLine(new Pen(ColorTranslator.FromHtml("#c6c6c6")), columnOffset + 1, 0, columnOffset + 1, dgvAccountList.Height);
			}
			ControlPaint.DrawBorder(e.Graphics, dgvAccountList.DisplayRectangle, ColorTranslator.FromHtml("#c6c6c6"), ButtonBorderStyle.Solid);
		}
	}
}
