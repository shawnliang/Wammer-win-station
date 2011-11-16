#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Compoment.PopupControl;
using Waveface.Component;
using Waveface.Component.DropableNotifyIcon;
using Waveface.FilterUI;
using Waveface.ImageCapture;
using Waveface.Properties;
using Waveface.SettingUI;

#endregion

namespace Waveface
{
    public partial class MainForm : Form
    {
        public static MainForm THIS;

        #region Fields

        public static GCONST GCONST = new GCONST();
        private bool m_canAutoFetchNewestPosts = true;
        private bool m_loginOK;
        private List<string> m_delayPostPicList = new List<string>();

        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
        private DropableNotifyIcon m_dropableNotifyIcon = new DropableNotifyIcon();
        private Popup m_trayIconPopup;
        private TrayIconPanel m_trayIconPanel;

        //ScreenShot
        private ImageStorage m_imageStorage;

        //Main
        private Bitmap m_offlineImage;
        private Bitmap m_onlineImage;
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private VirtualFolderForm m_virtualFolderForm;
        private MyTaskbarNotifier m_taskbarNotifier;

        //Test
        private GroupManager m_groupManager;
        private TestForm m_testForm;
        private FilterManager m_filterManager;

        //V2
        private BEService2 m_serviceV2;
        private MR_auth_login m_login;

        private RunTimeData m_runTimeData = new RunTimeData();

        private string m_currentGroupID;

        #endregion

        #region Property

        public RunTimeData RT
        {
            get { return m_runTimeData; }
            set { m_runTimeData = value; }
        }

        #endregion

        public MainForm()
        {
            THIS = this;

            File.Delete(m_shellContentMenuFilePath); //Hack

            InitializeComponent();

            postsArea.PostsList.DetailView = detailView;

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            initVirtualFolderForm();

            InitTaskbarNotifier();

            ///// Test
            //m_groupManager = new GroupManager();
            //m_groupManager.Show();

            //m_testForm = new TestForm();
            //m_testForm.Show();

            m_filterManager = new FilterManager();
            m_filterManager.Show();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            //-- Main
            m_onlineImage = Resources.Outlook;
            m_offlineImage = Resources.Error;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            UpdateStatusBar();

            InitmDropableNotifyIcon();

            m_trayIconPopup = new Popup(m_trayIconPanel = new TrayIconPanel());

            //-- Screen Shot
            m_imageStorage = new ImageStorage(GCONST.CachePath);

            //-- Send To
            CreateFileWatcher();
        }

        private void InitmDropableNotifyIcon()
        {
            m_dropableNotifyIcon.Text = "Waveface: Drop something here to start post!";
            m_dropableNotifyIcon.NotifyIcon.Icon = Resources.Icon;
            m_dropableNotifyIcon.NotifyIcon.ContextMenuStrip = mnuTray;
            m_dropableNotifyIcon.NotifyIcon.Visible = true;
            m_dropableNotifyIcon.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            m_dropableNotifyIcon.InitDrop();
            m_dropableNotifyIcon.DragEnter += DropableNotifyIcon_DragEnter;
        }

        private void initVirtualFolderForm()
        {
            m_virtualFolderForm = new VirtualFolderForm(this);
            m_virtualFolderForm.Top = Screen.PrimaryScreen.Bounds.Height - m_virtualFolderForm.Height * 3;
            m_virtualFolderForm.Left = Screen.PrimaryScreen.Bounds.Width - m_virtualFolderForm.Width;
            m_virtualFolderForm.Show();
        }

        private void InitTaskbarNotifier()
        {
            m_taskbarNotifier = new MyTaskbarNotifier();
            m_taskbarNotifier.SetBackgroundBitmap(Resources.skin, Color.FromArgb(255, 0, 255));
            m_taskbarNotifier.SetCloseBitmap(Resources.close, Color.FromArgb(255, 0, 255), new Point(127, 8));
            m_taskbarNotifier.TitleRectangle = new Rectangle(40, 9, 70, 25);
            m_taskbarNotifier.ContentRectangle = new Rectangle(8, 41, 133, 68);
            m_taskbarNotifier.CloseClickable = true;
            m_taskbarNotifier.ContentClickable = true;
            m_taskbarNotifier.ContentClick += taskbarNotifier_ContentClick;
            m_taskbarNotifier.EnableSelectionRectangle = true;
            m_taskbarNotifier.KeepVisibleOnMousOver = true;
            m_taskbarNotifier.ReShowOnMouseOver = true;
        }

        #region Main

        #region Drag & Drop

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Enter(e);
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Drop(e);
        }

        private void Form_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave();
        }

        private void Form_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over(e);
        }

        #endregion

        #region Network Status

        private void UpdateStatusBar()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                connectedImageLabel.Text = " Connected";
                connectedImageLabel.Image = m_onlineImage;
            }
            else
            {
                connectedImageLabel.Text = " Disconnected";
                connectedImageLabel.Image = m_offlineImage;
            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Invoke(new MethodInvoker(UpdateStatusBar));
        }

        #endregion

        #region Windows Size

        private void MainForm_Resize(object sender, EventArgs e)
        {
            panelLeftInfo.Width = leftArea.MyWidth + 8;

            m_dropableNotifyIcon.NotifyIcon.BalloonTipTitle = "Wammer";
            m_dropableNotifyIcon.NotifyIcon.BalloonTipText = "Minimize to Tray App";

            if (FormWindowState.Minimized == WindowState)
            {
                m_dropableNotifyIcon.NotifyIcon.ShowBalloonTip(500);
                Hide();
            }
        }

        private void RestoreWindow()
        {
            Show();
            WindowState = FormWindowState.Maximized;
        }

        private void restoreMenuItem_Click(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        private void taskbarNotifier_ContentClick(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        #endregion

        #region Misc

        void DropableNotifyIcon_DragEnter(object sender, DragEventArgs e)
        {
            if (!m_trayIconPopup.Visible)
                m_trayIconPopup.Show(m_dropableNotifyIcon.GetLocation());
        }

        private void timerDelayPost_Tick(object sender, EventArgs e)
        {
            if (m_delayPostPicList.Count == 0)
                return;

            timerDelayPost.Enabled = false;

            Post(m_delayPostPicList, PostType.Photo);

            m_delayPostPicList.Clear();

            timerDelayPost.Enabled = true;
        }

        public void Post()
        {
            Post(new List<string>(), PostType.All);
        }

        public void Post(List<string> pics, PostType postType)
        {
            if (!m_loginOK)
            {
                if (!DoLogin())
                {
                    return; //未能Login
                }
            }

            m_canAutoFetchNewestPosts = false;

            PostForm _form = new PostForm(pics, postType);
            DialogResult _dr = _form.ShowDialog();

            switch (_dr)
            {
                case DialogResult.Yes:
                    showGroupPosts();
                    RestoreWindow();
                    break;

                case DialogResult.OK:
                    leftArea.AddNewPostItem(_form.NewPostItem); //@
                    break;
            }

            m_canAutoFetchNewestPosts = true;
        }

        public void AfterBatchPostDone()
        {
            m_canAutoFetchNewestPosts = false;

            showGroupPosts();

            m_canAutoFetchNewestPosts = true;
        }

        private void showTaskbarNotifier(Post post)
        {
            string _url = string.Empty;
            string _name = string.Empty;

            foreach (User _u in RT.AllUsers)
            {
                if (post.creator_id == _u.user_id)
                {
                    _url = _u.avatar_url;
                    _name = _u.nickname;
                }
            }

            m_taskbarNotifier.AvatarImage = ImageUtility.GetAvatarImage(post.creator_id, _url);
            m_taskbarNotifier.Show(_name, post.content, 333, 2000, 333);
        }

        private void FetchOlderPosts()
        {
            if (m_loginOK)
            {
            }
        }

        public void ReadMore()
        {
            timerFetchOlderPost.Enabled = true;
        }

        private void timerFetchOlderPost_Tick(object sender, EventArgs e)
        {
            timerFetchOlderPost.Enabled = false;

            m_canAutoFetchNewestPosts = false;

            FetchOlderPosts();

            m_canAutoFetchNewestPosts = true;
        }

        private void timerGetNewestPost_Tick(object sender, EventArgs e)
        {
            timerGetNewestPost.Enabled = false;

            if (m_canAutoFetchNewestPosts)
            {
                //if (m_loginOK)
                //    showGroupPosts();
            }

            timerGetNewestPost.Enabled = true;
        }

        private void pictureBoxPost_Click(object sender, EventArgs e)
        {
            Post();
        }

        #endregion

        #region Login V2

        public bool DoLogin()
        {
            bool _ret = false;

            m_loginOK = Login();

            Cursor.Current = Cursors.WaitCursor;

            if (m_loginOK)
            {
                Reset();

                getGroupAndUser();

                fillUserInformation();
                fillGroupAndUser();

                if (m_login.groups.Count > 0)
                    showGroupPosts(m_login.groups[0].group_id);

                _ret = true;
            }

            Cursor.Current = Cursors.Default;

            return _ret;
        }

        private bool Login()
        {
            LoginForm _loginForm = new LoginForm();
            DialogResult _dr = _loginForm.ShowDialog();

            if (_dr != DialogResult.OK)
                return false;

            Cursor.Current = Cursors.WaitCursor;

            m_serviceV2 = new BEService2();

            m_login = m_serviceV2.auth_login(_loginForm.User, _loginForm.Password);

            Cursor.Current = Cursors.Default;

            if (m_login == null)
            {
                MessageBox.Show("Login Error!");
                return false;
            }

            return (m_login.status == "200");
        }

        #endregion

        #region V2

        private void Reset()
        {
            RT.Reset();

            m_currentGroupID = string.Empty;
        }

        private void fillUserInformation()
        {
            labelName.Text = m_login.user.nickname;

            if (m_login.user.avatar_url == string.Empty)
            {
                pictureBoxAvatar.Image = null;
            }
            else
            {
                pictureBoxAvatar.LoadAsync(m_login.user.avatar_url);
            }
        }

        private void fillGroupAndUser()
        {
            leftArea.fillGroupAndUser();
        }

        private void getGroupAndUser()
        {
            foreach (Group _g in m_login.groups)
            {
                MR_groups_get _mrGroupsGet = m_serviceV2.groups_get(m_login.session_token, _g.group_id);

                if ((_mrGroupsGet != null) && (_mrGroupsGet.status == "200"))
                {
                    if (!RT.GroupSets.ContainsKey(_g.group_id)) //Hack
                        RT.GroupSets.Add(_g.group_id, _mrGroupsGet);

                    foreach (User _u in _mrGroupsGet.active_members)
                    {
                        if (!RT.AllUsers.Contains(_u))
                            RT.AllUsers.Add(_u);
                    }
                }
            }
        }

        public List<Post> getPostsByGroupID(string groupID)
        {
            bool _addnew;

            return getPostsByGroupID(groupID, out _addnew);
        }

        public List<Post> getPostsByGroupID(string groupID, out bool addNew)
        {
            List<Post> _ret = null;

            addNew = false;

            if (RT.GroupPosts.ContainsKey(groupID) && RT.GroupPosts[groupID].Count > 0)
            {
                addNew = FetchNewestPostsV2();

                return RT.GroupPosts[groupID];
            }
            else
            {
                MR_posts_getLatest _mrPostsGetLatest = m_serviceV2.posts_getLatest(m_login.session_token, groupID,
                                                                                   "50");

                if ((_mrPostsGetLatest != null) && (_mrPostsGetLatest.status == "200"))
                {
                    if (!RT.GroupPosts.ContainsKey(groupID))
                    {
                        RT.GroupPosts.Add(groupID, _mrPostsGetLatest.posts);
                        addNew = true;
                    }

                    return RT.GroupPosts[groupID];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool FetchNewestPostsV2()
        {
            if (m_loginOK)
            {
                if (RT.GroupPosts[m_currentGroupID].Count > 0)
                {
                    string _newestPostTime = RT.GroupPosts[m_currentGroupID][0].timestamp;
                    string _newestPostID = RT.GroupPosts[m_currentGroupID][0].post_id;

                    MR_posts_get _postsGet = m_serviceV2.posts_get(m_login.session_token, m_currentGroupID, "+100",
                                                                   _newestPostTime, "");

                    if ((_postsGet != null) && (_postsGet.status == "200"))
                    {
                        if (_postsGet.posts.Count > 0)
                        {
                            // 刪除比較基準的那個Post, 如果有回傳的話!
                            Post _toDel = null;

                            foreach (Post _p in _postsGet.posts)
                            {
                                if (_p.post_id == _newestPostID)
                                {
                                    _toDel = _p;
                                    break;
                                }
                            }

                            if (_toDel != null)
                                _postsGet.posts.Remove(_toDel);

                            RT.GroupPosts[m_currentGroupID].InsertRange(0, _postsGet.posts);

                            //showTaskbarNotifier(_posts[0]);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void showGroupPosts(string groupID)
        {
            m_currentGroupID = groupID;
            SetPostAraePost(getPostsByGroupID(groupID));
        }

        public void showGroupPosts()
        {
            bool _addNew;

            List<Post> _posts = getPostsByGroupID(m_currentGroupID, out _addNew);

            if (_addNew)
                SetPostAraePost(_posts);
        }

        private void SetPostAraePost(List<Post> posts)
        {
            setCalendarBoldedDates(posts);

            postsArea.PostsList.Posts = posts;
        }

        public MR_posts_new Post_CreateNewPost(string text, string files, string previews, string type)
        {
            MR_posts_new _postsNew = m_serviceV2.posts_new(m_login.session_token, m_currentGroupID, text, files, previews, type);

            if ((_postsNew != null) && (_postsNew.status == "200"))
            {
                return _postsNew;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_newComment Posts_NewComment(string post_id, string content, string objects, string previews)
        {
            MR_posts_newComment _newComment = m_serviceV2.posts_newComment(m_login.session_token, m_currentGroupID, post_id, content, objects, previews);

            if ((_newComment != null) && (_newComment.status == "200"))
            {
                return _newComment;
            }
            else
            {
                return null;
            }
        }

        public MR_posts_getSingle Posts_GetSingle(string post_id)
        {
            MR_posts_getSingle _getSingle = m_serviceV2.posts_getSingle(m_login.session_token, m_currentGroupID, post_id);

            if ((_getSingle != null) && (_getSingle.status == "200"))
            {
                return _getSingle;
            }
            else
            {
                return null;
            }
        }

        public MR_previews_get_adv Preview_GetAdvancedPreview(string url)
        {
            MR_previews_get_adv _previewsGetAdv = m_serviceV2.previews_get_adv(m_login.session_token, url);

            if ((_previewsGetAdv != null) && (_previewsGetAdv.status == "200"))
            {
                return _previewsGetAdv;
            }
            else
            {
                return null;
            }
        }

        public MR_attachments_upload File_UploadFile(string text, string filePath, bool isImage)
        {
            MR_attachments_upload _attachmentsUpload = null;

            if (isImage)
                _attachmentsUpload = m_serviceV2.attachments_upload(m_login.session_token, m_currentGroupID, filePath, text, "", "image", "origin");
            else
                _attachmentsUpload = m_serviceV2.attachments_upload(m_login.session_token, m_currentGroupID, filePath, text, "", "doc", "");

            if ((_attachmentsUpload != null) && (_attachmentsUpload.status == "200"))
            {
                return _attachmentsUpload;
            }
            else
            {
                return null;
            }
        }

        public string attachments_getRedirectURL(string orgURL)
        {
            return m_serviceV2.attachments_getRedirectURL(orgURL, m_login.session_token);
        }

        public void RefreshUIAfterPostComment(Post post)
        {
            List<Post> _posts = RT.GroupPosts[m_currentGroupID];

            int k = -1;

            for (int i = 0; i < _posts.Count; i++)
            {
                if (_posts[i].post_id.Equals(post.post_id))
                {
                    k = i;
                    break;
                }
            }

            // 不要將此段寫在上面迴圈的 if 裡
            if (k != -1)
            {
                _posts[k] = post;
                showGroupPosts(m_currentGroupID);
            }

        }

        #endregion

        #region Calendar

        private void setCalendarBoldedDates(List<Post> posts)
        {
            CustomControls.MonthCalendar _calendar = leftArea.MonthCalendar;

            _calendar.SuspendLayout();

            _calendar.BoldedDates.Clear();

            foreach (Post _p in posts)
            {
                DateTime _dt = DateTimeHelp.ISO8601ToDateTime(_p.timestamp);

                if (!_calendar.BoldedDates.Contains(_dt.Date))
                    _calendar.BoldedDates.Add(_dt.Date);
            }

            _calendar.ResumeLayout();
        }

        public void ClickCalendar(DateTime date)
        {
            CustomControls.MonthCalendar _calendar = leftArea.MonthCalendar;

            if (!_calendar.BoldedDates.Contains(date.Date))
                return;

            postsArea.PostsList.ScrollToDay(date.Date);
        }

        public void setCalendarDay(DateTime date)
        {
            CustomControls.MonthCalendar _calendar = leftArea.MonthCalendar;
            _calendar.SelectionStart = date;
            _calendar.SelectionEnd = date;
        }

        #endregion

        #endregion

        #region SendTo

        public void CreateFileWatcher()
        {
            FileSystemWatcher _watcher = new FileSystemWatcher();
            _watcher.Filter = "*.dat";
            _watcher.Created += watcher_FileCreated;
            _watcher.Path = Application.StartupPath;
            _watcher.EnableRaisingEvents = true;
        }

        private void watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                try
                {
                    if (e.FullPath == m_shellContentMenuFilePath)
                    {
                        string _fileContents;

                        using (StreamReader _textFile = new StreamReader(m_shellContentMenuFilePath))
                        {
                            _fileContents = _textFile.ReadToEnd();
                        }

                        m_delayPostPicList = new List<string>
                                                 {
                                                     _fileContents
                                                 };
                    }
                }
                catch
                {
                }
            }

            File.Delete(m_shellContentMenuFilePath);
        }

        #endregion

        #region Screen Shot

        private void capture(ShotType shotType)
        {
            if (m_imageStorage == null)
                return;

            try
            {
                CaptureForm _captureForm = new CaptureForm(shotType);

                if ((_captureForm.ShowDialog() != DialogResult.OK) || (_captureForm.Image == null))
                {
                    return;
                }

                ImageFormat _imgFormat = ImageFormat.Jpeg;

                string _filename =
                    string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmssff"), _imgFormat).ToLower();

                Image _img = _captureForm.Image;
                m_imageStorage.SaveImage(_img, _imgFormat, _filename);

                Post(new List<string> { GCONST.CachePath + _filename }, PostType.Photo);
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnMenuExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void regionMenuItem_Click(object sender, EventArgs e)
        {
            capture(ShotType.Region);
        }

        private void windowsMenuItem_Click(object sender, EventArgs e)
        {
            capture(ShotType.Window);
        }

        private void screenMenuItem_Click(object sender, EventArgs e)
        {
            capture(ShotType.Screen);
        }

        #endregion

        private void linkLabelLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DoLogin();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dropableNotifyIcon.Dispose();
        }

        private void preferencesMenuItem_Click(object sender, EventArgs e)
        {
            FormOption _form = new FormOption();
            _form.ShowDialog();
        }
    }
}