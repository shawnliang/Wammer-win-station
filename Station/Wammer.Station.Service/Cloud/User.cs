using System.Collections.Generic;
using System.Net;
using Wammer.Model;
using MongoDB.Driver.Builders;
using Wammer.Utility;

namespace Wammer.Cloud
{
	public class User
	{
		#region Public Property
		public string Name { get; private set; }
		public string Password { get; private set; }
		public string Token
		{
			get 
			{
				return LoginedInfo.session_token;
			}
		}

		public List<UserGroup> Groups { get; private set; }
		public List<UserStation> Stations { get; private set; }

		public string Id
		{
			get
			{
				return LoginedInfo.user.user_id;
			}
		}

		public LoginedSession LoginedInfo { get; set; }
		#endregion


		private User(string username, string passwd, string json)
		{
			this.Name = username;
			this.Password = passwd;
			this.LoginedInfo = fastJSON.JSON.Instance.ToObject<LoginedSession>(json);

			var response = CloudServer.ConvertFromJson<UserLogInResponse>(json);
			this.Groups = response.groups;
			this.Stations = response.stations;
		}

		public static GetUserResponse GetInfo(string user_id, string apikey, string session_token)
		{
			using (WebClient agent = new DefaultWebClient())
			{
				return CloudServer.requestPath<GetUserResponse>(agent, "users/get",
					new Dictionary<object, object>{
									   {"user_id", user_id},
									   {"apikey", apikey},
									   {"session_token", session_token}
					}, false);
			}
		}

		public static LoginedSession GetLoginInfo(string user_id, string apikey, string session_token)
		{
			using (WebClient agent = new DefaultWebClient())
			{
				return CloudServer.requestPath<LoginedSession>(agent, "users/get",
					new Dictionary<object, object>{
									   {"user_id", user_id},
									   {"apikey", apikey},
									   {"session_token", session_token}
					});
			}
		}

		public static User LogIn(WebClient agent, string username, string passwd, string deviceId, string deviceName)
		{
			return LogIn(agent, username, passwd, CloudServer.APIKey, deviceId, deviceName);
		}

		public static User LogIn(WebClient agent, string username, string passwd, string apiKey, string deviceId, string deviceName)
		{
			var json = LogInResponse(agent, CloudServer.BaseUrl, username, passwd, apiKey, deviceId, deviceName);
			return new User(username, passwd, json);
		}

		public static User LogIn(WebClient agent, string baseURL, string username, string passwd, string apiKey, string deviceId, string deviceName)
		{
			var json = LogInResponse(agent, baseURL, username, passwd, apiKey, deviceId, deviceName);
			return new User(username, passwd, json);
		}

		public static string LogInResponse(WebClient agent, string serverBase, string username, string passwd, string apiKey, string deviceId, string deviceName)
		{
			var parameters = new Dictionary<object, object>
			                                        	{
			                                        		{CloudServer.PARAM_EMAIL, username},
			                                        		{CloudServer.PARAM_PASSWORD, passwd},
			                                        		{CloudServer.PARAM_API_KEY, apiKey},
			                                        		{CloudServer.PARAM_DEVICE_ID, deviceId},
			                                        		{CloudServer.PARAM_DEVICE_NAME, deviceName}
			                                        	};

			return CloudServer.requestPath(agent, serverBase, "auth/login", parameters, false);
		}

		public static void LogOut(WebClient agent, string sessionToken, string apiKey)
		{
			var parameters = new Dictionary<object, object>
			                 	{{CloudServer.PARAM_SESSION_TOKEN, sessionToken}, {CloudServer.PARAM_API_KEY, apiKey}};

			CloudServer.requestPath(agent, "auth/logout", parameters);
		}

		public static void LogOut(WebClient agent, string sessionToken)
		{
			LogOut(agent, sessionToken, CloudServer.APIKey);
		}

		public static FindMyStationResponse FindMyStation(WebClient agent, string sessionToken)
		{
			var parameters = new Dictionary<object, object>
			                 	{{CloudServer.PARAM_API_KEY, CloudServer.APIKey}, {CloudServer.PARAM_SESSION_TOKEN, sessionToken}};

			return CloudServer.requestPath<FindMyStationResponse>(agent, "users/findMyStation", parameters);
		}
	}
}
