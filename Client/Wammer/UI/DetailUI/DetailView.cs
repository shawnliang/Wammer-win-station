#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.DetailUI;

#endregion

namespace Waveface
{
    public class DetailView : UserControl
    {
        private IContainer components;
        private Post m_post;

        private Text_Link_DV m_textLinkDv;
        private Photo_DV m_photoDv;
        private Document_DV m_documentDv;
        private RichText_DV m_richTextDv;

        private Panel panelTop;
        private Label labelWho;
        private Label labelTime;
        private Timer timerGC;
        private Component.XPButton btnComment;
        private Panel panelMain;
        private Timer timerShowCommentButton;
        private Localization.CultureManager cultureManager;

        private IDetailViewer m_detailViewer;

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                if (m_post != null)
                    doShow();
            }
        }

        public User User { get; set; }

        public DetailView()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailView));
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelWho = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timerGC = new System.Windows.Forms.Timer(this.components);
            this.timerShowCommentButton = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.btnComment = new Waveface.Component.XPButton();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.panelTop.Controls.Add(this.btnComment);
            this.panelTop.Controls.Add(this.labelWho);
            this.panelTop.Controls.Add(this.labelTime);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // labelWho
            // 
            resources.ApplyResources(this.labelWho, "labelWho");
            this.labelWho.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelWho.Name = "labelWho";
            // 
            // labelTime
            // 
            resources.ApplyResources(this.labelTime, "labelTime");
            this.labelTime.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelTime.Name = "labelTime";
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // timerGC
            // 
            this.timerGC.Interval = 30000;
            this.timerGC.Tick += new System.EventHandler(this.timerGC_Tick);
            // 
            // timerShowCommentButton
            // 
            this.timerShowCommentButton.Enabled = true;
            this.timerShowCommentButton.Interval = 2000;
            this.timerShowCommentButton.Tick += new System.EventHandler(this.timerShowCommentButton_Tick);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // btnComment
            // 
            this.btnComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnComment, "btnComment");
            this.btnComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnComment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnComment.Image = global::Waveface.Properties.Resources.write_comment;
            this.btnComment.Name = "btnComment";
            this.btnComment.UseVisualStyleBackColor = true;
            this.btnComment.Click += new System.EventHandler(this.btnComment_Click);
            // 
            // DetailView
            // 
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            resources.ApplyResources(this, "$this");
            this.Name = "DetailView";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush _brush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(106, 112, 128),
                                                                 Color.FromArgb(138, 146, 166),
                                                                 LinearGradientMode.ForwardDiagonal);

            e.Graphics.FillRectangle(_brush, ClientRectangle);

            base.OnPaint(e);
        }

        #endregion

        private void doShow()
        {
            btnComment.Visible = false;
            //linkLabelRemove.Visible = true;

            setupTitle();

            PostType _postType = getPostType();

            switch (_postType)
            {
                case PostType.Text:
                case PostType.Link:
                    ShowText_LinkView();
                    break;

                case PostType.Photo:
                    ShowPhoto();
                    break;

                case PostType.RichText:
                    ShowRichText();
                    break;
                case PostType.Document:
                    ShowDocument();
                    break;
            }
        }

        private void setupTitle()
        {
            labelTime.Text = DateTimeHelp.ISO8601ToDotNet(Post.timestamp);
            labelWho.Text = I18n.L.T("DetailView.Via") + " " + Post.code_name;

            labelWho.Left = labelTime.Right + 8;
        }

        private PostType getPostType()
        {
            bool _haveDOC = false;

            if (m_post.preview.url != null)
                return PostType.Link;

            if (m_post.attachment_count > 0)
            {
                foreach (Attachment _a in m_post.attachments)
                {
                    if (_a.type == "doc")
                    {
                        _haveDOC = true;
                        break;
                    }
                }

                if (m_post.type == "rtf")
                    return PostType.RichText;

                if (_haveDOC)
                    return PostType.Document;

                return PostType.Photo;
            }

            return PostType.Text;
        }

        private void ShowText_LinkView()
        {
            panelMain.Controls.Clear();

            if (m_textLinkDv != null)
                m_textLinkDv.Dispose();

            m_textLinkDv = null;

            m_textLinkDv = new Text_Link_DV();
            m_textLinkDv.MyParent = this;
            m_textLinkDv.User = User;
            m_textLinkDv.Dock = DockStyle.Fill;
            m_textLinkDv.Post = m_post; // ︽璶程

            panelMain.Controls.Add(m_textLinkDv);

            m_detailViewer = m_textLinkDv;
        }

        private void ShowPhoto()
        {
            panelMain.Controls.Clear();

            if (m_photoDv != null)
                m_photoDv.Dispose();

            m_photoDv = null;

            m_photoDv = new Photo_DV();
            m_photoDv.MyParent = this;
            m_photoDv.User = User;
            m_photoDv.Dock = DockStyle.Fill;
            m_photoDv.Post = m_post; // ︽璶程

            panelMain.Controls.Add(m_photoDv);

            m_detailViewer = m_photoDv;
        }

        private void ShowDocument()
        {
            panelMain.Controls.Clear();

            if (m_documentDv != null)
                m_documentDv.Dispose();

            m_documentDv = null;

            m_documentDv = new Document_DV();
            m_documentDv.MyParent = this;
            m_documentDv.User = User;
            m_documentDv.Dock = DockStyle.Fill;
            m_documentDv.Post = m_post; // ︽璶程

            panelMain.Controls.Add(m_documentDv);

            m_detailViewer = m_documentDv;
        }

        private void ShowRichText()
        {
            panelMain.Controls.Clear();

            if (m_richTextDv != null)
                m_richTextDv.Dispose();

            m_richTextDv = null;

            m_richTextDv = new RichText_DV();
            m_richTextDv.MyParent = this;
            m_richTextDv.User = User;
            m_richTextDv.Dock = DockStyle.Fill;
            m_richTextDv.Post = m_post; // ︽璶程

            panelMain.Controls.Add(m_richTextDv);

            m_detailViewer = m_richTextDv;
        }

        private void timerGC_Tick(object sender, EventArgs e)
        {
            GC.Collect(); //Hack
        }

        public void PostComment(TextBox textBox, Post post)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (textBox.Text.Equals(""))
            {
                MessageBox.Show(I18n.L.T("DetailView.CommentEmpty"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MR_posts_newComment _postsNewComment = Main.Current.RT.REST.Posts_NewComment(post.post_id, textBox.Text, "", "");

            if (_postsNewComment != null)
            {
                Main.Current.AfterPostComment(post.post_id);
            }

            textBox.Text = "";
        }

        public void SetComments(WebBrowser wb, Post post, bool changeBgColor)
        {
            string _html = "<div>";

            if (post.comment_count > 0)
                _html += "<HR>";

            int k = 1;

            foreach (Comment _c in post.comments)
            {
                StringBuilder _s = new StringBuilder();

                _s.Append("	<table bgcolor=#eeeeee border=\"0\">");
                _s.Append("    	   <tr>");
                //_s.Append("      	     <td><img src=\"[Avatar]\" width=\"40\" height=\"40\" /></td>");
                _s.Append("      	     <td>");
                _s.Append(" 		<table border=\"0\">");
                _s.Append("    			<tr>");

                string _t = "      				<td>[CommentTime] " + I18n.L.T("DetailView.Via") + " [code_name]</td>";

                _s.Append(_t);
                _s.Append("    			</tr>");
                _s.Append("    			<tr>");
                _s.Append("      				<td><strong>[Comment]<strong></td>");
                _s.Append("    			</tr>");
                _s.Append("		</table>");
                _s.Append("      	     </td>");
                _s.Append("    	  </tr>");
                _s.Append("	</table>");

                if (post.comment_count != k)
                    _s.Append("<p></p>");

                k++;

                _html += _s.ToString();
                _html = _html.Replace("[Comment]", _c.content);
                _html = _html.Replace("[CommentTime]", DateTimeHelp.ISO8601ToDotNet(_c.timestamp));
                _html = _html.Replace("[code_name]", _c.code_name);

                foreach (User _user in Main.Current.RT.AllUsers)
                {
                    if (_user.user_id == _c.creator_id)
                    {
                        _html = _html.Replace("[UserName]", _user.nickname);
                        _html = _html.Replace("[Avatar]", _user.avatar_url);
                    }
                }
            }

            _html += "</div>";

            _html = "<font face='稬硁タ堵砰, Helvetica, Arial, Verdana, sans-serif'>" + _html + "</font>";

            if (changeBgColor)
                wb.DocumentText = "<body bgcolor=\"rgb(243, 242, 238)\">" + _html + "</body>";
            else
                wb.DocumentText = _html;
        }

        private void btnComment_Click(object sender, EventArgs e)
        {
            if (m_detailViewer != null)
                m_detailViewer.ScrollToComment();
        }

        public void ShowCommentButton(bool flag)
        {
            btnComment.Visible = flag;
        }

        private void timerShowCommentButton_Tick(object sender, EventArgs e)
        {
            if (m_detailViewer != null)
            {
                btnComment.Visible = m_detailViewer.WantToShowCommentButton();
            }
        }

        /*
        private void linkLabelRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult _dr = MessageBox.Show("Do you really want to remove this post?", "Waveface", MessageBoxButtons.YesNo);

            if (_dr != DialogResult.Yes)
                return;

            Main.Current.HidePost(m_post.post_id);
        }
        */
    }
}