#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component;
using mshtml;

#endregion

namespace Waveface.DetailUI
{
    public class WebLink_DV : UserControl
    {
        private IContainer components;
        private Panel panelMain;
        private AutoScrollPanel panelRight;
        private WebBrowser webBrowserTop;
        private Post m_post;
        private Panel panelWebBrowser;
        private WebBrowser webBrowserSoul;
        private Localization.CultureManager cultureManager;
        private ContextMenuStrip contextMenuStripTop;
        private ToolStripMenuItem miCopyTop;
        private WebBrowserContextMenuHandler m_topBrowserContextMenuHandler;
        private WebBrowserContextMenuHandler m_soulBrowserContextMenuHandler;
        private ContextMenuStrip contextMenuStripSoul;
        private ToolStripMenuItem miCopySoul;


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
                    RefreshUI();
            }
        }

        public User User { get; set; }

        public DetailView MyParent { get; set; }

        public WebLink_DV()
        {
            InitializeComponent();
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
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStripTop = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyTop = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripSoul = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopySoul = new System.Windows.Forms.ToolStripMenuItem();
            this.panelRight = new Waveface.Component.AutoScrollPanel();
            this.panelWebBrowser = new System.Windows.Forms.Panel();
            this.webBrowserSoul = new System.Windows.Forms.WebBrowser();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.panelMain.SuspendLayout();
            this.contextMenuStripTop.SuspendLayout();
            this.contextMenuStripSoul.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelWebBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Name = "panelMain";
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // contextMenuStripTop
            // 
            this.contextMenuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyTop});
            this.contextMenuStripTop.Name = "contextMenuStripTop";
            resources.ApplyResources(this.contextMenuStripTop, "contextMenuStripTop");
            // 
            // miCopyTop
            // 
            this.miCopyTop.Name = "miCopyTop";
            resources.ApplyResources(this.miCopyTop, "miCopyTop");
            // 
            // contextMenuStripSoul
            // 
            this.contextMenuStripSoul.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopySoul});
            this.contextMenuStripSoul.Name = "contextMenuStripTop";
            resources.ApplyResources(this.contextMenuStripSoul, "contextMenuStripSoul");
            // 
            // miCopySoul
            // 
            this.miCopySoul.Name = "miCopySoul";
            resources.ApplyResources(this.miCopySoul, "miCopySoul");
            // 
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelRight.Controls.Add(this.panelWebBrowser);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // panelWebBrowser
            // 
            this.panelWebBrowser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelWebBrowser.Controls.Add(this.webBrowserSoul);
            resources.ApplyResources(this.panelWebBrowser, "panelWebBrowser");
            this.panelWebBrowser.Name = "panelWebBrowser";
            // 
            // webBrowserSoul
            // 
            this.webBrowserSoul.AllowNavigation = false;
            resources.ApplyResources(this.webBrowserSoul, "webBrowserSoul");
            this.webBrowserSoul.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserSoul.Name = "webBrowserSoul";
            this.webBrowserSoul.ScriptErrorsSuppressed = true;
            this.webBrowserSoul.ScrollBarsEnabled = false;
            this.webBrowserSoul.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserSoul_DocumentCompleted);
            // 
            // webBrowserTop
            // 
            this.webBrowserTop.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserTop, "webBrowserTop");
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // WebLink_DV
            // 
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "WebLink_DV";
            this.Resize += new System.EventHandler(this.WebLink_DV_Resize);
            this.panelMain.ResumeLayout(false);
            this.contextMenuStripTop.ResumeLayout(false);
            this.contextMenuStripSoul.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelWebBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /*
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.PageDown:
                    panelRight.VerticalScroll.Value = panelRight.VerticalScroll.Value + 64;
                    return true;

                case Keys.PageUp:
                    panelRight.VerticalScroll.Value = panelRight.VerticalScroll.Value - 64;
                    return true;

                case Keys.Down:
                    panelRight.VerticalScroll.Value = panelRight.VerticalScroll.Value + 16;
                    return true;

                case Keys.Up:
                    panelRight.VerticalScroll.Value = panelRight.VerticalScroll.Value - 16;
                    return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }
        */

        private void RefreshUI()
        {
            Set_MainContent_Preview_Part();
        }

        private void Set_MainContent_Preview_Part()
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' color='#eef'><p>[Text]</p></font>");

            string _html = _sb.ToString();

            string _content = Post.content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");

            _html = _html.Replace("[Text]", _content);

            _html += MyParent.GenCommentHTML(Post);

            _html = HtmlUtility.MakeLink(_html);

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

                _html += _s.ToString();

                _html = _html.Replace("[PreviewPic]", _p.thumbnail_url);
                _html = _html.Replace("[PriviewTitle]", _p.title);
                _html = _html.Replace("[PriviewLink]", _p.url);
                _html = _html.Replace("[PriviewLink2]", StringUtility.ExtractDomainNameFromURL(_p.url));
                _html = _html.Replace("[PriviewText]", _p.description);
            }

            webBrowserTop.DocumentText = HtmlUtility.TrimScript("<body bgcolor=\"rgb(243, 242, 238)\">" + _html + "</body>");
            webBrowserSoul.DocumentText = "<style type=\"text/css\">img {width: 50%;}</style>" + HtmlUtility.TrimScript("<body bgcolor=\"rgb(243, 242, 238)\"><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif'>" + m_post.soul + "</font></body>");
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;

            m_topBrowserContextMenuHandler = new WebBrowserContextMenuHandler(webBrowserTop, miCopyTop);
            contextMenuStripTop.Opening += contextMenuStripTop_Opening;
            miCopyTop.Click += m_topBrowserContextMenuHandler.CopyCtxMenuClickHandler;
            webBrowserTop.Document.ContextMenuShowing += webBrowserTop_ContextMenuShowing;
        }

        private void webBrowserSoul_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            changeWebBrowserSoulSize();

            if (m_post.soul.Trim() != string.Empty)
                panelWebBrowser.Visible = true;

            m_soulBrowserContextMenuHandler = new WebBrowserContextMenuHandler(webBrowserSoul, miCopySoul);
            contextMenuStripSoul.Opening += contextMenuStripSoul_Opening;
            miCopySoul.Click += m_soulBrowserContextMenuHandler.CopyCtxMenuClickHandler;
            webBrowserSoul.Document.ContextMenuShowing += webBrowserSoul_ContextMenuShowing;
        }

        private void changeWebBrowserSoulSize()
        {
            int _h = webBrowserSoul.Document.Body.ScrollRectangle.Height;
            int _w = webBrowserSoul.Document.Body.ScrollRectangle.Width;

            panelWebBrowser.Height = _h + 4;
            webBrowserSoul.Height = _h;

            //panelWebBrowser.Width = _w + 4;
            //webBrowserSoul.Width = _w;
        }

        #region ContextMenu

        void contextMenuStripTop_Opening(object sender, CancelEventArgs e)
        {
            m_topBrowserContextMenuHandler.UpdateButtons();
        }

        void webBrowserTop_ContextMenuShowing(object sender, HtmlElementEventArgs e)
        {
            contextMenuStripTop.Show(webBrowserTop.PointToScreen(e.MousePosition));
            e.ReturnValue = false;
        }

        void contextMenuStripSoul_Opening(object sender, CancelEventArgs e)
        {
            m_soulBrowserContextMenuHandler.UpdateButtons();
        }

        void webBrowserSoul_ContextMenuShowing(object sender, HtmlElementEventArgs e)
        {
            contextMenuStripSoul.Show(webBrowserSoul.PointToScreen(e.MousePosition));
            e.ReturnValue = false;
        }

        #endregion

        private void WebLink_DV_Resize(object sender, EventArgs e)
        {
            if ((webBrowserTop.Document != null) && (webBrowserTop.Document.Body != null))
                webBrowserTop.Height = webBrowserTop.Document.Body.ScrollRectangle.Height;

            if ((webBrowserSoul.Document != null) && (webBrowserSoul.Document.Body != null))
                changeWebBrowserSoulSize();
        }
    }
}