using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Wammer.IO;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class BypassHttpHandler: IHttpHandler
	{
		private readonly string host;
		private readonly int port;
		private readonly List<string> exceptPrefixes = new List<string>();

		public BypassHttpHandler(string host, int port)
		{
			this.host = host;
			this.port = port;
		}

		public void Handle(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			try
			{
				if (HasNotAllowedPrefix(ctx.Request.Url.AbsolutePath))
				{
					CloudResponse json = new CloudResponse(403, -1,
											"Station does not support this REST API; only Cloud does");
					HttpHelper.RespondFailure(ctx.Response, json);
				}


				HttpListenerRequest origReq = ctx.Request;
				Uri targetUri = GetTargetUri(origReq);
				HttpWebRequest newReq = (HttpWebRequest)WebRequest.Create(targetUri);

				CopyRequestHeaders(origReq, newReq);

				if (origReq.HasEntityBody)
				{
					using (Stream output = newReq.GetRequestStream())
					{
						StreamHelper.Copy(origReq.InputStream, output);
					}
				}

				HttpWebResponse newResp = (HttpWebResponse)newReq.GetResponse();
				CopyResponseHeaders(newResp, ctx.Response);
				using (Stream input = newResp.GetResponseStream())
				{
					StreamHelper.Copy(input, ctx.Response.OutputStream);
				}
				ctx.Response.OutputStream.Close();
			}
			catch (WebException e)
			{
				HttpWebResponse errResponse = (HttpWebResponse)e.Response;
				if (errResponse != null)
				{
					ctx.Response.StatusCode = (int)errResponse.StatusCode;

					using (Stream errStream = errResponse.GetResponseStream())
					using (Stream outStream = ctx.Response.OutputStream)
					{
						StreamHelper.Copy(errStream, outStream);
					}
				}
			}
			catch (Exception e)
			{
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.InternalServerError);
			}
		}

		public void AddExceptPrefix(string prefix)
		{
			if (!prefix.StartsWith("/") || !prefix.EndsWith("/"))
				throw new ArgumentException("prefix must start and end with slash");

			this.exceptPrefixes.Add(prefix);
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


		private bool HasNotAllowedPrefix(string reqPath)
		{
			foreach (string prefix in this.exceptPrefixes)
			{
				if (reqPath.StartsWith(prefix))
					return true;
			}

			return false;
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
