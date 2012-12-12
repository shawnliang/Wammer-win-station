using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
	public class CommandLineCommand
	{
		#region Property
		public string Name
		{
			get;
			private set;
		}
		public string ShortForm
		{
			get;
			private set;
		}
		public Action<IEnumerable<string>> Handler
		{
			get;
			private set;
		}
		#endregion

		#region Constructor
		public CommandLineCommand(string name, Action<IEnumerable<string>> handler, string shortForm)
		{
			Name = name;
			Handler = handler;
			ShortForm = shortForm;
		}

		public CommandLineCommand(string name, Action<IEnumerable<string>> handler)
		{
			Name = name;
			Handler = handler;
			ShortForm = null;
		} 
		#endregion


		#region Public Method
		public int InvokeHandler(string[] values)
		{
			Handler(values);
			return 1;
		} 
		#endregion
	}
}