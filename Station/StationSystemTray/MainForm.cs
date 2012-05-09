using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using MongoDB.Driver.Builders;
using StationSystemTray.Properties;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;
using Wammer.Station.Management;
using Wammer.Utility;

namespace StationSystemTray
{
	public partial class MainForm : Form, StationStateContext
	{
		#region DllImport
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
		#endregion

		#region Const
		private const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		const string STATION_SERVICE_NAME = "WavefaceStation";
		const string CLIENT_TITLE = "Waveface ";
		const int STATION_TIMER_LONG_INTERVAL = 60000;
		const int STATION_TIMER_SHORT_INTERVAL = 3000;

		const string WEB_BASE_URL = @"https://waveface.com";
		const string DEV_WEB_BASE_PAGE_URL = @"http://develop.waveface.com:4343";

		const string SIGNUP_URL_PATH = @"/signup";
		const string LOGIN_URL_PATH = @"/sns/facebook/signin";
		const string CALLBACK_URL_PATH = @"/client/callback";

		const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";
		const string EMAIL_MATCH_PATTERN = @"^(([^<>()[\]\\.,;:\s@\""]+"
										 + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
										 + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
										 + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
										 + @"[a-zA-Z]{2,}))$";
		#endregion


		#region Var
		private Messenger _messenger;
		private Timer _timer;
		private string _baseUrl;
		private string _signUpUrl;
		private string _fbLoginUrl;
		private string _callbackUrl;
		#endregion



		#region Private Property
		private string m_BaseUrl
		{
			get
			{
				if (_baseUrl == null)
				{
					if (Wammer.Cloud.CloudServer.BaseUrl.Contains("develop.waveface.com"))
					{
						_baseUrl = DEV_WEB_BASE_PAGE_URL;
					}
					else
					{
						_baseUrl = WEB_BASE_URL;
					}
				}
				return _baseUrl;
			}
		}

		private string m_CallbackUrl
		{
			get
			{
				if (_callbackUrl == null)
				{
					_callbackUrl = Path.Combine(m_BaseUrl, CALLBACK_URL_PATH);
				}
				return _callbackUrl;
			}
		}

		private string m_SignUpUrl
		{
			get
			{
				if (_signUpUrl == null)
				{
					_signUpUrl = m_BaseUrl + SIGNUP_URL_PATH + string.Format("?l={0}", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
				}
				return _signUpUrl;
			}
		}

		private string m_FBLoginUrl
		{
			get
			{
				if (_fbLoginUrl == null)
				{
					_fbLoginUrl = m_BaseUrl + LOGIN_URL_PATH + string.Format("?l={0}", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
				}
				return _fbLoginUrl;
			}
		}

		private Action m_LoginAction { get; set; }

		#endregion

		private readonly IPerfCounter m_UpRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false);
		private readonly IPerfCounter m_DownRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false);
		private readonly IPerfCounter m_UpStreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);
		private readonly IPerfCounter m_DownStreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false);



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
		#endregion

		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private UserLoginSettingContainer userloginContainer;
		private bool formCloseEnabled;
		public Process clientProcess;

		private StationStatusUIController uictrlStationStatus;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private bool initMinimized;
		private object cs = new object();
		public StationState CurrentState { get; private set; }
		
		public Icon iconRunning;
		public Icon iconPaused;
		public Icon iconWarning;
		public Icon iconSyncing1;
		public Icon iconSyncing2;

		public string TrayIconText
		{
			get { return TrayIcon.Text; }
			set
			{
				TrayIcon.Text = value;
				TrayIcon.BalloonTipText = value;
			}
		}

		public MainForm(bool initMinimized)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.initMinimized = initMinimized;
			this.Icon = Resources.Icon;

			m_Timer.Interval = 500;
			m_Timer.Tick += (sender, e) => { RefreshSyncingStatus(); };
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

			this.iconRunning = Icon.FromHandle(Properties.Resources.stream_tray_working.GetHicon());
			this.iconPaused = Icon.FromHandle(Properties.Resources.stream_tray_pause.GetHicon());
			this.iconWarning = Icon.FromHandle(Properties.Resources.stream_tray_warn.GetHicon());
			this.iconSyncing1 = Icon.FromHandle(Properties.Resources.stream_tray_syncing1.GetHicon());
			this.iconSyncing2 = Icon.FromHandle(Properties.Resources.stream_tray_syncing2.GetHicon());
			this.TrayIcon.Icon = this.iconPaused;

			this.uictrlStationStatus = new StationStatusUIController(this);
			this.uictrlStationStatus.UICallback += this.StationStatusUICallback;
			this.uictrlStationStatus.UIError += this.StationStatusUIError;

			this.uictrlPauseService = new PauseServiceUIController(this);
			this.uictrlPauseService.UICallback += this.PauseServiceUICallback;
			this.uictrlPauseService.UIError += this.PauseServiceUIError;

			this.uictrlResumeService = new ResumeServiceUIController(this);
			this.uictrlResumeService.UICallback += this.ResumeServiceUICallback;
			this.uictrlResumeService.UIError += this.ResumeServiceUIError;

			NetworkChange.NetworkAvailabilityChanged += checkStationTimer_Tick;
			NetworkChange.NetworkAddressChanged += checkStationTimer_Tick;

			this.CurrentState = CreateState(StationStateEnum.Initial);
			this.menuServiceAction.Text = Properties.Resources.PauseWFService;
			this.menuQuit.Text =  Properties.Resources.QuitWFService;
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
				var userlogins = new List<UserLoginSetting>();
				var res = StationController.ListUser();
				foreach (var driver in res.drivers)
				{
					var userlogin = userloginContainer.GetUserLogin(driver.email);
					if (userlogin != null)
					{
						cmbEmail.Items.Add(userlogin.Email);
						//var menu = new ToolStripMenuItem(userlogin.Email, null, menuSwitchUser_Click);
						//menu.Name = userlogin.Email;
						userlogins.Add(userlogin);
					}
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
				var handle = Win32Helper.FindWindow(null, CLIENT_TITLE);
				Win32Helper.SetForegroundWindow(handle);
				Win32Helper.ShowWindow(handle, 5);
				return;
			}

			var lastLogin = userloginContainer.GetLastLogin();
			if (!string.IsNullOrEmpty(lastLogin))
			{
				LaunchClient(lastLogin);
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
				Close();
				return;
			}

			LoginedSession loginedSession = null;

			if (userlogin != null)
				loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", userlogin.Email));

			if (loginedSession != null || (userlogin != null && userlogin.RememberPassword))
			{
				userloginContainer.UpsertUserLoginSetting(userlogin);
				
				if (LaunchWavefaceClient(userlogin))
				{
					// if the function is called by OnLoad, Close() won't hide the mainform
					// so we have to play some tricks here
					this.WindowState = FormWindowState.Minimized;
					this.ShowInTaskbar = false;

					Close();
				}
			}
			else
			{
				GotoTabPage(tabSignIn, userlogin);
			}
		}


		private void WavefaceClientUIError(object sender, SimpleEventArgs evt)
		{
			var ex = (Exception)evt.param;
			messenger.ShowMessage(ex.Message);
		}

		private void PauseServiceUICallback(object sender, SimpleEventArgs evt)
		{
			CurrentState.Offlined();
		}

		private void PauseServiceUIError(object sender, SimpleEventArgs evt)
		{
			var ex = (Exception)evt.param;

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

			var ex = (Exception)evt.param;

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
				{
					clientProcess.Exited -= new EventHandler(clientProcess_Exited);
					clientProcess.CloseMainWindow();
				}

				StationController.SuspendSync(1000);
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
						var st = new StationStateInitial(this);
						st.Entering += this.BecomeInitialState;
						return st;
					}
				case StationStateEnum.Starting:
					{
						var st = new StationStateStarting(this);
						st.Entering += this.BecomeStartingState;
						return st;
					}
				case StationStateEnum.Running:
					{
						var st = new StationStateRunning(this);
						st.Entering += this.BecomeRunningState;
						return st;
					}
				case StationStateEnum.Syncing:
					{
						var st = new StationStateSyncing(this);
						st.Entering += this.BecomeSyncingState;
						return st;
					}
				case StationStateEnum.Stopping:
					{
						var st = new StationStateStopping(this);
						st.Entering += this.BecomeStoppingState;
						return st;
					}
				case StationStateEnum.Stopped:
					{
						var st = new StationStateStopped(this);
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
			if (CurrentState.Value == StationStateEnum.Running || CurrentState.Value == StationStateEnum.Syncing)
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
				TrayIconText = Properties.Resources.StartingWFService;

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

				var runningText = Properties.Resources.WFServiceRunning;
				menuServiceAction.Text = Properties.Resources.PauseWFService;

				menuServiceAction.Enabled = true;

				TrayIconText = runningText;
				//TrayIcon.ShowBalloonTip(1000, "Stream", runningText, ToolTipIcon.None);
			}
		}

		void BecomeSyncingState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeSyncingState), this, new EventArgs());
				}
			}
			else
			{
				TrayIcon.Icon = iconSyncing1;
				TrayIconText = Properties.Resources.WFServiceSyncing;
				menuServiceAction.Text = Properties.Resources.PauseWFService;

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
				
				var stoppedText = Properties.Resources.WFServiceStopped;
				menuServiceAction.Text = Properties.Resources.ResumeWFService;

				TrayIconText = stoppedText;
				//TrayIcon.ShowBalloonTip(1000, "Stream", stoppedText, ToolTipIcon.None);

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
				TrayIconText = Properties.Resources.StartingWFService;

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
				TrayIcon.Text = Properties.Resources.PausingWFService;

				this.uictrlPauseService.PerformAction();
			}
		}


		private void GotoTabPage(TabPage tabpage, UserLoginSetting userlogin = null)
		{
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
				//btnOK.Tag = userlogin;
				btnOK.Select();
				this.AcceptButton = btnOK;
			}
			else if (tabpage == tabSecondStationSetup)
			{
				btnOK2.Select();
				this.AcceptButton = btnOK2;
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
			if ((cmbEmail.Text == string.Empty) || (txtPassword.Text == string.Empty))
			{
				MessageBox.Show(Properties.Resources.FillAllFields, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!TestEmailFormat(cmbEmail.Text))
			{
				MessageBox.Show(Properties.Resources.InvalidEmail, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				Cursor = Cursors.WaitCursor;
				UserLoginSetting userlogin = userloginContainer.GetUserLogin(cmbEmail.Text);

				if (userlogin == null)
				{
					AddUserResult res = StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text, StationRegistry.StationId, Environment.MachineName);

					userlogin = new UserLoginSetting
						{
							Email = cmbEmail.Text.ToLower(),
							Password = SecurityHelper.EncryptPassword(txtPassword.Text),
							RememberPassword = chkRememberPassword.Checked
						};
					userloginContainer.UpsertUserLoginSetting(userlogin);
					RefreshUserList();

					m_LoginAction = () =>
					{
						LoginAndLaunchClient(userlogin);
					};

					if (res.IsPrimaryStation)
					{
						GotoTabPage(tabMainStationSetup, userlogin);
					}
					else
					{
						GotoTabPage(tabSecondStationSetup, userlogin);
					}
				}
				else
				{
					// In case the user is in AppData but not in Station DB (usually in testing environment)
					bool userAlreadyInDB = Wammer.Model.DriverCollection.Instance.FindOne(Query.EQ("email", cmbEmail.Text.ToLower())) != null;
					if (!userAlreadyInDB)
						StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text, StationRegistry.StationId, Environment.MachineName);

					userlogin.Password = SecurityHelper.EncryptPassword(txtPassword.Text);
					userlogin.RememberPassword = chkRememberPassword.Checked;
					userloginContainer.UpsertUserLoginSetting(userlogin);
					RefreshUserList();

					m_LoginAction = () =>
					{
						LoginAndLaunchClient(userlogin);
					};

					m_LoginAction();
				}
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Properties.Resources.AuthError, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;
				txtPassword.Focus();
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(Properties.Resources.StationDown, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Properties.Resources.ConnectCloudError, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception)
			{
				MessageBox.Show(Properties.Resources.UnknownSigninError, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private bool LaunchWavefaceClient()
		{
			return LaunchWavefaceClient(userloginContainer.GetLastUserLogin());
		}

		private bool LaunchWavefaceClient(UserLoginSetting userlogin)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				LoginedSession sessionData = LoginToStation(userlogin);

				string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
					"WavefaceWindowsClient.exe");
				clientProcess = Process.Start(execPath, "\"" + sessionData.session_token + "\"");
				clientProcess.EnableRaisingEvents = true;

				clientProcess.Exited -= new EventHandler(clientProcess_Exited);
				clientProcess.Exited += new EventHandler(clientProcess_Exited);

				return true;
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Properties.Resources.AuthError);
				GotoTabPage(tabSignIn, userlogin);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Properties.Resources.ConnectCloudError);
				GotoTabPage(tabSignIn, userlogin);
			}
			catch (Exception e)
			{
				MessageBox.Show(Properties.Resources.LogInError + Environment.NewLine + e.Message);
				GotoTabPage(tabSignIn, userlogin);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}

			return false;
		}

		private void LaunchClient(string sessionToken)
		{
			Cursor.Current = Cursors.WaitCursor;
			string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"WavefaceWindowsClient.exe");
			clientProcess = Process.Start(execPath, "\"" + sessionToken + "\"");
			clientProcess.EnableRaisingEvents = true;

			clientProcess.Exited -= new EventHandler(clientProcess_Exited);
			clientProcess.Exited += new EventHandler(clientProcess_Exited);
		}

		private LoginedSession LoginToStation(UserLoginSetting userlogin)
		{
			try
			{
				using (var agent = new DefaultWebClient())
				{
					var ret = User.LogIn(
						agent, 
						"http://localhost:9981/v2/",
						userlogin.Email, 
						SecurityHelper.DecryptPassword(userlogin.Password),
						CLIENT_API_KEY,
						(string)StationRegistry.GetValue("stationId", string.Empty),
						Environment.MachineName).LoginedInfo;

					userlogin.SessionToken = ret.session_token;
					userloginContainer.UpsertUserLoginSetting(userlogin);

					return ret;
				}
			}
			catch (WammerCloudException e)
			{
				throw StationController.ExtractApiRetMsg(e);
			}
		}

		void clientProcess_Exited(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
			{
				Invoke(new EventHandler(clientProcess_Exited), sender, e);
				return;
			}

			var exitCode = clientProcess.ExitCode;

			clientProcess.Exited -= new EventHandler(clientProcess_Exited);
			clientProcess = null;

			if (exitCode == -2)  // client logout
			{
				GotoTabPage(tabSignIn, userloginContainer.GetLastUserLogin());
			}
			else if (exitCode == -3)  // client unlink
			{
				var userlogin = userloginContainer.GetLastUserLogin();

				var isCleanResource = false;
				var cleanform = new CleanResourceForm(userlogin.Email);
				var cleanResult = cleanform.ShowDialog();
				if (cleanResult == DialogResult.Yes)
				{
					isCleanResource = true;
				}

				var res = StationController.ListUser();
				foreach (var driver in res.drivers)
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
			return Regex.IsMatch(emailAddress, EMAIL_MATCH_PATTERN);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (m_LoginAction == null)
				return;

			m_LoginAction();
		}

		private void ExitProgram()
		{
			formCloseEnabled = true;
			Close();
		}

		private void lblSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.Hide();

			var signUpUrl = string.Format("{0}/{1}/SignUp", m_CallbackUrl, FB_LOGIN_GUID);
			var postData = new FBLoginPostData()
			{
				device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
				device_name = Environment.MachineName,
				device = "windows",
				api_key = CLIENT_API_KEY,
				xurl = string.Format("{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s&account_type=%(account_type)s&email=%(email)s&password=%(password)s", signUpUrl)
			};

			var browser = new WebBrowser()
			{
				Dock = DockStyle.Fill
			};


			var dialog = new Form()
			{
				Width = 750,
				Height = 700,
				Text = this.Text,
				StartPosition = FormStartPosition.CenterParent
			};
			dialog.Controls.Add(browser);

			browser.Navigated += (s, ex) =>
			{
				var url = browser.Url;
				if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "SignUp"), RegexOptions.IgnoreCase))
				{
					dialog.DialogResult = DialogResult.OK;
				}
			};


			browser.Navigate(m_SignUpUrl,
				string.Empty,
				Encoding.UTF8.GetBytes(FastJSONHelper.ToFastJSON(postData)),
				"Content-Type: application/json");

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var url = browser.Url;
				var parameters = HttpUtility.ParseQueryString(url.Query);
				var apiRetCode = parameters["api_ret_code"];

				if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				{
					if (!this.IsDisposed)
						this.Show();
					return;
				}

				var sessionToken = parameters["session_token"];
				var userID = parameters["user_id"];
				var email = parameters["email"];
				var password = parameters["password"];
				var accountType = parameters["account_type"];

				if (accountType.Equals("native", StringComparison.CurrentCultureIgnoreCase))
				{
					this.cmbEmail.Text = email;
					this.txtPassword.Text = password;

					if (!this.IsDisposed)
						this.Show();
					return;
				}

				m_LoginAction = () =>
				{
					LoginAndLaunchClient(sessionToken, userID);
				};

				var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
				if (driver == null)
				{
					var res = StationController.AddUser(userID, sessionToken);

					//Show welcome msg
					if (res.IsPrimaryStation)
						GotoTabPage(tabMainStationSetup);
					else
						GotoTabPage(tabSecondStationSetup);

					if (!this.IsDisposed)
						this.Show();
					return;
				}

				m_LoginAction();
				return;
			}

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
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion

		public void LogOut(WebClient agent, string sessionToken, string apiKey)
		{
			InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);

			userloginContainer.UpdateLastLogin(string.Empty);

			var parameters = new Dictionary<object, object>{
			{CloudServer.PARAM_SESSION_TOKEN, sessionToken},
			{CloudServer.PARAM_API_KEY, apiKey}};

			CloudServer.requestPath(agent, "http://127.0.0.1:9981/v2/", "auth/logout", parameters, false);
		}

		private void menuSignIn_Click(object sender, EventArgs e)
		{
			if (menuSignIn.Text == Properties.Resources.LogoutMenuItem)
			{
				var lastLogin = userloginContainer.GetLastLogin();
				LoginedSession loginedSession = null;

				if (lastLogin != null)
				{
					if (clientProcess != null)
					{
						clientProcess.CloseMainWindow();
					}

					loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", lastLogin));
					LogOut(new WebClient(), loginedSession.session_token, loginedSession.apikey.apikey);
				}
			}
			GotoTabPage(tabSignIn, userloginContainer.GetLastUserLogin());
		}

		private void TrayMenu_VisibleChanged(object sender, EventArgs e)
		{
			var lastLogin = userloginContainer.GetLastLogin();
			LoginedSession loginedSession = null;

			if (lastLogin != null)
				loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", lastLogin));

			var isUserLogined = (loginedSession != null || (clientProcess != null && !clientProcess.HasExited));
			
			menuSignIn.Text = isUserLogined ? Properties.Resources.LogoutMenuItem :  Properties.Resources.LoginMenuItem;
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

		private void RefreshSyncingStatus()
		{
			m_Timer.Stop();

			var iconText = TrayIcon.BalloonTipText;
			var upRemainedCount = m_UpRemainedCountCounter.NextValue();
			var downloadRemainedCount = m_DownRemainedCountCounter.NextValue();

			if (upRemainedCount > 0 || downloadRemainedCount > 0)
			{
				if (CurrentState.Value == StationStateEnum.Running)
				{
					CurrentState.StartSyncing();
				}

				if (CurrentState.Value == StationStateEnum.Syncing)
				{
					var upSpeed = m_UpStreamRateCounter.NextValue() / 1024;
					var downloadSpeed = m_DownStreamRateCounter.NextValue() / 1024;

					var upSpeedUnit = (upSpeed <= 1024) ? "KB/s" : "MB/s";
					var downloadSpeedUnit = (downloadSpeed <= 1024) ? "KB/s" : "MB/s";

					upSpeed = upRemainedCount == 0 ? 0 : ((upSpeed >= 1024) ? upSpeed / 1024 : upSpeed);
					downloadSpeed = downloadSpeed == 0 ? 0 : ((downloadSpeed >= 1024) ? downloadSpeed / 1024 : downloadSpeed);

					iconText = string.Format("{0}{1}↑({2}): {3:0.0} {4}{5}↓({6}): {7:0.0}{8}",
						iconText,
						Environment.NewLine,
						upRemainedCount,
						upSpeed,
						upSpeedUnit,
						Environment.NewLine,
						downloadRemainedCount,
						downloadSpeed,
						downloadSpeedUnit);

					TrayIcon.Icon = (TrayIcon.Icon == iconSyncing1 ? iconSyncing2 : iconSyncing1);
				}
			}
			else
			{
				if (CurrentState.Value == StationStateEnum.Syncing)
				{
					CurrentState.StopSyncing();
					TrayIcon.ShowBalloonTip(1000, "Stream", Properties.Resources.WFServiceRunning, ToolTipIcon.None);
				}
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
			var lastLogin = userloginContainer.GetLastLogin();
			LoginedSession loginedSession = null;

			if (lastLogin != null)
				loginedSession = Wammer.Model.LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", lastLogin));

			var isUserLogined = (loginedSession != null || (clientProcess != null && !clientProcess.HasExited));

			tsmiOpenStream.Visible = isUserLogined;
		}


		private void fbLoginButton1_Click(object sender, EventArgs e)
		{
				this.Hide();
				var fbLoginUrl = string.Format("{0}/{1}/FBLogin", m_CallbackUrl, FB_LOGIN_GUID);
				var postData = new FBLoginPostData()
				{
					device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
					device_name = Environment.MachineName,
					device = "windows",
					api_key = CLIENT_API_KEY,
					xurl = string.Format("{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s", fbLoginUrl)
				};

				var browser = new WebBrowser()
				{

					Dock = DockStyle.Fill
				};


				var dialog = new Form()
				{
					Width = 750,
					Height = 700,
					Text = this.Text,
					StartPosition = FormStartPosition.CenterParent
				};
				dialog.Controls.Add(browser);

				browser.Navigated += (s, ex) =>
				{
					var url = browser.Url;
					if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "FBLogin"), RegexOptions.IgnoreCase))
					{
						dialog.DialogResult = DialogResult.OK;
					}
				};

				browser.Navigate(m_FBLoginUrl,
					string.Empty,
					Encoding.UTF8.GetBytes(FastJSONHelper.ToFastJSON(postData)),
					"Content-Type: application/json");

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var url = browser.Url.Query;
					var parameters = HttpUtility.ParseQueryString(url);
					var apiRetCode = parameters["api_ret_code"];

					if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
					{
						if (!this.IsDisposed)
							this.Show();
						return;
					}
					
					var sessionToken = parameters["session_token"];
					var userID = parameters["user_id"];

					m_LoginAction = () =>
										{
											LoginAndLaunchClient(sessionToken, userID);
										};

					var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
					if (driver == null)
					{
						var res = StationController.AddUser(userID, sessionToken);

						//Show welcome msg
						if (res.IsPrimaryStation)
							GotoTabPage(tabMainStationSetup);
						else
							GotoTabPage(tabSecondStationSetup);

						if (!this.IsDisposed)
							this.Show();
						return;
					}

					m_LoginAction();
					return;
				}
				if (!this.IsDisposed)
					this.Show();
		}

		private void LoginAndLaunchClient(UserLoginSetting loginSetting)
		{
			if (LaunchWavefaceClient(loginSetting))
				Close();
		}

		private void LoginAndLaunchClient(string sessionToken, string userID)
		{
			//Login user
			StationController.UserLogin(CLIENT_API_KEY, userID, sessionToken);

			userloginContainer.UpdateLastLogin(sessionToken);

			LaunchClient(sessionToken);
			Close();
		}

		private void btnOK2_Click(object sender, EventArgs e)
		{
			if (m_LoginAction == null)
				return;

			m_LoginAction();
		}

		private void tabSignIn_Click(object sender, EventArgs e)
		{

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

	public class FBLoginPostData
	{
		public string device_id { get; set; }
		public string device_name { get; set; }
		public string device { get; set; }
		public string api_key { get; set; }
		public string xurl { get; set; }
	}


	#region PauseServiceUIController
	public class PauseServiceUIController : SimpleUIController
	{
		public PauseServiceUIController(MainForm form)
			: base(form)
		{
		}

		protected override object Action(object obj)
		{
			StationController.SuspendSync(60000);
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
