using System;

namespace Waveface.Stream.ClientFramework
{
	public class ExceptionEventArgs : EventArgs
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
