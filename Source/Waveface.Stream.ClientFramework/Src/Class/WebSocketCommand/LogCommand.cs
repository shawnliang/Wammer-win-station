using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Waveface.Stream.Core;

namespace Waveface.Stream.ClientFramework
{
	[Obfuscation]
	public class LogCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "setLog"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			if (!LoginController.Instance.IsLogined)
				return null;

			var parameters = data.Parameters;

			LogManager.GetLogger(this.GetType()).DebugFormat("[{0}] {1}", parameters["type"], parameters["data"]);

			return null;
		}
		#endregion
	}
}
