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
using Waveface.Common;
using System.Threading;

namespace StationSystemTray
{
	public partial class SettingDialog : Form
	{
		#region Const
		private const string DEF_BASE_URL = "https://develop.waveface.com/v2/"; // https://api.waveface.com/v2/
		#endregion

		#region Var
		private bool _isMovingFolder;
		private MethodInvoker _closeClientProgram;
		private AutoUpdate _updator;
		private BackgroundWorker _updateBackgroundWorker;
		private SynchronizationContext _syncContext = SynchronizationContext.Current;
		private ProcessingDialog _processingDialog;
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
		/// Gets the m_ sync context.
		/// </summary>
		/// <value>The m_ sync context.</value>
		private SynchronizationContext m_SyncContext
		{
			get
			{
				return _syncContext;
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

		private string m_CurrentUserSession { get; set; }
		#endregion


		#region Event
		public event EventHandler<AccountEventArgs> AccountRemoving;
		public event EventHandler<AccountEventArgs> AccountRemoved;
		#endregion


		#region Constructor
		public SettingDialog(string currentUserSession, MethodInvoker closeClientProgram)
		{
			InitializeComponent();

			m_CurrentUserSession = currentUserSession;
			this._closeClientProgram = closeClientProgram;
		}
		#endregion


		#region Private Method
		private long GetStorageUsage(string userID)
		{
			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));

			if (driver == null)
				return 0;

			var fs = new FileStorage(driver);
			return fs.GetUsedSize();
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

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <param name="target">The target.</param>
		private void SendSyncContext(Action target)
		{
			m_SyncContext.Send((obj) => target(), null);
		}

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		private void SendSyncContext<T>(Action<T> target, Object o)
		{
			m_SyncContext.Send((obj) => target((T)obj), o);
		}

		private void RemoveAccount(string userID, string email, Boolean removeAllDatas)
		{
			MethodInvoker mi = new MethodInvoker(() =>
			{
				StationController.RemoveOwner(userID, removeAllDatas);
			});

			AutoResetEvent autoEvent = new AutoResetEvent(false);
			mi.BeginInvoke((result) =>
			{
				autoEvent.WaitOne();
				SendSyncContext(() =>
				{
					try
					{
						mi.EndInvoke(result);
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
					OnAccountRemoved(new AccountEventArgs(email));
					RefreshAccountList();

					m_ProcessingDialog = null;
				});
			}, null);

			OnAccountRemoving(new AccountEventArgs(email));
			m_ProcessingDialog.ProcessMessage = Resources.REMOVE_ACCOUNT_MESSAGE;
			m_ProcessingDialog.ProgressStyle = ProgressBarStyle.Marquee;
			m_ProcessingDialog.StartPosition = FormStartPosition.CenterParent;

			autoEvent.Set();
			m_ProcessingDialog.ShowDialog(this);
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:AccountRemoving"/> event.
		/// </summary>
		/// <param name="e">The <see cref="StationSystemTray.AccountEventArgs"/> instance containing the event data.</param>
		protected void OnAccountRemoving(AccountEventArgs e)
		{
			if (AccountRemoving == null)
				return;

			AccountRemoving(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:AccountRemoved"/> event.
		/// </summary>
		/// <param name="e">The <see cref="StationSystemTray.AccountEventArgs"/> instance containing the event data.</param>
		protected void OnAccountRemoved(AccountEventArgs e)
		{
			if (AccountRemoved == null)
				return;

			AccountRemoved(this, e);
		}
		#endregion


		#region Event Process
		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			btnUpdate.Text = Properties.Resources.CHECK_FOR_UPDATE;

			dgvAccountList.DefaultCellStyle.SelectionBackColor = dgvAccountList.DefaultCellStyle.BackColor;
			dgvAccountList.DefaultCellStyle.SelectionForeColor = dgvAccountList.DefaultCellStyle.ForeColor;

			string _execPath = Assembly.GetExecutingAssembly().Location;
			FileVersionInfo _version = FileVersionInfo.GetVersionInfo(_execPath);
			lblVersion.Text = _version.FileVersion;

			RefreshAccountList();
			RefreshCurrentResourceFolder();
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

				_closeClientProgram();

				lblResorcePath.Text = dialog.SelectedPath;

				BackgroundWorker bgWorker = new BackgroundWorker();
				bgWorker.DoWork += MoveResourceFolder_DoWork;
				bgWorker.RunWorkerCompleted += MoveResourceFolder_WorkCompleted;
				bgWorker.RunWorkerAsync(bgWorker);

				Cursor.Current = Cursors.WaitCursor;
				SetUIEnabled(false);
				_isMovingFolder = true;

				m_ProcessingDialog.ProcessMessage = Resources.MovingResourceFolder;
				m_ProcessingDialog.ProgressStyle = ProgressBarStyle.Marquee;
				m_ProcessingDialog.StartPosition = FormStartPosition.CenterParent;
				m_ProcessingDialog.ShowDialog(this);
			}
		}

		private void SetUIEnabled(bool enabled)
		{
			groupBox2.Enabled = groupBox3.Enabled = dgvAccountList.Enabled = enabled;
		}

		private void MoveResourceFolder_DoWork(object sender, DoWorkEventArgs args)
		{
			string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "move_folder.out");

			Process p = new Process();
			p.StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Station.Management.exe"),
				Arguments = string.Format("--moveFolder \"{0}\" --output \"{1}\"", lblResorcePath.Text, outputFilename),
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
			};

			if (Waveface.Env.IsWinVistaOrLater())
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
				RefreshCurrentResourceFolder();

				Cursor.Current = Cursors.Default;

				SetUIEnabled(true);
				_isMovingFolder = false;
				m_ProcessingDialog = null;
			}
		}

		private void SettingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_isMovingFolder)
				e.Cancel = true;
		}

		private void dgvAccountList_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;

			if (e.ColumnIndex == 2)
			{
				using (var dialog = new CleanResourceDialog(dgvAccountList.Rows[e.RowIndex].Cells[0].Value.ToString()))
				{
					dialog.TopMost = this.TopMost;
					dialog.BackColor = this.BackColor;
					dialog.ShowInTaskbar = false;

					if (dialog.ShowDialog() == DialogResult.Yes)
					{
						_closeClientProgram();
						RemoveAccount(dgvAccountList.Rows[e.RowIndex].Tag.ToString(), dgvAccountList.Rows[e.RowIndex].Cells[0].Value.ToString(), dialog.RemoveAllDatas);
					}
				}
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			m_UpdateBackgroundWorker.RunWorkerAsync();
			btnUpdate.Text = Properties.Resources.CHECKING_UPDATE;
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
					MessageBox.Show(e.Error.Message, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void dgvAccountList_Paint(object sender, PaintEventArgs e)
		{
			var columnOffset = 0;
			foreach (DataGridViewColumn column in dgvAccountList.Columns)
			{
				columnOffset += column.Width;
				e.Graphics.DrawLine(new Pen(ColorTranslator.FromHtml("#bcbcbc")), columnOffset + 1, 0, columnOffset + 1, dgvAccountList.Height);
			}
			ControlPaint.DrawBorder(e.Graphics, dgvAccountList.DisplayRectangle, ColorTranslator.FromHtml("#bcbcbc"), ButtonBorderStyle.Solid);
		}
		#endregion
	}
}
