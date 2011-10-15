using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Wammer.Cloud
{
	public class WammerCloudException : Exception
	{
		private WebExceptionStatus httpError = WebExceptionStatus.Success;
		private int wammerError = 0;
		private string request = null;
		private string response = null;
		public WammerCloudException()
			: base()
		{
		}

		public WammerCloudException(string msg)
			: base(msg)
		{
		}

		public WammerCloudException(string msg, string request, string response, int wammerError)
			: base(msg)
		{
			this.request = request;
			this.response = response;
			this.wammerError = wammerError;
		}

		public WammerCloudException(string msg, string request, string response, Exception innerException)
			: base(msg, innerException)
		{
			this.request = request;
			this.response = response;
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

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder(base.ToString());
			if (this.request != null)
			{
				buf.AppendLine("=== request ===");
				buf.AppendLine(this.request);
			}

			if (this.response != null)
			{
				buf.AppendLine("--- response ---");
				buf.AppendLine(this.response);
			}

			return buf.ToString();
		}
	}
}
