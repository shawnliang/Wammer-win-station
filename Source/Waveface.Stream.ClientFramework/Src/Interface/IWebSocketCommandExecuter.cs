﻿using System;
using System.Collections.Generic;

namespace Waveface.Stream.ClientFramework
{
	public interface IWebSocketCommandExecuter
	{
		#region Method
		/// <summary>
		/// Executes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		Dictionary<string, Object> Execute(WebSocketCommandData data);

		/// <summary>
		/// Executes the specified command name.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
		Dictionary<string, Object> Execute(string commandName, Dictionary<string, Object> parameters = null, object memo = null);
		#endregion
	}
}