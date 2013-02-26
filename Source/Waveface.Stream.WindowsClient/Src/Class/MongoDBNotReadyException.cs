using System;

namespace Waveface.Stream.WindowsClient
{
	class MongoDBNotReadyException : ApplicationException
	{
		public MongoDBNotReadyException(string msg)
			: base(msg)
		{
		}
	}

	class StationServiceNotReadyException : ApplicationException
	{
		public StationServiceNotReadyException(string msg)
			: base(msg)
		{
		}
	}
}
