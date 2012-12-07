using System;

namespace Waveface.Stream.Model
{
	public class AuthenticationException : Exception
	{
		public AuthenticationException(string msg)
			: base(msg)
		{
		}
	}
}
