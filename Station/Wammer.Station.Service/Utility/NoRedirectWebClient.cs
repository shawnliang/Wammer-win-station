using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Utility
{
	class NoRedirectWebClient: WebClient
	{
		public NoRedirectWebClient()
			:base()
		{
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			if (request is HttpWebRequest)
				((HttpWebRequest)request).AllowAutoRedirect = false;

			return request;
		}
	}
}
