using System;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class CleanResourceDialog : Form
	{
		public Boolean RemoveAllDatas
		{
			get { return checkBox1.Checked; }
		}

		public CleanResourceDialog()
		{
			Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			pictureBox1.Image = SystemIcons.Question.ToBitmap();
		}

		private void btnYes_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
		}
	}
}