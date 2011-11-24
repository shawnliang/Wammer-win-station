using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Wammer.Utility;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class BypassHttpHandler: IHttpHandler
	{
		private readonly string host;
		private readonly int port;
		private readonly List<string> exceptPrefixes = new List<string>();

		public BypassHttpHandler(string baseUrl)
		{
			Uri url = new Uri(baseUrl);
			this.host = url.Host;
			this.port = url.Port;
		}

		public BypassHttpHandler(string host, int port)
		{
			this.host = host;
			this.port = port;
		}

		public BypassHttpHandler(string host, int port, List<string> exceptPrefixes)
		{
			this.host = host;
			this.port = port;
			this.exceptPrefixes = exceptPrefixes;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public void HandleRequest(HttpListenerRequest origReq, HttpListenerResponse response)
		{
			try
			{
				if (HasNotAllowedPrefix(origReq.Url.AbsolutePath))
				{
					CloudResponse json = new CloudResponse(403, -1,
										"Station does not support this REST API; only Cloud does");
					HttpHelper.RespondFailure(response, json);
				}

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

				CopyResponseData((HttpWebResponse)newReq.GetResponse(), response);
			}
			catch (WebException e)
			{
				HttpWebResponse errResponse = (HttpWebResponse)e.Response;
				if (errResponse != null)
				{
					try
					{
						CopyResponseData(errResponse, response);
					}
					catch (Exception ex)
					{
						log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BypassHttpHandler));
						logger.Error("Error reply error", ex);
					}
				}
			}
		}

		public void AddExceptPrefix(string prefix)
		{
			if (!prefix.StartsWith("/") || !prefix.EndsWith("/"))
				throw new ArgumentException("prefix must start and end with slash");

			this.exceptPrefixes.Add(prefix);
		}

		private static void CopyResponseData(HttpWebResponse from, HttpListenerResponse to)
		{
			to.StatusCode = (int)from.StatusCode;
			to.StatusDescription = from.StatusDescription;
			to.ContentType = from.ContentType;

			if (from.Cookies.Count > 0)
			{
				to.Cookies.Add(from.Cookies);
			}

			using (Stream input = from.GetResponseStream())
			using (Stream output = to.OutputStream)
			{
				StreamHelper.Copy(input, output);
			}
		}

		private bool HasNotAllowedPrefix(string reqPath)
		{
			if (!reqPath.EndsWith("/"))
				reqPath += "/";

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
