using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;

namespace Waveface.Stream.ClientFramework
{
	public interface IWebSocketCommandExecuter
	{
		#region Method
		/// <summary>
		/// Executes the specified command name.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
        Dictionary<string, Object> Execute(string commandName, Dictionary<string, Object> parameters = null);
		#endregion
	}
}
