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

		public SignInForm()
		{
			InitializeComponent();

			this.messenger = new Messenger(this);
		}

		private void btnSignIn_Click(object sender, EventArgs e)
		{
			if (this.txtEmail.Text == "" || this.txtPassword.Text == "")
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.FillAllFields"));
				return;
			}

			try
			{
				StationController.StationOnline(this.txtEmail.Text, this.txtPassword.Text);
				this.DialogResult = DialogResult.Yes;
				this.Close();
			}
			catch (AuthenticationException ex)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.LogInError"));
				this.txtPassword.Text = "";
				this.txtPassword.Focus();
			}
			catch (StationAlreadyHasDriverException ex)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.StationExpired"));
				string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
										   "StationUI.exe");
				Process.Start(_execPath);
				Application.Exit();
			}
			catch (Exception ex)
			{
				logger.Error("StationOnline failed", ex);
				messenger.ShowMessage(I18n.L.T("LoginForm.LogInError"));
				this.txtPassword.Text = "";
				this.txtPassword.Focus();
			}
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
	}
}
