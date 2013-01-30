using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Waveface.Stream.Core;
using WebSocketSharp;
using WebSocketSharp.Server;


namespace Waveface.Stream.ClientFramework
{
	public class WebClientControlService : WebSocketService
	{
		#region Var
		private IWebSocketCommandExecuter _webSocketCommandExecuter;
		private WebSocket _socketClient;
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

		/// <summary>
		/// Gets or sets the m_ socket client.
		/// </summary>
		/// <value>The m_ socket client.</value>
		private WebSocket m_SocketClient
		{
			get
			{
				lock (this)
				{
					if (_socketClient == null)
					{
						_socketClient = new WebSocket("ws://127.0.0.1:1338");
						_socketClient.OnError += new EventHandler<ErrorEventArgs>(_socketClient_OnError);
						_socketClient.OnMessage += new EventHandler<MessageEventArgs>(_socketClient_OnMessage);
						_socketClient.OnClose += new EventHandler<CloseEventArgs>(_socketClient_OnClose);

						_socketClient.Connect();
					}
					return _socketClient;
				}
			}
			set
			{
				lock (this)
				{
					if (_socketClient != null)
					{
						try
						{
							_socketClient.Dispose();
						}
						catch (Exception)
						{
						}
					}
					_socketClient = value;
				}
			}
		}
		#endregion

		#region Event
		public static event EventHandler ServiceAdded;
		public static event EventHandler ServiceRemoved;
		#endregion


		#region Constructor
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

				if (!m_WebSocketCommandExecuter.HasCommand(command))
				{
					//bypass command
					data.Parameters.Add("session_token", StreamClient.Instance.LoginedUser.SessionToken);
					var json = JsonConvert.SerializeObject(data, Formatting.Indented);
					m_SocketClient.Send(json);
					return;
				}

				var response = m_WebSocketCommandExecuter.Execute(data);

				if (response == null || response.Count == 0)
					return;

				var executedValue = new JObject(
					new JProperty("command", command),
					new JProperty("response", JObject.FromObject(response))
					);

				if (memo != null)
					executedValue.Add(new JProperty("memo", memo));

				var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

				LogManager.GetLogger(this.GetType()).DebugFormat("Response to {0}: {1}", this.ID, responseMessage);
				Send(responseMessage);
			}
			catch (Exception ex)
			{
				LogManager.GetLogger(this.GetType()).ErrorFormat("Command execute fail: {0}", ex.ToString());
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
					LogManager.GetLogger(this.GetType()).Error("Invalid websocket command format!!");
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
				LogManager.GetLogger(this.GetType()).Error("Invalid command format!!");
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
			LogManager.GetLogger(this.GetType()).DebugFormat("WebSocket server open connection {0}...", this.ID);

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
				LogManager.GetLogger(this.GetType()).DebugFormat("WebSocket server received message from {0}: {1}", this.ID, e.Data);

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
			LogManager.GetLogger(this.GetType()).DebugFormat("WebSocket server connection {0} close: {1}", this.ID, e.Reason);

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
			LogManager.GetLogger(this.GetType()).DebugFormat("WebSocket server connection {0} error: {1}", this.ID, e.Message);

			base.onError(sender, e);
		}
		#endregion

		#region Event Process
		void _socketClient_OnClose(object sender, CloseEventArgs e)
		{
			m_SocketClient = null;
		}

		void _socketClient_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Type == WebSocketSharp.Frame.Opcode.TEXT)
			{
				var responseMessage = e.Data;
				LogManager.GetLogger(this.GetType()).DebugFormat("Response to {0}: {1}", this.ID, responseMessage);
				Send(responseMessage);
			}
		}

		void _socketClient_OnError(object sender, ErrorEventArgs e)
		{
			m_SocketClient = null;
		}
		#endregion
	}
}
