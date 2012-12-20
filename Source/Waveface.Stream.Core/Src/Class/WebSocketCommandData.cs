using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class WebSocketCommandData
	{
		#region Var
		private Dictionary<string, object> _parameters;
		#endregion

		#region Public Method
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <value>
		/// The name of the command.
		/// </value>
		[JsonProperty("command", NullValueHandling = NullValueHandling.Ignore)]
		public String Command { get; private set; }

		/// <summary>
		/// Gets the params.
		/// </summary>
		/// <value>
		/// The params.
		/// </value>
		[JsonProperty("params", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, object> Parameters
		{
			get
			{
				return _parameters ?? (_parameters = new Dictionary<string, object>());
			}
			private set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Gets the memo.
		/// </summary>
		/// <value>
		/// The memo.
		/// </value>
		[JsonProperty("memo", NullValueHandling = NullValueHandling.Ignore)]
		public Object Memo { get; private set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="WebSocketCommandData" /> class.
		/// </summary>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="memo">The memo.</param>
		public WebSocketCommandData(string commandName, Dictionary<string, object> parameters, Object memo = null)
		{
			this.Command = commandName;
			this.Parameters = parameters;
			this.Memo = memo;
		}
		#endregion
	}
}
