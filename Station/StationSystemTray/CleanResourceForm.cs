﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public partial class CleanResourceForm : Form
	{
		public CleanResourceForm(string email)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.pictureBox1.Image = SystemIcons.Question.ToBitmap();
			this.lblConfirm.Text = I18n.L.T("CleanResourceMsg", email);
		}

		private void btnYes_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
		}
	}
}