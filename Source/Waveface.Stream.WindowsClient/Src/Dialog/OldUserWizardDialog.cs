using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class OldUserWizardDialog : Form
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
		public OldUserWizardDialog()
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

			button1.Visible = selectedTab != tabPage1 && selectedTab != tabPage2;
			button2.Visible = selectedTab != tabPage1;

			button2.Text = m_TabControl.IsLastPage ? "Done" : "Next";
		}
		#endregion



		#region Event Process
		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void OldUserWizardDialog_Load(object sender, EventArgs e)
		{
			this.Icon = Properties.Resources.Icon;

			UpdateUI();

			loginControl1.LoginSuccess += loginControl1_LoginSuccess;
		}

		void loginControl1_LoginSuccess(object sender, EventArgs e)
		{
			m_TabControl.NextPage();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			m_TabControl.PreviousPage();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (m_TabControl.IsLastPage)
			{
				fileImportControl1.ChangeLocation();
				fileImportControl1.ImportSelectedPaths();
				this.DialogResult = DialogResult.OK;
				return;
			}

			m_TabControl.NextPage();
		}
		#endregion
	}
}
