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
        private FileUploadToStation m_fileUploadToStation;

        private string m_stationIP;

        //ScreenShot
        private ImageStorage m_imageStorage;

        //Main
        private Bitmap m_offlineImage;
        private Bitmap m_onlineImage;
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private VirtualFolderForm m_virtualFolderForm;
        private MyTaskbarNotifier m_taskbarNotifier;

        //V2
        private BEService2 m_serviceV2;
        private MR_auth_login m_login;
        private RunTimeData m_runTimeData = new RunTimeData();

        #endregion

        #region Properties

        public RunTimeData RT
        {
            get { return m_runTimeData; }
            set { m_runTimeData = value; }
        }

        private string SessionToken
        {
            get { return m_login.session_token; }
        }

        #endregion

        public MainForm()
        {
            THIS = this;

            File.Delete(m_shellContentMenuFilePath); //Hack

            InitializeComponent();

            postsArea.PostsList.DetailView = detailView;

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            //initVirtualFolderForm();

            InitTaskbarNotifier();

            m_serviceV2 = new BEService2();
        }

        #region Init

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

            m_fileUploadToStation = new FileUploadToStation();
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

        #endregion

        #region Main

        #region Event

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dropableNotifyIcon.Dispose();

            if (m_virtualFolderForm != null)
                m_virtualFolderForm.Close();
        }

        private void preferencesMenuItem_Click(object sender, EventArgs e)
        {
            PreferenceForm _form = new PreferenceForm();
            _form.ShowDialog();
        }

        #endregion

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

        void DropableNotifyIcon_DragEnter(object sender, DragEventArgs e)
        {
            if (!m_trayIconPopup.Visible)
                m_trayIconPopup.Show(m_dropableNotifyIcon.GetLocation());
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
                //Hide();
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

        #endregion

        #region API

        public string attachments_getRedirectURL(string orgURL, string object_id, bool isImage)
        {
            return ServerImageAddressUtility.attachments_getRedirectURL(orgURL, SessionToken, object_id, isImage);
        }

        public string attachments_getRedirectURL_Image(Attachment a, string imageType, out string url, out string fileName)
        {
            return ServerImageAddressUtility.attachments_getRedirectURL_Image(SessionToken, a, imageType, out url, out fileName);
        }

        public string attachments_getRedirectURL_PdfCoverPage(string orgURL)
        {
            return ServerImageAddressUtility.attachments_getRedirectURL_PdfCoverPage(orgURL, SessionToken);
        }

        public MR_posts_new Post_CreateNewPost(string text, string files, string previews, string type)
        {
            MR_posts_new _postsNew = m_serviceV2.posts_new(SessionToken, RT.CurrentGroupID, text, files, previews, type);

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
            MR_posts_newComment _newComment = m_serviceV2.posts_newComment(SessionToken, RT.CurrentGroupID, post_id, content, objects, previews);

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
            MR_posts_getSingle _getSingle = m_serviceV2.posts_getSingle(SessionToken, RT.CurrentGroupID, post_id);

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
            MR_previews_get_adv _previewsGetAdv = m_serviceV2.previews_get_adv(SessionToken, url);

            if ((_previewsGetAdv != null) && (_previewsGetAdv.status == "200"))
            {
                return _previewsGetAdv;
            }
            else
            {
                return null;
            }
        }

        public MR_attachments_upload File_UploadFile(string text, string filePath, string object_id, bool isImage)
        {
            MR_attachments_upload _attachmentsUpload;
            string _resizedImageFilePath = string.Empty;

            if (isImage)
            {
                if (RT.IsStationOK) //如果有Station則上傳原圖, 否則就上512中圖
                {
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID, filePath, text, "", "image", "origin", object_id);
                }
                else
                {
                    _resizedImageFilePath = ImageUtility.ResizeImage(filePath, text, "512", 50);
                    _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID, _resizedImageFilePath, text, "", "image", "medium", object_id);
                }
            }
            else
            {
                _attachmentsUpload = m_serviceV2.attachments_upload(SessionToken, RT.CurrentGroupID, filePath, text, "", "doc", "", "");
            }

            if ((_attachmentsUpload != null) && (_attachmentsUpload.status == "200"))
            {
                // 如果傳中圖到Cloud, 則要把原圖Cache起來, 待有Station在傳原圖
                if (_resizedImageFilePath != string.Empty)
                {
                    string _ext = ".jpg";

                    int _idx = text.IndexOf(".");

                    if (_idx != -1)
                        _ext = text.Substring(_idx);

                    string _originCacheFile = GCONST.ImageUploadCachePath + _attachmentsUpload.object_id + _ext;
                    File.Copy(filePath, _originCacheFile);
                }

                return _attachmentsUpload;
            }

            return null;
        }

        public MR_posts_get Posts_FetchByFilter(string filter_entity)
        {
            MR_posts_get _postsGet = m_serviceV2.posts_fetchByFilter(SessionToken, RT.CurrentGroupID, filter_entity);

            if ((_postsGet != null) && (_postsGet.status == "200"))
            {
                return _postsGet;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_list SearchFilters_List()
        {
            MR_fetchfilters_list _filtersList = m_serviceV2.fetchfilters_list(SessionToken);

            if ((_filtersList != null) && (_filtersList.status == "200"))
            {
                return _filtersList;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_item FetchFilters_New(string filter_name, string filter_entity, string tag)
        {
            MR_fetchfilters_item _item = m_serviceV2.fetchfilters_new(SessionToken, filter_name, filter_entity, tag);

            if ((_item != null) && (_item.status == "200"))
            {
                return _item;
            }
            else
            {
                return null;
            }
        }

        public MR_fetchfilters_item FetchFilters_Update(string searchfilter_id, string filter_name, string filter_entity, string tag)
        {
            MR_fetchfilters_item _item = m_serviceV2.fetchfilters_update(SessionToken, searchfilter_id, filter_name, filter_entity, tag);

            if ((_item != null) && (_item.status == "200"))
            {
                return _item;
            }
            else
            {
                return null;
            }
        }

        public MR_users_findMyStation Users_findMyStation()
        {
            MR_users_findMyStation _findMyStation = m_serviceV2.users_findMyStation(SessionToken);

            if ((_findMyStation != null) && (_findMyStation.status == "200"))
            {
                return _findMyStation;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Login

        public void Reset()
        {
            RT.Reset();

            BEService2.StationIP = "";

            postsArea.ShowTypeUI(false);
        }

        public bool Login(string email, string password)
        {
            bool _ret = false;

            m_loginOK = Login_Service(email, password);

            Cursor.Current = Cursors.WaitCursor;

            if (m_loginOK)
            {
                Reset();

                CheckStation(m_login.stations);

                getGroupAndUser();

                fillUserInformation();
                fillGroupAndUser();

                //預設群組
                RT.CurrentGroupID = m_login.groups[0].group_id;

                //顯示所有Post
                DoTimelineFilter(FilterHelper.CreateAllPostFilterItem(), true);

                //
                leftArea.FillCustomizedFilters();
                leftArea.SetUI();

                _ret = true;
            }

            Cursor.Current = Cursors.Default;

            return _ret;
        }

        private void CheckStation(List<Station> stations)
        {
            if (stations != null)
            {
                foreach (Station _station in stations)
                {
                    if (_station.status == "connected")
                    {
                        string _ip = _station.location;

                        if (_ip.EndsWith("/"))
                            _ip = _ip.Substring(0, _ip.Length - 1);

                        BEService2.StationIP = _ip;

                        //test
                        m_stationIP = _ip;
                        //panelStation.Visible = true;

                        RT.IsStationOK = true;

                        return;
                    }
                }
            }

            RT.IsStationOK = false;
        }

        private bool Login_Service(string email, string password)
        {
            Cursor.Current = Cursors.WaitCursor;

            m_login = m_serviceV2.auth_login(email, password);

            Cursor.Current = Cursors.Default;

            if (m_login == null)
            {
                return false;
            }

            return (m_login.status == "200");
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
                MR_groups_get _mrGroupsGet = m_serviceV2.groups_get(SessionToken, _g.group_id);

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

        #endregion

        #region Filter

        public void DoTimelineFilter(FilterItem item, bool isTimelineFilter)
        {
            if (!m_loginOK)
                return;

            if (item != null) //會null是由PostArea的comboBoxType發出
            {
                RT.IsAllPostMode = item.IsAllPost;
                RT.CurrentFilterItem = item;
            }

            RT.CurrentPosts = new List<Post>(); //Reset

            RT.IsTimelineFilter = isTimelineFilter; // 是Timeline才秀Type
            postsArea.ShowTypeUI(RT.IsTimelineFilter);

            string _title = "[" + RT.CurrentFilterItem.Name + "]";
            _title += (postsArea.GetPostTypeText() == "All Posts") ? "" : (" - " + postsArea.GetPostTypeText());

            FetchPostsAndShow(true);
        }

        #endregion

        #region Post

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
                MessageBox.Show("Please Login first.", "Waveface");
                return;
            }

            m_canAutoFetchNewestPosts = false;

            try
            {
                PostForm _form = new PostForm(pics, postType);
                DialogResult _dr = _form.ShowDialog();

                switch (_dr)
                {
                    case DialogResult.Yes:
                        // ------------------ OLD_showAllPosts();
                        RestoreWindow();
                        break;

                    case DialogResult.OK:
                        leftArea.AddNewPostItem(_form.NewPostItem); //@
                        break;
                }
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message, "Waveface");
            }

            m_canAutoFetchNewestPosts = true;
        }

        #endregion

        #region Hide Post、Attachment

        public void HidePost(string postId)
        {
            /*
            MR_hide_ret _ret = Hide_Set_Post(postId);

            if (_ret != null)
            {
                MessageBox.Show("Remove Post Success!");

                RT.HideList = Hide_List("all");
                ShowPostToUI(false);
            }
            */
        }

        #endregion

        #region AfterPostComment

        public void AfterPostComment(string post_id)
        {
            MR_posts_getSingle _singlePost = THIS.Posts_GetSingle(post_id);

            if ((_singlePost != null) && (_singlePost.post != null))
            {
                // AllPosts 跟 FilterPosts 都要更新, 如果有的話
                ReplacePostInList(_singlePost.post, RT.CurrentGroupPosts);
                ReplacePostInList(_singlePost.post, RT.FilterPosts);

                ShowPostToUI(true);
            }
        }

        private bool ReplacePostInList(Post post, List<Post> posts)
        {
            int k = -1;

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].post_id.Equals(post.post_id))
                {
                    k = i;
                    break;
                }
            }

            // 不要將此段寫在上面迴圈的 if 裡
            if (k != -1)
            {
                posts[k] = post;

                return true;
            }

            return false;
        }

        #endregion

        #region Newer or Older Post

        public void ReadMorePost()
        {
            timerFetchOlderPost.Enabled = true;
        }

        private void timerFetchOlderPost_Tick(object sender, EventArgs e)
        {
            timerFetchOlderPost.Enabled = false;

            m_canAutoFetchNewestPosts = false;

            FetchPostsAndShow(false);

            m_canAutoFetchNewestPosts = true;
        }

        private void timerGetNewestPost_Tick(object sender, EventArgs e)
        {
            timerGetNewestPost.Enabled = false;

            if (m_canAutoFetchNewestPosts)
            {
                //RefreshNewestPosts();
            }

            timerGetNewestPost.Enabled = true;
        }

        public bool RefreshNewestPosts()
        {
            if (m_loginOK)
            {
                if (RT.CurrentGroupPosts.Count > 0)
                {
                    string _newestPostTime = RT.CurrentGroupPosts[0].timestamp;
                    string _newestPostID = RT.CurrentGroupPosts[0].post_id;

                    MR_posts_get _postsGet = m_serviceV2.posts_get(SessionToken, RT.CurrentGroupID, "+100",
                                                                   _newestPostTime, "");

                    if ((_postsGet != null) && (_postsGet.status == "200"))
                    {
                        if (_postsGet.posts.Count > 0)
                        {
                            //刪除比較基準的那個Post, 如果有回傳的話!
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
                            {
                                _postsGet.posts.Remove(_toDel);
                            }

                            if (_postsGet.posts.Count > 0)
                            {
                                //@ RT.CurrentGroupPosts.InsertRange(0, _postsGet.posts);
                                // MessageBox.Show(_postsGet.posts.Count + " New Post");

                                postsArea.ShowNewPostInfo(_postsGet.posts.Count);
                            }

                            //showTaskbarNotifier(_posts[0]);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void AfterBatchPostDone()
        {
            m_canAutoFetchNewestPosts = false;

            // --------------------------   OLD_showAllPosts();

            m_canAutoFetchNewestPosts = true;
        }

        #endregion

        #region Misc

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

        #endregion

        #region Helper

        private void FetchPostsAndShow(bool firstTime)
        {
            int _offset = GCONST.GetPostOffset;

            if (RT.IsAllPostMode)
            {
                if (RT.CurrentGroupPosts != null)
                    _offset = RT.CurrentGroupPosts.Count;

                if (firstTime)
                    postsArea.ShowNewPostInfo(0);
            }
            else
            {
                _offset = RT.FilterPosts.Count;
            }

            string _filter = RT.CurrentFilterItem.Filter;
            _filter = _filter.Replace("[type]", postsArea.GetPostType());
            _filter = _filter.Replace("[offset]", _offset.ToString());

            if (RT.CurrentPostsAllCount == RT.CurrentPosts.Count) //已經都抓完了
                return;

            Cursor.Current = Cursors.WaitCursor;

            MR_posts_get _postsGet = Posts_FetchByFilter(_filter);

            if (_postsGet != null)
            {
                RT.CurrentPosts.AddRange(_postsGet.posts);

                if (firstTime)
                {
                    RT.CurrentPostsAllCount = _postsGet.remaining_count + _postsGet.get_count;

                    RT.IsFirstTimeGetData = true;
                }
                else
                {
                    RT.IsFirstTimeGetData = false;
                }
            }

            Cursor.Current = Cursors.Default;

            ShowPostToUI(false);
        }

        private void ShowPostToUI(bool refreshCurrentPost)
        {
            List<Post> _posts = RT.CurrentPosts;

            //List<Post> _unhidePosts = GetUnhidePosts(_posts);

            setCalendarBoldedDates(_posts);

            // int _dt = _posts.Count - _unhidePosts.Count;

            postsArea.ShowPostInfo(RT.CurrentPostsAllCount, _posts.Count);

            postsArea.PostsList.SetPosts(_posts, refreshCurrentPost, RT.IsFirstTimeGetData);
        }

        public void PostListClick(int clickIndex)
        {
            RT.IsFirstTimeGetData = false;
        }

        #endregion

        #region Calendar

        private void setCalendarBoldedDates(List<Post> posts)
        {
            CustomControls.MonthCalendar _mc = leftArea.MonthCalendar;

            _mc.SuspendLayout();

            _mc.BoldedDates.Clear();

            foreach (Post _p in posts)
            {
                DateTime _dt = DateTimeHelp.ISO8601ToDateTime(_p.timestamp);

                if (!_mc.BoldedDates.Contains(_dt.Date))
                    _mc.BoldedDates.Add(_dt.Date);
            }

            _mc.ResumeLayout();

            _mc.Invalidate();
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

        public void FillTimelineComboBox(List<FilterItem> list)
        {
            postsArea.FillTimelineComboBox(list);
        }

        public void ShowPostAreaTimelineComboBox(bool visible)
        {
            postsArea.ShowTimelineComboBox(visible);
        }

        private void radioButtonStation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCloud.Checked)
            {
                BEService2.StationIP = BEService2.CloundIP;
                RT.IsStationOK = false;
            }
            else
            {
                BEService2.StationIP = m_stationIP;
                RT.IsStationOK = true;
            }
        }

        private void linkLabelLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}