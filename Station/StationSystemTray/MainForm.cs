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

namespace StationSystemTray
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
	public partial class MainForm : Form, StationStateContext
	{
		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private ApplicationSettings settings;
		private bool formCloseEnabled;
		private Messenger messenger;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private PreferenceForm preferenceForm;
		private ReloginForm signInForm;

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

		public MainForm()
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.settings = new ApplicationSettings();
			this.formCloseEnabled = false;

			Type type = this.GetType();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);
			this.iconRunning = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_16.GetHicon());
			this.iconPaused = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_disable_16.GetHicon());
			this.iconWarning = Icon.FromHandle(StationSystemTray.Properties.Resources.station_icon_warn_16.GetHicon());
			this.TrayIcon.Icon = this.iconPaused;
			
			this.messenger = new Messenger(this);

			this.uictrlPauseService = new PauseServiceUIController(this);
			this.uictrlPauseService.UICallback += this.PauseServiceUICallback;
			this.uictrlPauseService.UIError += this.PauseServiceUIError;

			this.uictrlResumeService = new ResumeServiceUIController(this);
			this.uictrlResumeService.UICallback += this.ResumeServiceUICallback;
			this.uictrlResumeService.UIError += this.ResumeServiceUIError;

			NetworkChange.NetworkAvailabilityChanged += checkStationTimer_Tick;
			NetworkChange.NetworkAddressChanged += checkStationTimer_Tick;

			this.CurrentState = CreateState(StationStateEnum.Initial);
		}

		protected override void OnLoad(EventArgs e)
		{
			this.menuPreference.Text = I18n.L.T("WFPreference");
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");
			
			ListDriverResponse res = StationController.ListUser();

			foreach (UserLoginSetting userlogin in settings.Users)
			{
				foreach (Driver driver in res.drivers)
				{
					if (userlogin.Email == driver.email)
					{
						cmbEmail.Items.Add(userlogin.Email);
						if (settings.LastLogin == userlogin.Email)
						{
							cmbEmail.Text = userlogin.Email;
							if (userlogin.RememberPassword)
							{
								txtPassword.Text = SecurityHelper.DecryptPassword(userlogin.Password);
							}
						}
					}
				}
			}

			if (string.IsNullOrEmpty(cmbEmail.Text))
			{
				cmbEmail.Focus();
			}
			else if (string.IsNullOrEmpty(txtPassword.Text))
			{
				txtPassword.Focus();
			}
			else
			{
				btnSignIn.Focus();
			}

			this.checkStationTimer.Enabled = true;
			this.checkStationTimer.Start();

			CurrentState.Onlining();

			base.OnLoad(e);
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

		private void TrayMenu_Opening(object sender, CancelEventArgs e)
		{
			//// force window to have focus
			//// please refer http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
			//uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
			//uint appThread = GetCurrentThreadId();
			////const uint SW_SHOW = 5;
			//if (foreThread != appThread)
			//{
			//    AttachThreadInput(foreThread, appThread, true);
			//    BringWindowToTop(this.Handle);
			//    //ShowWindow(this.Handle, SW_SHOW);
			//    AttachThreadInput(foreThread, appThread, false);
			//}
			//else
			//{
			//    BringWindowToTop(this.Handle);
			//    //ShowWindow(this.Handle, SW_SHOW);
			//}
			//this.Activate();
		}

		private void menuPreference_Click(object sender, EventArgs e)
		{
			if (preferenceForm == null)
			{
				try
				{
					preferenceForm = new PreferenceForm(this);
					preferenceForm.FormClosed += new FormClosedEventHandler(preferenceForm_FormClosed);
					preferenceForm.Show();
				}
				catch (Exception ex)
				{
					logger.Warn("Unable to create preference form", ex);
					messenger.ShowMessage(I18n.L.T("SystemError"));
					preferenceForm = null;
				}
			}
			else
			{
				preferenceForm.Activate();
			}
		}

		void preferenceForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			preferenceForm = null;
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
				menuPreference.Enabled = false;
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
				menuPreference.Enabled = true;
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
				menuPreference.Enabled = true;
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
				menuPreference.Enabled = false;
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
				menuPreference.Enabled = false;
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

				if (preferenceForm != null)
					preferenceForm.Close();

				menuPreference.Enabled = false;
				menuServiceAction.Enabled = false;

				TrayIcon.Icon = this.iconWarning;
				TrayIcon.BalloonTipClicked -= ClickBallonFor401Exception;
				TrayIcon.BalloonTipClicked += ClickBallonFor401Exception;
				TrayIcon.DoubleClick -= menuManageAccounts_Click;
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
				TrayIcon.DoubleClick += menuManageAccounts_Click;
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

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!formCloseEnabled)
			{
				this.Visible = false;
				e.Cancel = true;
			}
		}

		private void menuManageAccounts_Click(object sender, EventArgs e)
		{
			this.Visible = true;
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

			foreach (UserLoginSetting userlogin in settings.Users)
			{
				if (cmbEmail.Text.ToLower() == userlogin.Email.ToLower())
				{
					StationController.StationOnline(cmbEmail.Text.ToLower(), txtPassword.Text);
					LaunchWavefaceClient(cmbEmail.Text.ToLower(), txtPassword.Text);
					Close();
					return;
				}
			}

			AddUser(cmbEmail.Text.ToLower(), txtPassword.Text, chkRememberPassword.Checked);

			return;
		}

		private void AddUser(string email, string password, bool remember)
		{
			Cursor = Cursors.WaitCursor;

			try
			{
				StationController.AddUser(email, password);

				settings.Users.Add(new UserLoginSetting { 
					Email = email, 
					Password = SecurityHelper.EncryptPassword(password), 
					RememberPassword = remember
				});
				settings.LastLogin = email;
				settings.Save();

				Cursor = Cursors.Default;

				this.tabControl.SelectedTab = this.tabMainStationSetup;
				this.AcceptButton = this.btnOK;
			}
			catch (AuthenticationException)
			{
				Cursor = Cursors.Default;

				MessageBox.Show(I18n.L.T("AuthError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				txtPassword.Text = string.Empty;

				return;
			}
			catch (UserAlreadyHasStationException _e)
			{
				Cursor = Cursors.Default;

				RemoveStationForm _form = new RemoveStationForm(_e.ComputerName);
				DialogResult _dr = _form.ShowDialog();

				if (_dr == DialogResult.Yes)
				{
					try
					{
						Cursor = Cursors.WaitCursor;

						StationController.SignoffStation(_e.Id, email, password);
						StationController.AddUser(email, password);

						Close();
					}
					catch (Exception e)
					{
						Cursor = Cursors.Default;

						ShowErrorDialogAndExit(I18n.L.T("SignOffStationError") + " : " + e.ToString());
					}
				}
				else
				{
					ShowErrorDialogAndExit(I18n.L.T("MustRemoveOld"));
				}
			}
			catch (StationAlreadyHasDriverException)
			{
				Cursor = Cursors.Default;

				ShowErrorDialogAndExit(I18n.L.T("StationHasDriverError"));
			}
			catch (StationServiceDownException)
			{
				Cursor = Cursors.Default;
				MessageBox.Show(I18n.L.T("StationDown"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception)
			{
				Cursor = Cursors.Default;
				MessageBox.Show(I18n.L.T("UnknownSigninError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void ShowErrorDialogAndExit(string message)
		{
			MessageBox.Show(message, "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Close();
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
			LaunchWavefaceClient(cmbEmail.Text.ToLower(), txtPassword.Text);
			Close();
		}

		private void LaunchWavefaceClient(string email, string password)
		{
			string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
		   "WavefaceWindowsClient.exe");
			Process.Start(_execPath, email + " " + password);
		}

		private void ExitProgram()
		{
			formCloseEnabled = true;
			Close();
		}

        private void lblSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl.SelectedTab = tabSignUp;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabSignUp)
            {
                webBrowser1.ObjectForScripting = this;
                webBrowser1.Navigate(@"C:\Users\larry\Desktop\Test.html");
            }
        }

        public void SignUpCompleted(string functionName)
        {
            string ret = webBrowser1.Document.InvokeScript(functionName, new object[] { string.Empty, string.Empty }).ToString();
            Boolean isSignUpCompleted = !string.IsNullOrEmpty(ret);
            if (isSignUpCompleted)
            {
                tabControl.SelectedTab = tabSignIn;
                cmbEmail.Text = "test";
                txtPassword.Text = "test";
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Tab))
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

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
