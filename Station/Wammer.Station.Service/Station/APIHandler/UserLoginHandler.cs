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
		#region Var
		private DriverController _driverAgent;
		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the m_ station ID.
		/// </summary>
		/// <value>The m_ station ID.</value>
		private String m_StationID { get; set; }

		/// <summary>
		/// Gets the m_ driver agent.
		/// </summary>
		/// <value>The m_ driver agent.</value>
		private DriverController m_DriverAgent
		{
			get { return _driverAgent ?? (_driverAgent = new DriverController()); }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="UserLoginHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		public UserLoginHandler(string stationId)
		{
			m_StationID = stationId;
		} 
		#endregion

		public event EventHandler<UserLoginEventArgs> UserLogined;

		private void CheckAndUpdateDriver(LoginedSession loginInfo)
		{
			if (loginInfo == null)
				return;

			var user = loginInfo.user;

			Debug.Assert(user != null, "user != null");

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));
			
			if (driver != null)
				return;

			driver = DriverCollection.Instance.FindOne(Query.EQ("email", user.email));

			if(driver == null)
				throw new WammerStationException("Driver not existed", (int)StationLocalApiError.NotFound);

			m_DriverAgent.RemoveDriver(m_StationID, user.user_id);

			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				var sessionToken = Parameters[CloudServer.SessionToken];
				var userID = Parameters[CloudServer.PARAM_USER_ID];

				m_DriverAgent.AddDriver("", m_StationID, userID, sessionToken);
			}
			else
			{
				CheckParameter(CloudServer.PARAM_EMAIL,
							   CloudServer.PARAM_PASSWORD,
							   CloudServer.PARAM_DEVICE_ID,
							   CloudServer.PARAM_DEVICE_NAME);

				var email = Parameters[CloudServer.PARAM_EMAIL];
				var password = Parameters[CloudServer.PARAM_PASSWORD];
				var deviceId = Parameters[CloudServer.PARAM_DEVICE_ID];
				var deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];

				m_DriverAgent.AddDriver("", m_StationID, email, password, deviceId, deviceName);
			}
		}

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY);
			var apikey = Parameters[CloudServer.PARAM_API_KEY];

			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];

				try
				{
					var userId = Parameters[CloudServer.PARAM_USER_ID];

					var loginInfo = User.GetLoginInfo(userId, apikey, sessionToken);
					CheckAndUpdateDriver(loginInfo);

					LoginedSessionCollection.Instance.Save(loginInfo);

					RespondSuccess(loginInfo);

					OnUserLogined(new UserLoginEventArgs(loginInfo.user.email, loginInfo.session_token, apikey, userId));
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
					var deviceId = Parameters[CloudServer.PARAM_DEVICE_ID];
					var deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];
					var user = User.LogIn(email, password, apikey, deviceId, deviceName, 2500);
					

					Debug.Assert(user != null, "user != null");
					var loginInfo = user.LoginedInfo;

					CheckAndUpdateDriver(loginInfo);

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