using System;

namespace Wammer.Station.Notify
{
	public class NotifyChannelEventArgs : EventArgs
	{
		private WebSocketNotifyChannel channel;

		public NotifyChannelEventArgs(WebSocketNotifyChannel channel)
		{
			this.channel = channel;
		}

		public INotifyChannel Channel { get { return channel; } }
		public string UserId { get { return channel.UserId; } }
		public string ApiKey { get { return channel.ApiKey; } }
		public string SessionToken { get { return channel.SessionToken; } }
	}
}
