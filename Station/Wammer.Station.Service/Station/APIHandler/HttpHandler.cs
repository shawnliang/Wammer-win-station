using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Wammer.MultiPart;
using Wammer.Model;
using Wammer.PerfMonitor;
using System.Linq;
using Wammer.Cloud;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Wammer.Station
{
	public class UploadedFile
	{
		public UploadedFile(string name, ArraySegment<byte> data, string contentType)
		{
			this.Name = name;
			this.Data = data;
			this.ContentType = contentType;
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

		public HttpListenerRequest Request { get; internal set; }
		public HttpListenerResponse Response { get; internal set; }
		public NameValueCollection Parameters { get; internal set; }
		public List<UploadedFile> Files { get; private set; }
		public LoginedSession Session { get; set; }
		public byte[] RawPostData { get; internal set; }
		private long beginTime;

		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("HttpHandler");

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		protected HttpHandler()
		{
			this.ProcessSucceeded += HttpRequestMonitor.Instance.OnProcessSucceeded;
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

			var nullArgumentNames = from arguementName in arguementNames
									where Parameters[arguementName] == null
									select arguementName;

			var IsAllParameterReady = !nullArgumentNames.Any();
			if (!IsAllParameterReady)
			{
				throw new FormatException(string.Format("Parameter {0} is null.", string.Join("¡B", nullArgumentNames.ToArray())));
			}
		}

		protected void TunnelToCloud()
		{
			Debug.Assert(this.Request != null);

			var apiPath = Regex.Match(this.Request.Url.LocalPath, API_PATH_MATCH_PATTERN, RegexOptions.IgnoreCase).Groups[API_PATH_GROUP_NAME].Value;
			var forwardParams = Parameters.AllKeys.ToDictionary<string, object, object>(key => key, key => Parameters[key]);
			RespondSuccess(CloudServer.requestPath(new WebClient(), apiPath, forwardParams, false));
		}

		protected void TunnelToCloud<T>()
		{
			Debug.Assert(this.Request != null);

			var apiPath = Regex.Match(this.Request.Url.LocalPath, API_PATH_MATCH_PATTERN, RegexOptions.IgnoreCase).Groups[API_PATH_GROUP_NAME].Value;
			var forwardParams = Parameters.AllKeys.ToDictionary<string, object, object>(key => key, key => Parameters[key]);
			RespondSuccess(CloudServer.requestPath<T>(new WebClient(), apiPath, forwardParams, false));
		}
		#endregion

		public void SetBeginTimestamp(long beginTime)
		{
			this.beginTime = beginTime;
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			this.Files = new List<UploadedFile>();
			this.Request = request;
			this.Response = response;
			this.RawPostData = InitRawPostData();
			this.Parameters = InitParameters(request);

			if (HasMultiPartFormData(request))
			{
				ParseMultiPartData(request);
			}

			LogRequest();

			HandleRequest();

			long end = System.Diagnostics.Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
		}

		private void LogRequest()
		{
			if (logger.IsDebugEnabled)
			{
				Debug.Assert(Request.RemoteEndPoint != null, "Request.RemoteEndPoint != null");
				logger.Debug("====== Request " + Request.Url.AbsolutePath +
								" from " + Request.RemoteEndPoint.Address.ToString() + " ======");
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
			EventHandler<HttpHandlerEventArgs> handler = this.ProcessSucceeded;

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
					w.Write(this.RawPostData);
				}
				logger.Warn("Parsing multipart data error. Post data written to log\\" + filename);
				throw;
			}
		}

		private byte[] InitRawPostData()
		{
			if (string.Compare(Request.HttpMethod, "POST", true) == 0)
			{
				var initialSize = (int)Request.ContentLength64;
				if (initialSize <= 0)
					initialSize = 65535;

				using (var buff = new MemoryStream(initialSize))
				{
					Wammer.Utility.StreamHelper.Copy(Request.InputStream, buff);
					return buff.ToArray();
				}
			}
			else
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
					this.Files.Add(file);
				}
				else
				{
					string name = part.ContentDisposition.Parameters["name"];
					this.Parameters.Add(name, part.Text);
				}
			}
		}

		public abstract void HandleRequest();
		public virtual object Clone()
		{
			return this.MemberwiseClone();
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
			if (this.RawPostData != null &&
				req.ContentType.StartsWith(URL_ENCODED_FORM, StringComparison.CurrentCultureIgnoreCase))
			{
				string postData = Encoding.UTF8.GetString(this.RawPostData);
				return HttpUtility.ParseQueryString(postData);
			}
			else if (req.HttpMethod.ToUpper().Equals("GET"))
			{
				return HttpUtility.ParseQueryString(Request.Url.Query);//req.QueryString;
			}

			return new NameValueCollection();
		}

		protected void RespondSuccess()
		{
			HttpHelper.RespondSuccess(Response, new Cloud.CloudResponse(200, DateTime.UtcNow));
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
		public long DurationInTicks { get; private set; }
		public HttpHandlerEventArgs(long durationInTicks)
		{
			this.DurationInTicks = durationInTicks;
		}
	}
}
