using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Wammer.Cloud;
using Wammer.PerfMonitor;
using Wammer.Utility;
using log4net;

namespace Wammer.Station
{
	public class BypassHttpHandler : IHttpHandler
	{
		private static readonly ILog logger = LogManager.GetLogger("BypassTraffic");
		private readonly List<string> exceptPrefixes = new List<string>();
		private readonly string host;
		private readonly int port;
		private readonly string scheme;
		private long beginTime;

		public BypassHttpHandler(string baseUrl)
		{
			var url = new Uri(baseUrl);
			host = url.Host;
			port = url.Port;
			scheme = url.Scheme;
			ProcessSucceeded += HttpRequestMonitor.Instance.OnProcessSucceeded;
		}

		#region IHttpHandler Members

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void SetBeginTimestamp(long beginTime)
		{
			this.beginTime = beginTime;
		}

		public void HandleRequest(HttpListenerRequest origReq, HttpListenerResponse response)
		{
			logger.Info("Forward to cloud: " + origReq.Url.AbsolutePath);
			try
			{
				if (HasNotAllowedPrefix(origReq.Url.AbsolutePath))
				{
					var json = new CloudResponse(403, -1,
					                             "Station does not support this REST API; only Cloud does");
					HttpHelper.RespondFailure(response, json);
				}

				Uri targetUri = GetTargetUri(origReq);
				var newReq = (HttpWebRequest) WebRequest.Create(targetUri);

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


			long end = Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		public void HandleRequest()
		{
			throw new NotImplementedException();
		}

		#endregion

		protected void OnProcessSucceeded(HttpHandlerEventArgs evt)
		{
			EventHandler<HttpHandlerEventArgs> handler = ProcessSucceeded;

			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static void RequestStreamGotten(IAsyncResult ar)
		{
			var ctx = (BypassContext) ar.AsyncState;

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
			var ctx = (BypassContext) ar.AsyncState;
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
			var ctx = (BypassContext) ar.AsyncState;

			try
			{
				ctx.cloudResponse = (HttpWebResponse) ctx.cloudRequest.EndGetResponse(ar);

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
			var ctx = (BypassResponseContext) ar.AsyncState;

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
			var errResponse = (HttpWebResponse) e.Response;
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

			exceptPrefixes.Add(prefix);
		}

		private static void CopyResponseData(HttpWebResponse from, HttpListenerResponse to)
		{
			to.StatusCode = (int) from.StatusCode;
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

			return exceptPrefixes.Any(prefix => reqPath.StartsWith(prefix));
		}

		private void CopyRequestHeaders(HttpListenerRequest from, HttpWebRequest to)
		{
			to.ContentLength = from.ContentLength64;
			to.ContentType = from.ContentType;
			to.Method = from.HttpMethod;

			if (from.Cookies.Count > 0)
			{
				foreach (Cookie cookie in from.Cookies)
					cookie.Domain = host;

				to.CookieContainer = new CookieContainer();
				to.CookieContainer.Add(from.Cookies);
			}
		}

		private Uri GetTargetUri(HttpListenerRequest request)
		{
			var url = new UriBuilder(request.Url) {Host = host, Port = port, Scheme = scheme};
			Uri targetUri = url.Uri;
			return targetUri;
		}
	}

	internal class BypassContext
	{
		public BypassContext(HttpListenerRequest request, HttpListenerResponse response, HttpWebRequest cloudRequest)
		{
			this.request = request;
			this.response = response;
			this.cloudRequest = cloudRequest;
		}

		public HttpListenerRequest request { get; private set; }
		public HttpListenerResponse response { get; private set; }
		public HttpWebRequest cloudRequest { get; private set; }
		public Stream cloudRequestStream { get; set; }
		public HttpWebResponse cloudResponse { get; set; }
	}

	internal class BypassResponseContext
	{
		public BypassResponseContext(HttpListenerResponse response, HttpWebResponse bypassResponse)
		{
			this.response = response;
			this.bypassResponse = bypassResponse;
		}

		public HttpListenerResponse response { get; private set; }
		public HttpWebResponse bypassResponse { get; private set; }
	}
}