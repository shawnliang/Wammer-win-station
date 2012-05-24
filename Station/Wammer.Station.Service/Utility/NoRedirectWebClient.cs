using System;
using System.Net;

namespace Wammer.Utility
{
	internal class NoRedirectWebClient : WebClientEx
	{
		public NoRedirectWebClient()
		{
			AllowAutoRedirect = false;
		}
	}
}