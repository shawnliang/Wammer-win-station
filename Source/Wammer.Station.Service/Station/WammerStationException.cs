using System;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	public class WammerStationException : Exception
	{
		private readonly int wammerError;

		public WammerStationException(string msg, int wammerError)
			: base(msg)
		{
			this.wammerError = wammerError;
		}

		public WammerStationException(string msg, int wammerError, Exception innerException)
			: base(msg, innerException)
		{
			this.wammerError = wammerError;
		}

		public WammerStationException(CloudResponse errorResp)
			: base(errorResp.api_ret_message)
		{
			wammerError = errorResp.api_ret_code;
			ErrorResponse = errorResp;
		}

		public int WammerError
		{
			get { return wammerError; }
		}

		public CloudResponse ErrorResponse { get; private set; }
	}
}