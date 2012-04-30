using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Net;

using Wammer.Station.Management;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using System.Security.Permissions;
using Waveface.Localization;
using MongoDB.Driver.Builders;
using Wammer.PerfMonitor;

namespace StationSystemTray
{
	public partial class MainForm : Form, StationStateContext
	{
		#region Const
		const string STATION_SERVICE_NAME = "WavefaceStation";
		const string CLIENT_TITLE = "Waveface ";
		const int STATION_TIMER_LONG_INTERVAL = 60000;
		const int STATION_TIMER_SHORT_INTERVAL = 3000;
		#endregion

		#region Var
		//private CounterSample _PreUpSpeedSample;
		//private CounterSample _PreDownloadSpeedSample;
		private Messenger _messenger;
		private SignUpDialog _SignUpDialog;
		private System.Windows.Forms.Timer _timer;
		#endregion

		private IPerfCounter m_UpRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false);
		private IPerfCounter m_DownRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false);
		private IPerfCounter m_UpStreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);
		private IPerfCounter m_DownStreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false);



		#region Private Property
		private System.Windows.Forms.Timer m_Timer
		{
			get 
			{
				if (_timer == null)
					_timer = new System.Windows.Forms.Timer();
				return _timer; 
			}
		}

		private Messenger messenger
		{
			get
			{
				if (_messenger == null)
					_messenger = new Messenger(this);
				return _messenger;
			}
		}

		private SignUpDialog m_SignUpDialog 
		{
			get
			{
				if (_SignUpDialog == null)
					_SignUpDialog = new SignUpDialog()
					{
						Text = this.Text,
						Icon = this.Icon,
						StartPosition = FormStartPosition.CenterParent
					};
				return _SignUpDialog;
			}
			set
			{
				_SignUpDialog = value;
			}
		}
		#endregion

		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private UserLoginSettingContainer userloginContainer;
		private bool formCloseEnabled;
		public Process clientProcess;

		private StationStatusUIController uictrlStationStatus;
		//private WavefaceClientController uictrlWavefaceClient;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private bool initMinimized;
		private object cs = new object();
		public StationState CurrentState { get; private set; }
		
		public Icon iconRunning;
		public Icon iconPaused;
		public Icon iconWarning;

		public string TrayIconText
		{
			get { return TrayIcon.Text; }
			set
			{
				TrayIcon.Text = value;
				TrayIcon.BalloonTipText = value;
				TrayIcon.ShowBalloonTip(1000, "Waveface", value, ToolTipIcon.None);
			}
		}

		public MainForm(bool initMinimized)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.initMinimized = initMinimized;

			m_Timer.Interval = 1000;
			m_Timer.Tick += (sender, e) => { AdjustIconText(); };
			m_Timer.Start();
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x401)
			{
				GotoTimeline(userloginContainer.GetLastUserLogin());
				return;
			}
			base.WndProc(ref m);
		}

		protected override void OnLoad(EventArgs e)
		{
			ApplicationSettings settings = new ApplicationSettings();
			if (!settings.isUpgraded)
			{
				settings.Upgrade();
				settings.isUpgraded = true;
			}

			this.userloginContainer = new UserLoginSettingContainer(settings);

			this.iconRunning = Icon.FromHandle(StationSystemTray.Properties.Resources.stream_tray_working.GetHicon());
			this.iconPaused = Icon.FromHandle(StationSystemTray.Properties.Resources.stream_tray_pause.GetHicon());
			this.iconWarning = Icon.FromHandle(StationSystemTray.Properties.Resources.stream_tray_warn.GetHicon());
			this.TrayIcon.Icon = this.iconPaused;

			this.uictrlStationStatus = new StationStatusUIController(this);
			this.uictrlStationStatus.UICallback += this.StationStatusUICallback;
			this.uictrlStationStatus.UIError += this.StationStatusUIError;

			//this.uictrlWavefaceClient = new WavefaceClientController(this);
			//this.uictrlWavefaceClient.UICallback += this.WavefaceClientUICallback;
			//this.uictrlWavefaceClient.UIError += this.WavefaceClientUIError;

			this.uictrlPauseService = new PauseServiceUIController(this);
			this.uictrlPauseService.UICallback += this.PauseServiceUICallback;
			this.uictrlPauseService.UIError += this.PauseServiceUIError;

			this.uictrlResumeService = new ResumeServiceUIController(this);
			this.uictrlResumeService.UICallback += this.ResumeServiceUICallback;
			this.uictrlResumeService.UIError += this.ResumeServiceUIError;

			NetworkChange.NetworkAvailabilityChanged += checkStationTimer_Tick;
			NetworkChange.NetworkAddressChanged += checkStationTimer_Tick;

			this.CurrentState = CreateState(StationStateEnum.Initial);
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");
			RefreshUserList();

			this.checkStationTimer.Enabled = true;
			this.checkStationTimer.Start();

			CurrentState.Onlining();

			if (this.initMinimized)
			{
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
				this.initMinimized = false;
			}
			else
			{
				GotoTimeline(userloginContainer.GetLastUserLogin());
			}
		}

		private void RefreshUserList()
		{
			cmbEmail.Items.Clear();
			try
			{
				List<UserLoginSetting> userlogins = new List<UserLoginSetting>();
				ListDriverResponse res = StationController.ListUser();
				foreach (Driver driver in res.drivers)
				{
					UserLoginSetting userlogin = userloginContainer.GetUserLogin(driver.email);
					if (userlogin != null)
					{
						cmbEmail.Items.Add(userlogin.Email);
						ToolStripMenuItem menu = new ToolStripMenuItem(userlogin.Email, null, menuSwitchUser_Click);
						menu.Name = userlogin.Email;
						userlogins.Add(userlogin);
					}
				}

				if (userlogins.Count > 0)
				{
					UserLoginSetting lastUserLogin = userloginContainer.GetLastUserLogin();
					userloginContainer.ResetUserLoginSetting(userlogins, lastUserLogin == null ? "" : lastUserLogin.Email);
				}
			}
			catch (Exception)
			{
			}
		}

		private void GotoTimeline(UserLoginSetting userlogin)
		{
			if (clientProcess != null && !clientProcess.HasExited)
			{
				Debug.Assert(userlogin != null, "param userlogin cannot be empty when timeline opened");

				if (userlogin.Email == userloginContainer.GetLastUserLogin().Email)
				{
					IntPtr handle = Win32Helper.FindWindow(null, CLIENT_TITLE);
					Win32Helper.SetForegroundWindow(handle);
					Win32Helper.ShowWindow(handle, 5);
					return;
				}
					//uictrlWavefaceClient.Terminate();
					if (clientProcess != null)
						clientProcess.Close();
				}

			LoginedSession loginedSession = null;

			if (userlogin != null)
				loginedSession = Wammer.Model.LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", userlogin.Email));

			if (loginedSession != null || (userlogin != null && userlogin.RememberPassword))
			{
				userloginContainer.UpsertUserLoginSetting(userlogin);
				//RefreshUserList();

				LaunchWavefaceClient(userlogin);

				// if the function is called by OnLoad, Close() won't hide the mainform
				// so we have to play some tricks here
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;

				Close();
			}
			else
			{
				GotoTabPage(tabSignIn, userlogin);
			}
		}


		private void WavefaceClientUIError(object sender, SimpleEventArgs evt)
		{
			Exception ex = (Exception)evt.param;
			messenger.ShowMessage(ex.Message);
		}

		private void PauseServiceUICallback(object sender, SimpleEventArgs evt)
		{
			CurrentState.Offlined();
		}

		private void PauseServiceUIError(object sender, SimpleEventArgs evt)
		{
			Exception ex = (Exception)evt.param;

			if (ex is ConnectToCloudException)
			{
				CurrentState.Offlined();
			}
			else
			{
				CurrentState.Error();
			}
		}

		private void ResumeServiceUICallback(object sender, SimpleEventArgs evt)
		{
			CurrentState.Onlined();
		}

		private void ResumeServiceUIError(object sender, SimpleEventArgs evt)
		{
				CurrentState.Error();
			}

		private void StationStatusUICallback(object sender, SimpleEventArgs evt)
		{
			checkStationTimer.Interval = STATION_TIMER_LONG_INTERVAL;

			if (CurrentState.Value != StationStateEnum.Running)
				CurrentState.Onlining();
		}

		private void StationStatusUIError(object sender, SimpleEventArgs evt)
		{
			// recover quickly if falling to error/offline state unexpectedly
			if (CurrentState.Value == StationStateEnum.Running)
				checkStationTimer.Interval = STATION_TIMER_SHORT_INTERVAL;

			Exception ex = (Exception)evt.param;

			if (ex is ConnectToCloudException)
			{
				logger.Debug("Unable to connect to Waveface cloud", ex);
				CurrentState.Offlined();
			}
			else if (ex is WebException)
			{
				logger.Debug("Unable to connect to internet", ex);
				CurrentState.Offlined();
			}
			else
			{
				logger.Warn("Unexpected exception in station status cheking", ex);
				CurrentState.Error();
			}
		}

		private void menuQuit_Click(object sender, EventArgs e)
		{
			try
			{
				//uictrlWavefaceClient.Terminate();
				if (clientProcess != null)
					clientProcess.Close();
				StationController.SuspendSync();
			}
			catch (Exception ex)
			{
				logger.Error("StationOffline fail", ex);
			}
			finally
			{
				ExitProgram();
			}
		}

		private StationState CreateState(StationStateEnum state)
		{
			switch (state)
			{
				case StationStateEnum.Initial:
					{
						StationStateInitial st = new StationStateInitial(this);
						st.Entering += this.BecomeInitialState;
						return st;
					}
				case StationStateEnum.Starting:
					{
						StationStateStarting st = new StationStateStarting(this);
						st.Entering += this.BecomeStartingState;
						return st;
					}
				case StationStateEnum.Running:
					{
						StationStateRunning st = new StationStateRunning(this);
						st.Entering += this.BecomeRunningState;
						return st;
					}
				case StationStateEnum.Stopping:
					{
						StationStateStopping st = new StationStateStopping(this);
						st.Entering += this.BecomeStoppingState;
						return st;
					}
				case StationStateEnum.Stopped:
					{
						StationStateStopped st = new StationStateStopped(this);
						st.Entering += this.BecomeStoppedState;
						return st;
					}
				default:
					throw new NotImplementedException();
			}
		}

		public void GoToState(StationStateEnum state)
		{
			lock (cs)
			{
				CurrentState.OnLeaving(this, new EventArgs());
				CurrentState = CreateState(state);
				CurrentState.OnEntering(this, new EventArgs());
			}
		}

		private void menuServiceAction_Click(object sender, EventArgs e)
		{
			if (CurrentState.Value == StationStateEnum.Running)
			{
				CurrentState.Offlining();
			}
			else
			{
				CurrentState.Onlining();
			}
		}

		private void menuPreference_Click(object sender, EventArgs e)
		{
			GotoTimeline(userloginContainer.GetLastUserLogin());
		}


		private void checkStationTimer_Tick(object sender, EventArgs e)
		{
#if !DEBUG
			uictrlStationStatus.PerformAction();
#endif
		}

		void ClickBallonFor401Exception(object sender, EventArgs e)
		{
		}

		void BecomeInitialState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeInitialState), this, new EventArgs());
				}
			}
			else
			{
				TrayIcon.Icon = iconPaused;
				TrayIconText = I18n.L.T("StartingWFService");

				menuServiceAction.Enabled = false;
			}
		}

		void BecomeRunningState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeRunningState), this, new EventArgs());
				}
			}
			else
			{
				TrayIcon.Icon = iconRunning;
				TrayIconText = I18n.L.T("WFServiceRunning");
				menuServiceAction.Text = I18n.L.T("PauseWFService");

				menuServiceAction.Enabled = true;
			}
		}

		void BecomeStoppedState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeStoppedState), this, new EventArgs());
				}
			}
			else
			{
				TrayIcon.Icon = iconPaused;
				TrayIconText = I18n.L.T("WFServiceStopped");
				menuServiceAction.Text = I18n.L.T("ResumeWFService");

				menuServiceAction.Enabled = true;
			}
		}

		void BecomeStartingState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeStartingState), this, new EventArgs());
				}
			}
			else
			{
				menuServiceAction.Enabled = false;
				TrayIconText = I18n.L.T("StartingWFService");

				this.uictrlResumeService.PerformAction();
			}
		}

		void BecomeStoppingState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeStoppingState), this, new EventArgs());
				}
			}
			else
			{
				menuServiceAction.Enabled = false;
				TrayIconText = I18n.L.T("PausingWFService");

				this.uictrlPauseService.PerformAction();
			}
		}


		private void GotoTabPage(TabPage tabpage, UserLoginSetting userlogin)
		{
			if (m_SignUpDialog != null)
			{
				m_SignUpDialog.Dispose();
				m_SignUpDialog = null;
			}

			tabControl.SelectedTab = tabpage;

			if (this.WindowState == FormWindowState.Minimized)
			{
				this.WindowState = FormWindowState.Normal;
				this.ShowInTaskbar = true;
			}

			uint foreThread = Win32Helper.GetWindowThreadProcessId(Win32Helper.GetForegroundWindow(), IntPtr.Zero);
			uint appThread = Win32Helper.GetCurrentThreadId();
			if (foreThread != appThread)
			{
				Win32Helper.AttachThreadInput(foreThread, appThread, true);
				Win32Helper.BringWindowToTop(this.Handle);
				Show();
				Win32Helper.AttachThreadInput(foreThread, appThread, false);
			}
			else
			{
				Win32Helper.BringWindowToTop(this.Handle);
				Show();
			}
			Activate();

			if (tabpage == tabSignIn)
			{
				if (userlogin == null)
				{
					cmbEmail.Text = string.Empty;
					txtPassword.Text = string.Empty;
					chkRememberPassword.Checked = false;
				}
				else
				{
					cmbEmail.Text = userlogin.Email;
					if (userlogin.RememberPassword)
					{
						txtPassword.Text = SecurityHelper.DecryptPassword(userlogin.Password);
					}
					else
					{
						txtPassword.Text = string.Empty;
					}
					chkRememberPassword.Checked = userlogin.RememberPassword;
				}

				if (string.IsNullOrEmpty(cmbEmail.Text))
				{
					cmbEmail.Select();
				}
				else if (string.IsNullOrEmpty(txtPassword.Text))
				{
					txtPassword.Select();
				}
				else
				{
					btnSignIn.Select();
				}

				this.AcceptButton = btnSignIn;
			}
			else if (tabpage == tabMainStationSetup)
			{
				btnOK.Tag = userlogin;
				btnOK.Select();
				this.AcceptButton = btnOK;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!formCloseEnabled)
			{
				Hide();
				e.Cancel = true;
			}
		}

		private void menuSwitchUser_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menu = (ToolStripMenuItem)sender;

			UserLoginSetting userlogin = userloginContainer.GetUserLogin(menu.Text);

			GotoTimeline(userlogin);
		}

		private void btnSignIn_Click(object sender, EventArgs e)
		{
			if (this.TrayIcon.Icon != iconRunning)
			{
				//TODO: multi-languange support
				MessageBox.Show("Please start service first", "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if ((cmbEmail.Text == string.Empty) || (txtPassword.Text == string.Empty))
			{
				MessageBox.Show(I18n.L.T("FillAllFields"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!TestEmailFormat(cmbEmail.Text))
			{
				MessageBox.Show(I18n.L.T("InvalidEmail"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				Cursor = Cursors.WaitCursor;
				UserLoginSetting userlogin = userloginContainer.GetUserLogin(cmbEmail.Text);

				if (userlogin == null)
				{
					AddUserResult res = StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text);

					userlogin = new UserLoginSetting
						{
							Email = cmbEmail.Text.ToLower(),
							Password = SecurityHelper.EncryptPassword(txtPassword.Text),
							RememberPassword = chkRememberPassword.Checked
						};
					userloginContainer.UpsertUserLoginSetting(userlogin);
					RefreshUserList();

					if (res.IsPrimaryStation)
					{
						GotoTabPage(tabMainStationSetup, userlogin);
					}
					else
					{
						LaunchWavefaceClient(userlogin);
						Close();
					}
				}
				else
				{
					// In case the user is in AppData but not in Station DB (usually in testing environment)
					bool userAlreadyInDB = Wammer.Model.DriverCollection.Instance.FindOne(Query.EQ("email", cmbEmail.Text.ToLower())) != null;
					if (!userAlreadyInDB)
						StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text);

					userlogin.Password = SecurityHelper.EncryptPassword(txtPassword.Text);
					userlogin.RememberPassword = chkRememberPassword.Checked;
					userloginContainer.UpsertUserLoginSetting(userlogin);
					RefreshUserList();

					LaunchWavefaceClient(userlogin);

					Close();
				}
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(I18n.L.T("AuthError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;
				txtPassword.Focus();
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(I18n.L.T("StationDown"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(I18n.L.T("ConnectCloudError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception)
			{
				MessageBox.Show(I18n.L.T("UnknownSigninError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void LaunchWavefaceClient(UserLoginSetting userlogin)
		{
			if (userlogin == null)
			{
				userlogin = userloginContainer.GetLastUserLogin();
			}

			//uictrlWavefaceClient.PerformAction(userlogin);
			string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
		   "WavefaceWindowsClient.exe");
			clientProcess = Process.Start(execPath, userlogin.Email + " " + SecurityHelper.DecryptPassword(userlogin.Password));
			clientProcess.EnableRaisingEvents = true;

			clientProcess.Exited -= new EventHandler(clientProcess_Exited);
			clientProcess.Exited += new EventHandler(clientProcess_Exited);
		}

		void clientProcess_Exited(object sender, EventArgs e)
		{
			int exitCode = clientProcess.ExitCode;

			clientProcess.Exited -= new EventHandler(clientProcess_Exited);
			clientProcess = null;

			if (exitCode == -2)  // client logout
			{
				GotoTabPage(tabSignIn, userloginContainer.GetLastUserLogin());
			}
			else if (exitCode == -3)  // client unlink
			{
				UserLoginSetting userlogin = userloginContainer.GetLastUserLogin();

				bool isCleanResource = false;
				CleanResourceForm cleanform = new CleanResourceForm(userlogin.Email);
				DialogResult cleanResult = cleanform.ShowDialog();
				if (cleanResult == DialogResult.Yes)
				{
					isCleanResource = true;
				}

				ListDriverResponse res = StationController.ListUser();
				foreach (Driver driver in res.drivers)
				{
					if (driver.email == userlogin.Email)
					{
						StationController.RemoveOwner(driver.user_id, isCleanResource);
						userloginContainer.RemoveUserLogin(driver.email);
						RefreshUserList();
						break;
					}
				}
				GotoTabPage(tabSignIn, null);
			}
		}

		private bool TestEmailFormat(string emailAddress)
		{
			const string _patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
										 + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
										 + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
										 + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
										 + @"[a-zA-Z]{2,}))$";

			Regex _reStrict = new Regex(_patternStrict);
			return _reStrict.IsMatch(emailAddress);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			LaunchWavefaceClient((UserLoginSetting)btn.Tag);
			Close();
		}

		private void ExitProgram()
		{
			formCloseEnabled = true;
			Close();
		}

		private void lblSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.Hide();
			m_SignUpDialog.ShowDialog();
			if (m_SignUpDialog.DialogResult == System.Windows.Forms.DialogResult.OK)
			{
				cmbEmail.Text = m_SignUpDialog.EMail;
				txtPassword.Text = m_SignUpDialog.Password;
			}
			m_SignUpDialog.Dispose();
			m_SignUpDialog = null;
			tabControl.SelectedTab = tabSignIn;

			if (!this.IsDisposed)
				this.Show();
		}


		#region Protected Method
		/// <summary>
		/// Processes a command key.
		/// </summary>
		/// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the Win32 message to process.</param>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
		/// <returns>
		/// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
		/// </returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//prevent ctrl+tab to switch signin pages
			if (keyData == (Keys.Control | Keys.Tab))
			{
				return true;
			}
			else
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}
		}
		#endregion

		public static void LogOut(WebClient agent, string sessionToken, string apiKey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, sessionToken);
			parameters.Add(CloudServer.PARAM_API_KEY, apiKey);

			CloudServer.requestPath(agent, "http://127.0.0.1:9981/v2/", "auth/logout", parameters);
		}

		private void menuSignIn_Click(object sender, EventArgs e)
		{
			if (menuSignIn.Text == I18n.L.T("LogoutMenuItem"))
			{
				var userlogin = userloginContainer.GetLastUserLogin();
				LoginedSession loginedSession = null;

				if (userlogin != null)
				{
					if (clientProcess != null)
					{
						clientProcess.CloseMainWindow();
						clientProcess = null;
					}

					loginedSession = Wammer.Model.LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", userlogin.Email));
					LogOut(new WebClient(), loginedSession.session_token, loginedSession.apikey.apikey);
				}
			}
			GotoTabPage(tabSignIn, userloginContainer.GetLastUserLogin());
		}

		private void TrayMenu_VisibleChanged(object sender, EventArgs e)
		{
			var userlogin = userloginContainer.GetLastUserLogin();
			LoginedSession loginedSession = null;

			if (userlogin != null)
				loginedSession = Wammer.Model.LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", userlogin.Email));

			var isUserLogined = (loginedSession != null || (clientProcess != null && !clientProcess.HasExited));
			
			menuSignIn.Text = isUserLogined ? I18n.L.T("LogoutMenuItem") : I18n.L.T("LoginMenuItem");
		}

		private void cmbEmail_TextChanged(object sender, EventArgs e)
		{
			UserLoginSetting userlogin = userloginContainer.GetUserLogin(cmbEmail.Text);
			if (userlogin != null)
			{
				if (userlogin.RememberPassword)
				{
					txtPassword.Text = SecurityHelper.DecryptPassword(userlogin.Password);
				}
				else
				{
					txtPassword.Text = string.Empty;
				}
				chkRememberPassword.Checked = userlogin.RememberPassword;
			}
			else
			{
				txtPassword.Text = string.Empty;
				chkRememberPassword.Checked = false;
			}
		}


		private void AdjustIconText()
		{
			m_Timer.Stop();

			var iconText = TrayIcon.BalloonTipText;

			if (iconPaused != this.TrayIcon.Icon)
			{
				var upRemainedCount = m_UpRemainedCountCounter.NextSample().RawValue;
				var downloadRemainedCount = m_DownRemainedCountCounter.NextSample().RawValue;
				var upSpeed = m_UpStreamRateCounter.NextValue() / 1024;
				var downloadSpeed = m_DownStreamRateCounter.NextValue() / 1024;

				var upSpeedUnit = (upSpeed <= 1024) ? "KB/s" : "MB/s";
				var downloadSpeedUnit = (downloadSpeed <= 1024) ? "KB/s" : "MB/s";

				upSpeed = upRemainedCount == 0? 0: ((upSpeed >= 1024) ? upSpeed / 1024 : upSpeed);
				downloadSpeed = downloadSpeed == 0? 0: ((downloadSpeed >= 1024) ? downloadSpeed / 1024 : downloadSpeed);

				iconText = string.Format("{0}{1}({2}): {3:0.0} {4}{5}({6}): {7:0.0}{8}",
					iconText,
					Environment.NewLine,
					upRemainedCount,
					upSpeed,
					upSpeedUnit,
					Environment.NewLine,
					downloadRemainedCount,
					downloadSpeed,
					downloadSpeedUnit);
			}
			SetNotifyIconText(this.TrayIcon, iconText);
			m_Timer.Start();
		}

		public static void SetNotifyIconText(NotifyIcon ni, string text)
		{
			if (text.Length >= 128) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
			Type t = typeof(NotifyIcon);
			BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
			t.GetField("text", hidden).SetValue(ni, text);
			if ((bool)t.GetField("added", hidden).GetValue(ni))
				t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
		}

		private void tsmiOpenStream_Click(object sender, EventArgs e)
		{
			GotoTimeline(userloginContainer.GetLastUserLogin());
		}

		private void TrayMenu_Opening(object sender, CancelEventArgs e)
		{
			var userlogin = userloginContainer.GetLastUserLogin();
			LoginedSession loginedSession = null;

			if (userlogin != null)
				loginedSession = Wammer.Model.LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", userlogin.Email));

			var isUserLogined = (loginedSession != null || (clientProcess != null && !clientProcess.HasExited));

			tsmiOpenStream.Visible = isUserLogined;
		}
	}

	#region StationStatusUIController
	public class StationStatusUIController : SimpleUIController
	{
		private MainForm mainform;
		object csStationTimerTick = new object();

		public StationStatusUIController(MainForm form)
			: base(form)
		{
			mainform = form;
		}

		protected override object Action(object obj)
		{
			lock (csStationTimerTick)
			{
				StationController.PingForServiceAlive();
				StationController.ConnectToInternet();
				StationController.PingForAvailability();

				return null;
			}
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{	
		}
	}
	#endregion

	//#region WavefaceClientController
	//public class WavefaceClientController : SimpleUIController
	//{
	//    private MainForm mainform;
	//    private object cs = new object();

	//    public WavefaceClientController(MainForm form)
	//        : base(form)
	//    {
	//        mainform = form;
	//    }

	//    public void Terminate()
	//    {
	//        if (mainform.clientProcess != null)
	//        {
	//            mainform.clientProcess.Kill();
	//        }
	//    }

	//    protected override object Action(object obj)
	//    {
	//        lock (cs)
	//        {
	//            UserLoginSetting userlogin = (UserLoginSetting)obj;
	//            string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
	//           "WavefaceWindowsClient.exe");
	//            mainform.clientProcess = Process.Start(execPath, userlogin.Email + " " + SecurityHelper.DecryptPassword(userlogin.Password));

	//            mainform.clientProcess.Exited += new EventHandler(clientProcess_Exited);

	//            if (mainform.clientProcess != null)
	//                mainform.clientProcess.WaitForExit();

	//            int exitCode = mainform.clientProcess.ExitCode;
	//            mainform.clientProcess = null;

	//            return exitCode;
	//        }
	//    }

	//    void clientProcess_Exited(object sender, EventArgs e)
	//    {
	//        var process = sender as Process;
	//        var exitCode = process.ExitCode;
	//    }

	//    protected override void ActionCallback(object obj)
	//    {
	//    }

	//    protected override void ActionError(Exception ex)
	//    {
	//        mainform.clientProcess = null;
	//    }
	//}
	//#endregion

	#region PauseServiceUIController
	public class PauseServiceUIController : SimpleUIController
	{
		public PauseServiceUIController(MainForm form)
			: base(form)
		{
		}

		protected override object Action(object obj)
		{
			StationController.SuspendSync();
			return null;
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			MainForm.logger.Error("Unable to stop service", ex);
		}
	}
	#endregion

	#region ResumeServiceUIController
	public class ResumeServiceUIController : SimpleUIController
	{
		public ResumeServiceUIController(MainForm form)
			: base(form)
		{
		}

		protected override object Action(object obj)
		{
			StationController.ResumeSync();
			return null;
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			MainForm.logger.Error("Unable to start service", ex);
		}
	}
	#endregion


}
