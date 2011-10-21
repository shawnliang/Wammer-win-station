using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Wammer.IO;

namespace Wammer.Station
{
	public class BypassHttpHandler: IHttpHandler
	{
		private string host;
		private int port;

		public BypassHttpHandler(string host, int port)
		{
			this.host = host;
			this.port = port;
		}

		public void Handle(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			HttpListenerRequest origReq = ctx.Request;
			Uri targetUri = GetTargetUri(origReq);
			HttpWebRequest newReq = (HttpWebRequest)WebRequest.Create(targetUri);

			CopyRequestHeaders(origReq, newReq);

			using (Stream output = newReq.GetRequestStream())
			{
				StreamHelper.Copy(origReq.InputStream, output);
			}

			HttpWebResponse newResp = (HttpWebResponse)newReq.GetResponse();
			CopyResponseHeaders(newResp, ctx.Response);
			using (Stream input = newResp.GetResponseStream())
			{
				StreamHelper.Copy(input, ctx.Response.OutputStream);
			}

			ctx.Response.OutputStream.Close();
		}

		private static void CopyResponseHeaders(HttpWebResponse from, HttpListenerResponse to)
		{
			to.StatusCode = (int)from.StatusCode;
			to.StatusDescription = from.StatusDescription;
			to.ContentType = from.ContentType;

			if (from.Cookies.Count > 0)
			{
				to.Cookies.Add(from.Cookies);
			}
		}

		private void CopyRequestHeaders(HttpListenerRequest from, HttpWebRequest to)
		{
			to.ContentLength = from.ContentLength64;
			to.ContentType = from.ContentType;
			to.Method = from.HttpMethod;

			if (from.Cookies.Count > 0)
			{
				foreach (Cookie cookie in from.Cookies)
					cookie.Domain = this.host;

				to.CookieContainer = new CookieContainer();
				to.CookieContainer.Add(from.Cookies);
			}
		}

		private Uri GetTargetUri(HttpListenerRequest request)
		{
			UriBuilder url = new UriBuilder(request.Url);
			url.Host = this.host;
			url.Port = this.port;
			Uri targetUri = url.Uri;
			return targetUri;
		}
	}
}
