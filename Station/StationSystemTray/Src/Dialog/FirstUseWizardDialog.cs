using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Src.Control;

namespace StationSystemTray
{
	public partial class FirstUseWizardDialog : Form
	{
		#region Private Property
		private string m_OriginalTitle { get; set; } 
		#endregion


		#region Constructor
		public FirstUseWizardDialog()
		{
			InitializeComponent();

			m_OriginalTitle = this.Text;

			wizardControl1.SetWizardPages(new Control[]
			{
				new BuildPersonalCloudUserControl()
			});
		} 
		#endregion


		#region Event Process
		private void FirstUseWizardDialog_Load(object sender, EventArgs e)
		{
			this.Text = string.Format("{0} ({1} of {2})", m_OriginalTitle, wizardControl1.PageIndex, wizardControl1.PageCount);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			wizardControl1.NextPage();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			wizardControl1.PreviousPage();
		} 
		#endregion
	}
}
