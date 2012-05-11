using System;
using System.Net;

namespace Wammer.Utility
{
	public class DefaultWebClient : WebClient
	{
		private int readWriteTimeout;
		private int timeout;

		static DefaultWebClient()
		{
			DefaultTimeout = 20*1000;
			DefaultReadWriteTimeout = 10*1000;
		}

		public int Timeout
		{
			get { return (timeout == 0) ? DefaultTimeout : timeout; }

			set { timeout = value; }
		}

		public int ReadWriteTimeout
		{
			get { return (readWriteTimeout == 0) ? DefaultReadWriteTimeout : readWriteTimeout; }

			set { readWriteTimeout = value; }
		}

		public static int DefaultTimeout { get; set; }
		public static int DefaultReadWriteTimeout { get; set; }

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest req = base.GetWebRequest(address);

			var httpReq = req as HttpWebRequest;
			if (httpReq != null)
			{
				httpReq.Timeout = Timeout;
				httpReq.ReadWriteTimeout = ReadWriteTimeout;
			}

			return req;
		}
	}
}