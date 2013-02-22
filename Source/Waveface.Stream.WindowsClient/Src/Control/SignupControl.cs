using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class SignupControl : UserControl
	{
		ISignupAction signup = new StreamSignup();
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
	}


	public interface ISignupAction
	{
		void SignUpWithEmail(string email, string password, string name);
		void SignUpWithFacebook();
	}
}
