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
	public partial class MainForm : Form
	{
		public static log4net.ILog logger = log4net.LogManager.GetLogger("MainForm");

		private Messenger messenger;
		private ServiceActionUIController uictrlServiceAction;
		private InitMainUIController uictrlInitMain;
		private bool serviceRunning;
		private PreferenceForm preferenceForm;

		public Icon iconRunning;
		public Icon iconPaused;

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

		public bool ServiceRunning
		{
			// TODO: add lock here
			get { return serviceRunning; }
			set { serviceRunning = value; }
		}

		public MainForm()
		{
			InitializeComponent();
			
			Type type = this.GetType();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);
			this.iconRunning = ((Icon)(resources.GetObject("Icon")));
			this.iconPaused = ((Icon)(resources.GetObject("Icon_gray")));
			this.Icon = this.iconPaused;
			
			this.serviceRunning = false;
			this.messenger = new Messenger(this);
			this.uictrlServiceAction = new ServiceActionUIController(this, messenger);
			this.uictrlInitMain = new InitMainUIController(this, messenger);

			this.checkStationTimer.Enabled = true;
			this.checkStationTimer.Start();
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

				if (!available && serviceRunning)
					StationController.StationOnline();
			}
			catch (AuthenticationException)
			{

				TrayIcon.Icon = iconPaused;
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
			mainform.ServiceRunning = true;
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

			mainform.ServiceRunning = false;
			MainForm.logger.Error("Unable to start mainform", ex);
		}

		protected override void SetFormControls(object obj)
		{
			mainform.MenuServiceActionEnabled = false;
			mainform.MenuPreferenceEnabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			mainform.MenuServiceActionEnabled = true;
			mainform.MenuPreferenceEnabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			messenger.ShowMessage(I18n.L.T("WFServiceStartFail"));
			Application.Exit();
		}

		protected override void UpdateUI(object obj)
		{
			mainform.TrayIconText = I18n.L.T("StartingWFService");
			mainform.TrayIconIcon = mainform.iconPaused;
		}

		protected override void UpdateUIInCallback(object obj)
		{
			mainform.TrayIconText = I18n.L.T("WFServiceRunning");
			mainform.TrayIconIcon = mainform.iconRunning;
		}

		protected override void UpdateUIInError(Exception ex)
		{
			mainform.TrayIconText = I18n.L.T("WFServiceStopped");
		}
	}
	#endregion

	#region ServiceActionUIController
	public class ServiceActionUIController : SimpleUIController
	{
		private MainForm mainform;
		private Messenger messenger;

		public ServiceActionUIController(MainForm form, Messenger messenger)
			: base(form)
		{
			this.mainform = form;
			this.messenger = messenger;
		}

		protected override object Action(object obj)
		{
			if (mainform.ServiceRunning)
				StationController.StationOffline();
			else
				StationController.StationOnline();

			return null;
		}

		protected override void ActionCallback(object obj)
		{
			mainform.ServiceRunning = !mainform.ServiceRunning;
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

			if (mainform.ServiceRunning)
				MainForm.logger.Error("Unable to stop service", ex);
			else
				MainForm.logger.Error("Unable to start service", ex);
		}

		protected override void SetFormControls(object obj)
		{
			mainform.MenuServiceActionEnabled = false;
			mainform.MenuPreferenceEnabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			mainform.MenuServiceActionEnabled = true;
			mainform.MenuPreferenceEnabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			mainform.MenuServiceActionEnabled = true;
			mainform.MenuPreferenceEnabled = true;
		}

		protected override void UpdateUI(object obj)
		{
			if (mainform.ServiceRunning)
			{
				mainform.TrayIconText = I18n.L.T("PausingWFService");
			}
			else
			{
				mainform.TrayIconText = I18n.L.T("ResumingWFService");
			}
		}

		protected override void UpdateUIInCallback(object obj)
		{
			if (mainform.ServiceRunning)
			{
				mainform.MenuServiceActionText = I18n.L.T("PauseWFService");
				mainform.TrayIconText = I18n.L.T("WFServiceRunning");
				mainform.TrayIconIcon = mainform.iconRunning;
			}
			else
			{
				mainform.MenuServiceActionText = I18n.L.T("ResumeWFService");
				mainform.TrayIconText = I18n.L.T("WFServiceStopped");
				mainform.TrayIconIcon = mainform.iconPaused;
			}
		}

		protected override void UpdateUIInError(Exception ex)
		{
			if (mainform.ServiceRunning)
			{
				mainform.MenuServiceActionText = I18n.L.T("PauseWFService");
				mainform.TrayIconText = I18n.L.T("WFServiceRunning");
			}
			else
			{
				mainform.MenuServiceActionText = I18n.L.T("ResumeWFService");
				mainform.TrayIconText = I18n.L.T("WFServiceStopped");
			}
		}
	}
	#endregion
}
