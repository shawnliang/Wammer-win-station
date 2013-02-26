using AppLimit.NetSparkle;
using Microsoft.Win32;
using System;
using System.Drawing;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public class AutoUpdate
	{
		public const string DEF_BASE_URL = "https://api.waveface.com/v3/";

		private Sparkle m_autoUpdator;
		private NetSparkleAppCastItem versionInfo;
		private bool forceUpgrade;

		public AutoUpdate(bool forceUpgrade)
		{
			m_autoUpdator = new Sparkle(UpdateURL);
			m_autoUpdator.ApplicationIcon = Resources.streamUpdate_icon_512;
			m_autoUpdator.ApplicationWindowIcon = Icon.FromHandle(Resources.streamUpdate_icon_512.GetHicon());

			this.forceUpgrade = forceUpgrade;
		}

		public void StartLoop()
		{
			m_autoUpdator.StartLoop(true, TimeSpan.FromHours(5.0));
		}
		public bool IsUpdateRequired()
		{
			var honorSkippedVersion = !forceUpgrade;

			return m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(),
				out versionInfo,
				honorSkippedVersion);
		}

		public void ShowUpdateNeededUI()
		{
			if (versionInfo == null)
				throw new InvalidOperationException("No version info. Call IsUpdateRequired() first");

			m_autoUpdator.ShowUpdateNeededUI(versionInfo, forceUpgrade);
		}

		private static string CloudBaseURL
		{
			get
			{
				var keypath = (IntPtr.Size == 8) ? @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Wammer\WinStation" :
					@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";

				var url = Registry.GetValue(keypath, "cloudBaseURL", DEF_BASE_URL) as string;

				return url ?? DEF_BASE_URL;
			}
		}

		private static string UpdateURL
		{
			get
			{
				if (CloudBaseURL.Contains("api.waveface.com"))
					return "https://waveface.com/extensions/windowsUpdate/versioninfo.xml";
				else if (CloudBaseURL.Contains("develop.waveface.com"))
					return "http://develop.waveface.com:4343/extensions/windowsUpdate/versioninfo_dev.xml";
				else if (CloudBaseURL.Contains("staging.waveface.com"))
					return "http://staging.waveface.com/extensions/windowsUpdate/versioninfo.xml";
				else
					return "https://waveface.com/extensions/windowsUpdate/versioninfo.xml";
			}
		}



	}
}
