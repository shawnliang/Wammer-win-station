using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Waveface.Stream.Core
{
	[InheritedExport]
	public interface IWebSocketCommand
	{
		#region Property
		string Name { get; }
		#endregion


		#region Method
		Dictionary<string, Object> Execute(WebSocketCommandData data);
		Dictionary<string, Object> Execute(string commandName, Dictionary<string, Object> parameters = null, object memo = null);
		Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null, object memo = null);
		#endregion
	}
}
