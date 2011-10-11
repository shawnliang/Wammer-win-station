using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class UserLogInResponse
    {
        private StatusResponse _status;
        private string _userToken;
        private string _uid;

        public UserLogInResponse()
        {
        }

        public UserLogInResponse(StatusResponse status, string uid, string token)
        {
            this._status = status;
            this._uid = uid;
            this._userToken = token;
        }

        public string uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public StatusResponse response
        {
            get { return _status; }
            set { _status = value; }
        }

        public string user_token
        {
            get { return _userToken; }
            set { _userToken = value; }
        }
    }
}
