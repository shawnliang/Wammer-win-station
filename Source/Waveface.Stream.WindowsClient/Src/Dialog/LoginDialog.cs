﻿using System;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class LoginDialog : Form
	{
		#region Static Var
		private static LoginDialog _instance;
		#endregion


		#region Property
		//public String Email 
		//{
		//    get
		//    {
		//        return loginInputBox1.Email;
		//    }
		//}

		//public String Password
		//{
		//    get
		//    {
		//        return loginInputBox1.Password;
		//    }
		//}

		public String SessionToken { get; private set; }
		public String UserId { get; private set; }
		#endregion



		#region Public Static Property
		public static LoginDialog Instance
		{
			get
			{
				return (_instance == null || _instance.IsDisposed) ? (_instance = new LoginDialog()) : _instance;
			}
		}
		#endregion


		#region Constructor
		private LoginDialog()
		{
			InitializeComponent();
		}
		#endregion

		#region Event Process
		private void loginButton1_Click(object sender, EventArgs e)
		{
			//if ((Email == string.Empty) || (Password == string.Empty))
			//{
			//    MessageBox.Show(Resources.FillAllFields, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//    return;
			//}

			//if (!TestEmailFormat(cmbEmail.Text))
			//{
			//    MessageBox.Show(Resources.InvalidEmail, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//    return;
			//}

			this.DialogResult = DialogResult.OK;
		}
		#endregion

		private void button1_Click(object sender, EventArgs e)
		{
			Hide();

			var wizard = new NewUserWizardDialog();
			wizard.StartPosition = FormStartPosition.CenterParent;
			wizard.FormClosed += wizard_FormClosed;
			wizard.ShowDialog();
		}

		void wizard_FormClosed(object sender, FormClosedEventArgs e)
		{
			var wizard = (StepByStepWizardDialog)sender;
			var parameters = wizard.Parameters;
			var session_token = (string)wizard.Parameters.Get("session_token");

			if (string.IsNullOrEmpty(session_token))
			{
				Show();
			}
			else
			{
				this.SessionToken = session_token;
				this.UserId = (string)wizard.Parameters.Get("user_id");

				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Hide();

			var wizard = new OldUserWizardDialog();
			wizard.StartPosition = FormStartPosition.CenterParent;
			wizard.FormClosed += wizard_FormClosed;
			wizard.ShowDialog();
		}
	}
}