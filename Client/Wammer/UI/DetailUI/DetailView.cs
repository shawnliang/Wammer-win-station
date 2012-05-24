#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component.PopupControl;
using Waveface.DetailUI;
using Waveface.Localization;
using Waveface.Properties;

#endregion

namespace Waveface
{
    public class DetailView : UserControl
    {
        #region Fields

        private IContainer components;

        private CultureManager cultureManager;

        private IDetailView m_currentView;
        private WebLink_DV m_webLinkDv;
        private Photo_DV m_photoDv;
        private Document_DV m_documentDv;
        private RichText_DV m_richTextDv;

        private DVTopPanel panelTop;
        private Label labelTitle;
        private Timer timerGC;
        private Panel panelMain;
        private Popup m_dateTimePopup;
        private ImageButton btnEdit;
        private Timer timerCanEdit;
        private ImageButton btnFavorite;
        private DateTimePopupPanel m_dateTimePopupPanel;
        private ImageButton btnFunction1;
        private Post m_post;
        private ToolTip toolTip;
        private ImageButton btnAddFootNote;
        private bool m_clockTest;
        private ImageButton m_childBtnFunction1;

        private bool m_existPostAddPhotos;
        private List<string> m_existPostPhotos;
        private int m_existPostAddPhotosIndex;

        #endregion

        #region Properties

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                m_existPostAddPhotos = false;
                m_existPostPhotos = null;

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
            btnFunction1.BackColor = Color.White;
            btnAddFootNote.BackColor = Color.White;

            MouseWheelRedirector.Attach(this);

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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddFootNote = new Waveface.Component.ImageButton();
            this.panelTop = new Waveface.DVTopPanel();
            this.btnFunction1 = new Waveface.Component.ImageButton();
            this.btnFavorite = new Waveface.Component.ImageButton();
            this.btnEdit = new Waveface.Component.ImageButton();
            this.labelTitle = new System.Windows.Forms.Label();
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
            this.timerGC.Interval = 15000;
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
            // btnAddFootNote
            // 
            resources.ApplyResources(this.btnAddFootNote, "btnAddFootNote");
            this.btnAddFootNote.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddFootNote.CenterAlignImage = false;
            this.btnAddFootNote.Image = global::Waveface.Properties.Resources.FB_edit_footnote;
            this.btnAddFootNote.ImageDisable = global::Waveface.Properties.Resources.FB_edit_footnote_hl;
            this.btnAddFootNote.ImageFront = null;
            this.btnAddFootNote.ImageHover = global::Waveface.Properties.Resources.FB_edit_footnote_hl;
            this.btnAddFootNote.Name = "btnAddFootNote";
            this.btnAddFootNote.TextShadow = true;
            this.toolTip.SetToolTip(this.btnAddFootNote, resources.GetString("btnAddFootNote.ToolTip"));
            this.btnAddFootNote.Click += new System.EventHandler(this.btnAddFootNote_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.btnFunction1);
            this.panelTop.Controls.Add(this.btnFavorite);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.labelTitle);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // btnFunction1
            // 
            resources.ApplyResources(this.btnFunction1, "btnFunction1");
            this.btnFunction1.BackColor = System.Drawing.SystemColors.Control;
            this.btnFunction1.CenterAlignImage = false;
            this.btnFunction1.Image = global::Waveface.Properties.Resources.FB_moreoption;
            this.btnFunction1.ImageDisable = global::Waveface.Properties.Resources.FB_moreoption_hl;
            this.btnFunction1.ImageFront = null;
            this.btnFunction1.ImageHover = global::Waveface.Properties.Resources.FB_moreoption_hl;
            this.btnFunction1.Name = "btnFunction1";
            this.btnFunction1.TextShadow = true;
            this.btnFunction1.Click += new System.EventHandler(this.btnMoreOption1_Click);
            this.btnFunction1.DoubleClick += new System.EventHandler(this.btnMoreOption1_DoubleClick);
            // 
            // btnFavorite
            // 
            resources.ApplyResources(this.btnFavorite, "btnFavorite");
            this.btnFavorite.BackColor = System.Drawing.SystemColors.Control;
            this.btnFavorite.CenterAlignImage = false;
            this.btnFavorite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFavorite.Image = global::Waveface.Properties.Resources.FB_fav;
            this.btnFavorite.ImageDisable = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.ImageFront = null;
            this.btnFavorite.ImageHover = global::Waveface.Properties.Resources.FB_fav_hl;
            this.btnFavorite.Name = "btnFavorite";
            this.btnFavorite.TextShadow = true;
            this.toolTip.SetToolTip(this.btnFavorite, resources.GetString("btnFavorite.ToolTip"));
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.BackColor = System.Drawing.Color.White;
            this.btnEdit.CenterAlignImage = false;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.ImageFront = null;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.TextShadow = true;
            this.toolTip.SetToolTip(this.btnEdit, resources.GetString("btnEdit.ToolTip"));
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
            this.Controls.Add(this.btnAddFootNote);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            resources.ApplyResources(this, "$this");
            this.Name = "DetailView";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public void ResetUI()
        {
            btnFunction1.Visible = false;
            btnEdit.Visible = false;
            btnFavorite.Visible = false;
            btnAddFootNote.Visible = false;

            labelTitle.Text = "";

            panelMain.Controls.Clear();
        }

        private void ShowContent()
        {
            if (m_post == null)
                return;

            panelTop.Refresh();

            btnFunction1.Visible = false;

            btnEdit.Visible = true;
            btnFavorite.Visible = true;
            btnAddFootNote.Visible = true;

            setTitle();
            setFavoriteButton();

            panelTop.Enabled = !Main.Current.BatchPostManager.CheckPostInQueue(m_post.post_id);

            PostType _postType = getPostType(m_post.type);

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

            getMoreFunction1();
        }

        private void getMoreFunction1()
        {
            m_childBtnFunction1 = m_currentView.GetMoreFonction1();

            if (m_childBtnFunction1 != null)
            {
                btnFunction1.Visible = true;

                btnFunction1.Image = m_childBtnFunction1.Image;
                btnFunction1.ImageDisable = m_childBtnFunction1.ImageDisable;
                btnFunction1.ImageHover = m_childBtnFunction1.ImageHover;

                toolTip.SetToolTip(btnFunction1, m_childBtnFunction1.Text);
            }
        }

        private void setFavoriteButton()
        {
            int _value = int.Parse(m_post.favorite);

            if (_value == 0)
            {
                btnFavorite.Image = Resources.FB_unfav;
                btnFavorite.ImageDisable = Resources.FB_unfav_hl;
                btnFavorite.ImageHover = Resources.FB_unfav_hl;

                toolTip.SetToolTip(btnFavorite, I18n.L.T("Favorite"));
            }
            else
            {
                btnFavorite.Image = Resources.FB_fav;
                btnFavorite.ImageDisable = Resources.FB_fav_hl;
                btnFavorite.ImageHover = Resources.FB_fav_hl;

                toolTip.SetToolTip(btnFavorite, I18n.L.T("Unfavorite"));
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
            try
            {
                iso8601Time = DateTimeHelp.ISO8601ToDotNet(iso8601Time, false);
                iso8601Time = DateTimeHelp.PrettyDate(iso8601Time, true);
                return iso8601Time;
            }
            catch
            {
                return DateTime.Now.ToString("MM/dd HH:mm:ss");
            }
        }

        private PostType getPostType(string postType)
        {
            switch (postType)
            {
                case "text":
                    return PostType.Text;

                case "doc":
                    return PostType.Document;

                case "link":
                    return PostType.Link;

                case "image":
                    return PostType.Photo;

                case "rtf":

                    return PostType.RichText;
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

        public bool PostComment(RichTextBox textBox, Post post)
        {
            if (!Main.Current.CheckNetworkStatus())
                return false;

            if (textBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("DetailView.CommentEmpty"), "Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }

            MR_posts_newComment _postsNewComment = Main.Current.RT.REST.Posts_NewComment(post.post_id,
                                                                                         StringUtility.LimitByteLength(
                                                                                             StringUtility.
                                                                                                 RichTextBox_ReplaceNewline
                                                                                                 (textBox.Text), 1000),
                                                                                         "", "");

            if (_postsNewComment != null)
            {
                Main.Current.RefreshSinglePost_ByID(post.post_id);
            }

            textBox.Text = "";

            return true;
        }

        public string GenCommentHTML(Post post, bool endHR)
        {
            string _html =
                "<div style='border-left:2px solid #559aae; padding-left:4px'><font face='·L³n¥¿¶ÂÅé, Helvetica, Arial, Verdana, sans-serif' color='#eef'>";

            foreach (Comment _c in post.comments)
            {
                StringBuilder _s = new StringBuilder();

                _s.Append("	<table border=\"0\">");
                _s.Append("    	   <tr>");
                _s.Append("      	     <td>");
                _s.Append(" 		<table border=\"0\">");
                _s.Append("    			<tr>");

                string _t = "      				<td><font size='1.75pt' color=#68b0c5>[CommentTime] " +
                            I18n.L.T("DetailView.Via") +
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

        private void AddComment_Form()
        {
            CommentForm _form = new CommentForm();
            _form.Left = Main.Current.Right - _form.Width - 16;
            _form.Top = Main.Current.Top + 96;

            DialogResult _dr = _form.ShowDialog();

            if (_dr == DialogResult.OK)
            {
                PostComment(_form.CommentTextBox, m_post);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            Main.Current.EditPost(Post, null, -1);
        }

        private void timerCanEdit_Tick(object sender, EventArgs e)
        {
            if (m_currentView != null)
            {
                btnEdit.Enabled = m_currentView.CanEdit();
                btnFunction1.Enabled = m_currentView.CanEdit();

                if(btnEdit.Enabled)
                {
                    if(m_existPostAddPhotos)
                    {
                        m_existPostAddPhotos = false;

                        if(m_existPostPhotos != null)
                        {
                            Main.Current.EditPost(Post, m_existPostPhotos, m_existPostAddPhotosIndex);
                        }
                    }
                }
            }
        }

        public void ExistPostAddPhotos(List<string> pics, int index)
        {
            m_existPostAddPhotos = true;
            m_existPostPhotos = pics;
            m_existPostAddPhotosIndex = index;
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Main.Current.ChangePostFavorite(m_post);

            Cursor = Cursors.Default;
        }

        private void btnMoreOption1_Click(object sender, EventArgs e)
        {
            if (m_childBtnFunction1 != null)
            {
                m_currentView.MoreFonction1();
            }
        }

        public void SetClock(bool visible, DateTime dateTime)
        {
            if (m_clockTest)
            {
                m_dateTimePopupPanel.DateTime = dateTime;

                m_dateTimePopup.Show(this, 4, 44);
            }
        }

        private void btnMoreOption1_DoubleClick(object sender, EventArgs e)
        {
            m_clockTest = !m_clockTest;
        }

        private void btnAddFootNote_Click(object sender, EventArgs e)
        {
            AddComment_Form();
        }
    }
}