#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component.PopupControl;
using Waveface.Component;
using Waveface.Component.RichEdit;
using Waveface.DetailUI;
using Waveface.ImageCapture.States;

#endregion

namespace Waveface
{
    public class DetailView : UserControl
    {
        #region Fields

        private IContainer components;

        private Localization.CultureManager cultureManager;

        private IDetailView m_currentView;
        private WebLink_DV m_webLinkDv;
        private Photo_DV m_photoDv;
        private Document_DV m_documentDv;
        private RichText_DV m_richTextDv;

        private DVTopPanel panelTop;
        private Label labelTitle;
        private Timer timerGC;
        private Panel panelMain;
        private Popup m_commentPopup;
        private Popup m_dateTimePopup;
        private ImageButton btnEdit;
        private Timer timerCanEdit;
        private ImageButton btnFavorite;
        private CommentPopupPanel m_commentPopupPanel;
        private DateTimePopupPanel m_dateTimePopupPanel;
        private ImageButton btnMoreOptions;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem miRemovePost;
        private ToolStripMenuItem miAddFootnote;

        private Post m_post;
        private bool m_loadOK;
        private bool m_clockTest;

        #endregion

        #region Properties

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                ShowContent();
            }
        }

        public User User { get; set; }

        #endregion

        public DetailView()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            btnEdit.BackColor = Color.White;
            btnFavorite.BackColor = Color.White;
            btnMoreOptions.BackColor = Color.White;

            MouseWheelRedirector.Attach(this);

            m_commentPopup = new Popup(m_commentPopupPanel = new CommentPopupPanel())
                                 {
                                     Resizable = true
                                 };

            m_commentPopupPanel.buttonAddComment.Click += buttonAddComment_Click;

            m_dateTimePopup = new Popup(m_dateTimePopupPanel = new DateTimePopupPanel());
            m_dateTimePopupPanel.MyParent = m_dateTimePopup;
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.timerGC = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.timerCanEdit = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRemovePost = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddFootnote = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new Waveface.DVTopPanel();
            this.btnMoreOptions = new Waveface.Component.ImageButton();
            this.btnFavorite = new Waveface.Component.ImageButton();
            this.btnEdit = new Waveface.Component.ImageButton();
            this.labelTitle = new System.Windows.Forms.Label();
            this.contextMenuStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
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
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRemovePost,
            this.miAddFootnote});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // miRemovePost
            // 
            this.miRemovePost.Image = global::Waveface.Properties.Resources.FB_remove;
            this.miRemovePost.Name = "miRemovePost";
            resources.ApplyResources(this.miRemovePost, "miRemovePost");
            this.miRemovePost.Click += new System.EventHandler(this.miRemovePost_Click);
            // 
            // miAddFootnote
            // 
            this.miAddFootnote.Image = global::Waveface.Properties.Resources.FB_edit_footnote;
            this.miAddFootnote.Name = "miAddFootnote";
            resources.ApplyResources(this.miAddFootnote, "miAddFootnote");
            this.miAddFootnote.Click += new System.EventHandler(this.miAddFootnote_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.btnMoreOptions);
            this.panelTop.Controls.Add(this.btnFavorite);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.labelTitle);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            // 
            // btnMoreOptions
            // 
            resources.ApplyResources(this.btnMoreOptions, "btnMoreOptions");
            this.btnMoreOptions.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoreOptions.CenterAlignImage = false;
            this.btnMoreOptions.Image = global::Waveface.Properties.Resources.FB_moreoption;
            this.btnMoreOptions.ImageDisable = global::Waveface.Properties.Resources.FB_moreoption_hl;
            this.btnMoreOptions.ImageHover = global::Waveface.Properties.Resources.FB_moreoption_hl;
            this.btnMoreOptions.Name = "btnMoreOptions";
            this.btnMoreOptions.Click += new System.EventHandler(this.btnMoreOptions_Click);
            this.btnMoreOptions.DoubleClick += new System.EventHandler(this.btnMoreOptions_DoubleClick);
            // 
            // btnFavorite
            // 
            resources.ApplyResources(this.btnFavorite, "btnFavorite");
            this.btnFavorite.BackColor = System.Drawing.SystemColors.Control;
            this.btnFavorite.CenterAlignImage = false;
            this.btnFavorite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFavorite.Image = global::Waveface.Properties.Resources.FB_fav;
            this.btnFavorite.ImageDisable = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.ImageHover = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.Name = "btnFavorite";
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.CenterAlignImage = false;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageDisable = ((System.Drawing.Image)(resources.GetObject("btnEdit.ImageDisable")));
            this.btnEdit.ImageHover = ((System.Drawing.Image)(resources.GetObject("btnEdit.ImageHover")));
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(121)))), ((int)(((byte)(143)))));
            this.labelTitle.Name = "labelTitle";
            // 
            // DetailView
            // 
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            resources.ApplyResources(this, "$this");
            this.Name = "DetailView";
            this.contextMenuStrip.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private void ShowContent()
        {
            if (m_post == null)
                return;

            m_loadOK = true;
            panelTop.Refresh();

            btnEdit.Visible = true;
            btnFavorite.Visible = true;
            btnMoreOptions.Visible = true;

            setTitle();
            setFavoriteButton();

            panelTop.Enabled = !Main.Current.BatchPostManager.CheckPostInQueue(m_post.post_id);

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

        private void setTitle()
        {
            var _postTime = GetTime(Post.timestamp);

            string _s = _postTime + " " + I18n.L.T("DetailView.Via") + " " + Post.code_name;

            labelTitle.Text = _s;
        }

        private string GetTime(string iso8601Time)
        {
            iso8601Time = DateTimeHelp.ISO8601ToDotNet(iso8601Time, false);
            iso8601Time = DateTimeHelp.PrettyDate(iso8601Time, true);
            return iso8601Time;
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
            m_webLinkDv.Post = m_post;

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
            m_photoDv.Post = m_post;

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
            m_documentDv.Post = m_post;

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
            m_richTextDv.Post = m_post;

            m_currentView = m_richTextDv;

            panelMain.Controls.Add(m_richTextDv);
        }

        private void timerGC_Tick(object sender, EventArgs e)
        {
            GC.Collect(); // Hack
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
            string _html = "<div style='border-left:2px solid #559aae; padding-left:4px'><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif' color='#eef'>";

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

                string _content = HttpUtility.HtmlEncode(_c.content);
                _content = _content.Replace(Environment.NewLine, "<BR>");
                _content = _content.Replace("\n", "<BR>");
                _content = _content.Replace("\r", "<BR>");

                _html += _s.ToString();
                _html = _html.Replace("[Comment]", _content);
                _html = _html.Replace("[CommentTime]", GetTime(_c.timestamp));
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

            if (endHR)
                _html += "<HR>";

            return _html;
        }

        private void AddComment()
        {
            m_commentPopup.Width = (Width * 3) / 4;
            m_commentPopup.Height = 144;
            m_commentPopupPanel.CommentTextBox.Text = string.Empty;
            m_commentPopup.Show(btnMoreOptions, (-1 * m_commentPopupPanel.Width) + btnMoreOptions.Width, btnMoreOptions.Height);
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

        private void RemovePost()
        {
            DialogResult _dr = MessageBox.Show(I18n.L.T("AskRemovePost"), "Waveface", MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

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
                btnMoreOptions.Enabled = m_currentView.CanEdit();
            }
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Main.Current.ChangePostFavorite(m_post, true);

            Cursor = Cursors.Default;
        }

        private void btnMoreOptions_Click(object sender, EventArgs e)
        {
            Point _ptLowerLeft = new Point(0, btnMoreOptions.Height);
            _ptLowerLeft = btnMoreOptions.PointToScreen(_ptLowerLeft);

            _ptLowerLeft.X -= contextMenuStrip.Width - btnMoreOptions.Width;

            contextMenuStrip.Show(_ptLowerLeft);
        }

        private void miRemovePost_Click(object sender, EventArgs e)
        {
            RemovePost();
        }

        private void miAddFootnote_Click(object sender, EventArgs e)
        {
            AddComment();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            while (contextMenuStrip.Items.Count > 2) //除共同的之外都移除
            {
                contextMenuStrip.Items.RemoveAt(contextMenuStrip.Items.Count - 1);
            }

            List<ToolStripMenuItem> _items = m_currentView.GetMoreMenuItems();

            if (_items != null)
            {
                foreach (ToolStripMenuItem _item in _items)
                {
                    contextMenuStrip.Items.Add(_item);
                }
            }
        }

        #region BorderlessForm

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, long lParam, long wParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Main.Current.BorderlessFormTheme.HostWindow.WinMaxed)
            {
                if ((MouseButtons.ToString() == "Left") && (e.Y < 24))
                {
                    ReleaseCapture();

                    uint WM_NCLBUTTONDOWN = 161;
                    int HT_CAPTION = 0x2;

                    SendMessage(Parent.Parent.Parent.Parent.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        #endregion

        public void SetClock(bool visible, DateTime dateTime)
        {
            if (m_clockTest)
            {
                m_dateTimePopupPanel.DateTime = dateTime;

                m_dateTimePopup.Show(this, 4, 44);    
            }
        }

        private void btnMoreOptions_DoubleClick(object sender, EventArgs e)
        {
            m_clockTest = !m_clockTest;
        }
    }
}