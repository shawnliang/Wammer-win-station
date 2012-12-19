using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	class StreamLogin : IStreamLoginable
	{
		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string CALLBACK_URL_PATH = @"/client/callback";
		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";
		private const string LOGIN_URL_PATH = @"/sns/facebook/signin";

		public UserSession LoginWithFacebook()
		{
			var baseurl = (CloudServer.Type == CloudType.Production) ? WEB_BASE_URL :
				(CloudServer.Type == CloudType.Development) ? DEV_WEB_BASE_PAGE_URL : STAGING_BASE_URL;

			var callbackUrl = Path.Combine(baseurl, CALLBACK_URL_PATH);


			string fbLoginUrl = string.Format("{0}/{1}/FBLogin", callbackUrl, FB_LOGIN_GUID);
			var postData = new FBPostData
			{
				device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
				device_name = Environment.MachineName,
				device = "windows",
				api_key = StationAPI.API_KEY,
				xurl =
					string.Format(
						"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s&is_new_user=%(is_new_user)s",
						fbLoginUrl),
				locale = Thread.CurrentThread.CurrentCulture.ToString(),
				show_tutorial = "false"
			};

			var dialog = new SignUpDialog()
			{
				Text = Resources.FB_CONNECT_PAGE_TITLE,
				StartPosition = FormStartPosition.CenterParent
			};
			var browser = dialog.Browser;
			var signupOK = false;

			browser.Navigated += (s, ex) =>
			{
				Uri url = browser.Url;
				if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "FBLogin"),
								  RegexOptions.IgnoreCase))
				{
					var parameters = HttpUtility.ParseQueryString(url.Query);

					var isNewUser = parameters["is_new_user"];

					dialog.DialogResult = DialogResult.OK;

					signupOK = true;
				}
			};

			var fbUrl = baseurl + LOGIN_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture);
			browser.Navigate(fbUrl,
							 string.Empty,
							 Encoding.UTF8.GetBytes(postData.ToFastJSON()),
							 "Content-Type: application/json");

			dialog.ShowDialog();
			if (signupOK)
			{
				string url = browser.Url.Query;
				NameValueCollection parameters = HttpUtility.ParseQueryString(url);
				string apiRetCode = parameters["api_ret_code"];

				if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				{
					throw new Exception("fb login error. api ret code = " + apiRetCode);
				}

				string sessionToken = parameters["session_token"];
				string userID = parameters["user_id"];

				Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
				if (driver == null)
				{
					AddUserResponse res = JsonConvert.DeserializeObject<AddUserResponse>(StationAPI.AddUser(userID, sessionToken));
				}

				StreamClient.Instance.LoginSNS(userID, sessionToken);

				return new UserSession { session_token = sessionToken, user_id = userID };


			}
			else
				throw new OperationCanceledException();
		}

		public UserSession Login(string email, string password)
		{
			AddUserResponse res = JsonConvert.DeserializeObject<AddUserResponse>(StationAPI.AddUser(email.ToLower(), password,
				StationRegistry.StationId, Environment.MachineName));


			var session = LoginToStation(email, password);

			return new UserSession { user_id = res.UserId, session_token = session.session_token };
		}

		private LoginedSession LoginToStation(string email, string password)
		{
			return JsonConvert.DeserializeObject<LoginedSession>(StreamClient.Instance.Login(
					email,
					password));
		}


		public void ForgotPassword()
		{
			switch (CloudServer.Type)
			{
				case CloudType.Production:
					Process.Start(@"https://waveface.com/password/forgot");
					return;
				case CloudType.Development:
					Process.Start(@"https://devweb.waveface.com/password/forgot");
					return;
				case CloudType.Staging:
					Process.Start(@"http://staging.waveface.com/password/forgot");
					return;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
