﻿using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class SignupControl : UserControl
	{
		private ISignupAction signup = new StreamSignup();
		const string EMAIL_VERIFY_PATTERN = @"\b([A-Z0-9._%+-]+)@[A-Z0-9.-]+\.[A-Z]{2,4}\b";


		public event EventHandler SignUpSuccess;

		public SignupControl()
		{
			InitializeComponent();
		}


		public ISignupAction SignupAction { get; set; }

		private void fbButton_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				signup.SignUpWithFacebook();
				this.RaiseEvent(SignUpSuccess, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.GetDisplayDescription(), Resources.UNKNOW_SIGNUP_ERROR);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void nativeSignupButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(emailBox.Text) || string.IsNullOrEmpty(passwordBox.Text) || string.IsNullOrEmpty(nameBox.Text))
					throw new Exception(Resources.FILL_ALL_FIELDS);

				if (!Regex.IsMatch(emailBox.Text, EMAIL_VERIFY_PATTERN, RegexOptions.IgnoreCase))
				{
					emailBox.Focus();
					throw new Exception(Resources.INVALID_EMAIL);
				}

				if (passwordBox.Text.Length < 6 || 16 < passwordBox.Text.Length || passwordBox.Text.Contains(" "))
				{
					passwordBox.Text = "";
					passwordBox.Focus();
					throw new Exception(Resources.INVALID_PASSWORD);
				}

				Cursor.Current = Cursors.WaitCursor;
				signup.SignUpWithEmail(emailBox.Text, passwordBox.Text, nameBox.Text);
				this.RaiseEvent(SignUpSuccess, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.GetDisplayDescription(), Resources.UNKNOW_SIGNUP_ERROR);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void nameBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' || e.KeyChar == '\n')
				nativeSignupButton_Click(sender, e);
		}

		private void emailBox_Leave(object sender, EventArgs e)
		{
			var m = Regex.Match(emailBox.Text, EMAIL_VERIFY_PATTERN, RegexOptions.IgnoreCase);
			if (!m.Success)
			{
				return;
			}

			nameBox.Text = m.Groups[1].Value;
		}
	}


	public interface ISignupAction
	{
		void SignUpWithEmail(string email, string password, string name);
		void SignUpWithFacebook();
	}
}