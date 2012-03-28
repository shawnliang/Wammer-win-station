using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Wammer.Utility;
using Wammer.PerfMonitor;

namespace Wammer.Station
{
	public interface IHttpHandler : ICloneable
	{
		void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
		void SetBeginTimestamp(long beginTime);
		void OnTaskEnqueue(EventArgs e);
		event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;
	}

	public class HttpServer : IDisposable
	{
		private int port;
		private HttpListener listener;
		private Dictionary<string, IHttpHandler> handlers;
		private IHttpHandler defaultHandler;
		private bool started = false;
		private static log4net.ILog logger = log4net.LogManager.GetLogger("HttpServer");
		private object cs = new object();
		private bool authblocked;
		private HttpRequestMonitor monitor;

		public HttpServer(int port, HttpRequestMonitor monitor = null)
		{
			this.port = port;
			this.listener = new HttpListener();
			this.handlers = new Dictionary<string, IHttpHandler>();
			this.defaultHandler = null;
			this.authblocked = false;
			this.monitor = monitor;
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

			handlers.Add(absPath, handler);
			listener.Prefixes.Add(urlPrefix);

			if (monitor != null)
				handler.ProcessSucceeded += monitor.OnProcessSucceeded;
		}

		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			defaultHandler = handler;
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
				long beginTime = System.Diagnostics.Stopwatch.GetTimestamp();

				listener.BeginGetContext(this.ConnectionAccepted, null);

				if (context != null)
				{
					if (authblocked)
					{
						respond401Unauthorized(context);
					}
					else
					{
						IHttpHandler handler = FindBestMatch(context.Request.Url.AbsolutePath);

						if (handler != null)
						{
							handler.OnTaskEnqueue(EventArgs.Empty);
							TaskQueue.Enqueue(new HttpHandlingTask(handler, context, beginTime), TaskPriority.High);
						}
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
		private IHttpHandler FindBestMatch(string requestAbsPath)
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

	class HttpHandlingTask : ITask
	{
		private IHttpHandler handler;
		private HttpListenerContext context;
		private long beginTime;
		private static log4net.ILog logger = log4net.LogManager.GetLogger("HttpHandler");

		public HttpHandlingTask(IHttpHandler handler, HttpListenerContext context, long beginTime)
		{
			this.handler = (IHttpHandler)handler.Clone();
			this.context = context;
			this.beginTime = beginTime;
		}

		public void Execute()
		{
			try
			{
				handler.SetBeginTimestamp(beginTime);
				handler.HandleRequest(context.Request, context.Response);
			}
			catch (Cloud.WammerCloudException e)
			{
				// cannot connect to Waveface cloud
				if (e.HttpError != WebExceptionStatus.ProtocolError)
				{
					HttpHelper.RespondFailure(context.Response, new WammerStationException(e.InnerException.Message, (int)StationApiError.ConnectToCloudError), (int)HttpStatusCode.BadRequest);
					logger.Warn("Connection to cloud error", e);
					return;
				}

				// if cloud returns bad request error, bypass it to client
				WebException webex = (WebException)e.InnerException;
				if (webex != null)
				{
					HttpWebResponse webres = (HttpWebResponse)webex.Response;
					if (webres != null)
					{
						if (webres.StatusCode == HttpStatusCode.BadRequest)
						{
							Cloud.CloudResponse cloudres = fastJSON.JSON.Instance.ToObject<Cloud.CloudResponse>(e.response);
							HttpHelper.RespondFailure(context.Response, cloudres);
							logger.Warn("Connection to cloud error", e);
							return;
						}

						HttpHelper.RespondFailure(context.Response,
							new WammerStationException(e.InnerException.Message, e.WammerError), (int)webres.StatusCode);
						logger.Warn("Connecting to cloud error", e);
						return;
					}
				}

				HttpHelper.RespondFailure(context.Response,
					new WammerStationException(e.Message, e.WammerError), (int)HttpStatusCode.BadRequest);
				logger.Warn("Connecting to cloud error", e);
			}
			catch (ServiceUnavailableException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.ServiceUnavailable);
				logger.Warn("Service unavailable", e);
			}
			catch (WammerStationException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Warn("Http handler error", e);
			}
			catch (FormatException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Warn("Request format is incorrect", e);
			}
			catch (WebException webex)
			{
				Wammer.Cloud.WammerCloudException ex = new Cloud.WammerCloudException("Request to cloud failed", webex);
				HttpHelper.RespondFailure(context.Response, webex, (int)HttpStatusCode.InternalServerError);
				logger.Warn("Connecting to cloud error", ex);
			}
			catch (Exception e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.InternalServerError);
				logger.Warn("Internal server error", e);
			}
		}
	}
}
