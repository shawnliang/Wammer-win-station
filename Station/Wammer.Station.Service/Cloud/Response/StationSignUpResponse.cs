using System;
using System.Collections.Generic;

namespace Wammer.Cloud
{
	public class StationSignUpResponse : CloudResponse
	{
		public string session_token { get; set; }
		public UserStation station { get; set; }
		public StationSignUpResponse(int status, DateTime timestamp, string stationToken)
			: base(status, timestamp)
		{
			this.session_token = stationToken;
		}

		public StationSignUpResponse()
			: base()
		{
		}
	}

	public class StationLogOnResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }

		public StationLogOnResponse()
			: base()
		{
		}

		public StationLogOnResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}
	}

	public class StationHeartbeatResponse : CloudResponse
	{
		public string session_token { get; set; }

		public StationHeartbeatResponse()
			: base()
		{
		}

		public StationHeartbeatResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			session_token = token;
		}
	}
}
