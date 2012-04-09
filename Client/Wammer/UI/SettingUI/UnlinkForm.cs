using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface.SettingUI
{
	public partial class UnlinkForm : Form
	{
		public UnlinkForm()
		{
			InitializeComponent();
			this.pictureBox1.Image = SystemIcons.Warning.ToBitmap();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}
	}
}
