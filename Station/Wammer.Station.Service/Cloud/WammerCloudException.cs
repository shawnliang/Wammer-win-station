using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Wammer.Cloud
{
	public class WammerCloudException : Exception
	{
		private readonly WebExceptionStatus httpError = WebExceptionStatus.Success;
		private readonly int wammerError;
		public string response { get; private set; }
		public WammerCloudException()
			: base()
		{
		}

		public WammerCloudException(string msg)
			: base(msg)
		{
		}

		public WammerCloudException(string msg, string response, int wammerError)
			: base(msg)
		{
			this.response = response;
			this.wammerError = wammerError;
		}

		public WammerCloudException(string msg, string response, Exception innerException)
			: base(msg, innerException)
		{
			var webException = innerException as WebException;
			if (webException != null)
			{
				this.httpError = webException.Status;
			}

			this.response = response;
			this.wammerError = TryParseWammerError(this.response);
		}

		public WammerCloudException(string msg, WebException innerException)
			:base(msg, innerException)
		{
			this.response = GetErrResponseText(innerException);
			this.wammerError = TryParseWammerError(this.response);
			this.httpError = innerException.Status;
		}

		public WammerCloudException(string msg, WebExceptionStatus httpError, int wammerError)
			: base(msg)
		{
			this.httpError = httpError;
			this.wammerError = wammerError;
		}

		public WammerCloudException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public WammerCloudException(string msg, WebExceptionStatus httpError, int wammerError, Exception innerException)
			: base(msg, innerException)
		{
			this.httpError = httpError;
			this.wammerError = wammerError;
		}

		public WebExceptionStatus HttpError
		{
			get { return httpError; }
		}

		public int WammerError
		{
			get { return wammerError; }
		}

		private static string GetErrResponseText(WebException e)
		{
			try
			{
				Debug.Assert(e != null, "e != null");
				using (var r = new StreamReader(e.Response.GetResponseStream()))
				{
					return r.ReadToEnd();
				}
			}
			catch
			{
				// don't care if error response is unavailable
				return null;
			}
		}

		private static int TryParseWammerError(string resText)
		{
			if (resText == null)
				return -1;

			try
			{
				var res = fastJSON.JSON.Instance.ToObject<CloudResponse>(resText);
				return res.api_ret_code;
			}
			catch
			{
				return -1;
			}
		}

		public override string ToString()
		{
			var buf = new StringBuilder(base.ToString());
			buf.AppendLine();
			if (this.response != null)
			{
				buf.AppendLine("--- response ---");
				buf.AppendLine(this.response);
			}

			return buf.ToString();
		}
	}
}
