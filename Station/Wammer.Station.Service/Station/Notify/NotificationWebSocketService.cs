using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using Wammer.Utility;

namespace Wammer.Station.Notify
{
	public class NotificationWebSocketService : WebSocketService
	{
		private bool connected;
		private static List<WebSocketNotifyChannel> allChannels = new List<WebSocketNotifyChannel>();

		public static event EventHandler<EventArgs> ChannelAdded;
		public static event EventHandler<EventArgs> ChannelRemoved;

		protected override void onMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			base.onMessage(sender, e);

			if (e.Type != WebSocketSharp.Frame.Opcode.TEXT)
			{
				Stop(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, "Not supported opcode by station");
				return;
			}

			var cmd = fastJSON.JSON.Instance.ToObject<GenericCommand>(e.Data);
			var connect = cmd.connect;
			if (connect != null && connect.IsValid())
			{
				connected = true;
				addChannel(new WebSocketNotifyChannel(this, connect.user_id, connect.session_token, connect.apikey), this);
			}
		}

		protected override void onClose(object sender, WebSocketSharp.CloseEventArgs e)
		{
			base.onClose(sender, e);

			removeChannel(this);
		}

		private void removeChannel(WebSocketService wsSvc)
		{
			lock (allChannels)
			{
				var channel = allChannels.Find((x) => x.svc == wsSvc);
				allChannels.Remove(channel);

				OnChannelRemoved(this);
			}
		}

		private static void addChannel(WebSocketNotifyChannel channel, WebSocketService sender)
		{
			lock (allChannels)
			{
				allChannels.Add(channel);
				OnChannelAdded(sender);
			}
		}

		private static void OnChannelAdded(object sender)
		{
			var handler = ChannelAdded;
			if (handler != null)
				handler(sender, EventArgs.Empty);
		}

		private static void OnChannelRemoved(object sender)
		{
			var handler = ChannelRemoved;
			if (handler != null)
				handler(sender, EventArgs.Empty);
		}

		public static IEnumerable<INotifyChannel> GetChannels(string user_id)
		{
			lock (allChannels)
			{
				return allChannels.Where(x => x.UserId.Equals(user_id)).Select(x => x as INotifyChannel);
			}
		}

		public static void ClearAllChannels()
		{
			lock (allChannels)
			{
				allChannels.Clear();
			}
		}
	}
}
