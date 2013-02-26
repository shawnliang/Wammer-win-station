using System;
using System.Collections.Generic;
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
		public Billing billing { get; set; }
		public Quota quota { get; set; }
		public Usage usage { get; set; }

		public bool IsPaidUser()
		{
			return billing != null && billing.type.Equals("paid", StringComparison.InvariantCultureIgnoreCase);
		}
	}

	public class FindMyStationResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
	}

	public class Billing
	{
		public long cycle_start { get; set; }
		public string type { get; set; }
		public string plan { get; set; }
		public int cycle { get; set; }
	}

	public class Quota
	{
		public Volumn doc { get; set; }
		public Volumn image { get; set; }
		public Volumn total { get; set; }
	}

	public class Volumn
	{
		public int meta_files { get; set; }
		public int meta_size { get; set; }
		public int objects { get; set; }
		public int origin_files { get; set; }
		public int origin_size { get; set; }
	}

	public class Usage
	{
		public Volumn image { get; set; }
		public Volumn doc { get; set; }
	}
}