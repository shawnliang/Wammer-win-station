using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace Wammer.Station
{
	public interface IHttpHandler
	{
		void Handle(object state);
	}

	public class HttpServer : IDisposable
	{
		private int port;
		private HttpListener listener;
		private Dictionary<string, IHttpHandler> handlers;
		private IHttpHandler defaultHandler;
		private bool stopping = false;
		private bool started = false;

		public HttpServer(int port)
		{
			this.port = port;
			this.listener = new HttpListener();
			this.handlers = new Dictionary<string, IHttpHandler>();
			this.defaultHandler = null;
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
		}

		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			defaultHandler = handler;
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
			}
			catch (Exception e)
			{
				if (stopping)
					return;

				//TODO: write log...
			}

			listener.BeginGetContext(this.ConnectionAccepted, null);

			if (context != null)
			{
				IHttpHandler handler = FindBestMatch(
											context.Request.Url.AbsolutePath);

				if (handler != null)
					ThreadPool.QueueUserWorkItem(handler.Handle, context);
				else
					respond404NotFound(context);
			}
		}

		private static void respond404NotFound(HttpListenerContext ctx)
		{
			ctx.Response.StatusCode = 404;
			ctx.Response.Close();
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
}
