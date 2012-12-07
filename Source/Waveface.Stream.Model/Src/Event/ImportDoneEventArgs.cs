using System;

namespace Waveface.Stream.Model
{
	public class ImportDoneEventArgs : EventArgs
	{
		public Exception Error { get; set; }
	}
}
