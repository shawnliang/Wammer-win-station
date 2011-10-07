using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace Wammer.Cloud
{
    public class UserSigninResponse
    {
        private int _status;
        private string _userToken;
        private string _uid;

        public UserSigninResponse()
        {
        }

        public string uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public int status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string userToken
        {
            get { return _userToken; }
            set { _userToken = value; }
        }
    }

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

        public static User SignIn(string username, string passwd, string apiKey)
        {
            WebClient http = new WebClient();
            string address = string.Format("http://{0}:{1}/api/v2/auth/login/email/{2}/password/{3}/apiKey/{4}",
                "127.0.0.1", 80, 
                HttpUtility.UrlEncode(username), 
                HttpUtility.UrlEncode(passwd), 
                HttpUtility.UrlEncode(apiKey));

            string response = http.DownloadString(address);
            UserSigninResponse res = fastJSON.JSON.Instance.ToObject<UserSigninResponse>(response);

            User user = new User(username, passwd);
            user.id = res.uid;
            user.token = res.userToken;
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
