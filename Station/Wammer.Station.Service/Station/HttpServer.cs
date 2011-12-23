using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

using Wammer.Utility;

namespace Wammer.Station
{
	public interface IHttpHandler : ICloneable
	{
		void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
	}

	public class HttpServer : IDisposable
	{
		private int port;
		private HttpListener listener;
		private Dictionary<string, HttpHandlerProxy> handlers;
		private HttpHandlerProxy defaultHandler;
		private bool started = false;
		private static log4net.ILog logger = log4net.LogManager.GetLogger("HttpServer");
		private object cs = new object();
		private bool authblocked;

		public HttpServer(int port)
		{
			this.port = port;
			this.listener = new HttpListener();
			this.handlers = new Dictionary<string, HttpHandlerProxy>();
			this.defaultHandler = null;
			this.authblocked = false;
		}

		public void AddHandler(string path, IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			string absPath = null;
			string urlPrefix = "http://+:" + port;

			if (path.StartsWith("/"))
			{
				urlPrefix += path;
				absPath = path;
			}
			else
			{
				urlPrefix += "/" + path;
				absPath = "/" + absPath;
			}

			if (!path.EndsWith("/"))
			{
				urlPrefix += "/";
				absPath += "/";
			}

			handlers.Add(absPath, new HttpHandlerProxy(handler));
			listener.Prefixes.Add(urlPrefix);
		}

		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			defaultHandler = new HttpHandlerProxy(handler);
		}

		public void Start()
		{
			lock (cs)
			{
				if (started)
					return;

				listener.Start();
				started = true;
				listener.BeginGetContext(this.ConnectionAccepted, null);
			}
		}

		public void Stop()
		{
			lock (cs)
			{
				if (started)
				{
					listener.Stop();
					started = false;
				}
			}
		}

		public void BlockAuth(bool blocked)
		{
			authblocked = blocked;
		}

		public void Close()
		{
			Stop();
			listener.Close();
		}

		public void Dispose()
		{
			Close();
		}

		private void ConnectionAccepted(IAsyncResult result)
		{
			HttpListenerContext context = null;

			try
			{
				context = listener.EndGetContext(result);

				listener.BeginGetContext(this.ConnectionAccepted, null);

				if (context != null)
				{
					if (authblocked)
					{
						respond401Unauthorized(context);
					}
					else
					{
						HttpHandlerProxy handler = FindBestMatch(
													context.Request.Url.AbsolutePath);

						if (handler != null)
							ThreadPool.QueueUserWorkItem(handler.Do, context);
						else
							respond404NotFound(context);
					}
				}
			}
			catch (ObjectDisposedException)
			{
				logger.Info("Http server disposed. Shutdown server");
			}
			catch (Exception e)
			{
				logger.Info("Shutdown server", e);
				return;
			}
		}

		private static void respond404NotFound(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
				ctx.Response.Close();
			}
			catch (Exception e)
			{
				logger.Warn("Unable to respond 404 Not Found", e);
			}
		}

		private static void respond503Unavailable(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
				using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
				{
					Cloud.CloudResponse json = new Cloud.CloudResponse(
						ctx.Response.StatusCode,
						DateTime.UtcNow,
						(int)StationApiError.ServerOffline,
						"Server offline");
					w.Write(json.ToFastJSON());
				}
			}
			catch (Exception e)
			{
				logger.Warn("Unable to respond 503 Service Unavailable", e);
			}
		}

		private static void respond401Unauthorized(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
				{
					Cloud.CloudResponse json = new Cloud.CloudResponse(
						ctx.Response.StatusCode,
						DateTime.UtcNow,
						(int)StationApiError.AlreadyHasStaion,
						"Driver already registered another station"
					);
					w.Write(json.ToFastJSON());
				}
			}
			catch (Exception e)
			{
				logger.Warn("Unable to respond 401 Unauthorized", e);
			}
		}
		private HttpHandlerProxy FindBestMatch(string requestAbsPath)
		{
			string path = requestAbsPath;
			if (!path.EndsWith("/"))
				path += "/";


			if (handlers.ContainsKey(path))
				return handlers[path];
			else
				return defaultHandler;
		}
	}

	class HttpHandlerProxy
	{
		private readonly IHttpHandler handler;
		private static log4net.ILog logger = log4net.LogManager.GetLogger("HttpHandler");

		public HttpHandlerProxy(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			this.handler = handler; // (IHttpHandler)handler.Clone();
		}

		public void Do(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			try
			{
				IHttpHandler clonedHandler = (IHttpHandler)this.handler.Clone();
				clonedHandler.HandleRequest(ctx.Request, ctx.Response);
			}
			catch (Cloud.WammerCloudException e)
			{
				// if cloud returns bad request error, bypass it to client
				WebException webex = (WebException)e.InnerException;
				if (webex != null)
				{
					HttpWebResponse webres = (HttpWebResponse)webex.Response;
					if (webres != null && webres.StatusCode == HttpStatusCode.BadRequest)
					{
							Cloud.CloudResponse cloudres = fastJSON.JSON.Instance.ToObject<Cloud.CloudResponse>(e.response);
							HttpHelper.RespondFailure(ctx.Response, cloudres);
							logger.Warn("Connection to cloud error", e);
							return;
					}
				}

				HttpHelper.RespondFailure(ctx.Response,
					new WammerStationException(e.ToString(), e.WammerError), (int)HttpStatusCode.BadRequest);
				logger.Warn("Connecting to cloud error", e);
			}
			catch (ServiceUnavailableException e)
			{
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.ServiceUnavailable);
				logger.Warn("Service unavailable", e);
			}
			catch (WammerStationException e)
			{
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Warn("Http handler error", e);
			}
			catch (FormatException e)
			{
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Warn("Request format is incorrect", e);
			}
			catch (WebException webex)
			{
				Wammer.Cloud.WammerCloudException ex = new Cloud.WammerCloudException("Request to cloud failed", webex);              
				HttpHelper.RespondFailure(ctx.Response, webex, (int)HttpStatusCode.InternalServerError);
				logger.Warn("Connecting to cloud error", ex);
			}
			catch (Exception e)
			{
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.InternalServerError);
				logger.Warn("Internal server error", e);
			}
		}
	}

}
