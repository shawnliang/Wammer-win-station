using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class StationSignUpResponse : CloudResponse
	{
		private string _token;

		public StationSignUpResponse(int status, DateTime timestamp, string stationToken)
			: base(status, timestamp)
		{
			this._token = stationToken;
		}

		public StationSignUpResponse()
			: base()
		{
		}

		public string session_token
		{
			get { return _token; }
			set { _token = value; }
		}
	}

	public class StationLogOnResponse : CloudResponse
	{
		private string _token;

		public StationLogOnResponse()
			: base()
		{
		}

		public StationLogOnResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			_token = token;
		}

		public string session_token
		{
			get { return _token; }
			set { _token = value; }
		}
	}
}
