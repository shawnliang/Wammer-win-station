using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
	class MongoDBNotReadyException : ApplicationException
	{
		public MongoDBNotReadyException(string msg)
			:base(msg)
		{
		}
	}

	class StationServiceNotReadyException: ApplicationException
	{
		public StationServiceNotReadyException(string msg)
			:base(msg)
		{
		}
	}
}
