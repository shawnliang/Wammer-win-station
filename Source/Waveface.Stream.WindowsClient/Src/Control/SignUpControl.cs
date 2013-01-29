using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class SignUpControl : UserControl
	{
		private IStreamSignUp signup;
		private SignUpData signupData;

		#region Event
		public event EventHandler SignUpSuccess;
		#endregion

		public SignUpControl()
			:this(new StreamSignup())
		{
		}

		public SignUpControl(IStreamSignUp signup)
		{
			InitializeComponent();

			this.signup = signup;
			this.webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
		}

		#region Protected Method
		protected void OnSignUpSuccess(EventArgs e)
		{
			this.RaiseEvent(SignUpSuccess, e);
		}
		#endregion

		public void ShowSignUpPage()
		{
			try
			{
				signup.ShowSignUpPage(webBrowser1);
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			try
			{
				this.signupData = signup.TryParseSignUpDataFromUrl(e.Url);

				if (this.signupData != null)
					OnSignUpSuccess(EventArgs.Empty);
			}
			catch (OperationCanceledException)
			{
				// user closed signup page without signing up
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
					var update = new AutoUpdate(true);
					if (update.IsUpdateRequired())
						update.ShowUpdateNeededUI();
					else
						MessageBox.Show(Resources.ALREAD_UPDATED, Application.ProductName);
				}
			}
#if DEBUG
			catch (WebException ex)
			{
				using (var sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					var msg = sr.ReadToEnd();
					MessageBox.Show(msg);
				}
			}
#endif
			catch(Exception)
			{
				MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}
