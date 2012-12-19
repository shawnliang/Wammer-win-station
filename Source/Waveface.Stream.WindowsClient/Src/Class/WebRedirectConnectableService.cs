using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	abstract class WebRedirectConnectableService : IConnectableService
	{
		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string CALLBACK_URL_PATH = @"/client/callback";
		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";
		private const string FB_TURN_ON_URL_PATH = @"/sns/{0}/connect";

		private string svcType;

		protected WebRedirectConnectableService(string svcType, string displayName)
		{
			this.svcType = svcType;
			this.Name = displayName;
		}

		public string Name { get; private set; }

		public bool IsEnabled(string user_id, string session_token, string api_key)
		{
			var loginInfo = JsonConvert.DeserializeObject<MR_users_get>(StationAPI.GetUser(session_token, user_id));

			return loginInfo.user.sns != null &&
				loginInfo.user.sns.Any(x => x.type.Equals(svcType, StringComparison.InvariantCultureIgnoreCase));
		}


		public System.Drawing.Image Icon
		{
			get { return Properties.Resources.f_logo; }
		}


		public void Connect(string session_token, string api_key)
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
				api_key = api_key,
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
				Text = string.Format(Properties.Resources.FB_CONNECT_PAGE_TITLE, Name),
				StartPosition = FormStartPosition.CenterParent,
				Icon = Properties.Resources.Icon
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

			var connectUrl = baseurl + string.Format(FB_TURN_ON_URL_PATH, svcType) + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture);

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
					else if (apiRetCode != "0")
					{
						throw new Exception(parameters["api_ret_message"]);
					}

					return;
				}
			}
			else
				throw new OperationCanceledException();
		}

		public void Disconnect(string session_token, string api_key)
		{
			StationAPI.SNSDisconnect(session_token, svcType);
		}
	}
}
