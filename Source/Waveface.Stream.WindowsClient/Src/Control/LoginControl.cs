using System;
using System.Windows.Forms;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
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
			Cursor.Current = Cursors.WaitCursor;
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
				MessageBox.Show(Resources.AUTH_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (StationServiceDownException)
			{
				MessageBox.Show(Resources.STATION_SERVICE_DOWN, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (ConnectToCloudException)
			{
				MessageBox.Show(Resources.CONNECT_CLOUD_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (VersionNotSupportedException)
			{
				var result = MessageBox.Show(Resources.NEED_UPGRADE, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					AutoUpdate update = new AutoUpdate(true);
					if (update.IsUpdateRequired())
						update.ShowUpdateNeededUI();
					else
						MessageBox.Show(Resources.ALREAD_UPDATED, Application.ProductName);
				}
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UNKNOW_SIGNIN_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			parameters.Set("user_id", session.user_id);
			parameters.Set("session_token", session.session_token);
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

		private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				loginButton1_Click(this, e);
			}
		}

	}
}
