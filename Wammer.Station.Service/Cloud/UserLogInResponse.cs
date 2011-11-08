using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class UserGroup
	{
		public string description { get; set; }
		public string creator_id { get; set; }
		public string group_id { get; set; }
		public string name { get; set; }
	}

	public class UserInfo
	{
		public string user_id { get; set; }
		public string avatar_url { get; set; }
		public string nickname { get; set; }
	}

	public class UserLogInResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public UserInfo user { get; set; }

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
