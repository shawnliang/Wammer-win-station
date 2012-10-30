using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using System.Threading;
using StationSystemTray.Dialog;
using StationSystemTray.Properties;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using Wammer.Utility;
using System.Collections.Specialized;
using Wammer.Model;
using Wammer.Station.Management;
using System.IO;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace StationSystemTray
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

						if (isNewUser.Equals("true", StringComparison.CurrentCultureIgnoreCase))
							dialog.ShowTutorial();
						else
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
						//if (!IsDisposed)
						//    Show();
						//return;
						throw new Exception("fb login error. api ret code = " + apiRetCode);
					}

					string sessionToken = parameters["session_token"];
					string userID = parameters["user_id"];

					//m_LoginAction = () => LoginAndLaunchClient(sessionToken, userID);

					Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
					if (driver == null)
					{
						AddUserResponse res = StationController.AddUser(userID, sessionToken);

						//UserStation station = GetPrimaryStation(res.Stations);
						//lblMainStationSetup.Text = string.Format(lblMainStationSetupText,
						//                                         (station == null) ? "None" : station.computer_name);
						//lblSecondStationSetup.Text = string.Format(lblSecondStationSetupText,
						//                                           (station == null) ? "None" : station.computer_name);

						////Show welcome msg
						//GotoTabPage(res.IsPrimaryStation ? tabMainStationSetup : tabSecondStationSetup);

						//if (!IsDisposed)
						//    Show();
						//return;	
					}

					return new UserSession { session_token = sessionToken, user_id = userID };

					//m_LoginAction();
					//return;
				}
				else
					throw new OperationCanceledException();
				//if (!IsDisposed)
				//    Show();
			//}
			//catch (AuthenticationException)
			//{
			//    MessageBox.Show(Resources.AuthError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//}
			//catch (StationServiceDownException)
			//{
			//    //if (!IsDisposed)
			//    //    Show();
			//    MessageBox.Show(Resources.StationServiceDown, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//}
			//catch (ConnectToCloudException)
			//{
			//    //if (!IsDisposed)
			//    //    Show();
			//    MessageBox.Show(Resources.ConnectCloudError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//}
			//catch (VersionNotSupportedException)
			//{
			//    throw new NotImplementedException();
			//    //handleVersionNotSupported();
			//}
			//catch (Exception)
			//{
			//    //if (!IsDisposed)
			//    //    Show();
			//    MessageBox.Show(Resources.UnknownSigninError, Resources.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//}
		}

		public UserSession Login(string email, string password)
		{
			AddUserResponse res = StationController.AddUser(email.ToLower(), password,
				StationRegistry.StationId, Environment.MachineName);


			var session = LoginToStation(email, password);

			return new UserSession { user_id = res.UserId, session_token = session.session_token };
		}

		private LoginedSession LoginToStation(string email, string password)
		{
			try
			{
				return User.LogIn(
					"http://localhost:9981/v2/",
					email,
					password,
					StationAPI.API_KEY,
					(string)StationRegistry.GetValue("stationId", string.Empty),
					Environment.MachineName).LoginedInfo;
			}
			catch (WammerCloudException e)
			{
				throw StationController.ExtractApiRetMsg(e);
			}
		}
	}
}
