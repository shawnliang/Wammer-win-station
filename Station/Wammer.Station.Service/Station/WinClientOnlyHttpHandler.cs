using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

using Wammer.Cloud;
using Wammer.MultiPart;


namespace Wammer.Station
{
	public class WinClientOnlyHttpHandler : IHttpHandler
	{
		public HttpListenerRequest Request { get; private set; }
		public HttpListenerResponse Response { get; private set; }
		public NameValueCollection Parameters { get; private set; }
		public List<UploadedFile> Files { get; private set; }
		public byte[] RawPostData { get; private set; }

		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		private IHttpHandler bypass = new BypassHttpHandler(CloudServer.BaseUrl);
		private IHttpHandler handler;

		public WinClientOnlyHttpHandler(IHttpHandler handler)
		{
			this.handler = handler;
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

			if (IsWindowsClient(this.Parameters["session_token"]))
			{
				bypass.HandleRequest(request, response);
			}
			else
			{
				handler.HandleRequest(request, response);
			}
		}

		public void SetBeginTimestamp(long beginTime)
		{
			bypass.SetBeginTimestamp(beginTime);
			handler.SetBeginTimestamp(beginTime);
		}

		public object Clone()
		{
			//this.bypass = (IHttpHandler)this.bypass.Clone();
			//this.handler = (IHttpHandler)this.handler.Clone();
			return this.MemberwiseClone();
		}

		public bool IsWindowsClient(string token)
		{
			return true;
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
				this.LogWarnMsg("Parsing multipart data error. Post data written to log\\" + filename);
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

			if (disp.Value.Equals("form-data", StringComparison.CurrentCultureIgnoreCase))
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
				String.Compare(req.ContentType, URL_ENCODED_FORM, true) == 0)
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
	}
}
