using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class LoginControl : UserControl
	{
		#region Const
		const string EMAIL_VALIDATE_MATCH_PATTERN = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";
		#endregion

		#region Var
		private IStreamLoginable login;
		private UserSession session;
		#endregion


		#region Event
		public event EventHandler LoginSuccess;
		#endregion


		#region Constructor
		public LoginControl()
			: this(new StreamLogin())
		{
		}

		public LoginControl(IStreamLoginable login)
		{
			InitializeComponent();
			this.login = login;
		}
		#endregion



		#region Private Method
		private void loginAndHandleError(Func<UserSession> func)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				session = func();

				OnLoginSuccess(EventArgs.Empty);
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

		private void Login()
		{
			var email = loginInputBox1.Email;
			var pwd = loginInputBox1.Password;

			if (!Regex.IsMatch(email, EMAIL_VALIDATE_MATCH_PATTERN, RegexOptions.IgnoreCase))
			{
				MessageBox.Show(Resources.INVALID_EMAIL);
				return;
			}

			Func<UserSession> func = () => { return login.Login(email, pwd); };
			loginAndHandleError(func);
		}

		private void LoginWithFB()
		{
			Func<UserSession> func = () => { return login.LoginWithFacebook(); };
			loginAndHandleError(func);
		}
		#endregion


		#region Protected Method
		protected void OnLoginSuccess(EventArgs e)
		{
			this.RaiseEvent(LoginSuccess, e);
		}
		#endregion


		#region Event Process
		private void fbLoginButton1_Click(object sender, EventArgs e)
		{
			LoginWithFB();
		}

		private void loginButton1_Click(object sender, EventArgs e)
		{
			Login();
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

		private void loginInputBox1_InputDone(object sender, EventArgs e)
		{
			loginButton.PerformClick();
		}
		#endregion
	}
}
