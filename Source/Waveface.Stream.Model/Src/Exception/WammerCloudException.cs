using fastJSON;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Waveface.Stream.Model
{
	public class WammerCloudException : Exception
	{
		private readonly WebExceptionStatus httpError = WebExceptionStatus.Success;
		private readonly int wammerError;

		public WammerCloudException()
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
				httpError = webException.Status;
			}

			this.response = response;
			wammerError = TryParseWammerError(this.response);
		}

		public WammerCloudException(string msg, WebException innerException)
			: base(msg, innerException)
		{
			response = GetErrResponseText(innerException);
			wammerError = TryParseWammerError(response);
			httpError = innerException.Status;
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
			: this(msg, innerException)
		{
			this.httpError = httpError;
			this.wammerError = wammerError;
		}

		public string response { get; private set; }

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
				var res = JSON.Instance.ToObject<CloudResponse>(resText);
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
			if (response != null)
			{
				buf.AppendLine("--- response ---");
				buf.AppendLine(response);
			}

			return buf.ToString();
		}

		public string GetCloudRetMsg()
		{
			if (string.IsNullOrEmpty(this.response))
				return "---Unknown error---";
			else
			{
				var res = fastJSON.JSON.Instance.ToObject<CloudResponse>(response);
				return res.api_ret_message;
			}

		}
	}
}
