using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;


namespace Waveface.Stream.ClientFramework
{
	/// <summary>
	/// 
	/// </summary>
	public class WebSocketCommandExecuter : IWebSocketCommandExecuter
	{
		#region Static Var
		private static IWebSocketCommandExecuter _instance;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ web socket commands.
		/// </summary>
		/// <value>The m_ web socket commands.</value>
		[ImportMany(typeof(IWebSocketCommand))]
		private IEnumerable<IWebSocketCommand> m_WebSocketCommands { get; set; }

		/// <summary>
		/// Gets or sets the m_ web socket pool.
		/// </summary>
		/// <value>The m_ web socket pool.</value>
		public Lazy<Dictionary<string, IWebSocketCommand>> m_WebSocketCommandPool { get; set; }
		#endregion



		#region Public Static Property
		public static IWebSocketCommandExecuter Instance
		{
			get
			{
				return _instance ?? (_instance = new WebSocketCommandExecuter());
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="WebSocketCommandExecuter"/> class.
		/// </summary>
		private WebSocketCommandExecuter()
		{
			m_WebSocketCommandPool = new Lazy<Dictionary<string, IWebSocketCommand>>(() =>
			{
				try
				{
					var catalog = new AssemblyCatalog(this.GetType().Assembly);

					var container = new CompositionContainer(catalog);
					container.ComposeParts(this);

					var webSocketPool = new Dictionary<string, IWebSocketCommand>();
					foreach (var webSocketCommand in m_WebSocketCommands)
					{
						webSocketPool.Add(webSocketCommand.Name, webSocketCommand);
					}
					return webSocketPool;
				}
				catch (Exception)
				{
					throw;
				}
			});
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public Dictionary<string, object> Execute(WebSocketCommandData data)
		{
			var webSocketCommandPool = m_WebSocketCommandPool.Value;

			if (webSocketCommandPool == null)
				return null;

			var commandName = data.CommandName;

			if (!webSocketCommandPool.ContainsKey(commandName))
				return null; //TODO: Throw unsupport command exception

			return webSocketCommandPool[commandName].Execute(data);
		}

		/// <summary>
		/// Executes the specified command name.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="memo"></param>
		/// <returns></returns>
		public Dictionary<string, object> Execute(string commandName, Dictionary<string, object> parameters = null, object memo = null)
		{
			return Execute(new WebSocketCommandData(commandName, parameters, memo));
		}
		#endregion
	}
}
