using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Utility
{
	class NoRedirectWebClient: WebClient
	{
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			var httpWebRequest = request as HttpWebRequest;

			if (httpWebRequest != null)
				httpWebRequest.AllowAutoRedirect = false;

			return request;
		}
	}
}
