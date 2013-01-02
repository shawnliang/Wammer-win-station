using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using System.Linq;


namespace Waveface.Stream.Core
{
	public class WebClientControlService : WebSocketService
	{
		#region Var
		IWebSocketCommandExecuter _webSocketCommandExecuter;
		private Dictionary<SystemEventType, WebSocketCommandData> _subscribedEvents;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ web socket command executer.
		/// </summary>
		/// <value>The m_ web socket command executer.</value>
		private IWebSocketCommandExecuter m_WebSocketCommandExecuter
		{
			get
			{
				return _webSocketCommandExecuter ?? (_webSocketCommandExecuter = WebSocketCommandExecuter.Instance);
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the subscribed system event.
		/// </summary>
		/// <value>
		/// The subscribed system event.
		/// </value>
		public Dictionary<SystemEventType, WebSocketCommandData> SubscribedEvents
		{
			get
			{
				return _subscribedEvents ?? (_subscribedEvents = new Dictionary<SystemEventType, WebSocketCommandData>());
			}
		}
		#endregion


		#region Event
		public static event EventHandler ServiceAdded;
		public static event EventHandler ServiceRemoved;
		#endregion


		#region Constructor
		public WebClientControlService()
		{
			var eventSubscriber = SystemEventSubscriber.Instance;
			eventSubscriber.EventSubscribed += WebClientControlService_EventSubscribed;
			eventSubscriber.EventUnSubscribed += WebClientControlService_EventUnSubscribed;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Executes the and response result.
		/// </summary>
		/// <param name="data">The data.</param>
		private void ExecuteAndResponseResult(WebSocketCommandData data)
		{
			try
			{
				var command = data.Command;
				var memo = data.Memo;

				var systemArgs = new Dictionary<string, object>() 
				{
					{"ChannelID", this.ID}
				};

				var response = m_WebSocketCommandExecuter.Execute(data, systemArgs);

				if (response == null || response.Count == 0)
					return;

				var executedValue = new JObject(
					new JProperty("command", command),
					new JProperty("response", JObject.FromObject(response))
					);

				if (memo != null)
					executedValue.Add(new JProperty("memo", memo));

				var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

				Trace.WriteLine(string.Format("Response to {0}: {1}", this.ID, responseMessage));
				Send(responseMessage);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Command execute fail: {0}", ex.ToString()));
			}
		}

		/// <summary>
		/// Parses the and execute command.
		/// </summary>
		/// <param name="json">The json.</param>
		private void ParseAndExecuteCommand(string json)
		{
			if (json.Length == 0)
				return;

			try
			{
				var jObject = JObject.Parse(json);

				if (jObject["command"] == null)
				{
					Trace.WriteLine("Invalid websocket command format!!");
					return;
				}

				var command = jObject["command"].ToString();
				var commandParameters = jObject["params"];

				var parameters = new Dictionary<string, object>();
				if (commandParameters != null && commandParameters.HasValues)
				{
					foreach (JProperty commandParameter in commandParameters)
					{
						parameters.Add(commandParameter.Name, commandParameter.Value);
					}
				}

				ExecuteAndResponseResult(new WebSocketCommandData(command, parameters, jObject["memo"]));
			}
			catch (Exception)
			{
				Trace.WriteLine("Invalid command format!!");
			}
		}

		/// <summary>
		/// Asyncs the parse and execute command.
		/// </summary>
		/// <param name="json">The json.</param>
		private void AsyncParseAndExecuteCommand(string json)
		{
			if (json.Length == 0)
				return;

			(new Task(() =>
			{
				ParseAndExecuteCommand(json);
			})).Start();
		}

		private void UpdateSystemEventBinding()
		{
			var eventSubscriber = SystemEventSubscriber.Instance;
			foreach (SystemEventType eventType in Enum.GetValues(typeof(SystemEventType)))
			{
				switch (eventType)
				{
					case SystemEventType.PostAdded:
						eventSubscriber.PostAdded -= Instance_PostAdded;
						break;
					case SystemEventType.PostUpdated:
						eventSubscriber.PostUpdated -= Instance_PostUpdated;
						break;
					case SystemEventType.AttachmentAdded:
						eventSubscriber.AttachmentAdded -= Instance_AttachmentAdded;
						break;
					case SystemEventType.AttachmentUpdated:
						eventSubscriber.AttachmentUpdated -= Instance_AttachmentUpdated;
						break;
					case SystemEventType.AttachmentArrived:
						eventSubscriber.AttachmentArrived -= Instance_AttachmentArrived;
						break;
					case SystemEventType.CollectionAdded:
						eventSubscriber.CollectionAdded -= Instance_CollectionAdded;
						break;
					case SystemEventType.CollectionUpdated:
						eventSubscriber.CollectionUpdated -= Instance_CollectionUpdated;
						break;
					default:
						break;
				}
			}

			foreach (var eventType in SubscribedEvents.Keys)
			{
				switch (eventType)
				{
					case SystemEventType.PostAdded:
						eventSubscriber.PostAdded += Instance_PostAdded;
						break;
					case SystemEventType.PostUpdated:
						eventSubscriber.PostUpdated += Instance_PostUpdated;
						break;
					case SystemEventType.AttachmentAdded:
						eventSubscriber.AttachmentAdded += Instance_AttachmentAdded;
						break;
					case SystemEventType.AttachmentUpdated:
						eventSubscriber.AttachmentUpdated += Instance_AttachmentUpdated;
						break;
					case SystemEventType.AttachmentArrived:
						eventSubscriber.AttachmentArrived += Instance_AttachmentArrived;
						break;
					case SystemEventType.CollectionAdded:
						eventSubscriber.CollectionAdded += Instance_CollectionAdded;
						break;
					case SystemEventType.CollectionUpdated:
						eventSubscriber.CollectionUpdated += Instance_CollectionUpdated;
						break;
					default:
						break;
				}
			}
		}
		#endregion


		#region Protected Method
		protected virtual void OnServiceAdded(EventArgs e)
		{
			if (ServiceAdded == null)
				return;

			ServiceAdded(this, e);
		}

		protected virtual void OnServiceRemoved(EventArgs e)
		{
			if (ServiceRemoved == null)
				return;

			ServiceRemoved(this, e);
		}

		/// <summary>
		/// Ons the open.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void onOpen(object sender, EventArgs e)
		{
			Trace.WriteLine(String.Format("WebSocket server open connection {0}...", this.ID));

			OnServiceAdded(EventArgs.Empty);

			base.onOpen(sender, e);
		}

		/// <summary>
		/// Ons the message.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="WebSocketSharp.MessageEventArgs"/> instance containing the event data.</param>
		protected override void onMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
			{
				Trace.WriteLine(string.Format("WebSocket server received message from {0}: {1}", this.ID, e.Data));

				AsyncParseAndExecuteCommand(e.Data);
			}
		}

		/// <summary>
		/// Ons the close.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="WebSocketSharp.CloseEventArgs"/> instance containing the event data.</param>
		protected override void onClose(object sender, WebSocketSharp.CloseEventArgs e)
		{
			Trace.WriteLine(String.Format("WebSocket server connection {0} close: {1}", this.ID, e.Reason));

			OnServiceRemoved(EventArgs.Empty);

			base.onClose(sender, e);
		}

		/// <summary>
		/// Ons the error.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="WebSocketSharp.ErrorEventArgs"/> instance containing the event data.</param>
		protected override void onError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			Trace.WriteLine(string.Format("WebSocket server connection {0} error: {1}", this.ID, e.Message));
			base.onError(sender, e);
		}
		#endregion


		#region Event Process
		void WebClientControlService_EventUnSubscribed(object sender, SystemEventSubscribeEventArgs e)
		{
			if (!e.WebSocketChannelID.Equals(this.ID, StringComparison.CurrentCultureIgnoreCase))
				return;

			try
			{
				var subscribedEvents = SubscribedEvents;
				var eventType = e.EventType;
				var data = e.Data;

				if (eventType == SystemEventType.All)
				{
					data.Parameters.Clear();
					foreach (SystemEventType systemEventType in Enum.GetValues(typeof(SystemEventType)))
					{
						if (subscribedEvents.ContainsKey(systemEventType))
							subscribedEvents.Remove(systemEventType);
					}
					return;
				}

				if (subscribedEvents.ContainsKey(eventType))
					subscribedEvents.Remove(eventType);
			}
			finally
			{
				UpdateSystemEventBinding();
			}
		}

		void WebClientControlService_EventSubscribed(object sender, SystemEventSubscribeEventArgs e)
		{
			if (!e.WebSocketChannelID.Equals(this.ID, StringComparison.CurrentCultureIgnoreCase))
				return;

			try
			{
				var subscribedEvents = SubscribedEvents;
				var eventType = e.EventType;
				var data = e.Data;

				if (eventType == SystemEventType.All)
				{
					data.Parameters.Clear();
					foreach (SystemEventType systemEventType in Enum.GetValues(typeof(SystemEventType)))
					{
						if (subscribedEvents.ContainsKey(systemEventType))
							subscribedEvents.Remove(systemEventType);

						subscribedEvents.Add(systemEventType, data);
					}
					return;
				}

				if (subscribedEvents.ContainsKey(eventType))
					subscribedEvents.Remove(eventType);

				subscribedEvents.Add(eventType, data);
			}
			finally
			{
				UpdateSystemEventBinding();
			}
		}


		void Instance_CollectionUpdated(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.CollectionUpdated;

			ProcessEvent(eventType, "getCollections", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_CollectionAdded(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.CollectionAdded;

			ProcessEvent(eventType, "getCollections", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_AttachmentUpdated(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.AttachmentAdded;

			ProcessEvent(eventType, "getAttachments", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_AttachmentAdded(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.AttachmentUpdated;

			ProcessEvent(eventType, "getAttachments", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_AttachmentArrived(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.AttachmentArrived;

			ProcessEvent(eventType, "getAttachments", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_PostUpdated(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.PostUpdated;

			ProcessEvent(eventType, "getPosts", (e.Data as IEnumerable<string>).ToArray());
		}

		void Instance_PostAdded(object sender, SystemEventEventArgs e)
		{
			var eventType = SystemEventType.PostAdded;

			ProcessEvent(eventType, "getPosts", (e.Data as IEnumerable<string>).ToArray());
		}


		private void ProcessEvent(SystemEventType eventType, string command, params string[] ids)
		{
			if (!SubscribedEvents.ContainsKey(eventType))
				return;

			var commandData = SubscribedEvents[eventType];
			var parameters = commandData.Parameters;

			if (parameters == null)
				parameters = new Dictionary<string, object>();

			if (parameters.ContainsKey("post_id_array"))
				parameters.Remove("post_id_array");

			parameters.Add("post_id_array", new JArray(ids));

			if (parameters.ContainsKey("page_size"))
				parameters.Remove("page_size");

			parameters.Add("page_size", ids.Length);

			var response = WebSocketCommandExecuter.Instance.Execute(command, parameters);

			var responseParams = new Dictionary<String, Object>(response)
				{
					{"event_id", (int)eventType}
				};

			responseParams.Remove("page_no");
			responseParams.Remove("page_size");
			responseParams.Remove("page_count");
			responseParams.Remove("total_count");


			var executedValue = new JObject(
					new JProperty("command", "subscribeEvent"),
					new JProperty("response", JObject.FromObject(responseParams))
					);

			var memo = commandData.Memo;
			if (memo != null)
				executedValue.Add(new JProperty("memo", memo));

			var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

			Send(responseMessage);
		}
		#endregion
	}
}
