using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Diagnostics;

namespace Waveface.Stream.ClientFramework
{
	public class WebClientControlServer
	{
		#region Const
		const string WEB_SOCKET_SERVER_IP_PATTERN = "ws://0.0.0.0:{0}";
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
					_webSocketServer.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(_webSocketServer_OnError);
				}
				return _webSocketServer;
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port { get; private set; }
		#endregion


		#region Constructor
        public WebClientControlServer(int port)
		{
			this.Port = port;
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
		#endregion


		#region Event Process
		void _webSocketServer_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			Trace.WriteLine(e.Message);
		}
		#endregion
	}
}
