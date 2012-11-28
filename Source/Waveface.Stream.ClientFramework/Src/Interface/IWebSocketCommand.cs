using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Waveface.Stream.ClientFramework
{
	[InheritedExport]
	public interface IWebSocketCommand
	{
		#region Property
		string Name { get; }
		#endregion


		#region Method
        Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null);
		#endregion
	}
}
