#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component.PopupControl;
using Waveface.Component;
using Waveface.Component.RichEdit;
using Waveface.DetailUI;

#endregion

namespace Waveface
{
    public class DetailView : UserControl
    {
        private const int FavoriteIconSize = 18;

        private IContainer components;
        private Post m_post;

        private IDetailView m_currentView;
        private WebLink_DV m_webLinkDv;
        private Photo_DV m_photoDv;
        private Document_DV m_documentDv;
        private RichText_DV m_richTextDv;

        private Panel panelTop;
        private Label labelTitle;
        private Timer timerGC;
        private ImageButton btnComment;
        private Panel panelMain;
        private Localization.CultureManager cultureManager;

        private Popup m_commentPopup;
        private ImageButton btnRemove;
        private ImageButton btnEdit;
        private Timer timerCanEdit;
        private ImageButton btnFavorite;
        private CommentPopupPanel m_commentPopupPanel;

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                ShowContent(false);
            }
        }

        public User User { get; set; }

        public DetailView()
        {
            InitializeComponent();

            MouseWheelRedirector.Attach(this);

            m_commentPopup = new Popup(m_commentPopupPanel = new CommentPopupPanel());
            m_commentPopup.Resizable = true;
            m_commentPopupPanel.buttonAddComment.Click += buttonAddComment_Click;
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timerGC = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.timerCanEdit = new System.Windows.Forms.Timer(this.components);
            this.btnFavorite = new Waveface.Component.ImageButton();
            this.btnRemove = new Waveface.Component.ImageButton();
            this.btnEdit = new Waveface.Component.ImageButton();
            this.btnComment = new Waveface.Component.ImageButton();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelTop.Controls.Add(this.btnFavorite);
            this.panelTop.Controls.Add(this.btnRemove);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.btnComment);
            this.panelTop.Controls.Add(this.labelTitle);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(121)))), ((int)(((byte)(143)))));
            this.labelTitle.Name = "labelTitle";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // timerGC
            // 
            this.timerGC.Interval = 60000;
            this.timerGC.Tick += new System.EventHandler(this.timerGC_Tick);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // timerCanEdit
            // 
            this.timerCanEdit.Enabled = true;
            this.timerCanEdit.Interval = 666;
            this.timerCanEdit.Tick += new System.EventHandler(this.timerCanEdit_Tick);
            // 
            // btnFavorite
            // 
            resources.ApplyResources(this.btnFavorite, "btnFavorite");
            this.btnFavorite.BackColor = System.Drawing.SystemColors.Control;
            this.btnFavorite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFavorite.Image = global::Waveface.Properties.Resources.FB_fav;
            this.btnFavorite.ImageDisable = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.ImageHover = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.Name = "btnFavorite";
            this.btnFavorite.TabStop = false;
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.BackColor = System.Drawing.SystemColors.Control;
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Image = global::Waveface.Properties.Resources.FB_remove;
            this.btnRemove.ImageDisable = global::Waveface.Properties.Resources.FB_remove_hl;
            this.btnRemove.ImageHover = global::Waveface.Properties.Resources.FB_remove_hl;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.TabStop = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.Image = global::Waveface.Properties.Resources.FB_edit;
            this.btnEdit.ImageDisable = global::Waveface.Properties.Resources.FB_edit_hl;
            this.btnEdit.ImageHover = global::Waveface.Properties.Resources.FB_edit_hl;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.TabStop = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnComment
            // 
            resources.ApplyResources(this.btnComment, "btnComment");
            this.btnComment.BackColor = System.Drawing.SystemColors.Control;
            this.btnComment.Image = null;
            this.btnComment.ImageDisable = null;
            this.btnComment.ImageHover = null;
            this.btnComment.Name = "btnComment";
            this.btnComment.TabStop = false;
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

        private void ShowContent(bool force)
        {
            if (m_post == null)
                return;

            // btnComment.Visible = true;
            btnRemove.Visible = true;
            btnEdit.Visible = true;
            btnFavorite.Visible = true;

            setupTitle();

            setFavoriteButton();

            panelTop.Enabled = !Main.Current.BatchPostManager.CheckPostInQueue(m_post.post_id);

            PostType _postType = getPostType();

            switch (_postType)
            {
                case PostType.Text:
                case PostType.Link:
                    if (!force)
                        ShowText_LinkView();

                    break;

                case PostType.Photo:
                    ShowPhoto();
                    break;

                case PostType.RichText:
                    if (!force)
                        ShowRichText();

                    break;
                case PostType.Document:
                    if (!force)
                        ShowDocument();

                    break;
            }
        }

        private void setupTitle()
        {
            string _postTime = Post.timestamp;
            _postTime = DateTimeHelp.ISO8601ToDotNet(_postTime, false);
            _postTime = DateTimeHelp.PrettyDate(_postTime);

            string _s = _postTime + " " + I18n.L.T("DetailView.Via") + " " + Post.code_name;

            labelTitle.Text = _s;
        }

        private void setFavoriteButton()
        {
            int _value = int.Parse(m_post.favorite);

            if (_value == 0)
            {
                btnFavorite.Image = Properties.Resources.FB_unfav;
                btnFavorite.ImageDisable = Properties.Resources.FB_unfav_hl;
                btnFavorite.ImageHover = Properties.Resources.FB_unfav_hl;
            }
            else
            {
                btnFavorite.Image = Properties.Resources.FB_fav;
                btnFavorite.ImageDisable = Properties.Resources.FB_fav_hl;
                btnFavorite.ImageHover = Properties.Resources.FB_fav_hl;
            }
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

            if (m_webLinkDv != null)
                m_webLinkDv.Dispose();

            m_webLinkDv = null;

            m_webLinkDv = new WebLink_DV();
            m_webLinkDv.MyParent = this;
            m_webLinkDv.User = User;
            m_webLinkDv.Dock = DockStyle.Fill;
            m_webLinkDv.Post = m_post; // ︽璶程

            m_currentView = m_webLinkDv;

            panelMain.Controls.Add(m_webLinkDv);
        }

        private void ShowPhoto()
        {
            panelMain.Controls.Clear();

            if (m_photoDv != null)
            {
                m_photoDv.ImageListView.ClearThumbnailCache();
                m_photoDv.Dispose();
            }

            m_photoDv = null;

            m_photoDv = new Photo_DV();
            m_photoDv.MyParent = this;
            m_photoDv.User = User;
            m_photoDv.Dock = DockStyle.Fill;
            m_photoDv.Post = m_post; // ︽璶程

            m_currentView = m_photoDv;

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

            m_currentView = m_documentDv;

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

            m_currentView = m_richTextDv;

            panelMain.Controls.Add(m_richTextDv);
        }

        private void timerGC_Tick(object sender, EventArgs e)
        {
            GC.Collect(); //Hack
        }

        public bool PostComment(WaterMarkRichTextBox textBox, Post post)
        {
            if (!Main.Current.CheckNetworkStatus())
                return false;

            if (textBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("DetailView.CommentEmpty"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            MR_posts_newComment _postsNewComment = Main.Current.RT.REST.Posts_NewComment(post.post_id, textBox.Text, "", "");

            if (_postsNewComment != null)
            {
                Main.Current.RefreshSinglePost_ByID(post.post_id);
            }

            textBox.Text = "";

            return true;
        }

        public string GenCommentHTML(Post post, bool endHR)
        {
            string _html = "<div style='border-left:2px solid #559aae; padding-left:4px'><font face='稬硁タ堵砰, Helvetica, Arial, Verdana, sans-serif' color='#eef'>";

            int k = 1;

            foreach (Comment _c in post.comments)
            {
                StringBuilder _s = new StringBuilder();

                _s.Append("	<table border=\"0\">");
                _s.Append("    	   <tr>");
                _s.Append("      	     <td>");
                _s.Append(" 		<table border=\"0\">");
                _s.Append("    			<tr>");

                string _t = "      				<td><font size='1.75pt' color=#68b0c5>[CommentTime] " + I18n.L.T("DetailView.Via") +
                            " [code_name]</font></td>";

                _s.Append(_t);
                _s.Append("    			</tr>");
                _s.Append("    			<tr>");
                _s.Append("      				<td><font size='2pt'>[Comment]</font></td>");
                _s.Append("    			</tr>");
                _s.Append("		</table>");
                _s.Append("      	     </td>");
                _s.Append("    	  </tr>");
                _s.Append("	</table>");

                //if (post.comment_count != k)
                //    _s.Append("<p></p>");

                k++;

                string _content = HttpUtility.HtmlEncode(_c.content);
                _content = _content.Replace(Environment.NewLine, "<BR>");
                _content = _content.Replace("\n", "<BR>");
                _content = _content.Replace("\r", "<BR>");

                _html += _s.ToString();
                _html = _html.Replace("[Comment]", _content);
                _html = _html.Replace("[CommentTime]", DateTimeHelp.ISO8601ToDotNet(_c.timestamp, true));
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

            _html += "</font></div>";

            if(endHR)
                _html += "<HR>";

            return _html;
        }

        private void btnComment_Click(object sender, EventArgs e)
        {
            m_commentPopup.Width = (Width * 3) / 4;
            m_commentPopup.Height = 144;
            m_commentPopupPanel.CommentTextBox.Text = string.Empty;
            m_commentPopup.Show(btnComment, (-1 * m_commentPopupPanel.Width) + btnComment.Width, btnComment.Height);
            m_commentPopupPanel.CommentTextBox.Focus();
        }

        void buttonAddComment_Click(object sender, EventArgs e)
        {
            if (PostComment(m_commentPopupPanel.CommentTextBox, m_post))
            {
                m_commentPopup.Hide();
            }
            else
            {
                m_commentPopupPanel.CommentTextBox.Focus();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DialogResult _dr = MessageBox.Show(I18n.L.T("AskRemovePost"), "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (_dr != DialogResult.Yes)
                return;

            Main.Current.HidePost(m_post.post_id);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            Main.Current.EditPost(Post);
        }

        private void timerCanEdit_Tick(object sender, EventArgs e)
        {
            if (m_currentView != null)
            {
                btnEdit.Enabled = m_currentView.CanEdit();
            }
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            Main.Current.ChangePostFavorite(m_post);
        }
    }
}