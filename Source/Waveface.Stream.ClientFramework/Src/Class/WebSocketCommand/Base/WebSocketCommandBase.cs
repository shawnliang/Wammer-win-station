﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Waveface.Stream.ClientFramework
{
	/// <summary>
	/// 
	/// </summary>
	[Obfuscation]
	public abstract class WebSocketCommandBase : IWebSocketCommand
	{
		#region Var
		private StreamClient _client;
		#endregion


		#region Protected Property
		/// <summary>
		/// Gets the m_ client.
		/// </summary>
		/// <value>The m_ client.</value>
		protected StreamClient m_Client
		{
			get
			{
				return _client ?? (_client = StreamClient.Instance);
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public abstract string Name { get; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value { get; protected set; }
		#endregion



		#region Protected Method
		protected void CheckParameters(Dictionary<string, Object> parameters, params string[] parameterNames)
		{
			if (parameterNames == null)
				throw new ArgumentNullException("parameterNames");

			var nullParamNames = from paramName in parameterNames
								 where !parameters.ContainsKey(paramName)
								 select paramName;

			if (!nullParamNames.Any())
				return;

			throw new FormatException(string.Format("Parameter {0} is null.", string.Join(",", nullParamNames.ToArray())));
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		public abstract Dictionary<string, Object> Execute(WebSocketCommandData data);

		/// <summary>
		/// Executes the specified command name.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="memo">The memo.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Dictionary<string, object> Execute(string commandName, Dictionary<string, object> parameters = null, object memo = null)
		{
			return Execute(new WebSocketCommandData(commandName, parameters, memo));
		}

		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="memo">The memo.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Dictionary<string, object> Execute(Dictionary<string, object> parameters = null, object memo = null)
		{
			return Execute(this.Name, parameters, memo);
		}
		#endregion
	}
}