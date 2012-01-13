using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
				StationController.StationOnline();
				this.DialogResult = DialogResult.Yes;
				this.Close();
			}
			catch (AuthenticationException _)
			{
				messenger.ShowMessage(I18n.L.T("LoginForm.LogInError"));
				this.txtEmail.Text = "";
				this.txtPassword.Text = "";
				this.txtEmail.Focus();
			}
			catch (Exception ex)
			{
				logger.Error("StationOnline failed", ex);
				messenger.ShowMessage(I18n.L.T("LoginForm.StationExpired"));
				Application.Exit();
			}
		}

		private void SignInForm_Load(object sender, EventArgs e)
		{
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
