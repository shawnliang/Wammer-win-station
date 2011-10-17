using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class UserLogInResponse : CloudResponse
	{
		private string _userToken;

		public UserLogInResponse()
			: base()
		{
		}

		public UserLogInResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			this._userToken = token;
		}

		public string session_token
		{
			get { return _userToken; }
			set { _userToken = value; }
		}
	}
}
