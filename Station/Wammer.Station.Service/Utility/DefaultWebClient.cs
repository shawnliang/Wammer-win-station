using System;
using System.Net;
using System.IO;
using System.ComponentModel;
using log4net;

namespace Wammer.Utility
{
	public class WebClientEx : WebClient
	{

		private static readonly ILog logger = LogManager.GetLogger("WebClientEx");

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

		/// <summary>
		/// Downloads the file.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="acceptResumeDownload">if set to <c>true</c> [accept resume download].</param>
		/// <param name="progressChangedCallBack">The progress changed call back.</param>
		public void DownloadFile(string address, string fileName, Boolean acceptResumeDownload, Action<object, ProgressChangedEventArgs>
																progressChangedCallBack)
		{			
			if(acceptResumeDownload == false)
			{
				DownloadFile(address, fileName);
				return;
			}

			const int CHECK_BYTE_COUNT = 3;
			const int BUFFER_SIZE = 1024;

			using (Stream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				var offset = fileStream.Length;
				
				if (offset > 0)
					logger.Info("Detect existed file, resume download \"" + fileName + "\"");

				if (offset > CHECK_BYTE_COUNT)
					offset -= CHECK_BYTE_COUNT;

				AddRange((int)offset);				
								
				using (var from = OpenRead(address))
				{
					if (offset > 0)
					{
						fileStream.Seek(offset, SeekOrigin.Begin);

						var remoteBuffer = new byte[CHECK_BYTE_COUNT];
						var localBuffer = new byte[CHECK_BYTE_COUNT];

						if (from.Read(remoteBuffer, 0, CHECK_BYTE_COUNT) != fileStream.Read(localBuffer, 0, CHECK_BYTE_COUNT))
							throw new Exception("Can't resume download specified file.");

						for (int idx = 0; idx < CHECK_BYTE_COUNT; ++idx)
						{
							if (localBuffer[idx] != remoteBuffer[idx])
								throw new Exception("Can't resume download specified file.");
						}
					}

					from.WriteTo(fileStream, 1024, progressChangedCallBack);

					if (fileStream != null) fileStream.Dispose();
				}
			}
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