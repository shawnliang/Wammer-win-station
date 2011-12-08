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
    public class Text_Link_DV : UserControl
    {
        private IContainer components = null;
        private Panel panelMain;
        private Panel panelRight;
        private WebBrowser webBrowserTop;
        private Panel PanelAddComment;
        private WebBrowser webBrowserComment;
        private Post m_post;
        private XPButton buttonAddComment;
        private WebBrowser webBrowser;
        private Panel panelWebBrowser;
        private TabControl tabControl;
        private TabPage tabPageSoul;
        private TabPage tabPageWebPage;
        private WebBrowser webBrowserSoul;
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

        public Text_Link_DV()
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.PanelAddComment = new System.Windows.Forms.Panel();
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.panelWebBrowser = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageWebPage = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.tabPageSoul = new System.Windows.Forms.TabPage();
            this.webBrowserSoul = new System.Windows.Forms.WebBrowser();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
            this.panelWebBrowser.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageWebPage.SuspendLayout();
            this.tabPageSoul.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Location = new System.Drawing.Point(3, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(529, 583);
            this.panelMain.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.PanelAddComment);
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.panelWebBrowser);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Location = new System.Drawing.Point(4, 4);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(518, 572);
            this.panelRight.TabIndex = 2;
            // 
            // PanelAddComment
            // 
            this.PanelAddComment.AutoScroll = true;
            this.PanelAddComment.AutoScrollMinSize = new System.Drawing.Size(345, 0);
            this.PanelAddComment.BackColor = System.Drawing.SystemColors.Window;
            this.PanelAddComment.Controls.Add(this.buttonAddComment);
            this.PanelAddComment.Controls.Add(this.textBoxComment);
            this.PanelAddComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelAddComment.Location = new System.Drawing.Point(0, 462);
            this.PanelAddComment.Name = "PanelAddComment";
            this.PanelAddComment.Size = new System.Drawing.Size(518, 84);
            this.PanelAddComment.TabIndex = 3;
            this.PanelAddComment.Visible = false;
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonAddComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Location = new System.Drawing.Point(423, 3);
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.Size = new System.Drawing.Size(66, 28);
            this.buttonAddComment.TabIndex = 1;
            this.buttonAddComment.Text = "Send";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // textBoxComment
            // 
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxComment.Location = new System.Drawing.Point(32, 3);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxComment.Size = new System.Drawing.Size(385, 44);
            this.textBoxComment.TabIndex = 0;
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            this.webBrowserComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserComment.Location = new System.Drawing.Point(0, 365);
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.Size = new System.Drawing.Size(518, 97);
            this.webBrowserComment.TabIndex = 2;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
            // 
            // panelWebBrowser
            // 
            this.panelWebBrowser.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelWebBrowser.Controls.Add(this.tabControl);
            this.panelWebBrowser.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWebBrowser.Location = new System.Drawing.Point(0, 200);
            this.panelWebBrowser.Name = "panelWebBrowser";
            this.panelWebBrowser.Size = new System.Drawing.Size(518, 165);
            this.panelWebBrowser.TabIndex = 5;
            this.panelWebBrowser.Visible = false;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageWebPage);
            this.tabControl.Controls.Add(this.tabPageSoul);
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(512, 158);
            this.tabControl.TabIndex = 5;
            // 
            // tabPageWebPage
            // 
            this.tabPageWebPage.Controls.Add(this.webBrowser);
            this.tabPageWebPage.Location = new System.Drawing.Point(4, 23);
            this.tabPageWebPage.Name = "tabPageWebPage";
            this.tabPageWebPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWebPage.Size = new System.Drawing.Size(504, 131);
            this.tabPageWebPage.TabIndex = 1;
            this.tabPageWebPage.Text = "Web Page";
            this.tabPageWebPage.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(498, 125);
            this.webBrowser.TabIndex = 4;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // tabPageSoul
            // 
            this.tabPageSoul.Controls.Add(this.webBrowserSoul);
            this.tabPageSoul.Location = new System.Drawing.Point(4, 22);
            this.tabPageSoul.Name = "tabPageSoul";
            this.tabPageSoul.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSoul.Size = new System.Drawing.Size(504, 132);
            this.tabPageSoul.TabIndex = 0;
            this.tabPageSoul.Text = "Soul";
            this.tabPageSoul.UseVisualStyleBackColor = true;
            // 
            // webBrowserSoul
            // 
            this.webBrowserSoul.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserSoul.Location = new System.Drawing.Point(3, 3);
            this.webBrowserSoul.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserSoul.Name = "webBrowserSoul";
            this.webBrowserSoul.ScriptErrorsSuppressed = true;
            this.webBrowserSoul.Size = new System.Drawing.Size(502, 125);
            this.webBrowserSoul.TabIndex = 0;
            // 
            // webBrowserTop
            // 
            this.webBrowserTop.AllowWebBrowserDrop = false;
            this.webBrowserTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserTop.Location = new System.Drawing.Point(0, 0);
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.Size = new System.Drawing.Size(518, 200);
            this.webBrowserTop.TabIndex = 0;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // Text_Link_DV
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Text_Link_DV";
            this.Size = new System.Drawing.Size(535, 589);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.panelWebBrowser.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageWebPage.ResumeLayout(false);
            this.tabPageSoul.ResumeLayout(false);
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
           MyParent.SetComments(webBrowserComment, Post);
        }

        private void Set_MainContent_Preview_Part()
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<p>[Text]</p>");

            string _html = _sb.ToString();
            _html = _html.Replace("[Text]", Post.content.Replace(Environment.NewLine, "<BR>"));

            if (Post.preview.url != null)
            {
                Preview_OpenGraph _p = Post.preview;
                StringBuilder _s = new StringBuilder();

                _s.Append("	<table border=\"0\">");
                _s.Append("    	   <tr>");
                _s.Append("      	     <td><a href=\"[PriviewLink]\" target=\"_blank\"><img src=\"[PreviewPic]\" width=\"180\" height=\"120\" /></td>");
                _s.Append("      	     <td>");
                _s.Append(" 		<table border=\"0\">");
                _s.Append("    			<tr>");
                _s.Append("      				<td><a href=\"[PriviewLink]\" target=\"_blank\">[PriviewTitle]</a></td>");
                _s.Append("    			</tr>");
                _s.Append("    			<tr>");
                _s.Append("      				<td>[PriviewText]</td>");
                _s.Append("    			</tr>");
                _s.Append("		</table>");
                _s.Append("      	     </td>");
                _s.Append("    	  </tr>");
                _s.Append("	</table>");

                _html += _s.ToString();

                _html = _html.Replace("[PreviewPic]", _p.thumbnail_url);
                _html = _html.Replace("[PriviewTitle]", _p.title);
                _html = _html.Replace("[PriviewLink]", _p.url);
                _html = _html.Replace("[PriviewText]", _p.description);

                ShowWebBrowser(Post.preview.url);
            }

            webBrowserTop.DocumentText = _html;
            webBrowserSoul.DocumentText = m_post.soul;
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

        private void ShowWebBrowser(string url)
        {
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.Navigate(url);
        }

        private void buttonAddComment_Click(object sender, EventArgs e)
        {
            MyParent.PostComment(textBoxComment, Post);
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowser.Document.Body.ScrollRectangle.Height;

            panelWebBrowser.Height = _h + 256;
            webBrowser.Height = _h + 128;

            panelWebBrowser.Visible = true;
        }
    }
}