#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class Document_DV : UserControl, IDetailViewer
    {
        private Panel panelMain;
        private Panel panelRight;
        private WebBrowser webBrowserTop;
        private Panel PanelAddComment;
        private WebBrowser webBrowserComment;
        private Post m_post;
        private XPButton buttonAddComment;
        private TextBox textBoxComment;
        private ListView listViewFiles;
        private PreviewHandlerHost previewHandlerHost;
        private ProgressBar progressBar;
        private Panel PanelDocumentView;

        private Button buttonSave;
        private SaveFileDialog saveFileDialog;
        private Button buttonOpen;
        private Attachment m_currentAttachment;

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

        public Document_DV()
        {
            InitializeComponent();

            saveFileDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
        }

        protected override void Dispose(bool disposing)
        {
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
            this.PanelDocumentView = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.previewHandlerHost = new Waveface.Component.PreviewHandlerHost();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
            this.PanelDocumentView.SuspendLayout();
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
            this.panelRight.Controls.Add(this.PanelAddComment);
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.PanelDocumentView);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Location = new System.Drawing.Point(16, 3);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(502, 481);
            this.panelRight.TabIndex = 2;
            // 
            // PanelAddComment
            // 
            this.PanelAddComment.AutoScroll = true;
            this.PanelAddComment.AutoScrollMinSize = new System.Drawing.Size(345, 0);
            this.PanelAddComment.AutoSize = true;
            this.PanelAddComment.BackColor = System.Drawing.SystemColors.Window;
            this.PanelAddComment.Controls.Add(this.buttonAddComment);
            this.PanelAddComment.Controls.Add(this.textBoxComment);
            this.PanelAddComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelAddComment.Location = new System.Drawing.Point(0, 355);
            this.PanelAddComment.Name = "PanelAddComment";
            this.PanelAddComment.Size = new System.Drawing.Size(502, 50);
            this.PanelAddComment.TabIndex = 3;
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonAddComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Location = new System.Drawing.Point(405, 3);
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
            this.textBoxComment.Size = new System.Drawing.Size(367, 44);
            this.textBoxComment.TabIndex = 0;
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            this.webBrowserComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserComment.Location = new System.Drawing.Point(0, 258);
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.Size = new System.Drawing.Size(502, 97);
            this.webBrowserComment.TabIndex = 2;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
            // 
            // PanelDocumentView
            // 
            this.PanelDocumentView.AutoScroll = true;
            this.PanelDocumentView.AutoScrollMinSize = new System.Drawing.Size(345, 0);
            this.PanelDocumentView.BackColor = System.Drawing.SystemColors.Window;
            this.PanelDocumentView.Controls.Add(this.progressBar);
            this.PanelDocumentView.Controls.Add(this.listViewFiles);
            this.PanelDocumentView.Controls.Add(this.previewHandlerHost);
            this.PanelDocumentView.Controls.Add(this.buttonSave);
            this.PanelDocumentView.Controls.Add(this.buttonOpen);
            this.PanelDocumentView.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelDocumentView.Location = new System.Drawing.Point(0, 97);
            this.PanelDocumentView.Name = "PanelDocumentView";
            this.PanelDocumentView.Size = new System.Drawing.Size(502, 161);
            this.PanelDocumentView.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(396, 3);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(103, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.Location = new System.Drawing.Point(3, 4);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(387, 30);
            this.listViewFiles.TabIndex = 3;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.SmallIcon;
            this.listViewFiles.Click += new System.EventHandler(this.listViewFiles_Click);
            // 
            // previewHandlerHost
            // 
            this.previewHandlerHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewHandlerHost.BackColor = System.Drawing.Color.White;
            this.previewHandlerHost.Location = new System.Drawing.Point(3, 36);
            this.previewHandlerHost.Name = "previewHandlerHost";
            this.previewHandlerHost.Size = new System.Drawing.Size(496, 122);
            this.previewHandlerHost.TabIndex = 5;
            this.previewHandlerHost.Text = "previewHandlerHost";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Enabled = false;
            this.buttonSave.Location = new System.Drawing.Point(396, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(51, 31);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Enabled = false;
            this.buttonOpen.Location = new System.Drawing.Point(448, 3);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(51, 31);
            this.buttonOpen.TabIndex = 8;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // webBrowserTop
            // 
            this.webBrowserTop.AllowWebBrowserDrop = false;
            this.webBrowserTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserTop.Location = new System.Drawing.Point(0, 0);
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.Size = new System.Drawing.Size(502, 97);
            this.webBrowserTop.TabIndex = 0;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // Document_DV
            // 
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Document_DV";
            this.Size = new System.Drawing.Size(537, 495);
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.PanelDocumentView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush _brush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(106, 112, 128), Color.FromArgb(138, 146, 166), LinearGradientMode.ForwardDiagonal);

            e.Graphics.FillRectangle(_brush, ClientRectangle);

            base.OnPaint(e);
        }

        #endregion

        private void RefreshUI()
        {
            Set_MainContent_Part();
            Set_Comments_Part();
            Set_Document();

            //@ PanelAddComment.Visible = true;
        }

        private void Set_Comments_Part()
        {
            MyParent.SetComments(webBrowserComment, Post, true);
        }

        private void Set_MainContent_Part()
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<p>[Text]</p>");

            string _html = _sb.ToString();
            _html = _html.Replace("[Text]", Post.content.Replace(Environment.NewLine, "<BR>"));

            webBrowserTop.DocumentText = HtmlUtility.MakeLink(HtmlUtility.TrimScript(_html));
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Application.DoEvents();

            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;
        }

        private void webBrowserComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Application.DoEvents();

            int _h = webBrowserComment.Document.Body.ScrollRectangle.Height;
            webBrowserComment.Height = _h;
        }

        private void Set_Document()
        {
            foreach (Attachment _a in m_post.attachments)
            {
                ListViewItem _item = new ListViewItem(HttpUtility.UrlDecode(_a.file_name));
                _item.Tag = _a;
                listViewFiles.Items.Add(_item);
            }

            listViewFiles.Items[0].Selected = true;
            listViewFiles.Items[0].Focused = true;

            downloadFile(m_post.attachments[0]);
        }

        private void downloadFile(Attachment attachment)
        {
            string _file = Main.GCONST.CachePath + attachment.object_id + attachment.file_name; //HttpUtility.UrlDecode(attachment.file_name)

            m_currentAttachment = attachment;

            if (File.Exists(_file))
            {
                if (new FileInfo(_file).Length == long.Parse(attachment.file_size))
                {
                    setPreview();                

                    return;
                }
            }

            progressBar.Visible = true;
            buttonSave.Enabled = false;
            buttonOpen.Enabled = false;

            WebClient _webClient = new WebClient();
            _webClient.DownloadFileCompleted += Completed;
            _webClient.DownloadProgressChanged += ProgressChanged;
            _webClient.DownloadFileAsync(new Uri(Main.Current.RT.REST.attachments_getRedirectURL(attachment.url, attachment.object_id, false)), _file);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            setPreview();
        }

        private void setPreview()
        {
            Application.DoEvents();

            progressBar.Visible = false;
            buttonSave.Enabled = true;
            buttonOpen.Enabled = true;

            if (!OsUtility.IsWinXP())
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    string _file = Main.GCONST.CachePath + m_currentAttachment.object_id + m_currentAttachment.file_name; //HttpUtility.UrlDecode(m_currentAttachment.file_name)
                    string _destFile = Main.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + m_currentAttachment.file_name; //HttpUtility.UrlDecode(m_currentAttachment.file_name)

                    File.Copy(_file, _destFile);

                    previewHandlerHost.Open(_destFile);
                }
                catch
                { }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }

            Application.DoEvents();
        }

        private void DetailView_Resize(object sender, EventArgs e)
        {
            PanelDocumentView.Height = (int)(previewHandlerHost.Width / 1.618);
        }

        private void buttonAddComment_Click(object sender, EventArgs e)
        {
            //@ MyParent.PostComment(textBoxComment, Post);
        }

        private void listViewFiles_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                downloadFile((Attachment)listViewFiles.SelectedItems[0].Tag);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = HttpUtility.UrlDecode(m_currentAttachment.file_name);
            DialogResult _dr = saveFileDialog.ShowDialog();

            if (_dr == DialogResult.OK)
            {
                try
                {
                    string _file = Main.GCONST.CachePath + m_currentAttachment.object_id + m_currentAttachment.file_name; //HttpUtility.UrlDecode(m_currentAttachment.file_name)
                    string _destFile = saveFileDialog.FileName;

                    File.Copy(_file, _destFile);

                    MessageBox.Show("File Save Successful!");
                }
                catch
                {
                    MessageBox.Show("File Save Error!");
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                string _file = Main.GCONST.CachePath + m_currentAttachment.object_id + m_currentAttachment.file_name; // HttpUtility.UrlDecode(m_currentAttachment.file_name)
                string _destFile = Main.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + m_currentAttachment.file_name; //HttpUtility.UrlDecode(m_currentAttachment.file_name)

                File.Copy(_file, _destFile);

                Process.Start(_destFile);
            }
            catch
            {}
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