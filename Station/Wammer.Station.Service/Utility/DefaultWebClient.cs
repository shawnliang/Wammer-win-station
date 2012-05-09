using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Utility
{
	public class DefaultWebClient : WebClient
	{
		private int timeout = 0;
		private int readWriteTimeout = 0;

		public DefaultWebClient()
			:base()
		{
		}

		static DefaultWebClient()
		{
			DefaultTimeout = 20 * 1000;
			DefaultReadWriteTimeout = 10 * 1000;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest req = base.GetWebRequest(address);

			if (req is HttpWebRequest)
			{
				HttpWebRequest httpReq = (HttpWebRequest)req;

				httpReq.Timeout = Timeout;
				httpReq.ReadWriteTimeout = ReadWriteTimeout;
			}

			return req;
		}

		public int Timeout
		{
			get
			{
				return (timeout == 0) ? DefaultTimeout : timeout;
			}

			set
			{
				timeout = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return (readWriteTimeout == 0) ? DefaultReadWriteTimeout : readWriteTimeout;
			}

			set
			{
				readWriteTimeout = value;
			}
		}
		public static int DefaultTimeout { get; set; }
		public static int DefaultReadWriteTimeout { get; set; }
	}
}
