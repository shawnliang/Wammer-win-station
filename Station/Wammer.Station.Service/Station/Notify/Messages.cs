using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Notify
{

	// See https://github.com/waveface/Wammer-station/wiki/Station-Websocket-APIs


	public class ConnectMsg
	{
		public string session_token { get; set; }
		public string apikey { get; set; }
		public string user_id { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(session_token) &&
				!string.IsNullOrEmpty(apikey) &&
				!string.IsNullOrEmpty(user_id);
		}
	}

	public class SubscribeMSg
	{
	}

	public class ErrorMsg
	{
		public int code { get; set; }
		public string reason { get; set; }
	}

	public class NotifyMsg
	{
		public bool updated { get; set; }
		public string message { get; set; }
	}

	public class GenericCommand
	{
		public ErrorMsg error { get; set; }
		public ConnectMsg connect { get; set; }
		public SubscribeMSg subscribe { get; set; }
		public NotifyMsg notify { get; set; }
	}

	public enum ErrorCode
	{
		AccessDenied = 3001,
	}
}
