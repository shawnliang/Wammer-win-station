using System;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	public class SessionNotExistException : Exception
	{
		private readonly int wammerError;

		public SessionNotExistException(string msg, int wammerError)
			: base(msg)
		{
			this.wammerError = wammerError;
		}

		public SessionNotExistException(CloudResponse errorResp)
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
