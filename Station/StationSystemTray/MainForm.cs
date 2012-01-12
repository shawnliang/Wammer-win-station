using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Wammer.Station.Management;
using Wammer.Cloud;

namespace StationSystemTray
{
	public partial class MainForm : Form
	{
		private ServiceActionUIController uictrlServiceAction;
		private InitMainUIController uictrlInitMain;
		private bool serviceRunning;
		public static readonly string MSGBOX_TITLE = "Waveface";

		public string MenuServiceActionText
		{
			get { return menuServiceAction.Text; }
			set { menuServiceAction.Text = value; }
		}

		public string TrayIconText
		{
			get { return TrayIcon.Text; }
			set { TrayIcon.Text = value; }
		}

		public bool TrayIconVisible
		{
			get { return TrayIcon.Visible; }
			set { TrayIcon.Visible = value; }
		}

		public bool TrayMenuEnabled
		{
			get { return this.TrayMenu.Enabled; }
			set { this.TrayMenu.Enabled = value; }
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

			this.serviceRunning = false;
			this.uictrlServiceAction = new ServiceActionUIController(this);
			this.uictrlInitMain = new InitMainUIController(this);
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Visible = false;
			this.ShowInTaskbar = false;
			this.menuPreference.Text = I18n.L.T("WFPreference");
			this.menuServiceAction.Text = I18n.L.T("PauseWFService");
			this.menuQuit.Text = I18n.L.T("QuitWFService");

			this.uictrlInitMain.PerformAction();

			base.OnLoad(e);
		}

		private void menuQuit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void menuServiceAction_Click(object sender, EventArgs e)
		{
			uictrlServiceAction.PerformAction();
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
			this.TrayMenu.Enabled = false;
			PreferenceForm preform = new PreferenceForm();
			preform.ShowDialog();
			this.TrayMenu.Enabled = true;
		}
	}

	#region InitMainUIController
	public class InitMainUIController : SimpleUIController
	{
		private MainForm mainform;

		public InitMainUIController(MainForm form)
			: base(form)
		{
			this.mainform = form;
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
			mainform.ServiceRunning = false;
		}

		protected override void SetFormControls(object obj)
		{
			mainform.TrayMenuEnabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			mainform.TrayMenuEnabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			mainform.TrayIconVisible = false;
			mainform.TrayMenuVisible = false;
			ShowMessage(I18n.L.T("WFServiceStartFail"), MainForm.MSGBOX_TITLE);
			Application.Exit();
		}

		protected override void UpdateUI(object obj)
		{
			mainform.TrayIconText = I18n.L.T("StartingWFService");
			// TODO: update Waveface Station icon
		}

		protected override void UpdateUIInCallback(object obj)
		{
			mainform.TrayIconText = I18n.L.T("WFServiceRunning");
			// TODO: update Waveface Station icon
		}

		protected override void UpdateUIInError(Exception ex)
		{
			mainform.TrayIconText = I18n.L.T("WFServiceStopped");
			// TODO: update Waveface Station icon
		}
	}
	#endregion

	#region ServiceActionUIController
	public class ServiceActionUIController : SimpleUIController
	{
		private MainForm mainform;

		public ServiceActionUIController(MainForm form)
			: base(form)
		{
			this.mainform = form;
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
		}

		protected override void SetFormControls(object obj)
		{
			mainform.TrayMenuEnabled = false;
		}

		protected override void SetFormControlsInCallback(object obj)
		{
			mainform.TrayMenuEnabled = true;
		}

		protected override void SetFormControlsInError(Exception ex)
		{
			mainform.TrayMenuEnabled = true;
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
			}
			else
			{
				mainform.MenuServiceActionText = I18n.L.T("ResumeWFService");
				mainform.TrayIconText = I18n.L.T("WFServiceStopped");
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
