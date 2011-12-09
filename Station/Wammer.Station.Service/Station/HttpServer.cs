using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

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
		private bool stopping = false;
		private bool started = false;
		log4net.ILog logger = null;

		public HttpServer(int port)
		{
			this.port = port;
			this.listener = new HttpListener();
			this.handlers = new Dictionary<string, HttpHandlerProxy>();
			this.defaultHandler = null;
			this.logger = log4net.LogManager.GetLogger("HttpServer");
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
			if (started)
				throw new InvalidOperationException("Http server already started");

			listener.Start();
			started = true;
			listener.BeginGetContext(this.ConnectionAccepted, null);
		}

		public void Stop()
		{
			if (started && !stopping)
			{
				stopping = true;
				listener.Stop();
			}
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
					HttpHandlerProxy handler = FindBestMatch(
												context.Request.Url.AbsolutePath);

					if (handler != null)
						ThreadPool.QueueUserWorkItem(handler.Do, context);
					else
						respond404NotFound(context);
				}
			}
			catch (ObjectDisposedException)
			{
				logger.Info("Http server disposed. Shutdown server");
			}
			catch (Exception e)
			{
				if (stopping)
				{
					logger.Info("Shutdown server");
					return;
				}
				else
				{
					logger.Info("Shutdown server", e);
					return;
				}
			}
		}

		private static void respond404NotFound(HttpListenerContext ctx)
		{
			ctx.Response.StatusCode = 404;
			ctx.Response.Close();
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

			this.handler = (IHttpHandler)handler.Clone();
		}

		public void Do(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			try
			{
				this.handler.HandleRequest(ctx.Request, ctx.Response);
			}
			catch (Cloud.WammerCloudException e)
			{
				HttpHelper.RespondFailure(ctx.Response, 
					new WammerStationException(e.ToString(), e.WammerError), (int)HttpStatusCode.BadRequest);
				logger.Warn("Connecting to cloud error", e);
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
			catch (WebException e)
			{
				Wammer.Cloud.WammerCloudException ex = new Cloud.WammerCloudException("Request to cloud failed", e);
				HttpHelper.RespondFailure(ctx.Response, e, (int)HttpStatusCode.InternalServerError);
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
