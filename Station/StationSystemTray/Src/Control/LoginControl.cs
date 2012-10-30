using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wammer.Station.Management;
using StationSystemTray.Properties;
using Waveface.Common;

namespace StationSystemTray
{
	public partial class LoginControl : StepPageControl
	{
		private IStreamLoginable login;
		private UserSession session;

		public LoginControl(IStreamLoginable login)
			: base()
		{
			InitializeComponent();

			this.login = login;
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			base.OnEnteringStep(parameters);
		}

		private void fbLoginButton1_Click(object sender, EventArgs e)
		{
			Func<UserSession> func = () => { return login.LoginWithFacebook(); };
			loginAndHandleError(func);
		}

		private void loginButton1_Click(object sender, EventArgs e)
		{
			var email = tbxEMail.Text;
			var pwd = txtPassword.Text;

			Func<UserSession> func = () => { return login.Login(email, pwd); };
			loginAndHandleError(func);
		}

		private void loginAndHandleError(Func<UserSession> func)
		{
			try
			{
				session = func();

				WizardControl.NextPage();
			}
			catch (OperationCanceledException)
			{
				// user cancel fb login
			}
			catch (AuthenticationException)
			{
				MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
			catch (Exception)
			{
				MessageBox.Show(Resources.UnknownSigninError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			parameters.Set("user_id", session.user_id);
			parameters.Set("session_token", session.session_token);
		}

		private void LoginControl_Load(object sender, EventArgs e)
		{

		}

		private void LoginControl_SizeChanged(object sender, EventArgs e)
		{
			pictureBox2.Left = label1.Left + label1.Width / 2 - pictureBox2.Width / 2;
		}

		public override bool RunOnce
		{
			get
			{
				return true;
			}
		}

		public override bool HasPrevAndBack
		{
			get
			{
				return false;
			}
		}

		private void forgotPwdLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				login.ForgotPassword();
			}
			catch
			{
			}
		}
	}
}
