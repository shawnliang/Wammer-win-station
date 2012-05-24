using System;
using System.Net;

namespace Wammer.Utility
{
	public class WebClientEx : WebClient
	{
		#region Public Property
		public WebRequest Request { get; private set; }
		#endregion

		#region Protected Method
		/// <summary>
		/// Returns a <see cref="T:System.Net.WebRequest"/> object for the specified resource.
		/// </summary>
		/// <param name="address">A <see cref="T:System.Uri"/> that identifies the resource to request.</param>
		/// <returns>
		/// A new <see cref="T:System.Net.WebRequest"/> object for the specified resource.
		/// </returns>
		protected override WebRequest GetWebRequest(Uri address)
		{
			Request = base.GetWebRequest(address);
			return Request;
		} 
		#endregion
	}

	public class DefaultWebClient : WebClientEx
	{
		private int readWriteTimeout;
		private int timeout;

		static DefaultWebClient()
		{
			DefaultTimeout = 20*1000;
			DefaultReadWriteTimeout = 10*1000;
		}

		public int Timeout
		{
			get { return (timeout == 0) ? DefaultTimeout : timeout; }

			set { timeout = value; }
		}

		public int ReadWriteTimeout
		{
			get { return (readWriteTimeout == 0) ? DefaultReadWriteTimeout : readWriteTimeout; }

			set { readWriteTimeout = value; }
		}

		public static int DefaultTimeout { get; set; }
		public static int DefaultReadWriteTimeout { get; set; }

		public DefaultWebClient()
		{
			var httpReq = this.Request as HttpWebRequest;
			if (httpReq != null)
			{
				httpReq.Timeout = Timeout;
				httpReq.ReadWriteTimeout = ReadWriteTimeout;
			}
		}
	}
}