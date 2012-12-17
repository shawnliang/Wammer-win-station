using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocketSharp.Server;


namespace Waveface.Stream.Core
{
	public class WebClientControlService : WebSocketService
	{
		#region Static Var
		private static IDictionary<string, WebSocketService> _services;
		#endregion


		#region Var
		IWebSocketCommandExecuter _webSocketCommandExecuter;
		#endregion


		#region Private Static Property
		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>
		/// The services.
		/// </value>
		private static IDictionary<string, WebSocketService> m_Services
		{
			get
			{
				return _services ?? (_services = new Dictionary<string, WebSocketService>());
			}
		}
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


		#region Public Static Property
		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>
		/// The services.
		/// </value>
		public static IEnumerable<WebSocketService> Services
		{
			get
			{
				return m_Services.Values;
			}
		}
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
		#endregion


		#region Protected Method
		/// <summary>
		/// Ons the open.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void onOpen(object sender, EventArgs e)
		{
			Trace.WriteLine(String.Format("WebSocket server open connection {0}...", this.ID));

			//StreamClient.Instance.LoginedUser.WebSocketChannelID = this.ID;

			if (!m_Services.ContainsKey(this.ID))
				m_Services.Add(this.ID, this);

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

			if (m_Services.ContainsKey(this.ID))
				m_Services.Remove(this.ID);

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


		#region Public Static Method
		/// <summary>
		/// Sends the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="data">The data.</param>
		public static void Send(String id, byte[] data)
		{
			if (!m_Services.ContainsKey(id))
				return;

			m_Services[id].Send(data);
		}

		/// <summary>
		/// Sends the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="data">The data.</param>
		public static void Send(String id, String data)
		{
			if (!m_Services.ContainsKey(id))
				return;

			m_Services[id].Send(data);
		}
		#endregion


		#region Event Process
		#endregion
	}
}
