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
        private const int FavoriteIconSize = 20;

        private IContainer components;
        private Post m_post;

        private WebLink_DV m_webLinkDv;
        private Photo_DV m_photoDv;
        private Document_DV m_documentDv;
        private RichText_DV m_richTextDv;

        private Panel panelTop;
        private Label labelWho;
        private Label labelTime;
        private Timer timerGC;
        private XPButton btnComment;
        private Panel panelMain;
        private Localization.CultureManager cultureManager;

        private Popup m_commentPopup;
        private XPButton btnRemove;
        private XPButton btnEdit;
        private CommentPopupPanel m_commentPopupPanel;

        public bool CanEdit
        {
            set { btnEdit.Visible = value; }
        }

        public Post Post
        {
            get { return m_post; }
            set
            {
                CanEdit = false;

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
            this.btnRemove = new Waveface.Component.XPButton();
            this.btnEdit = new Waveface.Component.XPButton();
            this.btnComment = new Waveface.Component.XPButton();
            this.labelWho = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timerGC = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.panelTop.Controls.Add(this.btnRemove);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.btnComment);
            this.panelTop.Controls.Add(this.labelWho);
            this.panelTop.Controls.Add(this.labelTime);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTop_Paint);
            this.panelTop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseClick);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            // 
            // btnRemove
            // 
            this.btnRemove.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnRemove.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnRemove.Image = global::Waveface.Properties.Resources.trash;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnEdit.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnEdit.Image = global::Waveface.Properties.Resources.page;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnComment
            // 
            this.btnComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnComment, "btnComment");
            this.btnComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnComment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnComment.Image = global::Waveface.Properties.Resources.white_edit;
            this.btnComment.Name = "btnComment";
            this.btnComment.UseVisualStyleBackColor = true;
            this.btnComment.Click += new System.EventHandler(this.btnComment_Click);
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
            this.timerGC.Interval = 60000;
            this.timerGC.Tick += new System.EventHandler(this.timerGC_Tick);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
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

        public void ReShow()
        {
            ShowContent(true);
        }

        private void ShowContent(bool force)
        {
            if (m_post == null)
                return;

            btnComment.Visible = true;
            btnRemove.Visible = true;

            setupTitle();
            drawFavorite();

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
            labelTime.Text = DateTimeHelp.ISO8601ToDotNet(Post.timestamp, true);
            labelWho.Text = I18n.L.T("DetailView.Via") + " " + Post.code_name;

            labelWho.Left = labelTime.Right + 8;
        }

        private void drawFavorite()
        {
            panelTop.Refresh();
        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {
            if (m_post == null)
                return;

            if (m_post.favorite == null)
                return;

            int _value = int.Parse(m_post.favorite);

            int _x = 6;

            if (_value == 0)
                e.Graphics.DrawImage(Properties.Resources.star_empty, _x, 6, FavoriteIconSize, FavoriteIconSize);
            else
                e.Graphics.DrawImage(Properties.Resources.star, _x, 6, FavoriteIconSize, FavoriteIconSize);
        }

        private void panelTop_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_post == null)
                return;

            if (m_post.favorite == null)
                return;

            int _x = 6;

            if((e.X > (_x - 2)) && (e.X < (_x + FavoriteIconSize + 2)))
            {
                Main.Current.ChangePostFavorite(m_post);
            }
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            int _x = 6;

            if((e.X > _x) && (e.X < (_x + FavoriteIconSize)))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
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

        public string GenCommentHTML(Post post)
        {
            string _html = "<blockquote><font face='稬硁タ堵砰, Helvetica, Arial, Verdana, sans-serif' color='#eef'>";

            int k = 1;

            foreach (Comment _c in post.comments)
            {
                StringBuilder _s = new StringBuilder();

                _s.Append("	<table border=\"0\">");
                _s.Append("    	   <tr>");
                //_s.Append("      	     <td>&nbsp;&nbsp;&nbsp;</td>");
                _s.Append("      	     <td>");
                _s.Append(" 		<table border=\"0\">");
                _s.Append("    			<tr>");

                string _t = "      				<td><font size='2pt' color='gray'>[CommentTime] " + I18n.L.T("DetailView.Via") +
                            " [code_name]</font></td>";

                _s.Append(_t);
                _s.Append("    			</tr>");
                _s.Append("    			<tr>");
                _s.Append("      				<td><font size='2.75pt'>[Comment]</font></td>");
                _s.Append("    			</tr>");
                _s.Append("		</table>");
                _s.Append("      	     </td>");
                _s.Append("    	  </tr>");
                _s.Append("	</table>");

                if (post.comment_count != k)
                    _s.Append("<p></p>");

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

            _html += "</font></blockquote>";

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
            DialogResult _dr = MessageBox.Show("Do you really want to remove this post?", "Waveface", MessageBoxButtons.YesNo);

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
    }
}