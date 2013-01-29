using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
	public class ExceptionEventArgs:EventArgs
	{
		#region Property
		public Exception Exception { get; private set; }
		#endregion

		#region Constructor
		public ExceptionEventArgs(Exception e)
		{
			this.Exception = e;
		}
		#endregion
	}
}
