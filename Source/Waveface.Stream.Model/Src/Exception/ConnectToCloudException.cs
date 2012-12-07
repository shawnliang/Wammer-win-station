using System;

namespace Waveface.Stream.Model
{
	public class ConnectToCloudException : Exception
	{
		public ConnectToCloudException(string msg)
			: base(msg)
		{
		}
	}
}
