#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class WebLink_DV : UserControl, IDetailView
    {
        private IContainer components;
        private Panel panelMain;
        private Post m_post;
        private Localization.CultureManager cultureManager;
        private WebBrowserContextMenuHandler m_soulBrowserContextMenuHandler;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem miCopySoul;
        private bool m_showCancelledNavigationMessage;
        private WebBrowser webBrowser;
        private bool m_addedLinkClickEventHandler;
        private List<string> m_clickableURL;
        private bool m_canOpenNewWindow;
        private ContextMenuStrip contextMenuStripMore;
        private ToolStripMenuItem miOpenInWebBrowser;
        private bool m_canEdit;

        public Post Post
        {
            get
            {
                return m_post;
            }
            set
            {
                m_post = value;

                if (m_post != null)
                {
                    m_canEdit = false;

                    SetHTML();
                }
            }
        }

        public User User { get; set; }

        public DetailView MyParent { get; set; }

        public WebLink_DV()
        {
            InitializeComponent();

            Visible = false;

            m_clickableURL = new List<string>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        // private void InitializeComponent()
        // {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebLink_DV));
            this.panelMain = new System.Windows.Forms.Panel();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopySoul = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMore = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miOpenInWebBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripMore.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelMain.Controls.Add(this.webBrowser);
            this.panelMain.Name = "panelMain";
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.CausesValidation = false;
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 18);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser_NewWindow);
            this.webBrowser.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowser_ProgressChanged);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopySoul});
            this.contextMenuStrip.Name = "contextMenuStripTop";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // miCopySoul
            // 
            this.miCopySoul.Name = "miCopySoul";
            resources.ApplyResources(this.miCopySoul, "miCopySoul");
            // 
            // contextMenuStripMore
            // 
            this.contextMenuStripMore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenInWebBrowser});
            this.contextMenuStripMore.Name = "contextMenuStripTop";
            resources.ApplyResources(this.contextMenuStripMore, "contextMenuStripMore");
            // 
            // miOpenInWebBrowser
            // 
            this.miOpenInWebBrowser.Image = global::Waveface.Properties.Resources.FB_openin;
            this.miOpenInWebBrowser.Name = "miOpenInWebBrowser";
            resources.ApplyResources(this.miOpenInWebBrowser, "miOpenInWebBrowser");
            this.miOpenInWebBrowser.Click += new System.EventHandler(this.miOpenInWebBrowser_Click);
            // 
            // WebLink_DV
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "WebLink_DV";
            this.panelMain.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripMore.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public bool CanEdit()
        {
            return m_canEdit;
        }

        private void SetHTML()
        {
            m_canEdit = true;

            StringBuilder _sb = new StringBuilder();

            _sb.Append("<font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' color='#eef'><p>[Text]</p></font>");

            string _htmlMainAndComment = _sb.ToString();

            string _content = HttpUtility.HtmlEncode(Post.content);
            _content = _content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");
            _content = _content.Replace("\r", "<BR>");

            _htmlMainAndComment = _htmlMainAndComment.Replace("[Text]", _content);

            _htmlMainAndComment += MyParent.GenCommentHTML(Post, (Post.preview.url != null));

            _htmlMainAndComment = HtmlUtility.MakeLink(_htmlMainAndComment, m_clickableURL);

            if (Post.preview.url != null)
            {
                Preview_OpenGraph _p = Post.preview;
                StringBuilder _s = new StringBuilder();

                _s.Append("<table width=\"99%\" border=\"0\">");

                _s.Append(" <tr>");
                _s.Append("     <td><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' size='5pt'><b><a style=\"text-decoration:none\" href=\"[PriviewLink]\" target=\"_blank\">[PriviewTitle]</a></b></font></td>");
                _s.Append(" </tr>");

                _s.Append(" <tr>");
                _s.Append("     <td><font size='2.75pt'>[PriviewLink2]</font></td>");
                _s.Append(" </tr>");

                _s.Append("</table>");

                _htmlMainAndComment += _s.ToString();

                _htmlMainAndComment = _htmlMainAndComment.Replace("[PreviewPic]", _p.thumbnail_url);
                _htmlMainAndComment = _htmlMainAndComment.Replace("[PriviewTitle]", _p.title);
                _htmlMainAndComment = _htmlMainAndComment.Replace("[PriviewLink]", _p.url);
                _htmlMainAndComment = _htmlMainAndComment.Replace("[PriviewLink2]", StringUtility.ExtractDomainNameFromURL(_p.url));
                _htmlMainAndComment = _htmlMainAndComment.Replace("[PriviewText]", _p.description);

                if (!m_clickableURL.Contains(_p.url))
                {
                    m_clickableURL.Add(_p.url);
                }
            }

            string _minimaxJS = Properties.Resources.minmax;
            _minimaxJS = "<script type=\"text/javascript\">" + _minimaxJS + "</script>";

            string _wfPreviewWin = Properties.Resources.WFPreviewWin;
            _wfPreviewWin = "<style type=\"text/css\">" + _wfPreviewWin + "</style>";

            webBrowser.DocumentText = "<html>" + _minimaxJS +
                                          "<style type=\"text/css\">img {height: auto; max-width: 95%;}</style>" +
                                          "<body bgcolor=\"rgb(255, 255, 255)\"><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif'>" +
                                          HtmlUtility.TrimScript(_htmlMainAndComment + m_post.soul) +
                                          "</font></body></html>";
        }

        private void webBrowser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            Visible = true;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!m_addedLinkClickEventHandler)
            {
                m_addedLinkClickEventHandler = true;

                for (int i = 0; i < webBrowser.Document.Links.Count; i++)
                {
                    webBrowser.Document.Links[i].Click += LinkClick;
                }
            }

            m_soulBrowserContextMenuHandler = new WebBrowserContextMenuHandler(webBrowser, miCopySoul);
            contextMenuStrip.Opening += contextMenuStrip_Opening;
            miCopySoul.Click += m_soulBrowserContextMenuHandler.CopyCtxMenuClickHandler;
            webBrowser.Document.ContextMenuShowing += webBrowser_ContextMenuShowing;
        }

        private void LinkClick(object sender, EventArgs e)
        {
            string _text1 = string.Empty;
            string _text2 = string.Empty;

            string _text = ((HtmlElement)sender).OuterHtml;
            Regex _r = new Regex(HtmlUtility.URL_RegExp_Pattern, RegexOptions.None);

            MatchCollection _ms = _r.Matches(_text);

            if (_ms.Count > 0)
                _text1 = _ms[0].Value;

            if (_ms.Count > 1)
                _text2 = _ms[1].Value;

            if (m_clickableURL.Contains(_text1) || m_clickableURL.Contains(_text2))
                m_canOpenNewWindow = true;
            else
                m_canOpenNewWindow = false;

            m_showCancelledNavigationMessage = true;

            // MessageBox.Show("Link Was Clicked Navigation was Cancelled", "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (m_showCancelledNavigationMessage)
            {
                e.Cancel = true;

                m_showCancelledNavigationMessage = false;
            }
        }

        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            if (!m_canOpenNewWindow)
                e.Cancel = true;
        }

        void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            m_soulBrowserContextMenuHandler.UpdateButtons();
        }

        void webBrowser_ContextMenuShowing(object sender, HtmlElementEventArgs e)
        {
            contextMenuStrip.Show(webBrowser.PointToScreen(e.MousePosition));

            e.ReturnValue = false;
        }

        public List<ToolStripMenuItem> GetMoreMenuItems()
        {
            List<ToolStripMenuItem> _items = new List<ToolStripMenuItem>();
            _items.Add(miOpenInWebBrowser);

            return _items;
        }

        private void miOpenInWebBrowser_Click(object sender, EventArgs e)
        {
            string _browser = WebBrowserUtility.GetSystemDefaultBrowser();

            Process _p = new Process();

            if (!string.IsNullOrEmpty(_browser))
                _p.StartInfo.FileName = _browser;

            _p.StartInfo.Arguments = Post.preview.url;

            _p.Start();
        }
    }
}