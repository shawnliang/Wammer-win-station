using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace Wammer.Cloud
{
	public class User
	{
		private string name;
		private string password;
		private string token;


		private User(string username, string passwd)
		{
			this.name = username;
			this.password = passwd;
		}

		public static User LogIn(WebClient agent, string username, string passwd)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>();
			parameters.Add("email", username);
			parameters.Add("password", passwd);
			parameters.Add("api_key", CloudServer.APIKey);

			UserLogInResponse res = CloudServer.requestPath<UserLogInResponse>(
				agent, "auth/login", parameters);

			User user = new User(username, passwd);
			user.token = res.session_token;
			return user;
		}

		public string Name
		{
			get { return name; }
		}

		public string Password
		{
			get { return password; }
		}

		public string Token
		{
			get { return this.token; }
		}
	}
}
