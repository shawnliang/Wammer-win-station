using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Wammer.MultiPart;
using Wammer.Model;
using System.Linq;

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
		public HttpListenerRequest Request { get; private set; }
		public HttpListenerResponse Response { get; private set; }
		public NameValueCollection Parameters { get; private set; }
		public List<UploadedFile> Files { get; private set; }
		public LoginedSession Session { get; set; }
		public byte[] RawPostData { get; private set; }
		private long beginTime;

		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";

		private static log4net.ILog logger = log4net.LogManager.GetLogger("HttpHandler");

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;
		
		protected HttpHandler()
		{
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

			if (logger.IsDebugEnabled)
			{
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
					}
				}
				foreach (UploadedFile file in Files)
					logger.DebugFormat("file: {0}, mime: {1}, size: {2}", file.Name, file.ContentType, file.Data.Count.ToString());
			}

			HandleRequest();

			long end = System.Diagnostics.Stopwatch.GetTimestamp();

			long duration = end - beginTime;
			if (duration < 0)
				duration += long.MaxValue;

			OnProcessSucceeded(new HttpHandlerEventArgs(duration));
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
				MultiPart.Parser parser = new Parser(boundary);

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
				using (BinaryWriter w = new BinaryWriter(File.OpenWrite(@"log\" + filename)))
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
				int initialSize = (int)Request.ContentLength64;
				if (initialSize <= 0)
					initialSize = 65535;

				using (MemoryStream buff = new MemoryStream(initialSize))
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

			if (disp.Value.Equals("form-data",StringComparison.CurrentCultureIgnoreCase))
			{
				string filename = disp.Parameters["filename"];

				if (filename != null)
				{
					UploadedFile file = new UploadedFile(filename, part.Bytes,
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

		protected abstract void HandleRequest();
		public abstract object Clone();

		private static bool HasMultiPartFormData(HttpListenerRequest request)
		{
			return request.ContentType != null &&
							request.ContentType.StartsWith(MULTIPART_FORM,StringComparison.CurrentCultureIgnoreCase);
		}

		private static string GetMultipartBoundary(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException();

			try {
				string[] parts = contentType.Split(';');
				foreach(string part in parts)
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
				return req.QueryString;
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

			using (BinaryWriter w = new BinaryWriter(Response.OutputStream))
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
