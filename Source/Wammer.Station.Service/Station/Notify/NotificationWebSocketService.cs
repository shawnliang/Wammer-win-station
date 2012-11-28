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
		private WebSocketNotifyChannel channelInfo;

		private static List<WebSocketNotifyChannel> allChannels = new List<WebSocketNotifyChannel>();
		public static event EventHandler<NotifyChannelEventArgs> ChannelAdded;
		public static event EventHandler<NotifyChannelEventArgs> ChannelRemoved;

		protected override void onMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			base.onMessage(sender, e);

			try
			{
				if (e.Type != WebSocketSharp.Frame.Opcode.TEXT)
				{
					Stop(WebSocketSharp.Frame.CloseStatusCode.PROTOCOL_ERROR, "Not supported opcode by station");
					return;
				}

				var cmd = fastJSON.JSON.Instance.ToObject<GenericCommand>(e.Data);
				var connect = cmd.connect;
				if (connect != null && connect.IsValid())
				{
					this.channelInfo = new WebSocketNotifyChannel(this, connect.user_id, connect.session_token, connect.apikey);
					addChannel(channelInfo);
				}


				// TODO: move "import" to another websocket server which servers web clients
				var import = cmd.import;
				if (import != null)
				{
					var task = new ImportTask(import.apikey, import.session_token, import.group_id, import.files);


					task.FileImported += (s, args) => {
						try
						{
							var notify = new GenericCommand
							{
								file_imported = new FileImportedMsg
								{
									file = args.FilePath
								}
							};

							Send(notify.ToFastJSON());
						}
						catch (Exception ex)
						{
						}
					};

					task.ImportDone += (s, args) => {
						try
						{
							var notify = new GenericCommand
							{
								import_done = new ImportDoneMsg
								{
									Error = args.Error == null ? null : args.Error.Message
								}
							};

							Send(notify.ToFastJSON());
						}
						catch (Exception ex)
						{
						}
					};

					task.MetadataUploaded += (s, args) => {
						try
						{
							var notify = new GenericCommand
							{
								metadata_uploaded = new MetadataUploadedMsg
								{
									count = args.Count
								}
							};

							Send(notify.ToFastJSON());
						}
						catch (Exception ex)
						{
						}
					};
					TaskQueue.Enqueue(task, TaskPriority.VeryLow);
				}
			}
			catch (Exception ex)
			{
				this.LogErrorMsg("Error while processing web socket message: " + ex.ToString());
			}
		}

		protected override void onClose(object sender, WebSocketSharp.CloseEventArgs e)
		{
			base.onClose(sender, e);
			RemoveChannel(this);
		}

		protected override void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			base.onError(sender, e);
			this.LogWarnMsg("Web socket channel error: " + e.Message + " from :" + channelInfo.ToString() );
		}

		protected override void onOpen(object sender, EventArgs e)
		{
			base.onOpen(sender, e);
			this.LogDebugMsg("web socket connected");
		}

		public static void RemoveChannel(WebSocketService wsSvc)
		{
			lock (allChannels)
			{
				var channel = allChannels.Find((x) => x.WSService == wsSvc);
				if (channel != null)
				{
					allChannels.Remove(channel);
					OnChannelRemoved(channel);
				}
			}
		}

		private static void addChannel(WebSocketNotifyChannel channel)
		{
			lock (allChannels)
			{
				allChannels.Add(channel);
				OnChannelAdded(channel);
			}
		}

		private static void OnChannelAdded(WebSocketNotifyChannel channel)
		{
			var handler = ChannelAdded;
			if (handler != null)
				handler(channel.WSService, new NotifyChannelEventArgs(channel));
		}

		private static void OnChannelRemoved(WebSocketNotifyChannel channel)
		{
			var handler = ChannelRemoved;
			if (handler != null)
				handler(channel.WSService, new NotifyChannelEventArgs(channel));
		}

		public static IEnumerable<INotifyChannel> GetChannels(string user_id)
		{
			lock (allChannels)
			{
				return allChannels.Where(x => x.UserId.Equals(user_id)).Select(x => x as INotifyChannel);
			}
		}

		public static IEnumerable<INotifyChannel> GetChannels(Func<WebSocketNotifyChannel, bool> where)
		{
			lock (allChannels)
			{
				return allChannels.Where(where).Select(x => x as INotifyChannel);
			}
		}

		public static void ClearAllChannels()
		{
			lock (allChannels)
			{
				allChannels.Clear();
			}

			if (ChannelAdded != null)
			{
				foreach (Delegate d in ChannelAdded.GetInvocationList())
				{
					ChannelAdded -= (EventHandler<NotifyChannelEventArgs>)d;
				}
			}

			if (ChannelRemoved != null)
			{
				foreach (Delegate d in ChannelRemoved.GetInvocationList())
				{
					ChannelRemoved -= (EventHandler<NotifyChannelEventArgs>)d;
				}
			}
		}
	}
}
