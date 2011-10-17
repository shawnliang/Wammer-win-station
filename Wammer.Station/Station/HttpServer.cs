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
		private bool stopping = false;

		public HttpServer(int port)
		{
			this.port = port;
			listener = new HttpListener();
			handlers = new Dictionary<string, IHttpHandler>();
		}

		public void AddHandler(string path, IHttpHandler handler)
		{
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

		public void Start()
		{
			listener.Start();
			listener.BeginGetContext(this.ConnectionAccepted, null);
		}

		public void Stop()
		{
			if (!stopping)
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
				ThreadPool.QueueUserWorkItem(handler.Handle, context);
			}
		}

		private IHttpHandler FindBestMatch(string requestAbsPath)
		{
			return handlers[requestAbsPath];
		}
	}
}
