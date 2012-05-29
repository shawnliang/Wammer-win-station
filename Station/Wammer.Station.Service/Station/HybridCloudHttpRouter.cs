using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.MultiPart;
using Wammer.Utility;
using log4net;

namespace Wammer.Station
{
	public class HybridCloudHttpRouter : IHttpHandler
	{
		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";
		private static readonly ILog logger = LogManager.GetLogger("HybridCloudHttpRouter");
		private long beginTime;

		private BypassHttpHandler bypass = new BypassHttpHandler(CloudServer.BaseUrl);
		private HttpHandler handler;

		public HybridCloudHttpRouter(HttpHandler handler)
		{
			this.handler = handler;
		}

		public HttpListenerRequest Request { get; private set; }
		public HttpListenerResponse Response { get; private set; }
		public NameValueCollection Parameters { get; private set; }
		public List<UploadedFile> Files { get; private set; }
		public byte[] RawPostData { get; private set; }

		#region IHttpHandler Members

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			Debug.Assert(request.RemoteEndPoint != null, "request.RemoteEndPoint != null");

			// handle request locally for local client, otherwise bypass to cloud
			if ("127.0.0.1" == request.RemoteEndPoint.Address.ToString())
			{
				Files = new List<UploadedFile>();
				Request = request;
				Response = response;
				RawPostData = InitRawPostData();
				Parameters = InitParameters(request);

				if (HasMultiPartFormData(request))
				{
					ParseMultiPartData(request);
				}

				LogRequest();

				LoginedSession session = GetSessionFromCache();
				if (session == null)
					throw new SessionNotExistException("session not exist", (int)GeneralApiError.SessionNotExist);

				handler.Session = session;
				handler.Request = request;
				handler.Response = response;
				handler.Parameters = Parameters;
				handler.RawPostData = RawPostData;
				(handler as IHttpHandler).HandleRequest();
			}
			else
			{
				bypass.HandleRequest(request, response);
			}

			long end = Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		public void SetBeginTimestamp(long beginTime)
		{
			this.beginTime = beginTime;
			bypass.SetBeginTimestamp(beginTime);
			handler.SetBeginTimestamp(beginTime);
		}

		public object Clone()
		{
			var router = (HybridCloudHttpRouter) MemberwiseClone();
			router.bypass = (BypassHttpHandler) bypass.Clone();
			router.handler = (HttpHandler) handler.Clone();
			return router;
		}

		public void HandleRequest()
		{
			throw new NotImplementedException();
		}

		#endregion

		private void LogRequest()
		{
			if (logger.IsDebugEnabled)
			{
				Debug.Assert(Request != null, "Request != null");
				Debug.Assert(Request.RemoteEndPoint != null, "Request.RemoteEndPoint != null");
				Debug.Assert(Request.Url != null, "Request.Url != null");

				logger.Debug("====== Request " + Request.Url.AbsolutePath +
				             " from " + Request.RemoteEndPoint.Address + " ======");
				foreach (string key in Parameters.AllKeys)
				{
					if (key == "password")
					{
						logger.DebugFormat("{0} : *", key);
					}
					else
					{
						logger.DebugFormat("{0} : {1}", key, Parameters[key]);
						if (key == "apikey" && CloudServer.CodeName.ContainsKey(Parameters[key]))
						{
							logger.DebugFormat("(code name : {0})", CloudServer.CodeName[Parameters[key]]);
						}
					}
				}
				foreach (UploadedFile file in Files)
					logger.DebugFormat("file: {0}, mime: {1}, size: {2}", file.Name, file.ContentType, file.Data.Count.ToString());
			}
		}

		protected void OnProcessSucceeded(HttpHandlerEventArgs evt)
		{
			EventHandler<HttpHandlerEventArgs> handler = ProcessSucceeded;

			if (handler != null)
			{
				handler(this, evt);
			}
		}

		public LoginedSession GetSessionFromCache()
		{
			if (Parameters["session_token"] != null && Parameters["apikey"] != null)
			{
				LoginedSession session = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", Parameters["session_token"]));
				if (session != null && session.apikey.apikey == Parameters["apikey"])
				{
					// currently station only saves windows client's session
					return session;
				}
			}
			return null;
		}

		private void ParseMultiPartData(HttpListenerRequest request)
		{
			try
			{
				string boundary = GetMultipartBoundary(request.ContentType);
				var parser = new Parser(boundary);

				Part[] parts = parser.Parse(RawPostData);
				foreach (Part part in parts)
				{
					if (part.ContentDisposition == null)
						continue;

					ExtractParamsFromMultiPartFormData(part);
				}
			}
			catch (FormatException)
			{
				string filename = Guid.NewGuid().ToString();
				using (var w = new BinaryWriter(File.OpenWrite(@"log\" + filename)))
				{
					w.Write(RawPostData);
				}
				this.LogWarnMsg("Parsing multipart data error. Post data written to log\\" + filename);
				throw;
			}
		}

		private byte[] InitRawPostData()
		{
			if (string.Compare(Request.HttpMethod, "POST", true) == 0)
			{
				var initialSize = (int) Request.ContentLength64;
				if (initialSize <= 0)
					initialSize = 65535;

				using (var buff = new MemoryStream(initialSize))
				{
					StreamHelper.Copy(Request.InputStream, buff);
					return buff.ToArray();
				}
			}
			return null;
		}

		private void ExtractParamsFromMultiPartFormData(Part part)
		{
			Disposition disp = part.ContentDisposition;

			if (disp == null)
				throw new ArgumentException("incorrect use of this function: " +
				                            "input part.ContentDisposition is null");

			if (disp.Value.Equals("form-data", StringComparison.CurrentCultureIgnoreCase))
			{
				string filename = disp.Parameters["filename"];

				if (filename != null)
				{
					var file = new UploadedFile(filename, part.Bytes,
					                            part.Headers["Content-Type"]);
					Files.Add(file);
				}
				else
				{
					string name = part.ContentDisposition.Parameters["name"];
					Parameters.Add(name, part.Text);
				}
			}
		}

		private static bool HasMultiPartFormData(HttpListenerRequest request)
		{
			return request.ContentType != null &&
			       request.ContentType.StartsWith(MULTIPART_FORM, StringComparison.CurrentCultureIgnoreCase);
		}

		private static string GetMultipartBoundary(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException();

			try
			{
				string[] parts = contentType.Split(';');
				foreach (string part in parts)
				{
					int idx = part.IndexOf(BOUNDARY);
					if (idx < 0)
						continue;

					return part.Substring(idx + BOUNDARY.Length);
				}

				throw new FormatException("Multipart boundary is nout found in content-type header");
			}
			catch (Exception e)
			{
				throw new FormatException("Error finding multipart boundary. Content-Type: " +
				                          contentType, e);
			}
		}

		private NameValueCollection InitParameters(HttpListenerRequest req)
		{
			Debug.Assert(req != null, "req != null");

			if (RawPostData != null &&
				req.ContentType != null &&
			    req.ContentType.StartsWith(URL_ENCODED_FORM, StringComparison.CurrentCultureIgnoreCase))
			{
				string postData = Encoding.UTF8.GetString(RawPostData);
				return HttpUtility.ParseQueryString(postData);
			}
			else if (req.HttpMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
			{
				return HttpUtility.ParseQueryString(Request.Url.Query);
			}

			return new NameValueCollection();
		}
	}
}