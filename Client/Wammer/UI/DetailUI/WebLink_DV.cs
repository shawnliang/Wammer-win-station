#region

using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class WebLink_DV : UserControl, IDetailViewer
    {
        private IContainer components;
        private Panel panelMain;
        private Panel panelRight;
        private WebBrowser webBrowserTop;
        private Panel PanelAddComment;
        private WebBrowser webBrowserComment;
        private Post m_post;
        private XPButton buttonAddComment;
        private Panel panelWebBrowser;
        private WebBrowser webBrowserSoul;
        private Localization.CultureManager cultureManager;
        private TextBox textBoxComment;

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
            this.panelRight = new System.Windows.Forms.Panel();
            this.PanelAddComment = new System.Windows.Forms.Panel();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.panelWebBrowser = new System.Windows.Forms.Panel();
            this.webBrowserSoul = new System.Windows.Forms.WebBrowser();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
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
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelRight.Controls.Add(this.PanelAddComment);
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.panelWebBrowser);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // PanelAddComment
            // 
            resources.ApplyResources(this.PanelAddComment, "PanelAddComment");
            this.PanelAddComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.PanelAddComment.Controls.Add(this.buttonAddComment);
            this.PanelAddComment.Controls.Add(this.textBoxComment);
            this.PanelAddComment.Name = "PanelAddComment";
            // 
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserComment, "webBrowserComment");
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
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
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.buttonAddComment, "buttonAddComment");
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // WebLink_DV
            // 
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "WebLink_DV";
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.panelWebBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private void RefreshUI()
        {
            Set_MainContent_Preview_Part();
            Set_Comments_Part();

            PanelAddComment.Visible = true;
            textBoxComment.Focus();
        }

        private void Set_Comments_Part()
        {
            MyParent.SetComments(webBrowserComment, Post, true);
        }

        private void Set_MainContent_Preview_Part()
        {
            StringBuilder _sb = new StringBuilder();

            if (Post.preview.url != null)
                _sb.Append("<font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' color='#eef'><p>[Text]</p><hr><br></font>");
            else
                _sb.Append("<font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' color='#eef'><p>[Text]</p><br></font>");

            string _html = _sb.ToString();

            string _content = Post.content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");

            _html = _html.Replace("[Text]", _content);

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

            webBrowserTop.DocumentText = HtmlUtility.TrimScript("<body bgcolor=\"rgb(243, 242, 238)\">" + _html + "</quote></body>");
            webBrowserSoul.DocumentText = HtmlUtility.TrimScript("<body bgcolor=\"rgb(243, 242, 238)\"><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif'>" + m_post.soul + "</font></body>"); //"<body bgcolor=\"rgb(243, 242, 238)\">" + m_post.soul + "</body>"
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;
        }

        private void webBrowserComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserComment.Document.Body.ScrollRectangle.Height;
            webBrowserComment.Height = _h;
        }

        private void buttonAddComment_Click(object sender, EventArgs e)
        {
            MyParent.PostComment(textBoxComment, Post);
        }

        private void webBrowserSoul_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserSoul.Document.Body.ScrollRectangle.Height;

            panelWebBrowser.Height = _h + 4;
            webBrowserSoul.Height = _h;

            if (m_post.soul.Trim() != string.Empty)
                panelWebBrowser.Visible = true;
        }

        #region IDetailViewer

        public void ScrollToComment()
        {
            if (panelRight.VerticalScroll.Visible)
            {
                panelRight.VerticalScroll.Value = PanelAddComment.Top;
                textBoxComment.Focus();
            }
        }

        public bool WantToShowCommentButton()
        {
            if (panelRight.VerticalScroll.Visible)
            {
                return PanelAddComment.Bottom > panelRight.Height;
            }

            return false;
        }

        #endregion
    }
}