using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Utility
{
	class DefaultWebClient : WebClient
	{
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

				httpReq.Timeout = DefaultTimeout;
				httpReq.ReadWriteTimeout = DefaultReadWriteTimeout;
			}

			return req;
		}

		public static int DefaultTimeout { get; set; }
		public static int DefaultReadWriteTimeout { get; set; }
	}
}
