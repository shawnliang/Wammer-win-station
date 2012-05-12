using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.MultiPart;
using Wammer.PerfMonitor;
using Wammer.Utility;
using log4net;

namespace Wammer.Station
{
	public class UploadedFile
	{
		public UploadedFile(string name, ArraySegment<byte> data, string contentType)
		{
			Name = name;
			Data = data;
			ContentType = contentType;
		}

		public string Name { get; private set; }
		public ArraySegment<byte> Data { get; private set; }
		public string ContentType { get; private set; }
	}


	public abstract class HttpHandler : IHttpHandler
	{
		#region Const

		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";
		private const string API_PATH_GROUP_NAME = @"APIPath";
		private const string API_PATH_MATCH_PATTERN = @"/V\d+/(?<" + API_PATH_GROUP_NAME + ">.+)";

		#endregion

		private static readonly ILog logger = LogManager.GetLogger("HttpHandler");
		private long beginTime;

		protected HttpHandler()
		{
			ProcessSucceeded += HttpRequestMonitor.Instance.OnProcessSucceeded;
		}

		#region Protected Method

		/// <summary>
		/// Checks the parameter.
		/// </summary>
		/// <param name="arguementNames">The arguement names.</param>
		protected void CheckParameter(params string[] arguementNames)
		{
			if (arguementNames == null)
				throw new ArgumentNullException("arguementNames");

			IEnumerable<string> nullArgumentNames = from arguementName in arguementNames
			                                        where Parameters[arguementName] == null
			                                        select arguementName;

			bool IsAllParameterReady = !nullArgumentNames.Any();
			if (!IsAllParameterReady)
			{
				throw new FormatException(string.Format("Parameter {0} is null.", string.Join("¡B", nullArgumentNames.ToArray())));
			}
		}

		protected void TunnelToCloud()
		{
			Debug.Assert(Request != null);

			string apiPath =
				Regex.Match(Request.Url.LocalPath, API_PATH_MATCH_PATTERN, RegexOptions.IgnoreCase).Groups[API_PATH_GROUP_NAME].
					Value;
			Dictionary<object, object> forwardParams = Parameters.AllKeys.ToDictionary<string, object, object>(key => key,
			                                                                                                   key =>
			                                                                                                   Parameters[key]);
			RespondSuccess(CloudServer.requestPath(new WebClient(), apiPath, forwardParams, false));
		}

		protected void TunnelToCloud<T>()
		{
			Debug.Assert(Request != null);

			string apiPath =
				Regex.Match(Request.Url.LocalPath, API_PATH_MATCH_PATTERN, RegexOptions.IgnoreCase).Groups[API_PATH_GROUP_NAME].
					Value;
			Dictionary<object, object> forwardParams = Parameters.AllKeys.ToDictionary<string, object, object>(key => key,
			                                                                                                   key =>
			                                                                                                   Parameters[key]);
			RespondSuccess(CloudServer.requestPath<T>(new WebClient(), apiPath, forwardParams, false));
		}

		#endregion

		public HttpListenerRequest Request { get; internal set; }
		public HttpListenerResponse Response { get; internal set; }
		public NameValueCollection Parameters { get; internal set; }
		public List<UploadedFile> Files { get; private set; }
		public LoginedSession Session { get; set; }
		public byte[] RawPostData { get; internal set; }

		#region IHttpHandler Members

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void SetBeginTimestamp(long beginTime)
		{
			this.beginTime = beginTime;
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
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

			HandleRequest();

			long end = Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		public abstract void HandleRequest();

		public virtual object Clone()
		{
			return MemberwiseClone();
		}

		#endregion

		private void LogRequest()
		{
			if (logger.IsDebugEnabled)
			{
				Debug.Assert(Request.RemoteEndPoint != null, "Request.RemoteEndPoint != null");
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
				logger.Warn("Parsing multipart data error. Post data written to log\\" + filename);
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
				var parts = contentType.Split(';');
				foreach (var part in parts)
				{
					var idx = part.IndexOf(BOUNDARY);
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
			if (RawPostData != null &&
			    req.ContentType.StartsWith(URL_ENCODED_FORM, StringComparison.CurrentCultureIgnoreCase))
			{
				string postData = Encoding.UTF8.GetString(RawPostData);
				return HttpUtility.ParseQueryString(postData);
			}
			else if (req.HttpMethod.ToUpper().Equals("GET"))
			{
				return HttpUtility.ParseQueryString(Request.Url.Query); //req.QueryString;
			}

			return new NameValueCollection();
		}

		protected void RespondSuccess()
		{
			HttpHelper.RespondSuccess(Response, new CloudResponse(200, DateTime.UtcNow));
		}

		protected void RespondSuccess(object json)
		{
			HttpHelper.RespondSuccess(Response, json);
		}

		protected void RespondSuccess(string contentType, byte[] data)
		{
			Response.StatusCode = 200;
			Response.ContentType = contentType;

			using (var w = new BinaryWriter(Response.OutputStream))
			{
				w.Write(data);
			}
		}
	}


	public class HttpHandlerEventArgs : EventArgs
	{
		public HttpHandlerEventArgs(long durationInTicks)
		{
			DurationInTicks = durationInTicks;
		}

		public long DurationInTicks { get; private set; }
	}
}