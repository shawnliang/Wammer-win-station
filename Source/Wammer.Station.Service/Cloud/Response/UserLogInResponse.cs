using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class UserLogInResponse : CloudResponse
	{
		public UserLogInResponse()
		{
		}

		public UserLogInResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			session_token = token;
		}

		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
	}

	public class GetUserResponse : CloudResponse
	{
		public GetUserResponse()
		{
		}

		public GetUserResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}

		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
	}

	public class FindMyStationResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
	}
}