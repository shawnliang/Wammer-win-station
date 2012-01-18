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
		private ServiceActionUIController uictrlServiceAction;
		private InitMainUIController uictrlInitMain;
		private PreferenceForm preferenceForm;

		private object cs = new object();
		public StationState CurrentState { get; private set; }

		public Icon iconRunning;
		public Icon iconPaused;
		public Icon iconWarning;

		public bool MenuServiceActionEnabled
		{
			get { return menuServiceAction.Enabled; }
			set { menuServiceAction.Enabled = value; }
		}

		public bool MenuPreferenceEnabled
		{
			get { return menuPreference.Enabled; }
			set { menuPreference.Enabled = value; }
		}

		public string MenuServiceActionText
		{
			get { return menuServiceAction.Text; }
			set { menuServiceAction.Text = value; }
		}

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

		public bool TrayIconVisible
		{
			get { return TrayIcon.Visible; }
			set { TrayIcon.Visible = value; }
		}

		public Icon TrayIconIcon
		{
			get { return TrayIcon.Icon; }
			set { TrayIcon.Icon = value; }
		}

		public bool TrayMenuVisible
		{
			get { return TrayMenu.Visible; }
			set { TrayMenu.Visible = value; }
		}

		public MainForm()
		{
			InitializeComponent();
			
			Type type = this.GetType();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);
			this.iconRunning = StationSystemTray.Properties.Resources.station_run;
			this.iconPaused = StationSystemTray.Properties.Resources.station_stop;
			this.iconWarning = StationSystemTray.Properties.Resources.station_warn;
			this.Icon = this.iconPaused;
			
			this.messenger = new Messenger(this);
			this.uictrlServiceAction = new ServiceActionUIController(this, messenger);
			this.uictrlInitMain = new InitMainUIController(this, messenger);

			this.checkStationTimer.Enabled = true;
			this.checkStationTimer.Start();

			this.GoToState(StationStateEnum.Initial);
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Visible = false;

			this.menuPreference.Text = I18n.L.T("WFPreference");
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");

			this.uictrlInitMain.PerformAction();

			base.OnLoad(e);
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

		private StationState Create(StationStateEnum state)
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
				CurrentState = Create(state);
				CurrentState.OnEntering(this, new EventArgs());
			}
		}

		private void menuServiceAction_Click(object sender, EventArgs e)
		{
			uictrlServiceAction.PerformAction();
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
				preferenceForm = new PreferenceForm();
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

		private void TrayIcon_DoubleClick(object sender, EventArgs e)
		{
			menuPreference_Click(sender, e);
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
				TrayIcon.Icon = this.iconWarning;
				TrayIcon.BalloonTipClicked -= ClickBallonFor401Exception;
				TrayIcon.BalloonTipClicked += ClickBallonFor401Exception;
				TrayIconText = I18n.L.T("Station401Exception");
			}
			catch (Exception ex)
			{
				logger.Warn("Unexpected exception in station status cheking", ex);
			}
		}

		void ClickBallonFor401Exception(object sender, EventArgs e)
		{
			messenger.ShowLoginDialog(false);
		}

		void BecomeInitialState(object sender, EventArgs evt)
		{
			if (IsDisposed)
				return;

			if (InvokeRequired)
				this.Invoke(new MethodInvoker(this.BecomeInitialStateUI));
			else
				BecomeInitialStateUI();
		}

		void BecomeInitialStateUI()
		{
			this.Icon = iconPaused;
			this.TrayIconText = I18n.L.T("StartingWFService");

			this.MenuServiceActionEnabled = false;
			this.MenuPreferenceEnabled = false;
		}

		void BecomeRunningState(object sender, EventArgs evt)
		{
			if (IsDisposed)
				return;

			if (InvokeRequired)
				this.Invoke(new MethodInvoker(this.BecomeRunningStateUI));
			else
				BecomeRunningStateUI();
		}

		void BecomeRunningStateUI()
		{
			this.Icon = iconRunning;
			this.TrayIconText = I18n.L.T("WFServiceRunning");
			this.MenuServiceActionText = I18n.L.T("PauseWFService");

			this.MenuServiceActionEnabled = true;
			this.MenuPreferenceEnabled = true;
		}

		void BecomeStoppedState(object sender, EventArgs evt)
		{
			if (IsDisposed)
				return;

			if (InvokeRequired)
				this.Invoke(new MethodInvoker(this.BecomeStoppedStateUI));
			else
				BecomeStoppedStateUI();
		}

		void BecomeStoppedStateUI()
		{
			this.Icon = iconPaused;
			this.TrayIconText = I18n.L.T("WFServiceStopped");
			this.MenuServiceActionText = I18n.L.T("ResumeWFService");

			this.MenuServiceActionEnabled = true;
			this.MenuPreferenceEnabled = true;
		}

		void BecomeStartingState(object sender, EventArgs evt)
		{
			if (IsDisposed)
				return;

			if (InvokeRequired)
				Invoke(new MethodInvoker(BecomeStartingStateUI));
			else
				BecomeStartingStateUI();
		}

		void BecomeStartingStateUI()
		{
			this.MenuServiceActionEnabled = false;
			this.MenuPreferenceEnabled = false;
			this.TrayIconText = I18n.L.T("StartingWFService");
		}

		void BecomeStoppingState(object sender, EventArgs evt)
		{
			if (IsDisposed)
				return;

			if (InvokeRequired)
				Invoke(new MethodInvoker(BecomeStoppingStateUI));
			else
				BecomeStoppingStateUI();
		}

		void BecomeStoppingStateUI()
		{
			this.MenuServiceActionEnabled = false;
			this.MenuPreferenceEnabled = false;
			this.TrayIconText = I18n.L.T("PausingWFService");
		}
	}

	#region InitMainUIController
	public class InitMainUIController : SimpleUIController
	{
		private MainForm mainform;
		private Messenger messenger;

		public InitMainUIController(MainForm form, Messenger messenger)
			: base(form)
		{
			this.mainform = form;
			this.messenger = messenger;
		}

		protected override object Action(object obj)
		{
			StationController.StationOnline();
			return null;
		}

		protected override void ActionCallback(object obj)
		{
			mainform.CurrentState.Onlined();
		}

		protected override void ActionError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				if (messenger.ShowLoginDialog(this, _parameter))
				{
					Thread.CurrentThread.Abort();
				}
			}
			else if (ex is UserAlreadyHasStationException)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.StationExpired"));
				string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
						   "StationUI.exe");
				Process.Start(_execPath);
				Application.Exit();
			}

			mainform.CurrentState.Error();
			MainForm.logger.Error("Unable to start mainform", ex);
		}

		protected override void SetFormControls(object obj)
		{
			mainform.CurrentState.Onlining();
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			if (ex is ConnectToCloudException)
			{
				messenger.ShowMessage(I18n.L.T("ConnectCloudError"));
			}
			else
			{
				messenger.ShowMessage(I18n.L.T("WFServiceStartFail"));
			}

			Application.Exit();
		}

		protected override void UpdateUI(object obj)
		{
		}

		protected override void UpdateUIInCallback(object obj)
		{
		}

		protected override void UpdateUIInError(Exception ex)
		{
		}
	}
	#endregion

	#region ServiceActionUIController
	enum ServiceAction
	{
		None,
		TakeOnline,
		TakeOffline
	}

	public class ServiceActionUIController : SimpleUIController
	{
		private MainForm mainform;
		private Messenger messenger;
		private ServiceAction action;

		public ServiceActionUIController(MainForm form, Messenger messenger)
			: base(form)
		{
			this.mainform = form;
			this.messenger = messenger;
			this.action = ServiceAction.None;
		}

		protected override object Action(object obj)
		{
			if (action == ServiceAction.TakeOffline)
			{
				StationController.StationOffline();
			}
			else
			{
				StationController.StationOnline();
			}

			return null;
		}

		protected override void ActionCallback(object obj)
		{
			if (action == ServiceAction.TakeOffline)
				mainform.CurrentState.Offlined();
			else if (action == ServiceAction.TakeOnline)
				mainform.CurrentState.Onlined();
		}

		protected override void ActionError(Exception ex)
		{
			mainform.CurrentState.Error();

			if (ex is AuthenticationException)
			{
				if (messenger.ShowLoginDialog(this, _parameter))
				{
					Thread.CurrentThread.Abort();
				}
			}

			if (action == ServiceAction.TakeOffline)
				MainForm.logger.Error("Unable to stop service", ex);
			else
				MainForm.logger.Error("Unable to start service", ex);
		}

		protected override void SetFormControls(object obj)
		{
			if (mainform.CurrentState.Value == StationStateEnum.Running)
			{
				action = ServiceAction.TakeOffline;
				mainform.CurrentState.Offlining();
			}
			else
			{
				action = ServiceAction.TakeOnline;
				mainform.CurrentState.Onlining();
			}
		}

		protected override void SetFormControlsInCallback(object obj)
		{
		}

		protected override void SetFormControlsInError(Exception ex)
		{
		}

		protected override void UpdateUI(object obj)
		{
		}

		protected override void UpdateUIInCallback(object obj)
		{
		}

		protected override void UpdateUIInError(Exception ex)
		{
		}
	}
	#endregion
}
