#region

using System;
using System.Windows.Forms;
using mshtml;

#endregion

namespace Waveface.Compoment
{
    public class WebBrowserContextMenuHandler
    {
        private WebBrowser m_webBrowser;
        private ToolStripMenuItem m_ctxMenuCopy;

        public WebBrowserContextMenuHandler(WebBrowser webBrowser, ToolStripMenuItem ctxMenuCopy)
        {
            m_webBrowser = webBrowser;
            m_ctxMenuCopy = ctxMenuCopy;
        }

        private bool IsCommandEnabled(string wCmd)
        {
            IHTMLDocument2 _doc2 = m_webBrowser.Document.DomDocument as IHTMLDocument2;

            if (_doc2 != null)
                return _doc2.queryCommandEnabled(wCmd);

            return false;
        }

        public void UpdateButtons()
        {
            m_ctxMenuCopy.Enabled = IsCommandEnabled("Copy");
        }

        public void CopyCtxMenuClickHandler(object sender, EventArgs e)
        {
            if (sender == m_ctxMenuCopy)
            {
                m_webBrowser.Document.ExecCommand("Copy", false, null);

                UpdateButtons();
            }
        }
    }
}