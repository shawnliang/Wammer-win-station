using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class ServiceUnavailableException : Exception
	{
		private int wammerError;

		public ServiceUnavailableException(string msg, int wammerError)
			:base(msg)
		{
			this.wammerError = wammerError;
		}

		public ServiceUnavailableException(Cloud.CloudResponse errorResp)
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
