using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class UserLogInResponse : CloudResponse
    {
        private string _userToken;
        private string _uid;

        public UserLogInResponse()
            :base()
        {
        }

        public UserLogInResponse(StatusResponse status, string uid, string token)
            :base(status)
        {
            this._uid = uid;
            this._userToken = token;
        }

        public string uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public string user_token
        {
            get { return _userToken; }
            set { _userToken = value; }
        }
    }
}
