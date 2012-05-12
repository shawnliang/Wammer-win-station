using System;
using System.Diagnostics;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class UserLoginHandler : HttpHandler
	{
		public event EventHandler<UserLoginEventArgs> UserLogined;

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY);
			string apikey = Parameters[CloudServer.PARAM_API_KEY];

			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				string sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];

				try
				{
					string userId = Parameters[CloudServer.PARAM_USER_ID];

					LoginedSession loginInfo = User.GetLoginInfo(userId, apikey, sessionToken);
					LoginedSessionCollection.Instance.Save(loginInfo);

					RespondSuccess(loginInfo);

					OnUserLogined(new UserLoginEventArgs(loginInfo.user.email, loginInfo.session_token, apikey, userId));
				}
				catch (WammerCloudException e)
				{
					if (CloudServer.IsNetworkError(e))
					{
						LoginedSession sessionData = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));
						if (sessionData != null)
							RespondSuccess(sessionData);
						else
							throw;
					}
					else
						throw;
				}
			}
			else
			{
				CheckParameter(CloudServer.PARAM_EMAIL, CloudServer.PARAM_PASSWORD, CloudServer.PARAM_DEVICE_ID,
				               CloudServer.PARAM_DEVICE_NAME);

				string email = Parameters[CloudServer.PARAM_EMAIL];

				try
				{
					string password = Parameters[CloudServer.PARAM_PASSWORD];
					string deviceId = Parameters[CloudServer.PARAM_DEVICE_ID];
					string deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];
					User user = null;
					using (var client = new DefaultWebClient())
					{
						client.Timeout = 2500;
						client.ReadWriteTimeout = 2000;
						user = User.LogIn(client, email, password, apikey, deviceId, deviceName);
					}

					Debug.Assert(user != null, "user != null");
					LoginedSession loginInfo = user.LoginedInfo;

					LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", email));
					LoginedSessionCollection.Instance.Save(loginInfo);

					RespondSuccess(loginInfo);

					OnUserLogined(new UserLoginEventArgs(email, loginInfo.session_token, apikey, user.Id));
				}
				catch (WammerCloudException e)
				{
					if (CloudServer.IsNetworkError(e))
					{
						// network error, use existing session
						LoginedSession sessionData = LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", email));
						if (sessionData != null)
							RespondSuccess(sessionData);
						else
							throw;
					}
					else
						throw;
				}
			}
		}

		protected void OnUserLogined(UserLoginEventArgs arg)
		{
			EventHandler<UserLoginEventArgs> handler = UserLogined;
			if (handler != null)
			{
				handler(this, arg);
			}
		}

		#endregion

		#region Public Method

		public override object Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}

	[Serializable]
	public class UserLoginEventArgs : EventArgs
	{
		public UserLoginEventArgs(string email, string session_token, string apikey, string user_id)
		{
			this.email = email;
			this.session_token = session_token;
			this.apikey = apikey;
			this.user_id = user_id;
		}

		public string email { get; private set; }
		public string session_token { get; private set; }
		public string apikey { get; private set; }
		public string user_id { get; private set; }
	}
}