using System;

namespace Waveface.Stream.Model
{
	public class VersionNotSupportedException : Exception
	{
		public VersionNotSupportedException(string msg)
			: base(msg)
		{
		}
	}
}
