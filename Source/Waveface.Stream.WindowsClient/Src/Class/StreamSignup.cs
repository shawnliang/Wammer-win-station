using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class StreamSignup : IStreamSignUp
	{
		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string CALLBACK_URL_PATH = @"/client/callback";
		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";
		private const string LOGIN_URL_PATH = @"/sns/facebook/signin";
		private const string SIGNUP_URL_PATH = @"/signup";

		public void ShowSignUpPage(WebBrowser browser)
		{
			var baseurl = (CloudServer.Type == CloudType.Production) ? WEB_BASE_URL :
								(CloudServer.Type == CloudType.Development) ? DEV_WEB_BASE_PAGE_URL : STAGING_BASE_URL;

			var callbackUrl = Path.Combine(baseurl, CALLBACK_URL_PATH);

			string signUpUrl = string.Format("{0}/{1}/SignUp", callbackUrl, FB_LOGIN_GUID);
			var postData = new FBPostData
			{
				device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
				device_name = Environment.MachineName,
				device = "windows",
				api_key = StationAPI.API_KEY,
				xurl =
					string.Format(
						"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s&session_token=%(session_token)s&user_id=%(user_id)s&account_type=%(account_type)s&email=%(email)s&password=%(password)s&is_new_user=%(is_new_user)s",
						signUpUrl),
				locale = Thread.CurrentThread.CurrentCulture.ToString(),
				show_tutorial = "false"
			};

			var navigateUrl = baseurl + SIGNUP_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture);
			browser.Navigate(navigateUrl,
							 string.Empty,
							 Encoding.UTF8.GetBytes(postData.ToFastJSON()),
							 "Content-Type: application/json");
		}

		public SignUpData TryParseSignUpDataFromUrl(Uri url)
		{
			if (!Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "SignUp"),
							  RegexOptions.IgnoreCase))
				return null;

			var parameters = HttpUtility.ParseQueryString(url.Query);

			var isNewUser = parameters["is_new_user"];

			Cursor.Current = Cursors.WaitCursor;

			try
			{
				//var parameters = HttpUtility.ParseQueryString(url.Query);
				var apiRetCode = parameters["api_ret_code"];

				if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				{
					throw new Exception("sign up error: " + apiRetCode);
				}

				var sessionToken = parameters["session_token"];
				var userID = parameters["user_id"];
				var email = parameters["email"];
				var password = parameters["password"];
				var accountType = parameters["account_type"];

				var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
				if (driver == null)
				{
					if (string.Compare(accountType, "native", true) == 0)
					{
						AddUserResponse res = JsonConvert.DeserializeObject<AddUserResponse>(StationAPI.AddUser(email, password, StationRegistry.GetValue("stationId", string.Empty).ToString(), Environment.MachineName));
						var session = LoginToStation(email, password);

						return new SignUpData
						{
							account_type = accountType,
							email = email,
							user_id = userID,
							password = password,
							session_token = session.session_token
						};
					}
					else
					{
						AddUserResponse res = JsonConvert.DeserializeObject<AddUserResponse>(StationAPI.AddUser(userID, sessionToken));
						StationAPI.Login(userID, sessionToken);

						return new SignUpData
						{
							account_type = accountType,
							email = email,
							user_id = userID,
							session_token = sessionToken
						};
					}
				}
				else
				{
					if (string.Compare(accountType, "native", true) == 0)
					{
						var session = LoginToStation(email, password);

						return new SignUpData
						{
							account_type = accountType,
							email = email,
							user_id = userID,
							password = password,
							session_token = session.session_token
						};
					}
					else
					{
						StationAPI.Login(userID, sessionToken);
						return new SignUpData
						{
							account_type = accountType,
							email = email,
							user_id = userID,
							session_token = sessionToken
						};
					}

				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}


		}

		private LoginedSession LoginToStation(string email, string password)
		{
			return JsonConvert.DeserializeObject<LoginedSession>(StreamClient.Instance.Login(
				email,
				password));
		}

	}
}
