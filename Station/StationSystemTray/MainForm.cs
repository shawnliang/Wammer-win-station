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

using Wammer.Station.Management;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using System.Security.Permissions;
using Waveface.Localization;

namespace StationSystemTray
{
	public partial class MainForm : Form, StationStateContext
	{
		private bool m_IsSignUpRunning { get; set; }
		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private UserLoginSettingContainer userloginContainer;
		private bool formCloseEnabled;
		public Process clientProcess;

		private Messenger messenger;
		private WavefaceClientController uictrlWavefaceClient;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private ReloginForm signInForm;
		private bool initMinimized;
		private object cs = new object();
		private object csStationTimerTick = new object();
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
				TrayIcon.ShowBalloonTip(3);
			}
		}

		public MainForm(bool initMinimized)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.userloginContainer = new UserLoginSettingContainer(new ApplicationSettings());
			this.formCloseEnabled = false;
			this.clientProcess = null;

			Type type = this.GetType();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);
			this.iconRunning = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_16.GetHicon());
			this.iconPaused = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_disable_16.GetHicon());
			this.iconWarning = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_warn_16.GetHicon());
			this.TrayIcon.Icon = this.iconPaused;

			this.messenger = new Messenger(this);

			this.uictrlWavefaceClient = new WavefaceClientController(this);
			this.uictrlWavefaceClient.UICallback += this.WavefaceClientUICallback;
			this.uictrlWavefaceClient.UIError += this.WavefaceClientUIError;

			this.uictrlPauseService = new PauseServiceUIController(this);
			this.uictrlPauseService.UICallback += this.PauseServiceUICallback;
			this.uictrlPauseService.UIError += this.PauseServiceUIError;

			this.uictrlResumeService = new ResumeServiceUIController(this);
			this.uictrlResumeService.UICallback += this.ResumeServiceUICallback;
			this.uictrlResumeService.UIError += this.ResumeServiceUIError;

			NetworkChange.NetworkAvailabilityChanged += checkStationTimer_Tick;
			NetworkChange.NetworkAddressChanged += checkStationTimer_Tick;

			this.CurrentState = CreateState(StationStateEnum.Initial);

			this.initMinimized = initMinimized;
		}

		protected override void OnLoad(EventArgs e)
		{
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");
			this.menuGotoTimeline.Text = "Go to Timeline";

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
				GotoTimeline(userloginContainer.GetLastUserLogin());
		}

		private void RefreshUserList()
		{
			cmbEmail.Items.Clear();
            menuGotoTimeline.DropDownItems.Remove(menuNewUser);
			menuGotoTimeline.DropDownItems.Clear();

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
					menuGotoTimeline.DropDownItems.Add(menu);
					userlogins.Add(userlogin);
				}
			}
            menuGotoTimeline.DropDownItems.Add(menuNewUser);

			if (userlogins.Count > 0)
			{
				string lastlogin = userloginContainer.GetLastUserLogin().Email;
				userloginContainer.ResetUserLoginSetting(userlogins, lastlogin);
			}
		}

		private void GotoTimeline(UserLoginSetting userlogin)
		{
			if (userlogin != null && userlogin.RememberPassword)
			{
				LaunchWavefaceClient(userlogin);
				Close();
			}

			GotoTabPage(tabSignIn, userlogin);
		}

		private void WavefaceClientUICallback(object sender, SimpleEventArgs evt)
		{
			int exitCode = (int)evt.param;

			if (exitCode == -2)  // client logout
			{
				GotoTabPage(tabSignIn, userloginContainer.GetLastUserLogin());
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

			if (ex is AuthenticationException)
			{
				CurrentState.SessionExpired();
			}
			else if (ex is UserAlreadyHasStationException)
			{
				messenger.ShowMessage(I18n.L.T("StationExpired"));
				ReregisterStation();
			}
			else if (ex is ConnectToCloudException)
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
			Exception ex = (Exception)evt.param;

			if (ex is AuthenticationException)
			{
				CurrentState.SessionExpired();
			}
			else if (ex is UserAlreadyHasStationException)
			{
				messenger.ShowMessage(I18n.L.T("StationExpired"));
				ReregisterStation();
			}
			else
			{
				CurrentState.Error();
			}
		}

		private void menuQuit_Click(object sender, EventArgs e)
		{
			try
			{
				uictrlWavefaceClient.Terminate();
				StationController.StationOffline();
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
				case StationStateEnum.SessionNotExist:
					{
						StationStateSessionNotExist st = new StationStateSessionNotExist(this, this.CurrentState);
						st.Entering += this.BecomeSessionNotExistState;
						st.Leaving += this.LeaveSessionNotExistState;
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
			if (m_IsSignUpRunning)
				return;

			if (clientProcess != null && !clientProcess.HasExited)
			{
				IntPtr handle = Win32Helper.FindWindow(null, "Waveface");
				Win32Helper.ShowWindow(handle, 1);
				Win32Helper.SetForegroundWindow(handle);
			}
			else
			{
				if (WindowState == FormWindowState.Minimized)
					WindowState = FormWindowState.Normal;
				Show();
				Activate();
			}
		}


		private void checkStationTimer_Tick(object sender, EventArgs e)
		{
			const int LONG_INTERVAL = 60000;
			const int SHORT_INTERVAL = 3000;

			lock (csStationTimerTick)
			{
				try
				{
					StationController.PingForServiceAlive();
				}
				catch (ConnectToCloudException)
				{
					CurrentState.Offlined();
					checkStationTimer.Interval = SHORT_INTERVAL;
					return;
				}

				checkStationTimer.Interval = LONG_INTERVAL;

				if (!StationController.IsConnectToInternet())
				{
					CurrentState.Offlined();
					return;
				}

				try
				{
					StationController.PingForAvailability();

					if (CurrentState.Value != StationStateEnum.Running)
						CurrentState.Onlining();
				}
				catch (AuthenticationException)
				{
					CurrentState.SessionExpired();
				}
				catch (ConnectToCloudException)
				{
					logger.Debug("Unable to detect function server");
				}
				catch (Exception ex)
				{
					logger.Warn("Unexpected exception in station status cheking", ex);
				}
			}
		}

		void ClickBallonFor401Exception(object sender, EventArgs e)
		{
			ShowLoginDialog();
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

		void BecomeSessionNotExistState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(BecomeSessionNotExistState), this, new EventArgs());
				}
			}
			else
			{
				menuRelogin.Visible = true;
				menuRelogin.Text = I18n.L.T("ReLoginMenuItem");


				menuServiceAction.Enabled = false;

				TrayIcon.Icon = this.iconWarning;
				TrayIcon.BalloonTipClicked -= ClickBallonFor401Exception;
				TrayIcon.BalloonTipClicked += ClickBallonFor401Exception;
				TrayIcon.DoubleClick -= menuPreference_Click;
				TrayIcon.DoubleClick += menuRelogin_Click;
				TrayIconText = I18n.L.T("Station401Exception");
			}
		}

		void LeaveSessionNotExistState(object sender, EventArgs evt)
		{
			if (InvokeRequired)
			{
				if (!IsDisposed)
				{
					Invoke(new EventHandler(LeaveSessionNotExistState), this, new EventArgs());
				}
			}
			else
			{
				if (signInForm != null)
					signInForm.Close();

				menuRelogin.Visible = false;
				TrayIcon.BalloonTipClicked -= ClickBallonFor401Exception;
				TrayIcon.DoubleClick -= menuRelogin_Click;
				TrayIcon.DoubleClick += menuPreference_Click;
			}
		}

		private void menuRelogin_Click(object sender, EventArgs e)
		{
			ShowLoginDialog();
		}

		void signInForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			signInForm = null;
		}

		private void ShowLoginDialog()
		{
			if (signInForm != null)
			{
				signInForm.Activate();
				return;
			}

			signInForm = new ReloginForm(this);
			signInForm.FormClosed += new FormClosedEventHandler(signInForm_FormClosed);
			signInForm.Show();
		}

		public void ReregisterStation()
		{
			string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
					   "StationUI.exe");
			Process.Start(_execPath);
			Application.Exit();
		}

		[DllImport("user32.dll")]
		private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll")]
		private static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		private void GotoTabPage(TabPage tabpage, UserLoginSetting userlogin)
		{
			tabControl.SelectedTab = tabpage;

			if (tabpage == tabSignIn)
			{
				RefreshUserList();
				if (userlogin == null)
				{
					cmbEmail.SelectedItem = string.Empty;
					txtPassword.Text = string.Empty;
					chkRememberPassword.Checked = false;
				}
				else
				{
					cmbEmail.SelectedItem = userlogin.Email;
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

				if (cmbEmail.SelectedItem == null)
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
				btnOK.Focus();
				this.AcceptButton = btnOK;
			}


			if (this.WindowState == FormWindowState.Minimized)
				this.WindowState = FormWindowState.Normal;

			// force window to have focus
			// please refer http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
			uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
			uint appThread = GetCurrentThreadId();
			if (foreThread != appThread)
			{
				AttachThreadInput(foreThread, appThread, true);
				BringWindowToTop(this.Handle);
				Show();
				AttachThreadInput(foreThread, appThread, false);
			}
			else
			{
				BringWindowToTop(this.Handle);
				Show();
			}
			Activate();
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
            if (userlogin.Email == userloginContainer.GetLastUserLogin().Email)
            {
                if (clientProcess != null && !clientProcess.HasExited)
                {
                    IntPtr handle = Win32Helper.FindWindow(null, "Waveface");
                    Win32Helper.ShowWindow(handle, 1);
                    Win32Helper.SetForegroundWindow(handle);
                    return;
                }
            }
            uictrlWavefaceClient.Terminate();
            GotoTimeline(userlogin);
        }

		private void btnSignIn_Click(object sender, EventArgs e)
		{
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
					StationController.StationOnline(userlogin.Email, txtPassword.Text);

					userlogin.Password = SecurityHelper.EncryptPassword(txtPassword.Text);
					userlogin.RememberPassword = chkRememberPassword.Checked;

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
			else
			{
				userloginContainer.UpsertUserLoginSetting(userlogin);
				RefreshUserList();
			}

			uictrlWavefaceClient.PerformAction(userlogin);
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
			m_IsSignUpRunning = true;
			var dialog = new SignUpDialog()
			{
				Text = this.Text,
				Icon = this.Icon,
				StartPosition = FormStartPosition.CenterParent
			};

			this.Hide();
			dialog.ShowDialog();
			if (dialog.DialogResult == System.Windows.Forms.DialogResult.OK)
			{
				cmbEmail.Text = dialog.EMail;
				txtPassword.Text = dialog.Password;
			}
			tabControl.SelectedTab = tabSignIn;
			this.Show();
			m_IsSignUpRunning = false;
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

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uictrlWavefaceClient.Terminate();
            cmbEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            GotoTabPage(tabSignIn, null);
        }

        private void menuSignIn_Click(object sender, EventArgs e)
        {
            uictrlWavefaceClient.Terminate();
            GotoTabPage(tabSignIn, null);
        }

        private void TrayMenu_VisibleChanged(object sender, EventArgs e)
        {
           menuSignIn.Text = (clientProcess != null && !clientProcess.HasExited)? "SignOut...": "SignIn...";
        }


	}

	#region WavefaceClientController
	public class WavefaceClientController : SimpleUIController
	{
		private MainForm mainform;
		private object cs;

		public WavefaceClientController(MainForm form)
			: base(form)
		{
			mainform = form;
			cs = new object();
		}

		public void Terminate()
		{
			if (mainform.clientProcess != null)
			{
				mainform.clientProcess.Kill();
			}
		}

		protected override object Action(object obj)
		{
			lock (cs)
			{
				UserLoginSetting userlogin = (UserLoginSetting)obj;
				string execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			   "WavefaceWindowsClient.exe");
				mainform.clientProcess = Process.Start(execPath, userlogin.Email + " " + SecurityHelper.DecryptPassword(userlogin.Password));

				if (mainform.clientProcess != null)
					mainform.clientProcess.WaitForExit();

				int exitCode = mainform.clientProcess.ExitCode;
				mainform.clientProcess = null;

				return exitCode;
			}
		}

		protected override void ActionCallback(object obj)
		{
		}

		protected override void ActionError(Exception ex)
		{
			mainform.clientProcess = null;
		}
	}
	#endregion

	#region PauseServiceUIController
	public class PauseServiceUIController : SimpleUIController
	{
		public PauseServiceUIController(MainForm form)
			: base(form)
		{
		}

		protected override object Action(object obj)
		{
			StationController.StationOffline();
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
			//StationController.StationOnline();
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
