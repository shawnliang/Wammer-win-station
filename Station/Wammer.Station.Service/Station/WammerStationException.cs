using System;

namespace Wammer.Station
{
	public class WammerStationException : Exception
	{
		private int wammerError;

		public WammerStationException(string msg, int wammerError)
			:base(msg)
		{
			this.wammerError = wammerError;
		}

		public WammerStationException(Cloud.CloudResponse errorResp)
			: base(errorResp.api_ret_message)
		{
			this.wammerError = errorResp.api_ret_code;
			this.ErrorResponse = errorResp;
		}

		public int WammerError
		{
			get { return wammerError; }
		}

		public Cloud.CloudResponse ErrorResponse { get; private set; }
	}
}
