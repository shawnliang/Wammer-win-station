using System;
using System.Diagnostics;
using System.Windows.Forms;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	static class GoToWeb
	{
		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";

		private static string baseUrl
		{
			get
			{
				return (CloudServer.Type == CloudType.Production) ? WEB_BASE_URL :
				(CloudServer.Type == CloudType.Development) ? DEV_WEB_BASE_PAGE_URL : STAGING_BASE_URL;
			}
		}
		public static void OpenInBrowser(string relativeURL)
		{
			try
			{
				Process.Start(baseUrl + relativeURL);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Cannot open browser", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}
