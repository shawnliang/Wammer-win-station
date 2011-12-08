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
        private LinkLabel linkLabelRemove;
        private Panel panelMain;

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
            this.panelTop = new System.Windows.Forms.Panel();
            this.linkLabelRemove = new System.Windows.Forms.LinkLabel();
            this.labelWho = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timerGC = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(96)))), ((int)(((byte)(0)))));
            this.panelTop.Controls.Add(this.linkLabelRemove);
            this.panelTop.Controls.Add(this.labelWho);
            this.panelTop.Controls.Add(this.labelTime);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(535, 32);
            this.panelTop.TabIndex = 0;
            // 
            // linkLabelRemove
            // 
            this.linkLabelRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelRemove.AutoSize = true;
            this.linkLabelRemove.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelRemove.ForeColor = System.Drawing.SystemColors.Window;
            this.linkLabelRemove.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            this.linkLabelRemove.Location = new System.Drawing.Point(481, 10);
            this.linkLabelRemove.Name = "linkLabelRemove";
            this.linkLabelRemove.Size = new System.Drawing.Size(51, 14);
            this.linkLabelRemove.TabIndex = 2;
            this.linkLabelRemove.TabStop = true;
            this.linkLabelRemove.Text = "Remove";
            this.linkLabelRemove.Visible = false;
            this.linkLabelRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRemove_LinkClicked);
            // 
            // labelWho
            // 
            this.labelWho.AutoSize = true;
            this.labelWho.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWho.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelWho.Location = new System.Drawing.Point(78, 8);
            this.labelWho.Name = "labelWho";
            this.labelWho.Size = new System.Drawing.Size(0, 16);
            this.labelWho.TabIndex = 1;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTime.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelTime.Location = new System.Drawing.Point(4, 8);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(0, 16);
            this.labelTime.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 32);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(535, 461);
            this.panelMain.TabIndex = 1;
            // 
            // timerGC
            // 
            this.timerGC.Interval = 60000;
            this.timerGC.Tick += new System.EventHandler(this.timerGC_Tick);
            // 
            // DetailView
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DetailView";
            this.Size = new System.Drawing.Size(535, 493);
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
            setupTitle();
            linkLabelRemove.Visible = true;

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
            labelWho.Text = "via " + Post.code_name;

            labelWho.Left = labelTime.Right + 16;
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
        }

        private void timerGC_Tick(object sender, EventArgs e)
        {
            GC.Collect(); //Hack
        }

        public void PostComment(TextBox textBox, Post post)
        {
            if (textBox.Text.Equals(""))
            {
                MessageBox.Show("Comment Text cannot be empty!");
                return;
            }

            MR_posts_newComment _postsNewComment = MainForm.THIS.Posts_NewComment(post.post_id, textBox.Text, "", "");

            if (_postsNewComment != null)
            {
                MainForm.THIS.AfterPostComment(post.post_id);
            }

            textBox.Text = "";
        }

        public void SetComments(WebBrowser wb, Post post)
        {
            string _html = "<div style=\"margin: 20px;\">";

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
                _s.Append("      				<td>[CommentTime] via [code_name]</td>");
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

                foreach (User _user in MainForm.THIS.RT.AllUsers)
                {
                    if (_user.user_id == _c.creator_id)
                    {
                        _html = _html.Replace("[UserName]", _user.nickname);
                        _html = _html.Replace("[Avatar]", _user.avatar_url);
                    }
                }
            }

            _html += "</div>";

            wb.DocumentText = _html;
        }

        private void linkLabelRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult _dr = MessageBox.Show("Do you really want to remove this post?", "Waveface", MessageBoxButtons.YesNo);

            if (_dr != DialogResult.Yes)
                return;

            MainForm.THIS.HidePost(m_post.post_id);
        }
    }
}