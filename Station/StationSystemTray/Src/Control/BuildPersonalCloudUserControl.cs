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
	public partial class BuildPersonalCloudUserControl : StepPageControl
	{
		private const string FIREFOX_URL = @"https://addons.mozilla.org/zh-TW/firefox/addon/waveface-stream/";
		private const string CHROME_URL = @"https://chrome.google.com/webstore/detail/stream-visited-links-a-be/fneddinlohhbafadpaoidhgklemkknff";
		private const string GOOGLE_PLAY_URL = @"https://play.google.com/store/apps/details?id=com.waveface.wammer";
		private const string APPSTORE_URL = @"https://itunes.apple.com/us/app/waveface-stream/id487141623?mt=8";

		private InstallAppDialog installDialog;
		private int colorIndex = 0;
		private string user_id;

		public event EventHandler<AppInstalEventArgs> OnAppInstall;
		public event EventHandler<AppInstalEventArgs> OnAppInstallCanceled;


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

		public void ShowDeviceConnected(string device)
		{
			Color[] colors = { Color.Black, Color.DarkBlue, Color.DarkCyan, Color.DarkGoldenrod };

			

			tickedLabel.Visible = true;
			connectedLabel.Text = device + " connected";
			connectedLabel.ForeColor = colors[colorIndex++];

			if (colorIndex >= colors.Length)
				colorIndex = 0;

			connectedLabel.Visible = true;
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
			//showInstallDialog(Resources.button_googleplay, Resources.stream_googleplay_qr, GOOGLE_PLAY_URL);
		}

		private void appStoreBtn_Click(object sender, EventArgs e)
		{
			//showInstallDialog(Resources.button_appstore, Resources.stream_appstore_qr, APPSTORE_URL);
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
			EventHandler<AppInstalEventArgs> handler = OnAppInstall;
			if (handler != null)
				handler(this, new AppInstalEventArgs(user_id));
		}

		private void RaiseOnAppInstallCanceled()
		{
			EventHandler<AppInstalEventArgs> handler = OnAppInstallCanceled;
			if (handler != null)
				handler(this, new AppInstalEventArgs(user_id));
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			var user_id = parameters.Get("user_id");
			if (user_id == null)
				throw new ArgumentException("missing required parameter: user_id");

			this.user_id = (string)user_id;
		}
	}


	public class AppInstalEventArgs : EventArgs
	{
		public string user_id { get; private set; }

		public AppInstalEventArgs(string user_id)
		{
			this.user_id = user_id;
		}
	}
}
