using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Waveface.Stream.ClientFramework
{
	/// <summary>
	/// 
	/// </summary>
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
        public abstract Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null);
		#endregion
	}
}
