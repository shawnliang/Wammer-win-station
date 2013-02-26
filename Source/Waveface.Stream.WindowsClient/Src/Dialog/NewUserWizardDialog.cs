using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class NewUserWizardDialog : Form
	{
		#region Private Property
		/// <summary>
		/// Gets the m_ tab control.
		/// </summary>
		/// <value>The m_ tab control.</value>
		private TabControlEx m_TabControl
		{
			get
			{
				return tabControl1;
			}
		}
		#endregion


		#region Constructor
		public NewUserWizardDialog()
		{
			InitializeComponent();
		} 
		#endregion


		#region Private Method
		private void UpdateUI()
		{
			var selectedTab = m_TabControl.SelectedTab;

			if (selectedTab == null)
				return;

			this.Text = selectedTab.Text;

			buttonPrev.Visible = selectedTab != tabIntro1 && selectedTab != tabConnectCloudServices;
			buttonNext.Visible = selectedTab != tabSignup;

			buttonNext.Text = m_TabControl.IsLastPage ? Resources.CLOSE_BUTTON_TEXT : Resources.NEXT_BUTTON_TEXT;

			panel1.Visible = selectedTab != tabSignup;
		}
		#endregion


		#region Event Process
		private void NewUserWizardDialog_Load(object sender, EventArgs e)
		{
			this.Icon = Properties.Resources.Icon;

			UpdateUI();
			nativeSignupControl1.SignUpSuccess += signUpControl1_SignUpSuccess;
		}

		void signUpControl1_SignUpSuccess(object sender, EventArgs e)
		{
			m_TabControl.NextPage();
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			m_TabControl.PreviousPage();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (m_TabControl.IsLastPage)
			{
				fileImportControl1.ImportSelectedPaths();
				this.DialogResult = DialogResult.OK;
				return;
			}

			m_TabControl.NextPage();
		}
		#endregion
	}
}
