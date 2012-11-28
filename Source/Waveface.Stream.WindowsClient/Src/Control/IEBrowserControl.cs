using mshtml;
using System.Diagnostics;
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

#if DEBUG
                    ScriptErrorsSuppressed = true,
#endif
          
                    ObjectForScripting = this
                });
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="IEBrowserControl" /> class.
        /// </summary>
        public IEBrowserControl()
        {
            this.Controls.Add(m_Browser);

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
        } 
        #endregion


        #region Public Method
        /// <summary>
        /// Navigates the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        void IBrowserControl.Navigate(string uri)
        {
            m_Browser.Navigate(uri);
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

                Trace.WriteLine(string.Format(
                       "Error: {0}\nline: {1}\nurl: {2}",
                       we.LineNumber,
                       we.Description,
                       we.Url));
            };
        }
        #endregion
    }
}
