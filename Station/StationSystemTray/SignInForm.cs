using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using Wammer.Station.Management;

namespace StationSystemTray
{
	public partial class SignInForm : Form
	{
		public static log4net.ILog logger = log4net.LogManager.GetLogger("SignInForm");

		private Messenger messenger;
		private MainForm mainform;

		public SignInForm(MainForm mainform)
		{
			InitializeComponent();

			this.messenger = new Messenger(this);
			this.mainform = mainform;
		}

		private void btnSignIn_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			if (this.txtEmail.Text == "" || this.txtPassword.Text == "")
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.FillAllFields"));
				return;
			}

			try
			{
				StationController.StationOnline(this.txtEmail.Text, this.txtPassword.Text);
				mainform.CurrentState.Onlined();
			}
			catch (AuthenticationException)
			{
				messenger.ShowMessage(I18n.L.T("AuthError"));
				this.txtPassword.Text = "";
				this.txtPassword.Focus();
			}
			catch (UserDoesNotExistException)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.UserNotExisted"));
				mainform.ReregisterStation();
			}
			catch (UserAlreadyHasStationException)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.StationExpired"));
				mainform.ReregisterStation();
			}
			catch (InvalidDriverException)
			{
				messenger.ShowMessage(I18n.L.T("InvalidDriverError"));
				this.txtEmail.Text = "";
				this.txtPassword.Text = "";
				this.txtEmail.Focus();
			}
			catch (Exception ex)
			{
				logger.Error("StationOnline failed", ex);
				messenger.ShowMessage(I18n.L.T("LoginForm.LogInError"));
				this.txtPassword.Text = "";
				this.txtPassword.Focus();
			}

			Cursor = Cursors.Default;
		}

		private void SignInForm_Load(object sender, EventArgs e)
		{
			this.lblSignInMsg.Text = I18n.L.T("Station401Exception");
			this.txtEmail.Focus();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter && !this.btnSignIn.Focused)
			{
				btnSignIn_Click(null, null);
			}
			return base.ProcessDialogKey(keyData);
		}

		private void SignInForm_Activated(object sender, EventArgs e)
		{
			// force window to have focus
			// please refer http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
			uint foreThread = Win32Helper.GetWindowThreadProcessId(Win32Helper.GetForegroundWindow(), IntPtr.Zero);
			uint appThread = Win32Helper.GetCurrentThreadId();
			//const uint SW_SHOW = 5;
			if (foreThread != appThread)
			{
				Win32Helper.AttachThreadInput(foreThread, appThread, true);
				Win32Helper.BringWindowToTop(this.Handle);
				//ShowWindow(this.Handle, SW_SHOW);
				Win32Helper.AttachThreadInput(foreThread, appThread, false);
			}
			else
			{
				Win32Helper.BringWindowToTop(this.Handle);
				//ShowWindow(this.Handle, SW_SHOW);
			}
		}
	}
}
