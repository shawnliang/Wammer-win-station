using System;

namespace Waveface.Stream.Model
{
	public class StationServiceDownException : Exception
	{
		public StationServiceDownException(string msg)
			: base(msg)
		{
		}
	}
}
