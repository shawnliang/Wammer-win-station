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
		private readonly string scheme;
		private readonly List<string> exceptPrefixes = new List<string>();
		private static log4net.ILog logger = log4net.LogManager.GetLogger("BypassTraffic");

		public BypassHttpHandler(string baseUrl)
		{
			Uri url = new Uri(baseUrl);
			this.host = url.Host;
			this.port = url.Port;
			this.scheme = url.Scheme;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public void HandleRequest(HttpListenerRequest origReq, HttpListenerResponse response)
		{
			logger.Debug("Forward to cloud: " + origReq.Url.AbsolutePath);
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
						logger.Debug("Cloud responded error: " + errResponse.StatusCode);
						CopyResponseData(errResponse, response);
					}
					catch (Exception ex)
					{
						logger.Error("Error when replying cloud error", ex);
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
			url.Scheme = this.scheme;
			Uri targetUri = url.Uri;
			return targetUri;
		}
	}
}
