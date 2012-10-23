using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace StationSystemTray.Src.Dialog
{
	public partial class InstallAppDialog : Form
	{
		private string storeUrl;

		public InstallAppDialog(Image storePicture, Image qrCodePicture, string storeUrl)
		{
			InitializeComponent();

			this.storePicture.Image = storePicture;
			this.qrCodePicture.Image = qrCodePicture;
			this.storeUrl = storeUrl;
		}

		private void storePicture_Click(object sender, EventArgs e)
		{
			Process.Start(storeUrl);
		}
	}
}
