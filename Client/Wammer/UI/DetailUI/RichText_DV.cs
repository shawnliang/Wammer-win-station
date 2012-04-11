#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Waveface.API.V2;

#endregion

namespace Waveface.DetailUI
{
    public class RichText_DV : UserControl, IDetailView
    {
        private IContainer components;
        private Panel panelMain;
        private Panel panelRight;
        private WebBrowser webBrowser;
        private WebBrowser webBrowserComment;
        private Post m_post;
        private string m_htmlFile;

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

        public RichText_DV()
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
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
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
            this.panelMain.Size = new System.Drawing.Size(531, 489);
            this.panelMain.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.webBrowser);
            this.panelRight.Location = new System.Drawing.Point(4, 4);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(520, 478);
            this.panelRight.TabIndex = 2;
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            this.webBrowserComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserComment.Location = new System.Drawing.Point(0, 97);
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.Size = new System.Drawing.Size(520, 97);
            this.webBrowserComment.TabIndex = 2;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(520, 97);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // RichText_DV
            // 
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RichText_DV";
            this.Size = new System.Drawing.Size(537, 495);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public bool CanEdit()
        {
            return false;
        }

        private void RefreshUI()
        {
            Set_MainContent_Part();
            Set_Comments_Part();

            //@ PanelAddComment.Visible = true;
        }

        private void Set_Comments_Part()
        {
            //MyParent.SetComments(webBrowserComment, Post);
        }

        private void Set_MainContent_Part()
        {
            foreach (Attachment _a in m_post.attachments)
            {
                if (_a.mime_type == "text/html")
                {
                    m_htmlFile = Path.Combine(Main.GCONST.CachePath, _a.object_id + ".html");

                    if (File.Exists(m_htmlFile))
                    {
                        webBrowser.Navigate(m_htmlFile);
                    }
                    else
                    {
                        string _url = Main.Current.RT.REST.attachments_getRedirectURL(_a.url, _a.object_id, false);

                        WebClient _webClient = new WebClient();
                        _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
                        _webClient.DownloadProgressChanged += ProgressChanged;
                        _webClient.DownloadFileAsync(new Uri(_url), m_htmlFile);
                    }

                    return;
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            webBrowser.Navigate(m_htmlFile);

            Application.DoEvents();
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowser.Document.Body.ScrollRectangle.Height;
            webBrowser.Height = _h;

            Application.DoEvents();
        }

        private void webBrowserComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserComment.Document.Body.ScrollRectangle.Height;
            webBrowserComment.Height = _h;

            Application.DoEvents();
        }

        public List<ToolStripMenuItem> GetMoreMenuItems()
        {
            return null;
        }
    }
}