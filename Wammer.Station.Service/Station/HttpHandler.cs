using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

using Wammer.MultiPart;

namespace Wammer.Station
{
	public class UploadedFile
	{
		public UploadedFile(string name, byte[] data, string contentType)
		{
			this.Name = name;
			this.Data = data;
			this.ContentType = contentType;
		}

		public string Name { get; private set; }
		public byte[] Data { get; private set; }
		public string ContentType { get; private set; }
	}


	public abstract class HttpHandler : IHttpHandler
	{
		public HttpListenerRequest Request { get; private set; }
		public HttpListenerResponse Response { get; private set; }
		public NameValueCollection Parameters { get; private set; }
		public List<UploadedFile> Files { get; private set; }

		private const string BOUNDARY = "boundary=";
		private const string URL_ENCODED_FORM = "application/x-www-form-urlencoded";
		private const string MULTIPART_FORM = "multipart/form-data";

		protected HttpHandler()
		{
			this.Request = null;
			this.Response = null;
			this.Parameters = null;
			this.Files = new List<UploadedFile>();
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			this.Request = request;
			this.Response = response;
			this.Parameters = InitParameters(request);

			if (HasMultiPartFormData(request))
			{
				string boundary = GetMultipartBoundary(request.ContentType);
				MultiPart.Parser parser = new Parser(boundary);

				Part[] parts = parser.Parse(request.InputStream);
				foreach (Part part in parts)
				{
					if (part.ContentDisposition == null)
						continue;

					ExtractParamsFromMultiPartFormData(part);
				}
			}

			HandleRequest();
		}

		private void ExtractParamsFromMultiPartFormData(Part part)
		{
			Disposition disp = part.ContentDisposition;

			if (disp == null)
				throw new ArgumentException("incorrect use of this function: " +
														"input part.ContentDisposition is null");

			if (disp.Value.ToLower().Equals("form-data"))
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
							request.ContentType.ToLower().StartsWith(MULTIPART_FORM);
		}

		private static string GetMultipartBoundary(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException();

			try {
				int idx = contentType.ToLower().IndexOf(BOUNDARY);
				string boundary = contentType.Substring(idx + BOUNDARY.Length);
				return boundary;
			}
			catch (Exception e)
			{
				throw new FormatException("Error finding multipart boundary. Content-Type: " +
																					contentType, e);
			}
		}

		private static NameValueCollection InitParameters(HttpListenerRequest req)
		{
			if (req.HttpMethod.ToUpper().Equals("POST") &&
				req.ContentType.ToLower().Equals(URL_ENCODED_FORM))
			{
				using (StreamReader s = new StreamReader(req.InputStream))
				{
					string postData = s.ReadToEnd();
					return HttpUtility.ParseQueryString(postData);
				}
			}
			else if (req.HttpMethod.ToUpper().Equals("GET"))
			{
				return req.QueryString;
			}

			return new NameValueCollection();
		}
	}
}
