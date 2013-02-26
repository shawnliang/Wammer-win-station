using System;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	public class ServiceUnavailableException : Exception
	{
		private readonly int wammerError;

		public ServiceUnavailableException(string msg, int wammerError)
			: base(msg)
		{
			this.wammerError = wammerError;
		}

		public ServiceUnavailableException(CloudResponse errorResp)
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