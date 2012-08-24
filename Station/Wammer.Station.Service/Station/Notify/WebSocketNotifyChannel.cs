using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;

namespace Wammer.Station.Notify
{
	class WebSocketNotifyChannel : INotifyChannel
	{
		public string UserId { get; private set; }
		public string SessionToken { get; private set; }
		public string ApiKey { get; private set; }
		public WebSocketService WSService { get; private set; }

		public WebSocketNotifyChannel(WebSocketService notifySvc, string user_id, string session_token, string apikey)
		{
			UserId = user_id;
			SessionToken = session_token;
			ApiKey = apikey;
			WSService = notifySvc;
		}
	}
}
