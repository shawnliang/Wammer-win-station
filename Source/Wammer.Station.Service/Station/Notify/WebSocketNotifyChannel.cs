﻿using Wammer.Utility;
using WebSocketSharp.Server;

namespace Wammer.Station.Notify
{
	public class WebSocketNotifyChannel : INotifyChannel
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

		public void Notify()
		{
			var notifyMsg = new GenericCommand
			{
				notify = new NotifyMsg { updated = true }
			};

			WSService.Send(notifyMsg.ToFastJSON());
		}

		public override string ToString()
		{
			return string.Format("WebSocketNotifyChannel - user: {0}, session_token: {1}, apikey: {2}", UserId, SessionToken, ApiKey);
		}

		public void Close(int closeCode, string reason)
		{
			var result = new GenericCommand
			{
				error = new ErrorMsg
				{
					code = closeCode,
					reason = reason
				}
			};

			WSService.Send(result.ToFastJSON());
			WSService.Stop((ushort)closeCode, reason);
		}
	}
}