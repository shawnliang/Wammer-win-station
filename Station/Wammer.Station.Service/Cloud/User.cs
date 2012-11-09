using System.Collections.Generic;
using fastJSON;
using Wammer.Model;

namespace Wammer.Cloud
{
	public class User
	{
		#region Public Property

		public string Name { get; private set; }
		public string Password { get; private set; }

		public string Token
		{
			get { return LoginedInfo.session_token; }
		}

		public List<UserGroup> Groups { get; private set; }
		public List<UserStation> Stations { get; private set; }

		public string Id
		{
			get { return LoginedInfo.user.user_id; }
		}

		public LoginedSession LoginedInfo { get; set; }

		#endregion

		private User(string username, string passwd, string json)
		{
			Name = username;
			Password = passwd;
			LoginedInfo = JSON.Instance.ToObject<LoginedSession>(json);

			var response = CloudServer.ConvertFromJson<UserLogInResponse>(json);
			Groups = response.groups;
			Stations = response.stations;
		}

		public static GetUserResponse GetInfo(string user_id, string apikey, string session_token)
		{
			return CloudServer.requestPath<GetUserResponse>("users/get",
			                                                new Dictionary<object, object>
			                                                	{
			                                                		{"user_id", user_id},
			                                                		{"apikey", apikey},
			                                                		{"session_token", session_token}
			                                                	}, false);
		}

		public static LoginedSession GetLoginInfo(string user_id, string apikey, string session_token)
		{
			return CloudServer.requestPath<LoginedSession>("users/get",
			                                               new Dictionary<object, object>
			                                               	{
			                                               		{"user_id", user_id},
			                                               		{"apikey", apikey},
			                                               		{"session_token", session_token}
			                                               	});
		}

		public static User LogIn(string username, string passwd, string deviceId, string deviceName)
		{
			return LogIn(username, passwd, CloudServer.APIKey, deviceId, deviceName);
		}

		public static User LogIn(string username, string passwd, string apiKey, string deviceId,
								 string deviceName, int timeout = -1)
		{
			string json = LogInResponse(CloudServer.BaseUrl, username, passwd, apiKey, deviceId, deviceName, timeout);
			return new User(username, passwd, json);
		}

		public static User LogIn(string baseURL, string username, string passwd, string apiKey,
		                         string deviceId, string deviceName, int timeout = -1)
		{
			string json = LogInResponse(baseURL, username, passwd, apiKey, deviceId, deviceName, timeout);
			return new User(username, passwd, json);
		}

		public static string LogInResponse(string serverBase, string username, string passwd, string apiKey,
		                                   string deviceId, string deviceName, int timeout = -1)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_EMAIL, username},
			                 		{CloudServer.PARAM_PASSWORD, passwd},
			                 		{CloudServer.PARAM_API_KEY, apiKey},
			                 		{CloudServer.PARAM_DEVICE_ID, deviceId},
			                 		{CloudServer.PARAM_DEVICE_NAME, deviceName}
			                 	};

			return CloudServer.requestPath(serverBase, "auth/login", parameters, false, true, timeout);
		}

		public static void LogOut(string sessionToken, string apiKey)
		{
			var parameters = new Dictionary<object, object>
			                 	{{CloudServer.PARAM_SESSION_TOKEN, sessionToken}, {CloudServer.PARAM_API_KEY, apiKey}};

			CloudServer.requestPath("auth/logout", parameters);
		}

		public static void LogOut(string sessionToken)
		{
			LogOut(sessionToken, CloudServer.APIKey);
		}

		public static FindMyStationResponse FindMyStation(string sessionToken)
		{
			var parameters = new Dictionary<object, object>
			                 	{{CloudServer.PARAM_API_KEY, CloudServer.APIKey}, {CloudServer.PARAM_SESSION_TOKEN, sessionToken}};

			return CloudServer.requestPath<FindMyStationResponse>("users/findMyStation", parameters);
		}

		public static void DisconnectWithSns(string session, string apikey, string sns)
		{
			var parameters = new Dictionary<object, object> 
			{ 
				{ CloudServer.PARAM_API_KEY, apikey }, 
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{ CloudServer.PARAM_SNS, sns },
				{ CloudServer.PARAM_PURGE_ALL, "no"}
			};

			CloudServer.requestPath<CloudResponse>("users/SNSDisconnect", parameters);
		}
	}
}