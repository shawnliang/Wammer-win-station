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
        private string id;
        private string token;


        private User(string username, string passwd)
        {
            this.name = username;
            this.password = passwd;
        }

        public static User LogIn(WebClient agent, string username, string passwd)
        {
            string address = string.Format(
                "http://{0}:{1}/api/v2/auth/login/user_account/{2}/password/{3}/api_key/{4}",
                CloudServer.Address,
                CloudServer.Port,
                HttpUtility.UrlEncode(username), 
                HttpUtility.UrlEncode(passwd), 
                HttpUtility.UrlEncode(CloudServer.APIKey));

            string response = agent.DownloadString(address);
            UserLogInResponse res = fastJSON.JSON.Instance.ToObject<UserLogInResponse>(response);

            User user = new User(username, passwd);
            user.id = res.uid;
            user.token = res.user_token;
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

        public string Id
        {
            get { return this.id; }
        }

        public string Token
        {
            get { return this.token; }
        }
    }
}
