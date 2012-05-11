using System;
using System.Net;

namespace Wammer.Utility
{
	internal class NoRedirectWebClient : WebClient
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