using System;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;

namespace Waveface.Stream.WindowsClient
{
	public partial class LoginDialog : Form
	{
		#region Static Var
		private static LoginDialog _instance;
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
			if (StreamClient.Instance.IsLogined)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				return;
			}

			Show();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Hide();

			var wizard = new OldUserWizardDialog();
			wizard.StartPosition = FormStartPosition.CenterParent;
			wizard.FormClosed += wizard_FormClosed;
			wizard.ShowDialog();
		} 
		#endregion
	}
}
