using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wammer.Station.Management;
using StationSystemTray.Properties;
using Waveface.Common;
using System.Drawing;

namespace StationSystemTray.Src.Control
{
	public partial class SignUpControl : StepPageControl
	{
		private IStreamSignUp signup;
		private SignUpData signupData;

		public SignUpControl(IStreamSignUp signup)
		{
			InitializeComponent();

			this.signup = signup;
			this.PageTitle = "Sign Up";
			this.CustomSize = new Size(740, 590);
			this.webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
		}

		void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			try
			{
				this.signupData = signup.TryParseSignUpDataFromUrl(e.Url);
				if (this.signupData != null)
					WizardControl.NextPage();
			}
			catch (OperationCanceledException)
			{
				// user closed signup page without signing up
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
					var update = new AutoUpdate(true);
					if (update.IsUpdateRequired())
						update.ShowUpdateNeededUI();
					else
						MessageBox.Show(Resources.ALREAD_UPDATED, Resources.APP_NAME);
				}
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			parameters.Set("user_id", signupData.user_id);
			parameters.Set("session_token", signupData.session_token);
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			try
			{
				signup.ShowSignUpPage(webBrowser1);
			}
			catch (Exception)
			{
				MessageBox.Show(Resources.UNKNOW_SIGNUP_ERROR, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
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
	}
}
