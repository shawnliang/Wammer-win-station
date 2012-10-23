using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using StationSystemTray.Src.Dialog;
using StationSystemTray.Properties;

namespace StationSystemTray.Src.Control
{
	public partial class BuildPersonalCloudUserControl : UserControl
	{
		private const string FIREFOX_URL = @"https://addons.mozilla.org/zh-TW/firefox/addon/waveface-stream/";
		private const string CHROME_URL = @"https://chrome.google.com/webstore/detail/stream-visited-links-a-be/fneddinlohhbafadpaoidhgklemkknff";
		private const string GOOGLE_PLAY_URL = @"https://play.google.com/store/apps/details?id=com.waveface.wammer";
		private const string APPSTORE_URL = @"https://itunes.apple.com/us/app/waveface-stream/id487141623?mt=8";

		private InstallAppDialog installDialog;

		public event EventHandler OnAppInstall;
		public event EventHandler OnAppInstallCanceled;

		public BuildPersonalCloudUserControl()
		{
			InitializeComponent();
		}

		public void CloseInstallDialog()
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(CloseInstallDialog));
			else
			{
				if (installDialog != null)
				{
					installDialog.DialogResult = DialogResult.Yes;
					installDialog.Close();
				}
			}
		}

		private void firefoxBtn_Click(object sender, EventArgs e)
		{
			Process.Start(FIREFOX_URL);
		}

		private void chromeStoreBtn_Click(object sender, EventArgs e)
		{
			Process.Start(CHROME_URL);
		}

		private void googlePlayBtn_Click(object sender, EventArgs e)
		{
			showInstallDialog(Resources.button_googleplay, Resources.stream_googleplay_qr, GOOGLE_PLAY_URL);
		}

		private void appStoreBtn_Click(object sender, EventArgs e)
		{
			showInstallDialog(Resources.button_appstore, Resources.stream_appstore_qr, APPSTORE_URL);
		}

		private void showInstallDialog(Bitmap storePic, Bitmap qrCodePic, string url)
		{
			RaiseOnAppInstall();

			installDialog = new InstallAppDialog(storePic, qrCodePic, url);
			var result = installDialog.ShowDialog();
			installDialog = null;

			if (result != DialogResult.Yes)
			{
				RaiseOnAppInstallCanceled();
			}
		}

		private void RaiseOnAppInstall()
		{
			EventHandler handler = OnAppInstall;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private void RaiseOnAppInstallCanceled()
		{
			EventHandler handler = OnAppInstallCanceled;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
