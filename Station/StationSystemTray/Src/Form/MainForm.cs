using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using log4net;
using MongoDB.Driver.Builders;
using StationSystemTray.Properties;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;
using Wammer.Station.Management;
using Wammer.Utility;
using Timer = System.Windows.Forms.Timer;
using StationSystemTray.Dialog;
using Waveface.Common;
using StationSystemTray.Src.Dialog;

namespace StationSystemTray
{
	public partial class MainForm : Form, StationStateContext
	{
		#region DllImport

		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessageTimeout(
			IntPtr windowHandle,
			int Msg,
			IntPtr wParam,
			IntPtr lParam,
			int flags,
			uint timeout,
			out int result);

		#endregion DllImport

		#region Const

		private const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		//private const string CLIENT_TITLE = "Stream ";
		private const int STATION_TIMER_LONG_INTERVAL = 60000;
		private const int STATION_TIMER_SHORT_INTERVAL = 3000;

		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";

		private const string SIGNUP_URL_PATH = @"/signup";
		private const string LOGIN_URL_PATH = @"/sns/facebook/signin";
		private const string CALLBACK_URL_PATH = @"/client/callback";

		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";

		private const string EMAIL_MATCH_PATTERN = @"^(([^<>()[\]\\.,;:\s@\""]+"
												   + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
												   + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
												   + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
												   + @"[a-zA-Z]{2,}))$";

		#endregion Const

		#region Var

		private string _baseUrl;
		private string _callbackUrl;
		private string _fbLoginUrl;
		private string _signUpUrl;
		private Timer _timer;
		private CustomWindow _messageReceiver;
		private ContactUsDialog _contactUsDialog;
		#endregion Var

		#region Private Property

		private string m_BaseUrl
		{
			get {
				return _baseUrl ??
					   (_baseUrl = CloudServer.BaseUrl.Contains("develop.waveface.com") ? DEV_WEB_BASE_PAGE_URL : 
					   (CloudServer.BaseUrl.Contains("staging.waveface.com") ? STAGING_BASE_URL : WEB_BASE_URL));
			}
		}

		private string m_CallbackUrl
		{
			get { return _callbackUrl ?? (_callbackUrl = Path.Combine(m_BaseUrl, CALLBACK_URL_PATH)); }
		}

		private string m_SignUpUrl
		{
			get
			{
				return _signUpUrl ??
					   (_signUpUrl = m_BaseUrl + SIGNUP_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture));
			}
		}

		private string m_FBLoginUrl
		{
			get
			{
				return _fbLoginUrl ??
					   (_fbLoginUrl = m_BaseUrl + LOGIN_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture));
			}
		}

		private Action m_LoginAction { get; set; }

		private CustomWindow m_MessageReceiver 
		{
			get
			{
				return _messageReceiver ?? (_messageReceiver = new CustomWindow("SystemTrayMessageReceiver", "SystemTrayMessageReceiver"));
			}
		}

		#endregion Private Property

		public static ILog logger = LogManager.GetLogger("MainForm");
		private readonly object cs = new object();
		private readonly IPerfCounter m_DownRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false);
		private readonly IPerfCounter m_DownStreamRateCounter = PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false);
		private readonly IPerfCounter m_UpRemainedCountCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false);
		private readonly IPerfCounter m_UpStreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);

		public Process clientProcess;
		public Icon iconErrorStopped;

		public Icon iconInit;
		public Icon iconPaused;
		public Icon iconRunning;
		public Icon iconSyncing1;
		public Icon iconSyncing2;
		public Icon iconWarning;
		private bool initMinimized;

		private string lblMainStationSetupText;
		private string lblSecondStationSetupText;
		private ApplicationSetting appSetting;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private StationStatusUIController uictrlStationStatus;
		private UserLoginSettingContainer userloginContainer;

		#region Private Property

		private Timer m_Timer
		{
			get { return _timer ?? (_timer = new Timer()); }
		}

		#endregion Private Property

		public MainForm(bool initMinimized)
		{
			Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.initMinimized = initMinimized;
			
			this.appSetting = new ApplicationSetting();
			if (!appSetting.isUpgraded)
			{
				appSetting.Upgrade();
				appSetting.isUpgraded = true;
				appSetting.Save();
			}

			m_MessageReceiver.WndProc += new EventHandler<MessageEventArgs>(m_MessageReceiver_WndProc);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			m_Timer.Interval = 500;
			m_Timer.Tick += (sender, e) => RefreshSyncingStatus();
			m_Timer.Start();

			tbxEMail.DataBindings.Add("Text", cmbEmail, "Text");
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.LogDebugMsg("Unhandle exception: " + e.ExceptionObject);
			CloseTimelineProgram();
		}

		private void CloseTimelineProgram()
		{
			if (clientProcess != null)
			{
				clientProcess.Exited -= clientProcess_Exited;
				clientProcess.CloseMainWindow();

				if (!clientProcess.WaitForExit(300))
				{
					try
					{
						clientProcess.Kill();
						clientProcess.WaitForExit(300);
					}
					catch
					{
					}
				}
				clientProcess.Dispose();
				clientProcess = null;
			}
		}

		public StationState CurrentState { get; private set; }

		public string TrayIconText
		{
			get { return TrayIcon.Text; }
			set
			{
				TrayIcon.Text = value;
				TrayIcon.BalloonTipText = value;
			}
		}

		public string WindowsTitle
		{
			get { return Text; }
		}

		#region StationStateContext Members

		public void GoToState(StationStateEnum state)
		{
			lock (cs)
			{
				CurrentState.OnLeaving(this, new EventArgs());
				CurrentState = CreateState(state);
				CurrentState.OnEntering(this, new EventArgs());
			}
		}

		#endregion StationStateContext Members

		void m_MessageReceiver_WndProc(object sender, MessageEventArgs e)
		{
			switch (e.Message)
			{
				case 0x401:
					logger.Debug("Timeline trigger by new stream");
					GotoTimeline();
					break;
				case 0x402:
					logger.Debug("Closed by another application");
					QuitStream();
					break;
				case 0x403:
					logger.Debug("Setting trigger by windows client");
					OpenSettingDialog();
					break;
				case 0x404:
					SendSyncStatusToClient();
					break;
			}
		}

		private void SendSyncStatusToClient()
		{
			var handle = Win32Helper.FindWindow("WindowsClientMessageReceiver", null);
			if (handle != IntPtr.Zero)
			{
				int ret;
				SendMessageTimeout(handle, ((TrayIconText.StartsWith(Resources.WFServiceSyncing)) ? 0x402 : 0x403), IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			var settings = new ApplicationSettings();
			if (!settings.isUpgraded)
			{
				settings.Upgrade();
				settings.isUpgraded = true;
			}

			using (Graphics g = this.CreateGraphics())
			{
				if (g.DpiX == 120)
					label2.Font = new Font("Arial", 8);
			}

			userloginContainer = new UserLoginSettingContainer(settings);

			iconInit = Icon.FromHandle(Resources.stream_tray_init.GetHicon());
			iconRunning = Icon.FromHandle(Resources.stream_tray_working.GetHicon());
			iconPaused = Icon.FromHandle(Resources.stream_tray_pause.GetHicon());
			iconWarning = Icon.FromHandle(Resources.stream_tray_warn.GetHicon());
			iconSyncing1 = Icon.FromHandle(Resources.stream_tray_syncing1.GetHicon());
			iconSyncing2 = Icon.FromHandle(Resources.stream_tray_syncing2.GetHicon());
			iconErrorStopped = Icon.FromHandle(Resources.stream_tray_init.GetHicon());
			TrayIcon.Icon = iconInit;

			lblMainStationSetupText = lblMainStationSetup.Text;
			lblSecondStationSetupText = lblSecondStationSetup.Text;

			uictrlStationStatus = new StationStatusUIController(this);
			uictrlStationStatus.UICallback += StationStatusUICallback;
			uictrlStationStatus.UIError += StationStatusUIError;

			uictrlPauseService = new PauseServiceUIController(this);
			uictrlPauseService.UICallback += PauseServiceUICallback;
			uictrlPauseService.UIError += PauseServiceUIError;

			uictrlResumeService = new ResumeServiceUIController(this);
			uictrlResumeService.UICallback += ResumeServiceUICallback;
			uictrlResumeService.UIError += ResumeServiceUIError;

			NetworkChange.NetworkAvailabilityChanged += NetworkChanged;
			//NetworkChange.NetworkAddressChanged += NetworkChanged;

			CurrentState = CreateState(StationStateEnum.Initial);
			menuServiceAction.Text = Resources.PauseWFService;
			menuQuit.Text = Resources.QuitWFService;
			tsmiOpenStream.Text = Resources.OpenStream;

			RefreshUserList();

			checkStationTimer.Enabled = true;
			checkStationTimer.Start();

			CurrentState.Onlining();

			if (initMinimized)
			{
				WindowState = FormWindowState.Minimized;
				ShowInTaskbar = false;
				initMinimized = false;
			}
			else
			{
				GotoTimeline();
			}
		}

		private void RefreshUserList()
		{
			cmbEmail.Items.Clear();
			try
			{
				var userlogins = new List<UserLoginSetting>();
				ListDriverResponse res = StationController.ListUser();
				foreach (Driver driver in res.drivers)
				{
					UserLoginSetting userlogin = userloginContainer.GetUserLogin(driver.email);
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

		private void GotoTimeline()
		{
			if (clientProcess != null && !clientProcess.HasExited)
			{
				var handle = Win32Helper.FindWindow("WindowsClientMessageReceiver", null);

				if (handle == IntPtr.Zero)
					return;

				int ret;
				SendMessageTimeout(handle, 0x401, IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);

				return;
			}

			if (!string.IsNullOrEmpty(appSetting.CurrentSession))
			{
				LaunchClient(appSetting.CurrentSession);
				WindowState = FormWindowState.Minimized;
				ShowInTaskbar = false;
				Hide();
				return;
			}

			if (m_SettingDialog != null)
			{
				m_SettingDialog.Activate();
				return;
			}

			GotoTabPage(tabNewOrOldUser, null);

		}

		private void PauseServiceUICallback(object sender, SimpleEventArgs evt)
		{
			CurrentState.Offlined();
		}

		private void PauseServiceUIError(object sender, SimpleEventArgs evt)
		{
			CurrentState.Error();
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

			if (CurrentState.Value != StationStateEnum.Stopped)
				CurrentState.Onlining();
		}

		private void StationStatusUIError(object sender, SimpleEventArgs evt)
		{
			// recover quickly if falling to error/offline state unexpectedly
			checkStationTimer.Interval = STATION_TIMER_SHORT_INTERVAL;

			var ex = (Exception)evt.param;

			if (ex is ConnectToCloudException)
			{
				logger.Info("Unable to connect to Waveface cloud", ex);
				CurrentState.Error();
			}
			else if (ex is WebException)
			{
				logger.Info("Unable to connect to internet", ex);
				CurrentState.Error();
			}
			else
			{
				logger.Warn("Unexpected exception in station status cheking", ex);
				CurrentState.Error();
			}
		}

		private void menuQuit_Click(object sender, EventArgs e)
		{
			QuitStream();
		}

		private void QuitStream()
		{
			try
			{
				m_Timer.Stop();

				if (TrayIcon != null)
				{
					TrayIcon.Visible = false;
					TrayIcon.Dispose();
					TrayIcon = null;
				}

				CloseTimelineProgram();

				StationController.SuspendSync(1000);
			}
			catch (Exception ex)
			{
				logger.Error("StationOffline fail", ex);
			}
			finally
			{
				Application.Exit();
			}
		}

		private StationState CreateState(StationStateEnum state)
		{
			switch (state)
			{
				case StationStateEnum.Initial:
					{
						var st = new StationStateInitial(this);
						st.Entering += BecomeInitialState;
						return st;
					}
				case StationStateEnum.Starting:
					{
						var st = new StationStateStarting(this);
						st.Entering += BecomeStartingState;
						return st;
					}
				case StationStateEnum.Running:
					{
						var st = new StationStateRunning(this);
						st.Entering += BecomeRunningState;
						return st;
					}
				case StationStateEnum.Syncing:
					{
						var st = new StationStateSyncing(this);
						st.Entering += BecomeSyncingState;
						return st;
					}
				case StationStateEnum.Stopping:
					{
						var st = new StationStateStopping(this);
						st.Entering += BecomeStoppingState;
						return st;
					}
				case StationStateEnum.Stopped:
					{
						var st = new StationStateStopped(this);
						st.Entering += BecomeStoppedState;
						return st;
					}
				case StationStateEnum.ErrorStopped:
					{
						var st = new StationStateErrorStopped(this);
						st.Entering += BecomeErrorStoppedState;
						return st;
					}
				default:
					throw new NotImplementedException();
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

		private void trayIcon_DoubleClicked(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();
			GotoTimeline();
		}

		[DllImport("wininet")]
		public static extern bool InternetGetConnectedState(
			ref uint lpdwFlags,
			uint dwReserved
			);
		private void NetworkChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			//try
			//{

			if (!e.IsAvailable)
			{
				TrayIcon.ShowBalloonTip(1000, "Stream", Resources.NETWORK_UNAVAILABLE, ToolTipIcon.None);
				SendSyncStatusToClient();
			}

				////�s�u��Flag
				//uint flags = 0x0;

				//bool rtvl;

				////���o�����q���ثe���s�u���A
				//rtvl = InternetGetConnectedState(ref flags, 0);

				//if (!rtvl)
				//{
				//    TrayIcon.ShowBalloonTip(1000, "Stream", Resources.NETWORK_UNAVAILABLE, ToolTipIcon.None);
				//    SendSyncStatusToClient();
				//}

				//var p = new Ping();
				//if (p.Send("waveface.com", 500).Status != IPStatus.Success)
				//    throw new WebException();

				//using (var agent = new DefaultWebClient())
				//{
				//    agent.DownloadData("http://www.google.com");
				//}
			//}
			//catch
			//{
			//    TrayIcon.ShowBalloonTip(1000, "Stream", Resources.NETWORK_UNAVAILABLE, ToolTipIcon.None);
			//    SendSyncStatusToClient();
			//}
			checkStationTimer_Tick(sender, e);
		}

		private void checkStationTimer_Tick(object sender, EventArgs e)
		{
//#if !DEBUG
			uictrlStationStatus.PerformAction();
//#endif
		}

		private void BecomeInitialState(object sender, EventArgs evt)
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
				TrayIcon.Icon = iconInit;
				TrayIconText = Resources.StartingWFService;

				menuServiceAction.Enabled = false;
			}
		}

		private void BecomeRunningState(object sender, EventArgs evt)
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

				string runningText = Resources.WFServiceRunning;
				menuServiceAction.Text = Resources.PauseWFService;

				menuServiceAction.Enabled = true;

				TrayIconText = runningText;

				SendSyncStatusToClient();
				//TrayIcon.ShowBalloonTip(1000, "Stream", runningText, ToolTipIcon.None);
			}
		}

		private void BecomeSyncingState(object sender, EventArgs evt)
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
				TrayIconText = Resources.WFServiceSyncing;
				menuServiceAction.Text = Resources.PauseWFService;

				menuServiceAction.Enabled = true;

				SendSyncStatusToClient();
			}
		}

		private void BecomeStoppedState(object sender, EventArgs evt)
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
				var handle = Win32Helper.FindWindow("WindowsClientMessageReceiver", null);

				if (handle != IntPtr.Zero)
				{
					int ret;
					SendMessageTimeout(handle, 0x403, IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);
				}

				TrayIcon.Icon = iconPaused;

				string stoppedText = Resources.WFServiceStopped;
				menuServiceAction.Text = Resources.ResumeWFService;

				TrayIconText = stoppedText;

				menuServiceAction.Enabled = true;
			}
		}

		private void BecomeErrorStoppedState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeErrorStoppedState), this, new EventArgs());
				}
			}
			else
			{
				TrayIcon.Icon = iconErrorStopped;

				string stoppedText = Resources.WFServiceStopped;
				menuServiceAction.Text = Resources.ResumeWFService;

				TrayIconText = stoppedText;

				menuServiceAction.Enabled = true;
			}
		}

		private void BecomeStartingState(object sender, EventArgs evt)
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
				TrayIconText = Resources.StartingWFService;

				uictrlResumeService.PerformAction();
			}
		}

		private void BecomeStoppingState(object sender, EventArgs evt)
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
				TrayIcon.Text = Resources.PausingWFService;

				uictrlPauseService.PerformAction();
			}
		}

		private void GotoTabPage(TabPage tabpage, UserLoginSetting userlogin = null)
		{
			tabControl.SelectedTab = tabpage;

			if (WindowState == FormWindowState.Minimized)
			{
				WindowState = FormWindowState.Normal;
				ShowInTaskbar = true;
			}

			uint foreThread = Win32Helper.GetWindowThreadProcessId(Win32Helper.GetForegroundWindow(), IntPtr.Zero);
			uint appThread = Win32Helper.GetCurrentThreadId();
			if (foreThread != appThread)
			{
				Win32Helper.AttachThreadInput(foreThread, appThread, true);
				Win32Helper.BringWindowToTop(Handle);
				Show();
				Win32Helper.AttachThreadInput(foreThread, appThread, false);
			}
			else
			{
				Win32Helper.BringWindowToTop(Handle);
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
					txtPassword.Text = userlogin.RememberPassword ? SecurityHelper.DecryptPassword(userlogin.Password) : string.Empty;
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

				AcceptButton = loginButton1;
			}
			else if (tabpage == tabMainStationSetup)
			{
				//btnOK.Tag = userlogin;
				btnOK.Select();
				AcceptButton = btnOK;
			}
			else if (tabpage == tabSecondStationSetup)
			{
				btnOK2.Select();
				AcceptButton = btnOK2;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				logger.Info("application is closed by user");
				Hide();
				e.Cancel = true;
				return;
			}
			logger.Info("application is closed by code");
		}

		private void btnSignIn_Click(object sender, EventArgs e)
		{
			if ((cmbEmail.Text == string.Empty) || (txtPassword.Text == string.Empty))
			{
				MessageBox.Show(Resources.FillAllFields, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!TestEmailFormat(cmbEmail.Text))
			{
				MessageBox.Show(Resources.InvalidEmail, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				Cursor = Cursors.WaitCursor;
				UserLoginSetting userlogin = userloginContainer.GetUserLogin(cmbEmail.Text);

				if (userlogin == null)
				{
					AddUserResponse res = StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text,
																	StationRegistry.StationId, Environment.MachineName);

					userlogin = new UserLoginSetting
									{
										Email = cmbEmail.Text.ToLower(),
										Password = SecurityHelper.EncryptPassword(txtPassword.Text),
										RememberPassword = chkRememberPassword.Checked
									};
					userloginContainer.SaveCurLoginedUser(userlogin);
					RefreshUserList();

					m_LoginAction = () => LoginAndLaunchClient(userlogin);

					UserStation station = GetPrimaryStation(res.Stations);
					lblMainStationSetup.Text = string.Format(lblMainStationSetupText,
															 (station == null) ? "None" : station.computer_name);
					lblSecondStationSetup.Text = string.Format(lblSecondStationSetupText,
															   (station == null) ? "None" : station.computer_name);

					GotoTabPage(res.IsPrimaryStation ? tabMainStationSetup : tabSecondStationSetup, userlogin);
				}
				else
				{
					// In case the user is in AppData but not in Station DB (usually in testing environment)
					bool userAlreadyInDB = DriverCollection.Instance.FindOne(Query.EQ("email", cmbEmail.Text.ToLower())) != null;
					if (!userAlreadyInDB)
						StationController.AddUser(cmbEmail.Text.ToLower(), txtPassword.Text, StationRegistry.StationId,
												  Environment.MachineName);

					userlogin.Password = SecurityHelper.EncryptPassword(txtPassword.Text);
					userlogin.RememberPassword = chkRememberPassword.Checked;
					userloginContainer.SaveCurLoginedUser(userlogin);
					RefreshUserList();

					m_LoginAction = () => LoginAndLaunchClient(userlogin);

					m_LoginAction();
				}
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;
				txtPassword.Focus();
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (VersionNotSupportedException)
			{
				handleVersionNotSupported();
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UnknownSigninError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private UserStation GetPrimaryStation(IEnumerable<UserStation> stations)
		{
			return (from station in stations
					where station.type == "primary"
					select station).FirstOrDefault();
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
				if (clientProcess != null)
				{
					clientProcess.EnableRaisingEvents = true;

					clientProcess.Exited -= clientProcess_Exited;
					clientProcess.Exited += clientProcess_Exited;
				}

				return true;
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				GotoTabPage(tabSignIn, userlogin);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				GotoTabPage(tabSignIn, userlogin);
			}
			catch (VersionNotSupportedException)
			{
				handleVersionNotSupported();
			}
			catch (Exception e)
			{
				MessageBox.Show(Resources.LogInError + Environment.NewLine + e.Message, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				GotoTabPage(tabSignIn, userlogin);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}

			return false;
		}

		private static void handleVersionNotSupported()
		{
			var result = MessageBox.Show(Resources.NeedToUpgrade, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				AutoUpdate update = new AutoUpdate(true);
				if (update.IsUpdateRequired())
					update.ShowUpdateNeededUI();
				else
					MessageBox.Show(Resources.ALREAD_UPDATED, Resources.APP_NAME);
			}
		}

		private void LaunchClient(string sessionToken)
		{
			if(m_SettingDialog != null)
				m_SettingDialog.Close();

			Cursor.Current = Cursors.WaitCursor;
			string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										   "WavefaceWindowsClient.exe");

			clientProcess = Process.Start(execPath, "\"" + sessionToken + "\"");
			if (clientProcess != null)
			{
				clientProcess.EnableRaisingEvents = true;

				clientProcess.Exited -= clientProcess_Exited;
				clientProcess.Exited += clientProcess_Exited;
			}
		}

		private LoginedSession LoginToStation(UserLoginSetting userlogin)
		{
			try
			{
				LoginedSession ret = User.LogIn(
					"http://localhost:9981/v2/",
					userlogin.Email,
					SecurityHelper.DecryptPassword(userlogin.Password),
					CLIENT_API_KEY,
					(string) StationRegistry.GetValue("stationId", string.Empty),
					Environment.MachineName).LoginedInfo;

				userloginContainer.SaveCurLoginedUser(userlogin);
				userloginContainer.SaveCurLoginedSession(ret.session_token);

				return ret;
			}
			catch (WammerCloudException e)
			{
				throw StationController.ExtractApiRetMsg(e);
			}
		}

		private void clientProcess_Exited(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler(clientProcess_Exited), sender, e);
				return;
			}

			if(m_SettingDialog != null)
				m_SettingDialog.Close();

			int exitCode = clientProcess.ExitCode;

			clientProcess.Exited -= clientProcess_Exited;
			clientProcess = null;

			if (exitCode == -2) // client logout
			{
				Unlink();
			}
			else if (exitCode == -3) // client unlink
			{
				UserLoginSetting userlogin = userloginContainer.GetLastUserLogin();

				bool isCleanResource = false;
				var cleanform = new CleanResourceDialog(userlogin.Email);
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
				GotoTabPage(tabSignIn);
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

		private void lblSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Hide();

				string signUpUrl = string.Format("{0}/{1}/SignUp", m_CallbackUrl, FB_LOGIN_GUID);
				var postData = new FBPostData
								{
									device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
									device_name = Environment.MachineName,
									device = "windows",
									api_key = CLIENT_API_KEY,
									xurl =
										string.Format(
											"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s&account_type=%(account_type)s&email=%(email)s&password=%(password)s&is_new_user=%(is_new_user)s",
											signUpUrl),
									locale = Thread.CurrentThread.CurrentCulture.ToString(),
									show_tutorial = "false"
								};


				var dialog = new SignUpDialog()
								{
									Text = Resources.SIGNUP_PAGE_TITLE,
					StartPosition = FormStartPosition.CenterParent
								};
				var browser = dialog.Browser;
				var signupOK = false;
				browser.Navigated += (s, ex) =>
										{
											var url = browser.Url;

											if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "SignUp"),
															  RegexOptions.IgnoreCase))
											{
												var parameters = HttpUtility.ParseQueryString(url.Query);

												var isNewUser = parameters["is_new_user"];

												if (isNewUser.Equals("true",StringComparison.CurrentCultureIgnoreCase))
													dialog.ShowTutorial();
												else
												dialog.DialogResult = DialogResult.OK;

												signupOK = true;
											}
										};

				browser.Navigate(m_SignUpUrl,
								 string.Empty,
								 Encoding.UTF8.GetBytes(postData.ToFastJSON()),
								 "Content-Type: application/json");

				dialog.ShowDialog(this);
				if (signupOK)
				{
					var url = browser.Url;
					var parameters = HttpUtility.ParseQueryString(url.Query);
					var apiRetCode = parameters["api_ret_code"];

					if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
					{
						if (!IsDisposed)
							Show();
						return;
					}

					var sessionToken = parameters["session_token"];
					var userID = parameters["user_id"];
					var email = parameters["email"];
					var password = parameters["password"];
					var accountType = parameters["account_type"];

					if (accountType.Equals("native", StringComparison.CurrentCultureIgnoreCase))
					{
						cmbEmail.Text = email;
						txtPassword.Text = password;

						if (!IsDisposed)
							Show();
						return;
					}

					m_LoginAction = () => LoginAndLaunchClient(sessionToken, userID);

					var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
					if (driver == null)
					{
						AddUserResponse res = StationController.AddUser(userID, sessionToken);

						UserStation station = GetPrimaryStation(res.Stations);
						lblMainStationSetup.Text = string.Format(lblMainStationSetupText,
																 (station == null) ? "None" : station.computer_name);
						lblSecondStationSetup.Text = string.Format(lblSecondStationSetupText,
																   (station == null) ? "None" : station.computer_name);

						//Show welcome msg
						GotoTabPage(res.IsPrimaryStation ? tabMainStationSetup : tabSecondStationSetup);

						if (!IsDisposed)
							Show();
						return;
					}

					m_LoginAction();
					return;
				}

				if (!IsDisposed)
					Show();
			}
			catch (AuthenticationException)
			{
				if (!IsDisposed)
					Show();

				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;
				txtPassword.Focus();
			}
			catch (StationServiceDownException)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (VersionNotSupportedException)
			{
				handleVersionNotSupported();
			}
			catch (Exception)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void LogoutFB()
		{
			InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
		}

		public void LogOut(string sessionToken, string apiKey)
		{
			try
			{
				LogoutFB();
				appSetting.CurrentSession = null;
				appSetting.Save();
				StationController.UserLogout(apiKey, sessionToken);
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Unlink()
		{
			DebugInfo.ShowMethod();

			CloseTimelineProgram();

			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", appSetting.CurrentSession));

			if (loginedSession != null)
				LogOut(loginedSession.session_token, loginedSession.apikey.apikey);

			QuitStream();
		}

		private void TrayMenu_VisibleChanged(object sender, EventArgs e)
		{
		}

		private void cmbEmail_TextChanged(object sender, EventArgs e)
		{
			var userlogin = userloginContainer.GetUserLogin(cmbEmail.Text);
			if (userlogin != null)
			{
				txtPassword.Text = userlogin.RememberPassword ? SecurityHelper.DecryptPassword(userlogin.Password) : string.Empty;
				chkRememberPassword.Checked = userlogin.RememberPassword;
			}
			else
			{
				txtPassword.Text = string.Empty;
				chkRememberPassword.Checked = false;
			}
		}

		private void GetSpeedAndUnit(float value,ref float speed,ref string unit)
		{
			var units = new string[] { "B/s", "KB/s", "MB/s" };
			var index = Array.IndexOf(units, unit);

			if (index == -1)
				index = 0;

			if (value > 1024)
			{
				value = value / 1024;
				speed = value;
				unit = units[index + 1];
				GetSpeedAndUnit(value, ref speed, ref unit);
				return;
			}

			speed = value;
			unit = units[index];
		}

		Queue<float> _upRemainedCount = new Queue<float>();
		Queue<float> _downRemainedCount = new Queue<float>();
		Queue<float> _upSpeeds = new Queue<float>();
		Queue<float> _downSpeeds = new Queue<float>();
		private void RefreshSyncingStatus()
		{
			try
			{
				m_Timer.Stop();

				var iconText = TrayIcon.BalloonTipText;
				var upRemainedCount = m_UpRemainedCountCounter.NextValue();
				var downloadRemainedCount = m_DownRemainedCountCounter.NextValue();

				if (_upRemainedCount.Count >= 10)
					_upRemainedCount.Dequeue();

				if (_downRemainedCount.Count >= 10)
					_downRemainedCount.Dequeue();

				_upRemainedCount.Enqueue(upRemainedCount);
				_downRemainedCount.Enqueue(downloadRemainedCount);

				if (upRemainedCount > 0 || downloadRemainedCount > 0)
				{
					if (CurrentState != null && CurrentState.Value == StationStateEnum.Running)
					{
						CurrentState.StartSyncing();
					}

					if (CurrentState != null && CurrentState.Value == StationStateEnum.Syncing)
					{
						var upSpeed = m_UpStreamRateCounter.NextValue();
						var downloadSpeed = m_DownStreamRateCounter.NextValue();

						if (_upSpeeds.Count >= 5)
							_upSpeeds.Dequeue();
						_upSpeeds.Enqueue(upSpeed);

						if (_downSpeeds.Count >= 5)
							_downSpeeds.Dequeue();
						_downSpeeds.Enqueue(downloadSpeed);

						if (_upSpeeds.Count >= 0)
							upSpeed = _upSpeeds.Average();

						if (_downSpeeds.Count >= 0)
							downloadSpeed = _downSpeeds.Average();

						string upSpeedUnit = string.Empty;
						GetSpeedAndUnit(upSpeed, ref upSpeed, ref upSpeedUnit);

						string downloadSpeedUnit = string.Empty;
						GetSpeedAndUnit(downloadSpeed, ref downloadSpeed, ref downloadSpeedUnit);

						upSpeed = upRemainedCount == 0 ? 0 : upSpeed;
						downloadSpeed = downloadSpeed == 0 ? 0 : downloadSpeed;


						if (upRemainedCount > 0)
						{
							iconText = string.Format(Resources.INDICATOR_PATTERN,
													 iconText,
													 Environment.NewLine,
													 Resources.UPLOAD_INDICATOR,
													 (upRemainedCount > 999) ? "999+" : upRemainedCount.ToString(),
													 upSpeed,
													 upSpeedUnit);
						}

						if (downloadRemainedCount > 0)
						{
							iconText = string.Format(Resources.INDICATOR_PATTERN,
													 iconText,
													 Environment.NewLine,
													 Resources.DOWNLOAD_INDICATOR,
													 (downloadRemainedCount > 999) ? "999+" : downloadRemainedCount.ToString(),
													 downloadSpeed,
													 downloadSpeedUnit);
						}

						TrayIcon.Icon = (TrayIcon.Icon == iconSyncing1 ? iconSyncing2 : iconSyncing1);
					}
				}
				else
				{
					if (CurrentState != null && CurrentState.Value == StationStateEnum.Syncing)
					{
						if (_upRemainedCount.Count > 0 && _upRemainedCount.Average() > 0)
							return;

						if (_downRemainedCount.Count > 0 && _downRemainedCount.Average() > 0)
							return;

						CurrentState.StopSyncing();
						//TrayIcon.ShowBalloonTip(1000, Resources.APP_NAME, Resources.WFServiceRunning, ToolTipIcon.None);
					}
				}

				SetNotifyIconText(TrayIcon, iconText);
			}
			finally
			{
				m_Timer.Start();
			}
		}

		public static void SetNotifyIconText(NotifyIcon ni, string text)
		{
			if (text.Length >= 128) throw new ArgumentOutOfRangeException("text", "Text limited to 127 characters");
			Type t = typeof(NotifyIcon);
			const BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
			var fieldInfo = t.GetField("text", hidden);
			if (fieldInfo != null) fieldInfo.SetValue(ni, text);
			var field = t.GetField("added", hidden);
			if (field != null && (bool)field.GetValue(ni))
				t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
		}

		private void tsmiOpenStream_Click(object sender, EventArgs e)
		{
			GotoTimeline();
		}

		private void TrayMenu_Opening(object sender, CancelEventArgs e)
		{
			string lastLogin = userloginContainer.GetCurLoginedSession();
			LoginedSession loginedSession = null;

			if (lastLogin != null)
				loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", lastLogin));

			bool isUserLogined = (loginedSession != null || (clientProcess != null && !clientProcess.HasExited));

			tsmiOpenStream.Visible = isUserLogined;
		}

		private void fbLoginButton1_Click(object sender, EventArgs e)
		{
			try
			{
				Hide();
				string fbLoginUrl = string.Format("{0}/{1}/FBLogin", m_CallbackUrl, FB_LOGIN_GUID);
				var postData = new FBPostData
								{
									device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
									device_name = Environment.MachineName,
									device = "windows",
									api_key = CLIENT_API_KEY,
									xurl =
										string.Format(
											"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s&is_new_user=%(is_new_user)s",
											fbLoginUrl),
									locale = Thread.CurrentThread.CurrentCulture.ToString(),
									show_tutorial = "false"
								};

				var dialog = new SignUpDialog() 
								{
					Text = Resources.FB_CONNECT_PAGE_TITLE,
					StartPosition = FormStartPosition.CenterParent
								};
				var browser = dialog.Browser;
				var signupOK = false;

				browser.Navigated += (s, ex) =>
										{
											Uri url = browser.Url;
											if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "FBLogin"),
															  RegexOptions.IgnoreCase))
											{
												var parameters = HttpUtility.ParseQueryString(url.Query);

												var isNewUser = parameters["is_new_user"];

												if (isNewUser.Equals("true", StringComparison.CurrentCultureIgnoreCase))
													dialog.ShowTutorial();
												else
												dialog.DialogResult = DialogResult.OK;

												signupOK = true;
											}
										};

				browser.Navigate(m_FBLoginUrl,
								 string.Empty,
								 Encoding.UTF8.GetBytes(postData.ToFastJSON()),
								 "Content-Type: application/json");

				dialog.ShowDialog(this);
				if (signupOK)
				{
					string url = browser.Url.Query;
					NameValueCollection parameters = HttpUtility.ParseQueryString(url);
					string apiRetCode = parameters["api_ret_code"];

					if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
					{
						if (!IsDisposed)
							Show();
						return;
					}

					string sessionToken = parameters["session_token"];
					string userID = parameters["user_id"];

					m_LoginAction = () => LoginAndLaunchClient(sessionToken, userID);

					Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
					if (driver == null)
					{
						AddUserResponse res = StationController.AddUser(userID, sessionToken);

						UserStation station = GetPrimaryStation(res.Stations);
						lblMainStationSetup.Text = string.Format(lblMainStationSetupText,
																 (station == null) ? "None" : station.computer_name);
						lblSecondStationSetup.Text = string.Format(lblSecondStationSetupText,
																   (station == null) ? "None" : station.computer_name);

						//Show welcome msg
						GotoTabPage(res.IsPrimaryStation ? tabMainStationSetup : tabSecondStationSetup);

						if (!IsDisposed)
							Show();
						return;
					}

					m_LoginAction();
					return;
				}
				if (!IsDisposed)
					Show();
			}			
			catch (AuthenticationException)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;
				txtPassword.Focus();
			}
			catch (StationServiceDownException)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (VersionNotSupportedException)
			{
				handleVersionNotSupported();
			}
			catch (Exception)
			{
				if (!IsDisposed)
					Show();
				MessageBox.Show(Resources.UnknownSigninError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void LoginAndLaunchClient(UserLoginSetting loginSetting)
		{
			//Hide();

			//var session = LoginToStation(loginSetting);

			//var wizard = new FirstUseWizardDialog(session.user.user_id, session.session_token);

			//wizard.FormClosed += (s, e) =>
			//{
			//    //LaunchWavefaceClient(loginSetting);
			//    LaunchClient(session.session_token);
			//};

			//wizard.Show();
		}

		private void LoginAndLaunchClient(string sessionToken, string userID)
		{
			////Login user
			//StationController.UserLogin(CLIENT_API_KEY, userID, sessionToken);

			//userloginContainer.SaveCurLoginedSession(sessionToken);

			//Hide();

			//var wizard = new FirstUseWizardDialog(userID, sessionToken);

			//wizard.FormClosed += (s, e) =>
			//{
			//    LaunchClient(sessionToken);
			//};

			//wizard.Show();
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

		#endregion Protected Method

		private void button1_Click(object sender, EventArgs e)
		{
			cmbEmail.DroppedDown = true;
		}

		private void label1_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.DrawBorder(e.Graphics, label1.DisplayRectangle, ColorTranslator.FromHtml("#868686"), ButtonBorderStyle.Solid);
		}

		private SettingDialog m_SettingDialog;
		private void settingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSettingDialog();
		}


		private void OpenSettingDialog()
		{
			DebugInfo.ShowMethod();
			var isLoginPageOpened = this.Visible && this.ShowInTaskbar;

			if (m_SettingDialog != null)
			{
				m_SettingDialog.BringToFront();
				return;
			}

			using (m_SettingDialog = new SettingDialog(appSetting.CurrentSession, this.CloseTimelineProgram))
			{
				EventHandler<AccountEventArgs> removeAccountAction = (senderEx, ex) =>
				{
					var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", appSetting.CurrentSession));
					if (loginedUser != null && loginedUser.user.email == ex.EMail)
					{
						Unlink();
						return;
					}
				};

				m_SettingDialog.Location = this.Location;
				m_SettingDialog.Icon = this.Icon;
				m_SettingDialog.TopMost = true;
				m_SettingDialog.StartPosition = FormStartPosition.CenterScreen;
				m_SettingDialog.ShowInTaskbar = isLoginPageOpened;


				m_SettingDialog.FormClosed += (senderEx, ex) =>
				{
					m_SettingDialog.AccountRemoved -= removeAccountAction;
					m_SettingDialog = null;
				};

				m_SettingDialog.AccountRemoved += removeAccountAction;
				this.Hide();
				m_SettingDialog.ShowDialog();
			}
			if (clientProcess != null && !clientProcess.HasExited)
			{
				var handle = Win32Helper.FindWindow("WindowsClientMessageReceiver", null);

				if (handle == IntPtr.Zero)
					return;

				int ret;
				SendMessageTimeout(handle, 0x401, IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);

				return;
			}

			if (isLoginPageOpened)
				GotoTimeline();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(@"https://devweb.waveface.com/password/forgot");
		//    try
		//    {
		//        Hide();

		//        var browser = new WebBrowser
		//        {
		//            WebBrowserShortcutsEnabled = false,
		//            IsWebBrowserContextMenuEnabled = false,
		//            Dock = DockStyle.Fill
		//        };

		//        var dialog = new Form
		//        {
		//            Width = 750,
		//            Height = 600,
		//            Text = Resources.SIGNUP_PAGE_TITLE,
		//            StartPosition = FormStartPosition.CenterParent,
		//            Icon = Icon
		//        };
		//        dialog.Controls.Add(browser);

		//        browser.Navigate(@"https://devweb.waveface.com/password/forgot");

		//        if (dialog.ShowDialog() == DialogResult.OK)
		//        {
		//        }

		//        if (!IsDisposed)
		//            Show();
		//    }
		//    catch (AuthenticationException)
		//    {
		//        if (!IsDisposed)
		//            Show();

		//        MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);

		//        txtPassword.Text = string.Empty;
		//        txtPassword.Focus();
		//    }
		//    catch (StationServiceDownException)
		//    {
		//        if (!IsDisposed)
		//            Show();
		//        MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//    }
		//    catch (ConnectToCloudException)
		//    {
		//        if (!IsDisposed)
		//            Show();
		//        MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//    }
		//    catch (Exception)
		//    {
		//        if (!IsDisposed)
		//            Show();
		//        MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		//    }
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			var server = (string)StationRegistry.GetValue("cloudBaseURL", null);

			bool isProductionVersion = server != null && server.Contains("api.waveface.com");
			devVersionTag.Visible = !isProductionVersion;
		}

		private void menuItemContactUs_Click(object sender, EventArgs e)
		{
			if (_contactUsDialog == null)
			{
				_contactUsDialog = new ContactUsDialog();
				_contactUsDialog.FormClosed += (sendr, arg) =>
				{
					this._contactUsDialog = null;
				};
				_contactUsDialog.Show();
			}
			else
				_contactUsDialog.Activate();
		}

		private void newUserButton_Click(object sender, EventArgs e)
		{
			Hide();

			var wizard = new NewUserWizardDialog();
			wizard.FormClosed += wizard_FormClosed;
			wizard.Show();
		}

		private void oldUserButton_Click(object sender, EventArgs e)
		{
			Hide();

			var wizard = new OldUserWizardDialog();
			wizard.FormClosed += wizard_FormClosed;
			wizard.Show();
		}

		void wizard_FormClosed(object sender, FormClosedEventArgs e)
		{
			var wizard = (StepByStepWizardDialog)sender;
			var parameters = wizard.Parameters;
			var session_token = (string)wizard.Parameters.Get("session_token");

			if (string.IsNullOrEmpty(session_token))
			{
				Show();
			}
			else
			{
				LaunchClient(session_token);

				appSetting.CurrentSession = session_token;
				appSetting.Save();
			}
				
		}
	}

	#region StationStatusUIController

	public class StationStatusUIController : SimpleUIController
	{
		private readonly object csStationTimerTick = new object();

		public StationStatusUIController(MainForm form)
			: base(form)
		{
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

	#endregion StationStatusUIController

	public class FBPostData
	{
		public string device_id { get; set; }

		public string device_name { get; set; }

		public string device { get; set; }

		public string api_key { get; set; }

		public string xurl { get; set; }

		public string locale { get; set; }

		public string show_tutorial { get; set; }

		public string session_token { get; set; }
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
			StationController.ConnectToInternet();
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

	#endregion PauseServiceUIController

	#region ResumeServiceUIController

	public class ResumeServiceUIController : SimpleUIController
	{
		public ResumeServiceUIController(MainForm form)
			: base(form)
		{
		}

		protected override object Action(object obj)
		{
			StationController.ConnectToInternet();
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

	#endregion ResumeServiceUIController
}
