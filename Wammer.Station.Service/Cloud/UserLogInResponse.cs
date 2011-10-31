using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class UserLogInResponse : CloudResponse
	{
		public string session_token { get; set; }

		public UserLogInResponse()
			: base()
		{
		}

		public UserLogInResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			this.session_token = token;
		}
	}
}
