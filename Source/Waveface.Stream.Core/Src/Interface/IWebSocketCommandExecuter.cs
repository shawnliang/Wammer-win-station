using System;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	public interface IWebSocketCommandExecuter
	{
		#region Method
		/// <summary>
		/// Determines whether the specified command name has command.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <returns></returns>
		Boolean HasCommand(string commandName);

		/// <summary>
		/// Executes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null);

		/// <summary>
		/// Executes the specified command name.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
		Dictionary<string, Object> Execute(string commandName, Dictionary<string, Object> parameters = null, object memo = null, Dictionary<string, Object> systemArgs = null);
		#endregion
	}
}
