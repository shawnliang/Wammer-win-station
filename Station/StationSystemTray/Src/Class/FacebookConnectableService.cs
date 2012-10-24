using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Cloud;
using Wammer.Station;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace StationSystemTray.Src.Class
{
	class FacebookConnectableService : IConnectableService
	{
		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string CALLBACK_URL_PATH = @"/client/callback";
		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";
		private const string FB_TURN_ON_URL_PATH = @"/sns/facebook/connect";

		private string user_id;
		private string session_token;
		private string api_key;

		public FacebookConnectableService(string user_id, string session_token, string api_key)
		{
			this.user_id = user_id;
			this.session_token = session_token;
			this.api_key = api_key;
		}

		public string Name
		{
			get { return "Facebook"; }
		}

		public bool Enabled
		{
			get
			{
				var loginInfo = Wammer.Cloud.User.GetLoginInfo(user_id, api_key, session_token);
				
				return loginInfo.user.sns != null && 
					loginInfo.user.sns.Any(x => x.type.Equals("facebook"));
			}
			set
			{
				if (value)
					connect();
				else
					disconnect();
			}
		}

		public System.Drawing.Image Icon
		{
			get { return StationSystemTray.Properties.Resources.f_logo; }
		}


		private void connect()
		{
			var baseurl = (CloudServer.Type == CloudType.Production) ? WEB_BASE_URL :
				(CloudServer.Type == CloudType.Development) ? DEV_WEB_BASE_PAGE_URL : STAGING_BASE_URL;

			var fbLoginUrl = string.Format("{0}/client/callback/{2}/FBConnect", baseurl, CALLBACK_URL_PATH, FB_LOGIN_GUID);

			//string fbLoginUrl = string.Format("{0}/{1}/FBConnect", m_CallbackUrl, FB_LOGIN_GUID);
			var postData = new FBPostData
			{
				device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
				device_name = Environment.MachineName,
				device = "windows",
				api_key = CLIENT_API_KEY,
				xurl =
					string.Format(
						"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s",
						fbLoginUrl),
				locale = Thread.CurrentThread.CurrentCulture.ToString(),
				session_token = session_token
			};

			var browser = new WebBrowser
			{
				WebBrowserShortcutsEnabled = false,
				IsWebBrowserContextMenuEnabled = false,
				Dock = DockStyle.Fill
			};

			var dialog = new Form
			{
				Width = 750,
				Height = 600,
				Text = StationSystemTray.Properties.Resources.FB_CONNECT_PAGE_TITLE,
				StartPosition = FormStartPosition.CenterParent,
				Icon = StationSystemTray.Properties.Resources.Icon
			};

			dialog.Controls.Add(browser);

			browser.Navigated += (s, ex) =>
			{
				Uri url = browser.Url;
				if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "FBConnect"),
								  RegexOptions.IgnoreCase))
				{
					dialog.DialogResult = DialogResult.OK;
				}
			};

			var connectUrl = baseurl + FB_TURN_ON_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture);

			browser.Navigate(connectUrl,
							 string.Empty,
							 Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postData, Formatting.Indented)),
							 "Content-Type: application/json");

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var url = browser.Url;
				var parameters = HttpUtility.ParseQueryString(url.Query);
				var apiRetCode = parameters["api_ret_code"];

				if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				{
					if (apiRetCode == "4098")
					{
						throw new Exception(Properties.Resources.FB_CONNECT_FAILED);
					}

					return;
				}
			}
			else
				throw new OperationCanceledException();
		}

		private void disconnect()
		{
			Wammer.Cloud.User.DisconnectWithSns(session_token, api_key, "facebook");
		}
	}
}
