using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace UT_WammerStation
{
	class FakeCloud : IDisposable
	{
		private string requestedPath;
		private string postData;
		private string reqeustedContentType;

		//private string response;
		private System.Net.HttpListener listener;
		private List<string> responses = new List<string>();
		private int resIdx = 0;

		public FakeCloud(string response)
		{
			this.responses.Add(response);
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

		public FakeCloud(object response)
		{
			string js = fastJSON.JSON.Instance.ToJSON(response, false, false, false, false);
			this.responses.Add(js);
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

		public void addResponse(object response)
		{
			lock (this.responses)
			{
				if (response is string)
					this.responses.Add((string)response);
				else
					this.responses.Add(fastJSON.JSON.Instance.ToJSON(response));
			}
		}

		private void connected(IAsyncResult result)
		{
			System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
			HttpListenerContext context = null;

			try
			{
				context = listener.EndGetContext(result);
			}
			catch (Exception e)
			{
				return;
			}

			

			requestedPath = context.Request.Url.AbsolutePath;
			reqeustedContentType = context.Request.ContentType;

			using (StreamReader reader = new StreamReader(context.Request.InputStream))
			{
				postData = reader.ReadToEnd();
			}

			context.Response.StatusCode = 200;

			using (StreamWriter w = new StreamWriter(context.Response.OutputStream))
			{
				lock (this.responses)
				{
					if (this.resIdx < this.responses.Count)
					{
						w.Write(this.responses[resIdx++]);
					}
					else
					{
						w.Write(this.responses[this.responses.Count-1]);
					}
				}
			}

			try
			{
				listener.BeginGetContext(this.connected, listener);
			}
			catch (Exception e)
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
