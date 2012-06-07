using System;
using System.Net;

namespace Wammer.Utility
{
	public class WebClientEx : WebClient
	{
		#region Var
		private Boolean _allowAutoRedirect = true;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ web request init action.
		/// </summary>
		/// <value>The m_ web request init action.</value>
		private Action<WebRequest> m_WebRequestInitAction { get; set; }
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the request.
		/// </summary>
		/// <value>The request.</value>
		public WebRequest Request { get; private set; }

		public WebResponse Response { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow auto redirect].
		/// </summary>
		/// <value><c>true</c> if [allow auto redirect]; otherwise, <c>false</c>.</value>
		public Boolean AllowAutoRedirect
		{
			get
			{
				var webRequest = (Request as HttpWebRequest);

				if (webRequest == null)
					return _allowAutoRedirect;

				return webRequest.AllowAutoRedirect;
			}
			set
			{
				_allowAutoRedirect = value;
				var webRequest = (Request as HttpWebRequest);

				if (webRequest == null)
					return;

				webRequest.AllowAutoRedirect = value;
			}
		}
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

			var webRequest = (Request as HttpWebRequest);

			if (webRequest != null)
			{
				AllowAutoRedirect = _allowAutoRedirect;

				if (m_WebRequestInitAction != null)
					m_WebRequestInitAction(Request);
			}
			return Request;
		}

		/// <summary>
		/// Returns the <see cref="T:System.Net.WebResponse"/> for the specified <see cref="T:System.Net.WebRequest"/>.
		/// </summary>
		/// <param name="request">A <see cref="T:System.Net.WebRequest"/> that is used to obtain the response.</param>
		/// <returns>
		/// A <see cref="T:System.Net.WebResponse"/> containing the response for the specified <see cref="T:System.Net.WebRequest"/>.
		/// </returns>
		protected override WebResponse GetWebResponse(WebRequest request)
		{
			Response = base.GetWebResponse(request);
			return Response;
		}

		/// <summary>
		/// Returns the <see cref="T:System.Net.WebResponse"/> for the specified <see cref="T:System.Net.WebRequest"/> using the specified <see cref="T:System.IAsyncResult"/>.
		/// </summary>
		/// <param name="request">A <see cref="T:System.Net.WebRequest"/> that is used to obtain the response.</param>
		/// <param name="result">An <see cref="T:System.IAsyncResult"/> object obtained from a previous call to <see cref="M:System.Net.WebRequest.BeginGetResponse(System.AsyncCallback,System.Object)"/> .</param>
		/// <returns>
		/// A <see cref="T:System.Net.WebResponse"/> containing the response for the specified <see cref="T:System.Net.WebRequest"/>.
		/// </returns>
		protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			Response = base.GetWebResponse(request, result);
			return Response;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="range">The range.</param>
		public void AddRange(int range)
		{
			if (Request == null)
			{
				m_WebRequestInitAction = (request) =>
				{
					var webRequest = (request as HttpWebRequest);

					if (webRequest != null)
					{
						webRequest.AddRange(range);
					}
				};
				return;
			}

			(Request as HttpWebRequest).AddRange(range);
		}

		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		public void AddRange(int from, int to)
		{
			if (Request == null)
			{
				m_WebRequestInitAction = (request) =>
				{
					var webRequest = (request as HttpWebRequest);

					if (webRequest != null)
					{
						webRequest.AddRange(from, to);
					}
				};
				return;
			}

			(Request as HttpWebRequest).AddRange(from, to);
		}


		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="rangeSpecifier">The range specifier.</param>
		/// <param name="range">The range.</param>
		public void AddRange(string rangeSpecifier, int range)
		{
			if (Request == null)
			{
				m_WebRequestInitAction = (request) =>
				{
					var webRequest = (request as HttpWebRequest);

					if (webRequest != null)
					{
						webRequest.AddRange(rangeSpecifier, range);
					}
				};
				return;
			}

			(Request as HttpWebRequest).AddRange(rangeSpecifier, range);
		}


		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="rangeSpecifier">The range specifier.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		public void AddRange(string rangeSpecifier, int from, int to)
		{
			if (Request == null)
			{
				m_WebRequestInitAction = (request) =>
				{
					var webRequest = (request as HttpWebRequest);

					if (webRequest != null)
					{
						webRequest.AddRange(rangeSpecifier, from, to);
					}
				};
				return;
			}

			(Request as HttpWebRequest).AddRange(rangeSpecifier, from, to);
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
			var httpReq = Request as HttpWebRequest;
			if (httpReq != null)
			{
				httpReq.Timeout = Timeout;
				httpReq.ReadWriteTimeout = ReadWriteTimeout;
			}
		}
	}
}