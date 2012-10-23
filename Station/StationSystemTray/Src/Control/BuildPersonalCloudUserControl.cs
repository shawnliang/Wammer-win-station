using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace StationSystemTray.Src.Control
{
	public partial class BuildPersonalCloudUserControl : UserControl
	{
		private const string FIREFOX_URL = @"https://addons.mozilla.org/zh-TW/firefox/addon/waveface-stream/";
		private const string CHROME_URL = @"https://chrome.google.com/webstore/detail/stream-visited-links-a-be/fneddinlohhbafadpaoidhgklemkknff";

		public BuildPersonalCloudUserControl()
		{
			InitializeComponent();
		}

		private void firefoxBtn_Click(object sender, EventArgs e)
		{
			Process.Start(FIREFOX_URL);
		}

		private void chromeStoreBtn_Click(object sender, EventArgs e)
		{
			Process.Start(CHROME_URL);
		}
	}
}
