using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

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

		public static User LogIn(WebClient agent, string username, string passwd)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add(CloudServer.PARAM_EMAIL, username);
			parameters.Add(CloudServer.PARAM_PASSWORD, passwd);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			UserLogInResponse res = CloudServer.requestPath<UserLogInResponse>(
				agent, "auth/login", parameters);

			User user = new User(username, passwd);
			user.Token = res.session_token;
			user.Groups = res.groups;
			user.Id = res.user.user_id;
			user.Stations = res.stations;
			return user;
		}
	}
}
