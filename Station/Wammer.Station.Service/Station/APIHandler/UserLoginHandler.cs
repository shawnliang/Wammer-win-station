using System;
using System.Diagnostics;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/auth/login/")]
	public class UserLoginHandler : HttpHandler
	{
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			if (CloudServer.VersionNotCompatible)
				throw new WammerStationException("Version not supported", (int)GeneralApiError.NotSupportClient);

			CheckParameter(CloudServer.PARAM_API_KEY);
			var apikey = Parameters[CloudServer.PARAM_API_KEY];

			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
				try
				{
					var userID = Parameters[CloudServer.PARAM_USER_ID];

					var loginInfo = Station.Instance.Login(apikey, sessionToken, userID);
					RespondSuccess(loginInfo);
				}
				catch (WammerCloudException e)
				{
					if (CloudServer.IsNetworkError(e))
					{
						var sessionData = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));
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

				var email = Parameters[CloudServer.PARAM_EMAIL];
				try
				{
					var password = Parameters[CloudServer.PARAM_PASSWORD];
					var deviceID = Parameters[CloudServer.PARAM_DEVICE_ID];
					var deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];

					var loginInfo = Station.Instance.Login(apikey, email, password, deviceID, deviceName);

					RespondSuccess(loginInfo);
				}
				catch (WammerCloudException e)
				{
					if (CloudServer.IsNetworkError(e))
					{
						// network error, use existing session
						var sessionData = LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", email));
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
