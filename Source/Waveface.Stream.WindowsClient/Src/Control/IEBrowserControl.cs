using log4net;
using mshtml;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[ComVisible(true)]
	public class IEBrowserControl : Control, IBrowserControl
	{
		#region Var
		private WebBrowser _browser;
		private string _uri;
		private Boolean _isDebugMode;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ browser.
		/// </summary>
		/// <value>
		/// The m_ browser.
		/// </value>
		public WebBrowser m_Browser
		{
			get
			{
				return _browser ?? (_browser = new WebBrowser()
				{
					Dock = DockStyle.Fill,

					IsWebBrowserContextMenuEnabled = false,
					WebBrowserShortcutsEnabled = false,
					AllowWebBrowserDrop = false,
					ScriptErrorsSuppressed = true,


					ObjectForScripting = this
				});
			}
		}
		#endregion


		#region Public Property
		public string Uri
		{
			get
			{
				return _uri;
			}
			set
			{
				if (_uri == value)
					return;

				m_Browser.Navigate(value);
				_uri = value;
			}
		}

		public Boolean IsDebugMode
		{
			get
			{
				return _isDebugMode;
			}
			set
			{
				if (_isDebugMode == value)
					return;

				OnDebugModeChanging(EventArgs.Empty);
				_isDebugMode = value;
				OnDebugModeChanged(EventArgs.Empty);
			}
		}
		#endregion



		#region Event
		private event EventHandler DebugModeChanging;
		private event EventHandler DebugModeChanged;
		#endregion



		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="IEBrowserControl" /> class.
		/// </summary>
		public IEBrowserControl()
		{
			this.Controls.Add(m_Browser);

			this.DebugModeChanged += IEBrowserControl_DebugModeChanged;

			m_Browser.DocumentCompleted += m_Browser_DocumentCompleted;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Injects the hack code.
		/// </summary>
		private void InjectHackCode()
		{
			var head = m_Browser.Document.GetElementsByTagName("head")[0];
			var script = m_Browser.Document.CreateElement("script");
			var element = (IHTMLScriptElement)script.DomElement;
			element.text = @"if(navigator.userAgent.match(""MSIE 10.0"")){if(XMLHttpRequest !== undefined ) XMLHttpRequest = undefined;} // Hack the ie 10";
			head.AppendChild(script);

			if (IsDebugMode)
			{
				var htmlTag = m_Browser.Document.GetElementsByTagName("html")[0];
				htmlTag.SetAttribute("debug", "true");
				script = m_Browser.Document.CreateElement("script");
				script.SetAttribute("src", @"https://getfirebug.com/firebug-lite.js");
				head.AppendChild(script);
			}
		}
		#endregion


		#region Protected Method
		protected void OnDebugModeChanging(EventArgs e)
		{
			this.RaiseEvent(DebugModeChanging, e);
		}

		protected void OnDebugModeChanged(EventArgs e)
		{
			this.RaiseEvent(DebugModeChanged, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Navigates the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		void IBrowserControl.Navigate(string uri)
		{
			this.Uri = uri;
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the DocumentCompleted event of the IEBrowserControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="WebBrowserDocumentCompletedEventArgs" /> instance containing the event data.</param>
		void m_Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			InjectHackCode();

			m_Browser.Document.Window.Error += (w, we) =>
			{
				we.Handled = true;

				LogManager.GetLogger(this.GetType()).ErrorFormat("Error: {0}\nline: {1}\nurl: {2}", we.LineNumber, we.Description, we.Url);
			};
		}

		/// <summary>
		/// Handles the DebugModeChanged event of the IEBrowserControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		void IEBrowserControl_DebugModeChanged(object sender, EventArgs e)
		{
			m_Browser.IsWebBrowserContextMenuEnabled = IsDebugMode;
			m_Browser.WebBrowserShortcutsEnabled = IsDebugMode;
			m_Browser.AllowWebBrowserDrop = IsDebugMode;
			m_Browser.ScriptErrorsSuppressed = !IsDebugMode;
		}
		#endregion
	}
}
