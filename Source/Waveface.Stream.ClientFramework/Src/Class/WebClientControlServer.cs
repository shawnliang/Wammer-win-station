using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp.Server;

namespace Waveface.Stream.ClientFramework
{
	public class WebClientControlServer
	{
		#region Const
		const string WEB_SOCKET_SERVER_IP_PATTERN = "ws://0.0.0.0:{0}";
		#endregion

		#region Static Var
        private static WebClientControlServer _instance;
        #endregion

		#region Var
		private WebSocketServer<WebClientControlService> _webSocketServer;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ web socket server.
		/// </summary>
		/// <value>The m_ web socket server.</value>
		private WebSocketServer<WebClientControlService> m_WebSocketServer
		{
			get
			{
				if (_webSocketServer == null)
				{
					_webSocketServer = new WebSocketServer<WebClientControlService>(string.Format(WEB_SOCKET_SERVER_IP_PATTERN, Port));
					_webSocketServer.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(WebSocketServer_OnError);
				}
				return _webSocketServer;
			}
		}
		#endregion


		#region Public Static Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static WebClientControlServer Instance
		{
			get
			{
				return _instance ?? (_instance = new WebClientControlServer());
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port { get; private set; }

		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>
		/// The services.
		/// </value>
		public IEnumerable<WebSocketService> Services
		{
			get
			{
				return WebClientControlService.Services;
			}
		}
		#endregion


		#region Constructor
		private WebClientControlServer()
		{
			this.Port = 1337;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			m_WebSocketServer.Start();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			m_WebSocketServer.Stop();
		}

		public void Send(String id, byte[] data)
		{
			WebClientControlService.Send(id, data);
		}

		/// <summary>
		/// Sends the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="data">The data.</param>
		public void Send(String id, String data)
		{
			WebClientControlService.Send(id, data);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the OnError event of the WebSocketServer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="WebSocketSharp.ErrorEventArgs" /> instance containing the event data.</param>
		void WebSocketServer_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			Trace.WriteLine(e.Message);
		}
		#endregion
	}
}
