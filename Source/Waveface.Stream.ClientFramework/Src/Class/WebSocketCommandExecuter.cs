using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition.Primitives;
using Waveface.Stream.Core;


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
		private Dictionary<string, IWebSocketCommand> m_WebSocketCommandPool { get; set; }
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
			m_WebSocketCommandPool = new Dictionary<string, IWebSocketCommand>();

			try
			{
				var catalog = new AssemblyCatalog(this.GetType().Assembly);

				var container = new CompositionContainer(catalog);
				container.ComposeParts(this);

				foreach (var webSocketCommand in m_WebSocketCommands)
				{
					m_WebSocketCommandPool.Add(webSocketCommand.Name, webSocketCommand);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Determines whether the specified command name has command.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool HasCommand(string commandName)
		{
			return m_WebSocketCommandPool.ContainsKey(commandName);
		}

		/// <summary>
		/// Executes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public Dictionary<string, object> Execute(WebSocketCommandData data)
		{
			var webSocketCommandPool = m_WebSocketCommandPool;

			if (webSocketCommandPool == null)
				return null;

			var commandName = data.Command;

			if (!HasCommand(commandName))
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
