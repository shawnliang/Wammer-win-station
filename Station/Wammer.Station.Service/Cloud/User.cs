using System.Collections.Generic;
using System.Net;
using Wammer.Model;
using MongoDB.Driver.Builders;

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

		public static User LogIn(WebClient agent, string username, string passwd, string apiKey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add(CloudServer.PARAM_EMAIL, username);
			parameters.Add(CloudServer.PARAM_PASSWORD, passwd);
			parameters.Add(CloudServer.PARAM_API_KEY, apiKey);

			var json = CloudServer.requestPath(agent, "auth/login", parameters);

			return new User(username, passwd, json);
		}

		public static User LogIn(WebClient agent, string username, string passwd)
		{
			return LogIn(agent, username, passwd, CloudServer.APIKey);
		}

		public static void LogOut(WebClient agent, string sessionToken, string apiKey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, sessionToken);
			parameters.Add(CloudServer.PARAM_API_KEY, apiKey);

			CloudServer.requestPath(agent, "auth/logout", parameters);
		}

		public static void LogOut(WebClient agent, string sessionToken)
		{
			LogOut(agent, sessionToken, CloudServer.APIKey);
		}
	}
}
