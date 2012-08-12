using System;
using System.Drawing;
using System.Windows.Forms;
using StationSystemTray.Properties;

namespace StationSystemTray
{
	public partial class CleanResourceDialog : Form
	{
		public Boolean RemoveAllDatas
		{
			get { return checkBox1.Checked; }
		}

		public CleanResourceDialog(string email)
		{
			Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			pictureBox1.Image = SystemIcons.Question.ToBitmap();
			lblConfirm.Text = string.Format(Resources.CleanResourceMsg, email);
		}

		private void btnYes_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
		}
	}
}