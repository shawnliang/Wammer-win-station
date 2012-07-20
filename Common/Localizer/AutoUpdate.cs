using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.NetSparkle;
using Microsoft.Win32;
using Waveface.Properties;

namespace Waveface.Common
{
	public class AutoUpdate
	{
		public const string DEF_BASE_URL = "https://develop.waveface.com/v2/";

		private Sparkle m_autoUpdator;
		private NetSparkleAppCastItem versionInfo;
		private bool forceUpgrade;

		public AutoUpdate(bool forceUpgrade)
		{
			m_autoUpdator = new Sparkle(UpdateURL + "/extensions/windowsUpdate/versioninfo.xml");
			m_autoUpdator.ApplicationIcon = Resources.software_update_available;
			m_autoUpdator.ApplicationWindowIcon = Resources.UpdateAvailable;

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
				var url = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "cloudBaseURL", DEF_BASE_URL) as string;

				return url ?? DEF_BASE_URL;
			}
		}

		private static string UpdateURL
		{
			get
			{
				if (CloudBaseURL.Contains("api.waveface.com"))
					return "https://waveface.com";
				else if (CloudBaseURL.Contains("develop.waveface.com"))
					return "http://develop.waveface.com:4343";
				else if (CloudBaseURL.Contains("staging.waveface.com"))
					return "http://staging.waveface.com";
				else
					return "https://waveface.com";
			}
		}



	}
}
