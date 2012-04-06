using System.Collections.Generic;
using System.Net;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Cloud
{
	public class User
	{

		public string Name { get; private set; }
		public string Password { get; private set; }
		public string Token { get; private set; }
		public List<UserGroup> Groups { get; private set; }
		public List<UserStation> Stations { get; private set; }
		public string Id { get; private set; }

		private User(string username, string passwd)
		{
			this.Name = username;
			this.Password = passwd;
		}

		public static User LogIn(WebClient agent, string username, string passwd, string apiKey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add(CloudServer.PARAM_EMAIL, username);
			parameters.Add(CloudServer.PARAM_PASSWORD, passwd);
			parameters.Add(CloudServer.PARAM_API_KEY, apiKey);

			var json = CloudServer.requestPath(agent, "auth/login", parameters);

			LoginedSessionCollection.Instance.Save(fastJSON.JSON.Instance.ToObject<LoginedSession>(json));

			UserLogInResponse res = CloudServer.ConvertFromJson<UserLogInResponse>(json);

			User user = new User(username, passwd);
			user.Token = res.session_token;
			user.Groups = res.groups;
			user.Id = res.user.user_id;
			user.Stations = res.stations;
			return user;
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

			var json = CloudServer.requestPath(agent, "auth/logout", parameters);

			LoginedSessionCollection.Instance.Remove(Query.EQ("_id", sessionToken));
		}

		public static void LogOut(WebClient agent, string sessionToken)
		{
			LogIn(agent, sessionToken,CloudServer.APIKey);
		}
	}
}
