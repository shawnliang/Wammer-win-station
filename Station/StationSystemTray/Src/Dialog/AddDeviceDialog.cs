using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Properties;
using System.IO;
using System.Diagnostics;

namespace StationSystemTray
{
	public partial class AddDeviceDialog : Form
	{
		private const string GOOGLE_PLAY_URL = @"https://play.google.com/store/apps/details?id=com.waveface.wammer";
		private const string APPSTORE_URL = @"https://itunes.apple.com/us/app/waveface-stream/id487141623?mt=8";

		public AddDeviceDialog()
		{
			InitializeComponent();
			Icon = Resources.Icon;
		}

		private void AddDeviceDialog_Load(object sender, EventArgs e)
		{
			var rtfBytes = Encoding.UTF8.GetBytes(Resources.addDeviceInstruction);
			using (var rtf = new MemoryStream(rtfBytes))
			{
				richTextBox1.LoadFile(rtf, RichTextBoxStreamType.RichText);
			}
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void appStoreButton_Click(object sender, EventArgs e)
		{
			Process.Start(APPSTORE_URL);
		}

		private void googlePlayButton_Click(object sender, EventArgs e)
		{
			Process.Start(GOOGLE_PLAY_URL);
		}
	}
}
