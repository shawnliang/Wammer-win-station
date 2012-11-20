using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Waveface.Stream.ClientFramework
{
	public class WebClientControlService : WebSocketService
	{
		#region Var
		IWebSocketCommandExecuter _webSocketCommandExecuter;
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


		#region Constructor
		#endregion


        #region Private Method
        private void ExecuteAndResponseResult(string command, Dictionary<string, Object> parameters, object memo)
        {
            try
            {
                var response = m_WebSocketCommandExecuter.Execute(command, parameters);

                if (response == null)
                    return;

                var executedValue = new JObject(
                    new JProperty("command", command)
                    );

                if (response != null && response.Count > 0)
                    executedValue.Add(new JProperty("response", JObject.FromObject(response)));

                if (memo != null)
                    executedValue.Add(new JProperty("memo", memo));

                var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

                Trace.WriteLine(string.Format("Response to {0}: {1}", this.ID , responseMessage));
                Send(responseMessage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Command execute fail: {0}", ex.ToString()));
            }
        }

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

                ExecuteAndResponseResult(command, parameters, jObject["memo"]);
            }
            catch (Exception)
            {
                Trace.WriteLine("Invalid command format!!");
            }
        }

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
		#endregion
	}
}
