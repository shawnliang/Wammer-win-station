using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Wammer.Utility;
using log4net;
using Wammer;
using Wammer.PerfMonitor;

namespace Wammer.Station
{
	public interface IHttpHandler : ICloneable
	{
		void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
		void HandleRequest();
		void SetBeginTimestamp(long beginTime);
		event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;
	}

	public class HttpServer : IDisposable
	{

		#region Var
		private ILog _logger;
		private Object _lockSwitchObj;
		private int _port;
		private HttpListener _listener;
		private Dictionary<string, IHttpHandler> _handlers;
		private IHttpHandler _defaultHandler;
		private bool _started;
		#endregion


		#region Private Property
		private ILog m_Logger
		{
			get
			{
				if (_logger == null)
					_logger = LogManager.GetLogger("HttpServer");
				return _logger;
			}
		}

		private object m_LockSwitchObj
		{
			get
			{
				if (_lockSwitchObj == null)
					_lockSwitchObj = new object();
				return _lockSwitchObj;
			}
		}

		private int m_Port
		{
			get { return _port; }
			set { _port = value; }
		}

		private HttpListener m_Listener
		{
			get
			{
				if (_listener == null)
					_listener = new HttpListener();
				return _listener;
			}
		}

		public Dictionary<string, IHttpHandler> m_Handlers
		{
			get
			{
				if (_handlers == null)
					_handlers = new Dictionary<string, IHttpHandler>();
				return _handlers;
			}
		}
		#endregion


		#region Event
		public event EventHandler<TaskQueueEventArgs> TaskEnqueue;
		#endregion



		#region Constructor
		public HttpServer(int port)
		{
			m_Port = port;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Responds 404 not found.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		private void respond404NotFound(HttpListenerContext ctx)
		{
			try
			{
				ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
				ctx.Response.Close();
			}
			catch (Exception e)
			{
				m_Logger.Warn("Unable to respond 404 Not Found", e);
			}
		}

		///// <summary>
		///// Responds 503 unavailable.
		///// </summary>
		///// <param name="ctx">The CTX.</param>
		//private void respond503Unavailable(HttpListenerContext ctx)
		//{
		//    try
		//    {
		//        ctx.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
		//        using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
		//        {
		//            Cloud.CloudResponse json = new Cloud.CloudResponse(
		//                ctx.Response.StatusCode,
		//                DateTime.UtcNow,
		//                (int)StationApiError.ServerOffline,
		//                "Server offline");
		//            w.Write(json.ToFastJSON());
		//        }
		//    }
		//    catch (Exception e)
		//    {
		//        m_Logger.Warn("Unable to respond 503 Service Unavailable", e);
		//    }
		//}

		/// <summary>
		/// Responds 401 unauthorized.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		private void respond401Unauthorized(HttpListenerContext ctx)
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
				m_Logger.Warn("Unable to respond 401 Unauthorized", e);
			}
		}

		/// <summary>
		/// Finds the best match.
		/// </summary>
		/// <param name="requestAbsPath">The request abs path.</param>
		/// <returns></returns>
		private IHttpHandler FindBestMatch(string requestAbsPath)
		{
			string path = requestAbsPath;
			if (!path.EndsWith("/"))
				path += "/";

			if (m_Handlers.ContainsKey(path))
				return m_Handlers[path];
			else
				return _defaultHandler;
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:TaskQueue"/> event.
		/// </summary>
		/// <param name="e">The <see cref="Wammer.TaskQueueEventArgs"/> instance containing the event data.</param>
		protected void OnTaskQueue(TaskQueueEventArgs e)
		{
			this.RaiseEvent<TaskQueueEventArgs>(TaskEnqueue, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Adds the handler.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="handler">The handler.</param>
		public void AddHandler(string path, IHttpHandler handler)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			if (handler == null)
				throw new ArgumentNullException("handler");

			string absPath = null;
			string urlPrefix = "http://+:" + m_Port;

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

			m_Handlers.Add(absPath, handler);
			m_Listener.Prefixes.Add(urlPrefix);
		}

		/// <summary>
		/// Adds the default handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void AddDefaultHandler(IHttpHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException();

			_defaultHandler = handler;
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			lock (m_LockSwitchObj)
			{
				if (_started)
					return;

				m_Listener.Start();
				_started = true;
				m_Listener.BeginGetContext(this.ConnectionAccepted, null);
			}
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			lock (m_LockSwitchObj)
			{
				if (_started)
				{
					m_Listener.Stop();
					_started = false;
				}
			}
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public void Close()
		{
			Stop();
			m_Listener.Close();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		/// <summary>
		/// Connections the accepted.
		/// </summary>
		/// <param name="result">The result.</param>
		private void ConnectionAccepted(IAsyncResult result)
		{
			HttpListenerContext context = null;

			try
			{
				context = m_Listener.EndGetContext(result);
				long beginTime = System.Diagnostics.Stopwatch.GetTimestamp();

				m_Listener.BeginGetContext(this.ConnectionAccepted, null);

				if (context != null)
				{
					IHttpHandler handler = FindBestMatch(context.Request.Url.AbsolutePath);

					if (handler != null)
					{
						var task = new HttpHandlingTask(handler, context, beginTime);
						OnTaskQueue(new TaskQueueEventArgs(task, handler));
						TaskQueue.Enqueue(task, TaskPriority.High);
					}
					else
						respond404NotFound(context);
				}
			}
			catch (ObjectDisposedException)
			{
				m_Logger.Info("Http server disposed. Shutdown server");
			}
			catch (Exception e)
			{
				m_Logger.Info("Shutdown server", e);
				return;
			}
		}
		#endregion

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
					logger.Debug("Connection to cloud error", e);
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
							logger.Debug("Connection to cloud error", e);
							return;
						}

						HttpHelper.RespondFailure(context.Response,
							new WammerStationException(e.InnerException.Message, e.WammerError), (int)webres.StatusCode);
						logger.Debug("Connecting to cloud error", e);
						return;
					}
				}

				HttpHelper.RespondFailure(context.Response,
					new WammerStationException(e.Message, e.WammerError), (int)HttpStatusCode.BadRequest);
				logger.Debug("Connecting to cloud error", e);
			}
			catch (ServiceUnavailableException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.ServiceUnavailable);
				logger.Debug("Service unavailable", e);
			}
			catch (WammerStationException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Debug("Http handler error", e);
			}
			catch (FormatException e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.BadRequest);
				logger.Debug("Request format is incorrect", e);
			}
			catch (WebException webex)
			{
				Wammer.Cloud.WammerCloudException ex = new Cloud.WammerCloudException("Request to cloud failed", webex);
				HttpHelper.RespondFailure(context.Response, webex, (int)HttpStatusCode.InternalServerError);
				logger.Debug("Connecting to cloud error", ex);
			}
			catch (Exception e)
			{
				HttpHelper.RespondFailure(context.Response, e, (int)HttpStatusCode.InternalServerError);
				logger.Warn("Internal server error", e);
			}
		}
	}
}
