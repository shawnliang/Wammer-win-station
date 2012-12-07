using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Wammer.Utility;

namespace UT_WammerStation
{
	interface IResponseWiter
	{
		void writeResponse(HttpListenerResponse response);
	}

	class JsonResponseWriter : IResponseWiter
	{
		public string json { get; set; }
		public int status { get; set; }

		public JsonResponseWriter()
		{
			status = 200;
		}

		public JsonResponseWriter(string json)
		{
			this.json = json;
			status = 200;
		}

		public void writeResponse(HttpListenerResponse response)
		{
			response.StatusCode = status;
			response.ContentType = "application/json";
			StreamWriter w = new StreamWriter(response.OutputStream);
			w.Write(json);
			w.Flush();
		}
	}

	class RawDataResponseWriter : IResponseWiter
	{
		public byte[] RawData { get; set; }
		public string ContentType { get; set; }

		public void writeResponse(HttpListenerResponse response)
		{
			response.ContentType = ContentType;
			response.OutputStream.Write(RawData, 0, RawData.Length);
		}
	}

	class FakeCloud : IDisposable
	{
		private string requestedPath;
		private string postData;
		private string reqeustedContentType;

		private System.Net.HttpListener listener;
		private List<IResponseWiter> resWriters = new List<IResponseWiter>();
		private int resIdx = 0;

		public FakeCloud(IResponseWiter writer)
		{
			this.resWriters.Add(writer);
			this.listener = new System.Net.HttpListener();
			this.listener.Prefixes.Add("http://+:80/");

			// If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
			// you might need to reserve a namespace. Run a console window as Admin, and type something like
			//    netsh http add urlacl http://+:80/ user=domain\user
			// See this page:
			//    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
			this.listener.Start();
			this.listener.BeginGetContext(this.connected, listener);
		}

		public FakeCloud(string json)
		{
			this.resWriters.Add(new JsonResponseWriter(json));
			this.listener = new System.Net.HttpListener();
			this.listener.Prefixes.Add("http://+:80/");

			// If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
			// you might need to reserve a namespace. Run a console window as Admin, and type something like
			//    netsh http add urlacl http://+:80/ user=domain\user
			// See this page:
			//    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
			this.listener.Start();
			this.listener.BeginGetContext(this.connected, listener);
		}

		public FakeCloud(object json)
		{
			string js = json.ToFastJSON();
			this.resWriters.Add(new JsonResponseWriter(js));
			this.listener = new System.Net.HttpListener();
			this.listener.Prefixes.Add("http://+:80/");

			// If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
			// you might need to reserve a namespace. Run a console window as Admin, and type something like
			//    netsh http add urlacl http://+:80/ user=domain\user
			// See this page:
			//    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
			this.listener.Start();
			this.listener.BeginGetContext(this.connected, listener);
		}

		public FakeCloud(object json, int status)
		{
			string js = json.ToFastJSON();
			JsonResponseWriter w = new JsonResponseWriter(js);
			w.status = status;
			this.resWriters.Add(w);
			this.listener = new System.Net.HttpListener();
			this.listener.Prefixes.Add("http://+:80/");

			// If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
			// you might need to reserve a namespace. Run a console window as Admin, and type something like
			//    netsh http add urlacl http://+:80/ user=domain\user
			// See this page:
			//    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
			this.listener.Start();
			this.listener.BeginGetContext(this.connected, listener);
		}

		public void addJsonResponse(object response)
		{
			lock (this.resWriters)
			{
				if (response is string)
					this.resWriters.Add(new JsonResponseWriter((string)response));
				else
					this.resWriters.Add(
						new JsonResponseWriter(response.ToFastJSON()));
			}
		}

		private void connected(IAsyncResult result)
		{
			System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
			HttpListenerContext context = null;

			try
			{
				context = listener.EndGetContext(result);

				requestedPath = context.Request.Url.AbsolutePath;
				reqeustedContentType = context.Request.ContentType;

				using (StreamReader reader = new StreamReader(context.Request.InputStream))
				{
					postData = reader.ReadToEnd();
				}
			}
			catch (Exception)
			{
				return;
			}

			lock (this.resWriters)
			{
				if (resIdx < this.resWriters.Count)
				{
					resWriters[resIdx].writeResponse(context.Response);
				}
				else
				{
					resWriters[resWriters.Count - 1].writeResponse(context.Response);
				}

				resIdx++;
			}

			context.Response.OutputStream.Close();
			context.Response.Close();

			try
			{
				listener.BeginGetContext(this.connected, listener);
			}
			catch (Exception)
			{
				return;
			}
		}

		public string RequestedPath
		{
			get { return requestedPath; }
		}

		public string PostData
		{
			get { return postData; }
		}

		public string RequestedContentType
		{
			get { return reqeustedContentType; }
		}

		public void Dispose()
		{
			this.listener.Stop();
			this.listener.Close();
		}
	}

	class FakeErrorCloud : IDisposable
	{
		private string requestedPath;
		private int response;
		private System.Net.HttpListener listener;

		public FakeErrorCloud(int errStatus)
		{
			this.response = errStatus;
			this.listener = new System.Net.HttpListener();
			this.listener.Prefixes.Add("http://+:80/");

			// If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
			// you might need to reserve a namespace. Run a console window as Admin, and type something like
			//    netsh http add urlacl http://+:80/ user=domain\user
			// See this page:
			//    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
			this.listener.Start();
			this.listener.BeginGetContext(this.connected, listener);
		}

		private void connected(IAsyncResult result)
		{
			System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
			HttpListenerContext context = listener.EndGetContext(result);
			requestedPath = context.Request.Url.AbsolutePath;
			context.Response.StatusCode = this.response;

			context.Response.OutputStream.Close();
		}

		public string RequestedPath
		{
			get { return requestedPath; }
		}

		public void Dispose()
		{
			this.listener.Close();
		}
	}
}
