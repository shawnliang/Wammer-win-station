using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

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
		protected readonly Dictionary<string, byte[]> files;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			this.Request = request;
			this.Response = response;
			this.Parameters = GetRequestParams(request);

			HandleRequest();
		}

		protected abstract void HandleRequest();


		private static NameValueCollection GetRequestParams(HttpListenerRequest req)
		{
			if (req.HttpMethod.ToUpper().Equals("POST"))
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
			else
				throw new NotSupportedException(
									"Method is not support: " + req.HttpMethod);
		}
	}
}
