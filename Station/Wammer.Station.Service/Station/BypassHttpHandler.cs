using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	public class BypassHttpHandler: IHttpHandler
	{
		private readonly string host;
		private readonly int port;
		private readonly string scheme;
		private readonly List<string> exceptPrefixes = new List<string>();
		private static log4net.ILog logger = log4net.LogManager.GetLogger("BypassTraffic");
		private long beginTime;

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

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

		public void SetBeginTimestamp(long beginTime)
		{
			this.beginTime = beginTime;
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
					newReq.BeginGetRequestStream(RequestStreamGotten,
						new BypassContext(origReq, response, newReq));
				}
				else
				{
					newReq.BeginGetResponse(ResponseGotten, new BypassContext(origReq, response, newReq));
				}
			}
			catch (WebException e)
			{
				logger.Error("forward to cloud error", e);
				ReplyCloudError(response, e);
			}
			catch (Exception e)
			{
				logger.Error("Forward to cloud error", e);
				HttpHelper.RespondFailure(response, e, 400);
			}


			long end = System.Diagnostics.Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		protected void OnProcessSucceeded(HttpHandlerEventArgs evt)
		{
			EventHandler<HttpHandlerEventArgs> handler = this.ProcessSucceeded;

			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static void RequestStreamGotten(IAsyncResult ar)
		{
			BypassContext ctx = (BypassContext)ar.AsyncState;

			try
			{
				ctx.cloudRequestStream = ctx.cloudRequest.EndGetRequestStream(ar);

				StreamHelper.BeginCopy(ctx.request.InputStream, ctx.cloudRequestStream,
					RequestDone, ctx);
			}
			catch (Exception e)
			{
				logger.Warn("Error get request stream to cloud", e);
				HttpHelper.RespondFailure(ctx.response, e, 400);
			}
		}

		private static void RequestDone(IAsyncResult ar)
		{
			BypassContext ctx = (BypassContext)ar.AsyncState;
			try
			{
				StreamHelper.EndCopy(ar);

				ctx.cloudRequest.BeginGetResponse(ResponseGotten, ctx);
			}
			catch (WebException e)
			{
				logger.Warn("forward to cloud error", e);
				ReplyCloudError(ctx.response, e);
			}
			catch (Exception e)
			{
				logger.Warn("forward to cloud error", e);
				HttpHelper.RespondFailure(ctx.response, e, 400);
			}
		}

		private static void ResponseGotten(IAsyncResult ar)
		{
			BypassContext ctx = (BypassContext)ar.AsyncState;

			try
			{
				ctx.cloudResponse = (HttpWebResponse)ctx.cloudRequest.EndGetResponse(ar);

				CopyResponseData(ctx.cloudResponse, ctx.response);
			}
			catch (WebException e)
			{
				logger.Warn("forward to cloud error", e);
				ReplyCloudError(ctx.response, e);
			}
			catch (Exception e)
			{
				logger.Warn("forward to cloud error", e);
				HttpHelper.RespondFailure(ctx.response, e, 400);
			}
		}

		private static void RespondDone(IAsyncResult ar)
		{
			BypassResponseContext ctx = (BypassResponseContext)ar.AsyncState;

			try
			{
				StreamHelper.EndCopy(ar);

				ctx.bypassResponse.Close();
				ctx.response.Close();
			}
			catch (Exception e)
			{
				ctx.response.Abort();
				logger.Warn("Error responding cloud response", e);
			}
		}

		private static void ReplyCloudError(HttpListenerResponse response, WebException e)
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
					logger.Error("Error when replying cloud error", ex);
				}
			}
			else
				HttpHelper.RespondFailure(response, e, 400);
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

			StreamHelper.BeginCopy(from.GetResponseStream(), to.OutputStream, RespondDone, 
				new BypassResponseContext(to, from));
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
		

		public void OnTaskEnqueue(EventArgs e)
		{
		}
	}

	class BypassContext
	{
		public HttpListenerRequest request { get; private set; }
		public HttpListenerResponse response { get; private set; }
		public HttpWebRequest cloudRequest { get; private set; }
		public Stream cloudRequestStream { get; set; }
		public HttpWebResponse cloudResponse { get; set; }

		public BypassContext(HttpListenerRequest request, HttpListenerResponse response, HttpWebRequest cloudRequest)
		{
			this.request = request;
			this.response = response;
			this.cloudRequest = cloudRequest;
		}
	}

	class BypassResponseContext
	{
		public HttpListenerResponse response { get; private set; }
		public HttpWebResponse bypassResponse { get; private set; }

		public BypassResponseContext(HttpListenerResponse response, HttpWebResponse bypassResponse)
		{
			this.response = response;
			this.bypassResponse = bypassResponse;
		}
	}
}
