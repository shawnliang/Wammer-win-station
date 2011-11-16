using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class StationSignUpResponse : CloudResponse
	{
		public string session_token { get; set; }

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

		public StationLogOnResponse()
			: base()
		{
		}

		public StationLogOnResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			session_token = token;
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
