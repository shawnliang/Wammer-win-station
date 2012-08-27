using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;

namespace Wammer.Station.Notify
{
	public class WebSocketNotifyChannels
	{
		private WebSocketServer<NotificationWebSocketService> wsServer;

		public event EventHandler<NotifyChannelEventArgs> ChannelAdded
		{
			add { NotificationWebSocketService.ChannelAdded += value; }
			remove { NotificationWebSocketService.ChannelAdded -= value; }
		}

		public event EventHandler<NotifyChannelEventArgs> ChannelRemoved
		{
			add { NotificationWebSocketService.ChannelRemoved += value; }
			remove { NotificationWebSocketService.ChannelRemoved -= value; }
		}

		public WebSocketNotifyChannels(int port)
		{
			var url = string.Format("ws://0.0.0.0:{0}", port);
			wsServer = new WebSocketServer<NotificationWebSocketService>(url);
		}

		public void Start()
		{
			wsServer.Start();
		}

		public void Stop()
		{
			wsServer.Stop();
		}

		public IEnumerable<INotifyChannel> GetChannelsByUser(string user_id)
		{
			return NotificationWebSocketService.GetChannels(user_id);
		}

		public void NotifyToUserChannels(string user_id, string exceptSessionToken)
		{
			var channels = GetChannelsByUser(user_id).Where(x => x.SessionToken != exceptSessionToken);
			foreach (var channel in channels)
				channel.Notify();
		}

		public void CloseChannel(INotifyChannel channel, WebSocketSharp.Frame.CloseStatusCode status, string reason)
		{
			if (channel is WebSocketNotifyChannel)
			{
				var wsChannel = (WebSocketNotifyChannel)channel;
				wsChannel.WSService.Stop(status, reason == null ? "" : reason);
				NotificationWebSocketService.RemoveChannel(wsChannel.WSService);
			}
		}
	}

}
 