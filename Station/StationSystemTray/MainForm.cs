using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using Wammer.Station.Management;
using Wammer.Cloud;

namespace StationSystemTray
{
	public partial class MainForm : Form, StationStateContext
	{
		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private Messenger messenger;
		private PauseServiceUIController uictrlPauseService;
		private ResumeServiceUIController uictrlResumeService;
		private PreferenceForm preferenceForm;
		private SignInForm signInForm;

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
				TrayIcon.ShowBalloonTip(3);
			}
		}

		public MainForm()
		{
			InitializeComponent();
			
			Type type = this.GetType();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);
			this.iconRunning = StationSystemTray.Properties.Resources.station_run;
			this.iconPaused = StationSystemTray.Properties.Resources.station_stop;
			this.iconWarning = StationSystemTray.Properties.Resources.station_warn;
			this.TrayIcon.Icon = this.iconPaused;
			
			this.messenger = new Messenger(this);

			this.uictrlPauseService = new PauseServiceUIController(this);
			this.uictrlPauseService.UICallback += this.PauseServiceUICallback;
			this.uictrlPauseService.UIError += this.PauseServiceUIError;

			this.uictrlResumeService = new ResumeServiceUIController(this);
			this.uictrlResumeService.UICallback += this.ResumeServiceUICallback;
			this.uictrlResumeService.UIError += this.ResumeServiceUIError;

			this.checkStationTimer.Enabled = true;
			this.checkStationTimer.Start();

			this.CurrentState = CreateState(StationStateEnum.Initial);
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Visible = false;

			this.menuPreference.Text = I18n.L.T("WFPreference");
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");

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
				HandleAlreadyHasStation();
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
				HandleAlreadyHasStation();
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
				Application.Exit();
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
				preferenceForm = new PreferenceForm(this);
				preferenceForm.FormClosed += new FormClosedEventHandler(preferenceForm_FormClosed);
				preferenceForm.Show();
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
			try
			{
				bool available = StationController.PingForAvailability();

				if (!available && CurrentState.Value != StationStateEnum.Running)
					StationController.StationOnline();
			}
			catch (AuthenticationException)
			{
				CurrentState.SessionExpired();
			}
			catch (Exception ex)
			{
				logger.Warn("Unexpected exception in station status cheking", ex);
			}
		}

		void ClickBallonFor401Exception(object sender, EventArgs e)
		{
			ShowLoginDialog();
		}

		void BecomeInitialState(object sender, EventArgs evt)
		{
			TrayIcon.Icon = iconPaused;
			TrayIconText = I18n.L.T("StartingWFService");

			menuServiceAction.Enabled = false;
			menuPreference.Enabled = false;
		}

		void BecomeRunningState(object sender, EventArgs evt)
		{
			TrayIcon.Icon = iconRunning;
			TrayIconText = I18n.L.T("WFServiceRunning");
			menuServiceAction.Text = I18n.L.T("PauseWFService");

			menuServiceAction.Enabled = true;
			menuPreference.Enabled = true;
		}

		void BecomeStoppedState(object sender, EventArgs evt)
		{
			TrayIcon.Icon = iconPaused;
			TrayIconText = I18n.L.T("WFServiceStopped");
			menuServiceAction.Text = I18n.L.T("ResumeWFService");

			menuServiceAction.Enabled = true;
			menuPreference.Enabled = true;
		}

		void BecomeStartingState(object sender, EventArgs evt)
		{
			menuServiceAction.Enabled = false;
			menuPreference.Enabled = false;
			TrayIconText = I18n.L.T("StartingWFService");

			this.uictrlResumeService.PerformAction();
		}

		void BecomeStoppingState(object sender, EventArgs evt)
		{
			menuServiceAction.Enabled = false;
			menuPreference.Enabled = false;
			TrayIconText = I18n.L.T("PausingWFService");

			this.uictrlPauseService.PerformAction();
		}

		void BecomeSessionNotExistState(object sender, EventArgs evt)
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
			TrayIcon.DoubleClick -= menuPreference_Click;
			TrayIcon.DoubleClick += menuRelogin_Click;
			TrayIconText = I18n.L.T("Station401Exception");
		}

		void LeaveSessionNotExistState(object sender, EventArgs evt)
		{
			if (signInForm != null)
				signInForm.Close();

			menuRelogin.Visible = false;
			TrayIcon.BalloonTipClicked -= ClickBallonFor401Exception;
			TrayIcon.DoubleClick -= menuRelogin_Click;
			TrayIcon.DoubleClick += menuPreference_Click;
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

			signInForm = new SignInForm(this);
			signInForm.FormClosed += new FormClosedEventHandler(signInForm_FormClosed);
			signInForm.Show();
		}

		private void HandleAlreadyHasStation()
		{
			messenger.ShowMessage(I18n.L.T("LoginForm.StationExpired"));
			string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
					   "StationUI.exe");
			Process.Start(_execPath);
			Application.Exit();
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
			StationController.StationOnline();
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
