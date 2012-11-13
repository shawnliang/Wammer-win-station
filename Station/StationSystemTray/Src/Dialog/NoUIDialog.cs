using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray.Src.Dialog
{
	public partial class NoUIDialog : Form
	{
		private string msg = "";

		public NoUIDialog(string msg = "")
		{
			InitializeComponent();
			this.msg = msg;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void NoUIDialog_Load(object sender, EventArgs e)
		{
			textBox1.Text = msg;
		}
	}
}
