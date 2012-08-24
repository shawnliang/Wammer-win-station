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

		public event EventHandler<EventArgs> ChannelAdded
		{
			add { NotificationWebSocketService.ChannelAdded += value; }
			remove { NotificationWebSocketService.ChannelAdded -= value; }
		}

		public event EventHandler<EventArgs> ChannelRemoved
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
	}

}
