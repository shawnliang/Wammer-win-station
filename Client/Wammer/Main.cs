#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Compoment.PopupControl;
using Waveface.Component;
using Waveface.Component.DropableNotifyIcon;
using Waveface.Configuration;
using Waveface.FilterUI;
using Waveface.ImageCapture;
using Waveface.Properties;
using Waveface.SettingUI;
using MonthCalendar = CustomControls.MonthCalendar;

#endregion

namespace Waveface
{
    public partial class Main : Form
    {
        public static Main Current;
        public static GCONST GCONST = new GCONST();

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Fields

        //Main
        public SettingForm m_setting;

        private ProgramSetting m_settings = new ProgramSetting();

        private DropableNotifyIcon m_dropableNotifyIcon = new DropableNotifyIcon();
        private VirtualFolderForm m_virtualFolderForm;
        private MyTaskbarNotifier m_taskbarNotifier;
        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
        private Popup m_trayIconPopup;
        private TrayIconPanel m_trayIconPanel;

        private bool m_process401Exception;
        private bool m_canAutoFetchNewestPosts = true;
        private bool m_manualRefresh;
        private ShowTimelineIndexType m_showTimelineIndexType;

        private List<string> m_delayPostPicList = new List<string>();
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private RunTime m_runTime = new RunTime();

        private string m_stationIP;

        private bool m_firstTimeShowBalloonTipTitle;

        private PostType m_delayPostType;

        private FormSettings m_formSettings;
        private PostForm m_postForm;

        private PhotoDownloader m_photoDownloader;
        private UploadOriginPhotosToStationManager m_uploadOriginPhotosToStationManager;
        private NewPostManager m_newPostManager;
        private StationState m_stationState;
        private Post m_loadingAllPhotosPost;

        #endregion

        #region Properties

        public StationState StationState
        {
            get
            {
                if (m_stationState == null)
                {
                    m_stationState = new StationState();
                }

                return m_stationState;
            }
            set { m_stationState = value; }
        }

        public NewPostManager NewPostManager
        {
            get
            {
                if (m_newPostManager == null)
                {
                    m_newPostManager = NewPostManager.Load() ?? new NewPostManager();
                }

                return m_newPostManager;
            }
            set { m_newPostManager = value; }
        }

        public UploadOriginPhotosToStationManager UploadOriginPhotosToStationManager
        {
            get
            {
                if (m_uploadOriginPhotosToStationManager == null)
                {
                    m_uploadOriginPhotosToStationManager = UploadOriginPhotosToStationManager.Load() ?? new UploadOriginPhotosToStationManager();
                }

                return m_uploadOriginPhotosToStationManager;
            }
            set { m_uploadOriginPhotosToStationManager = value; }
        }

        public PhotoDownloader PhotoDownloader
        {
            get
            {
                return m_photoDownloader ?? (m_photoDownloader = new PhotoDownloader());
            }
            set { m_photoDownloader = value; }
        }

        public RunTime RT
        {
            get { return m_runTime; }
            set { m_runTime = value; }
        }

        public QuitOption QuitOption { get; private set; }

        #endregion

        public Main()
        {
            QuitOption = QuitOption.QuitProgram;

            Current = this;

            File.Delete(m_shellContentMenuFilePath);

            InitializeComponent();

            postsArea.PostsList.DetailView = detailView;

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            //initVirtualFolderForm();

            InitTaskbarNotifier();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = true;
            m_formSettings.UseLocation = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            System.Net.ServicePointManager.DefaultConnectionLimit = 64;

            s_logger.Trace("Constructor: OK");
        }

        #region Init

        private void Form_Load(object sender, EventArgs e)
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

            UpdateNetworkStatus();

            // InitDropableNotifyIcon();

            m_trayIconPopup = new Popup(m_trayIconPanel = new TrayIconPanel());

            //-- Send To
            CreateFileWatcher();

            CreateLoadingImage();

            s_logger.Trace("Form_Load: OK");
        }

        private void CreateLoadingImage()
        {
            try
            {
                Bitmap _img = new Bitmap(128, 128);
                Graphics _g = Graphics.FromImage(_img);
                _g.FillRectangle(new SolidBrush(Color.WhiteSmoke), new Rectangle(0, 0, 128, 128));
                _img.Save(GCONST.CachePath + "LoadingImage" + ".jpg");
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "CreateLoadingImage");
            }
        }

        private void InitDropableNotifyIcon()
        {
            m_dropableNotifyIcon.Text = "Waveface: Drop something here to start post!";
            m_dropableNotifyIcon.NotifyIcon.Icon = Resources.Icon;
            m_dropableNotifyIcon.NotifyIcon.ContextMenuStrip = mnuTray;
            m_dropableNotifyIcon.NotifyIcon.Visible = true;
            //m_dropableNotifyIcon.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            m_dropableNotifyIcon.InitDrop();
            m_dropableNotifyIcon.DragEnter += DropableNotifyIcon_DragEnter;
        }

        private void initVirtualFolderForm()
        {
            m_virtualFolderForm = new VirtualFolderForm();
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
            //m_taskbarNotifier.ContentClick += taskbarNotifier_ContentClick;
            m_taskbarNotifier.EnableSelectionRectangle = true;
            m_taskbarNotifier.KeepVisibleOnMousOver = true;
            m_taskbarNotifier.ReShowOnMouseOver = true;
        }

        private bool LoadRunTime()
        {
            RunTime _rt = RT.LoadJSON();

            if (_rt != null)
            {
                RT = _rt;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SaveRunTime()
        {
            RT.SaveJSON();
        }

        private void SetLastReadPos()
        {
            try
            {
                if (RT.CurrentGroupPosts.Count == 0)
                {
                    s_logger.Trace("SetLastReadPos RT.CurrentGroupPosts.Count = 0");

                    return;
                }

                string _id = RT.CurrentGroupPosts[0].post_id;

                if (_id != string.Empty)
                {
                    string _ret = RT.REST.Footprints_setLastScan(_id);

                    if (_ret == null)
                        s_logger.Trace("SetLastReadPos.Footprints_setLastScan: null");
                    else
                        s_logger.Info("SetLastReadPos.Footprints_setLastScan: " + _ret);
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SetLastReadPos");
            }
        }

        public void Station401ExceptionHandler(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { Station401ExceptionHandler(message); }
                           ));
            }
            else
            {
                MessageBox.Show(I18n.L.T("Station401Exception"), "Waveface", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                m_process401Exception = true;
                QuitOption = QuitOption.Logout;
                m_settings.IsLoggedIn = false;

                Close();
            }
        }

        #endregion

        #region Event

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dropableNotifyIcon.Dispose();

            if (m_virtualFolderForm != null)
                m_virtualFolderForm.Close();

            if (!m_process401Exception)
                SetLastReadPos();

            SaveRunTime();
            NewPostManager.Save();
            m_settings.Save();
        }

        public void Logout()
        {
            QuitOption = QuitOption.Logout;
            m_settings.IsLoggedIn = false;

            try
            {
                if (NewPostManager != null)
                {
                    NewPostManager.AbortThread();
                    NewPostManager = null;
                }

                if (PhotoDownloader != null)
                {
                    PhotoDownloader.AbortThread();
                    PhotoDownloader = null;
                }

                if (UploadOriginPhotosToStationManager != null)
                {
                    UploadOriginPhotosToStationManager.AbortThread();
                    UploadOriginPhotosToStationManager = null;
                }

                if (StationState != null)
                {
                    StationState.AbortThread();
                    statusStrip = null;
                }
            }
            catch
            {
            }

            Close();
        }

        public void Setting()
        {
            if (!Current.CheckNetworkStatus())
                return;

            m_setting = new SettingForm();
            m_setting.ShowDialog();

            m_setting = null;
        }

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        private void Main_Activated(object sender, EventArgs e)
        {
            if (m_postForm != null)
            {
                BringWindowToTop(m_postForm.Handle);
                ShowWindow(m_postForm.Handle, 5); //SW_SHOW
                m_postForm.Activate();
            }

            if (m_setting != null)
            {
                BringWindowToTop(m_setting.Handle);
                ShowWindow(m_setting.Handle, 5); //SW_SHOW
                m_setting.Activate();
            }
        }

        #endregion

        #region Windows Size

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                panelLeftInfo.Width = leftArea.MyWidth + 8;

                if (WindowState == FormWindowState.Minimized)
                {
                    SetLastReadPos();

                    s_logger.Trace("Main_SizeChanged: FormWindowState.Minimized");
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Main_SizeChanged");
            }
        }

        protected override bool ShowWithoutActivation // stops the window from stealing focus
        {
            get { return true; }
        }

        private void splitterRight_SplitterMoving(object sender, SplitterEventArgs e)
        {
            if (e.SplitX < (panelLeftInfo.Width + postsArea.MinimumSize.Width + 8))
                e.SplitX = (panelLeftInfo.Width + postsArea.MinimumSize.Width + 8);
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

        private void DropableNotifyIcon_DragEnter(object sender, DragEventArgs e)
        {
            //if (!m_trayIconPopup.Visible)
            //    m_trayIconPopup.Show(m_dropableNotifyIcon.GetLocation());
        }

        #endregion

        #region Network Status

        public void UpdateNetworkStatus()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                RT.REST.IsNetworkAvailable = true;

                StatusLabelNetwork.Text = I18n.L.T("NetworkConnected");
                StatusLabelNetwork.Image = Resources.network_receive;

                StatusLabelServiceStatus.Visible = true;

                s_logger.Info("UpdateNetworkStatus: Connected");
            }
            else
            {
                RT.REST.IsNetworkAvailable = false;

                StatusLabelNetwork.Text = I18n.L.T("NetworkDisconnected");
                StatusLabelNetwork.Image = Resources.network_error;

                StatusLabelServiceStatus.Visible = false;

                s_logger.Info("UpdateNetworkStatus: Disconnected");
            }
        }

        public bool CheckNetworkStatus()
        {
            if (RT.REST.IsNetworkAvailable)
            {
                return true;
            }
            else
            {
                Invoke(new MethodInvoker(() =>
                {
                    MessageBox.Show(I18n.L.T("NetworkDisconnected"), "Waveface", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                }));

                return false;
            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            try
            {
                if (IsHandleCreated)
                    Invoke(new MethodInvoker(UpdateNetworkStatus));
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "NetworkChange_NetworkAvailabilityChanged");
            }
        }

        #endregion

        #region Login

        public void Reset(bool online)
        {
            if (online)
                RT.Reset();

            m_process401Exception = false;

            WService.StationIP = "";

            postsArea.ShowTypeUI(false);
            postsArea.showRefreshUI(false);

            m_firstTimeShowBalloonTipTitle = true;

            s_logger.Trace("Reset.Online" + online.ToString());
        }

        public bool Login(string email, string password)
        {
            Cursor.Current = Cursors.WaitCursor;

            UpdateNetworkStatus();

            MR_auth_login _login = RT.REST.Auth_Login(email, password);

            if (_login == null)
            {
                s_logger.Trace("Login.Auth_Login: null");
                return false;
            }

            s_logger.Trace("Login.Auth_Login: OK");

            Reset(true);

            RT.Login = _login;

            m_settings.Email = email;
            m_settings.Password = password;
            m_settings.IsLoggedIn = true;

            getGroupAndUser();
            fillUserInformation();

            RT.CurrentGroupID = RT.Login.groups[0].group_id;
            RT.LoadGroupLocalRead();

            StartBgThreads();

            leftArea.SetUI(true);
            leftArea.SetNewPostManager();

            postsArea.showRefreshUI(true);

            Cursor.Current = Cursors.Default;

            GetAllDataAsync(ShowTimelineIndexType.GlobalLastRead, false);

            return true;
        }

        private void StartBgThreads()
        {
            UploadOriginPhotosToStationManager.Start();
            PhotoDownloader.Start();
            NewPostManager.Start();

            StationState.ShowStationState += StationState_ShowStationState;
            StationState.Start();
        }

        void StationState_ShowStationState(ConnectServiceStateType type)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { StationState_ShowStationState(type); }
                           ));
            }
            else
            {
                switch (type)
                {
                    case ConnectServiceStateType.NetworkDisconnected:
                    case ConnectServiceStateType.Cloud:
                        StatusLabelServiceStatus.Image = Resources.Cloud;
                        break;

                    case ConnectServiceStateType.Station_LocalIP:
                    case ConnectServiceStateType.Station_UPnP:
                        StatusLabelServiceStatus.Image = Resources.Station;
                        break;
                }

                s_logger.Trace("ConnectServiceStateType:" + type);
            }
        }

        private void fillUserInformation()
        {
            panelTop.UserName = RT.Login.user.nickname;

            /*
            if (RT.Login.user.avatar_url == string.Empty)
            {
                pictureBoxAvatar.Image = null;
            }
            else
            {
                pictureBoxAvatar.LoadAsync(RT.Login.user.avatar_url);
            }
            */
        }

        private void getGroupAndUser()
        {
            /*
            foreach (Group _g in RT.Login.groups)
            {
                MR_groups_get _mrGroupsGet = RT.REST.Groups_Get(_g.group_id);

                if (_mrGroupsGet != null)
                {
                    if (!RT.GroupGetReturnSets.ContainsKey(_g.group_id)) //Hack
                        RT.GroupGetReturnSets.Add(_g.group_id, _mrGroupsGet);

                    foreach (User _u in _mrGroupsGet.active_members)
                    {
                        if (!RT.AllUsers.Contains(_u))
                            RT.AllUsers.Add(_u);
                    }
                }
            }
            */
        }

        #endregion

        #region Filter (NOT USED NOW)

        public void DoTimelineFilter(FilterItem item, bool isFilterTimelineMode)
        {
            if (!RT.LoginOK)
                return;

            RT.FilterMode = true;

            if (item != null)
            {
                RT.CurrentFilterItem = item;
            }

            RT.FilterPosts = new List<Post>(); //Reset

            RT.FilterTimelineMode = isFilterTimelineMode;
            postsArea.ShowTypeUI(RT.FilterTimelineMode);

            FilterFetchPostsAndShow(true);
        }

        public void FilterReadMorePost()
        {
        }

        private void FilterFetchPostsAndShow(bool firstTime)
        {
            if (RT.FilterPostsAllCount == RT.FilterPosts.Count)
                return;

            int _offset = RT.FilterPosts.Count;

            string _filter = RT.CurrentFilterItem.Filter;
            _filter = _filter.Replace("[type]", postsArea.GetPostType());
            _filter = _filter.Replace("[offset]", _offset.ToString());

            Cursor.Current = Cursors.WaitCursor;

            MR_posts_get _postsGet = RT.REST.Posts_FetchByFilter(_filter);

            if (_postsGet != null)
            {
                RT.FilterPosts.AddRange(_postsGet.posts);

                if (firstTime)
                {
                    RT.FilterPostsAllCount = _postsGet.remaining_count + _postsGet.get_count;

                    RT.IsFilterFirstTimeGetData = true;
                }
                else
                {
                    RT.IsFilterFirstTimeGetData = false;
                }
            }

            Cursor.Current = Cursors.Default;

            ShowPostToUI(false);
        }

        private void ShowPostToUI(bool refreshCurrentPost)
        {
            List<Post> _posts = RT.FilterPosts;

            setCalendarBoldedDates(_posts);

            postsArea.ShowPostInfo(RT.FilterPostsAllCount, _posts.Count);

            postsArea.PostsList.SetFilterPosts(_posts, refreshCurrentPost, RT.IsFilterFirstTimeGetData);
        }

        #endregion

        #region Newest Post (NOT USED NOW)

        /*
        private void timerGetNewestPost_Tick(object sender, EventArgs e)
        {
            timerGetNewestPost.Enabled = false;

            if (m_canAutoFetchNewestPosts)
            {
                //RefreshNewestPosts();
            }

            timerGetNewestPost.Enabled = true;
        }
        */

        public bool RefreshNewestPosts()
        {
            if (RT.LoginOK)
            {
                if (RT.CurrentGroupPosts.Count > 0)
                {
                    string _newestPostTime = RT.CurrentGroupPosts[0].timestamp;
                    string _newestPostID = RT.CurrentGroupPosts[0].post_id;

                    MR_posts_get _postsGet = RT.REST.Posts_get("+200", _newestPostTime, "");

                    if (_postsGet != null)
                    {
                        if (_postsGet.posts.Count > 0)
                        {
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

        #region Helper

        private void GetLastReadAndShow()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { GetLastReadAndShow(); }
                           ));
            }
            else
            {
                LastScan _lastRead = RT.REST.Footprints_getLastScan();

                if ((_lastRead == null) || string.IsNullOrEmpty(_lastRead.post_id))
                {
                    s_logger.Trace("GetLastReadAndShow.getLastScan: null");

                    ShowAllTimeline(m_showTimelineIndexType);
                }
                else
                {
                    s_logger.Info("GetLastReadAndShow.getLastScan:" + _lastRead.post_id);

                    RT.SetCurrentGroupLastRead(_lastRead);

                    if (IsLastReadPostInCacheData(_lastRead.post_id))
                    {
                        ShowAllTimeline(m_showTimelineIndexType);
                    }
                    else
                    {
                        s_logger.Trace("GetLastReadAndShow: Get more posts");

                        timerReloadAllData.Enabled = true;
                    }
                }
            }
        }

        private void timerReloadAllData_Tick(object sender, EventArgs e)
        {
            timerReloadAllData.Enabled = false;

            GetAllDataAsync(m_showTimelineIndexType, false);
        }

        private bool IsLastReadPostInCacheData(string _postID)
        {
            foreach (Post _p in RT.CurrentGroupPosts)
            {
                if (_p.post_id == _postID)
                {
                    return true;
                }
            }

            return false;
        }

        public void GetAllDataAsync(ShowTimelineIndexType showTimelineIndexType, bool manualRefresh)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { GetAllDataAsync(showTimelineIndexType, manualRefresh); }
                           ));
            }
            else
            {
                s_logger.Info("GetAllDataAsync.showTimelineIndexType:" + showTimelineIndexType + ", manualRefresh:" +
                              manualRefresh);

                m_showTimelineIndexType = showTimelineIndexType;
                m_manualRefresh = manualRefresh;

                Cursor.Current = Cursors.WaitCursor;

                postsArea.updateRefreshUI(false);

                bgWorkerGetAllData.RunWorkerAsync();
            }
        }

        private void ShowAllTimeline(ShowTimelineIndexType showTimelineIndexType)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowAllTimeline(showTimelineIndexType); }
                           ));
            }
            else
            {
                if (!m_manualRefresh)
                    PrefetchImages();

                List<Post> _posts = RT.CurrentGroupPosts;

                setCalendarBoldedDates(_posts);

                int _index = RT.GetMyTimelinePosition(showTimelineIndexType);

                s_logger.Info("ShowAllTimeline: showTimelineIndexType=" + showTimelineIndexType + ", TimelineIndex=" +
                              _index);

                lock (postsArea.PostsList)
                {
                    postsArea.PostsList.SetPosts(_posts, _index, m_manualRefresh);
                }
            }
        }

        public void PrefetchImages()
        {
            backgroundWorkerPreloadAllImages.RunWorkerAsync();
        }

        public void PostListClick(int clickIndex, Post post)
        {
            s_logger.Info("SetCurrentGroupLocalLastRead:" + post.post_id + ", TimelineIndex=" + clickIndex);

            RT.SetCurrentGroupLocalLastRead(post);

            RT.IsFilterFirstTimeGetData = false;
        }

        #endregion

        #region Post

        private void timerDelayPost_Tick(object sender, EventArgs e)
        {
            if (m_delayPostPicList.Count == 0)
                return;

            timerDelayPost.Enabled = false;

            DoRealPost(m_delayPostPicList, m_delayPostType);

            m_delayPostPicList.Clear();

            timerDelayPost.Enabled = true;
        }

        public void Post()
        {
            DoRealPost(new List<string>(), PostType.All);
        }

        public void Post(List<string> pics, PostType postType)
        {
            m_delayPostType = postType;
            m_delayPostPicList = pics;
        }

        private void DoRealPost(List<string> pics, PostType postType)
        {
            if (!RT.LoginOK)
            {
                MessageBox.Show("Please Login first.", "Waveface");
                return;
            }

            m_canAutoFetchNewestPosts = false;

            try
            {
                m_postForm = new PostForm(pics, postType);
                DialogResult _dr = m_postForm.ShowDialog();

                switch (_dr)
                {
                    case DialogResult.Yes:
                        break;

                    case DialogResult.OK:
                        NewPostManager.Add(m_postForm.NewPostItem);
                        break;
                }
            }
            catch (Exception _e)
            {
                MessageBox.Show(I18n.L.T("PostError") + " : " + _e.Message, "Waveface");

                NLogUtility.Exception(s_logger, _e, "Post");
            }

            m_postForm = null;

            m_canAutoFetchNewestPosts = true;
        }

        #endregion

        #region AfterPostComment

        public void AfterPostComment(string post_id)
        {
            MR_posts_getSingle _singlePost = RT.REST.Posts_GetSingle(post_id);

            if ((_singlePost != null) && (_singlePost.post != null))
            {
                ReplacePostInList(_singlePost.post, RT.CurrentGroupPosts);
                //ReplacePostInList(_singlePost.post, RT.FilterPosts);

                s_logger.Trace("AfterPostComment.ShowAllTimeline(true)");

                ShowAllTimeline(ShowTimelineIndexType.LocalLastRead);
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

            if (k != -1)
            {
                posts[k] = post;

                return true;
            }

            return false;
        }

        #endregion

        #region Calendar

        private void setCalendarBoldedDates(List<Post> posts)
        {
            MonthCalendar _mc = leftArea.MonthCalendar;

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
            MonthCalendar _calendar = leftArea.MonthCalendar;

            if (!_calendar.BoldedDates.Contains(date.Date))
                return;

            postsArea.PostsList.ScrollToDay(date.Date);
        }

        public void setCalendarDay(DateTime date)
        {
            MonthCalendar _calendar = leftArea.MonthCalendar;
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

                        m_delayPostType = PostType.Photo;
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
            try
            {
                CaptureForm _captureForm = new CaptureForm(shotType);

                if ((_captureForm.ShowDialog() != DialogResult.OK) || (_captureForm.Image == null))
                {
                    return;
                }

                string _filename =
                    string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmssff"), ImageFormat.Jpeg).ToLower();

                Image _img = _captureForm.Image;

                string _pathToSave = Path.Combine(GCONST.CachePath, _filename);

                _img.Save(_pathToSave, ImageFormat.Jpeg);

                Post(new List<string> { GCONST.CachePath + _filename }, PostType.Photo);
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        #region Misc

        public void HidePost(string postId)
        {
            MR_posts_hide_ret _ret = RT.REST.Posts_hide(postId);

            if (_ret != null)
            {
                MessageBox.Show("Remove Post Success!");

                ShowPostToUI(false);
            }
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

        private void radioButtonStation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCloud.Checked)
            {
                WService.StationIP = WService.CloudIP;
                RT.StationMode = false;
            }
            else
            {
                WService.StationIP = m_stationIP;
                RT.StationMode = true;

                backgroundWorkerPreloadAllImages_DoWork(null, null);
            }
        }

        public void ShowStatuMessage(string message, bool timeout)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowStatuMessage(message, timeout); }
                           ));
            }
            else
            {
                if (timeout)
                {
                    timerShowStatuMessage.Enabled = true;

                    StatusLabelPost.Text = message;
                    postsArea.ShowStatusText(message);
                }
                else
                {
                    StatusLabelUpload.Text = message;
                }
            }
        }

        private void timerShowStatuMessage_Tick(object sender, EventArgs e)
        {
            timerShowStatuMessage.Enabled = false;

            StatusLabelPost.Text = "";
            postsArea.ShowStatusText("");
        }

        #endregion

        #region GetAllData

        private void bgWorkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
        {
            string _firstGetCount = "100"; //200
            string _continueGetCount = "-100"; //200
            Dictionary<string, Post> _allPosts = new Dictionary<string, Post>();
            string _datum = string.Empty;

            MR_posts_getLatest _getLatest = RT.REST.Posts_getLatest(_firstGetCount);

            if (_getLatest != null)
            {
                foreach (Post _p in _getLatest.posts)
                {
                    _allPosts.Add(_p.post_id, _p);
                    _datum = _p.timestamp;
                }

                if (_getLatest.get_count < _getLatest.total_count)
                {
                    int _remainingCount = int.MaxValue;

                    while (_remainingCount > 0)
                    {
                        _datum =
                            DateTimeHelp.ToUniversalTime_ToISO8601(DateTimeHelp.ISO8601ToDateTime(_datum).AddSeconds(1));

                        MR_posts_get _postsGet = RT.REST.Posts_get(_continueGetCount, _datum, "");

                        if (_postsGet != null)
                        {
                            foreach (Post _p in _postsGet.posts)
                            {
                                if (!_allPosts.ContainsKey(_p.post_id))
                                {
                                    _allPosts.Add(_p.post_id, _p);
                                    _datum = _p.timestamp;
                                }
                            }

                            _remainingCount = _postsGet.remaining_count;
                        }
                    }
                }
            }

            List<Post> _tmpPosts = new List<Post>();

            foreach (Post _p in _allPosts.Values)
            {
                _tmpPosts.Add(_p);
            }

            if (m_manualRefresh)
            {
                RT.SetAllCurrentGroupPostHaveRead();
            }

            RT.CurrentGroupPosts = _tmpPosts;

            if (m_manualRefresh)
            {
                SetLastReadPos();
            }

            s_logger.Info("bgWorkerGetAllData_DoWork. Get Post Count:" + _tmpPosts.Count);
        }

        private void bgWorkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor.Current = Cursors.Default;

            postsArea.updateRefreshUI(true);

            GetLastReadAndShow();
        }

        #endregion

        #region PrefetchImages

        public void PrefetchImages(List<Post> posts, bool allSize)
        {
            foreach (Post post in posts)
            {
                switch (post.type)
                {
                    case "image":
                        {
                            if (post.attachments.Count == 0)
                                break;

                            Attachment _a = post.attachments[0];

                            string _url = string.Empty;
                            string _fileName = string.Empty;
                            Current.RT.REST.attachments_getRedirectURL_Image(_a, "small", out _url, out _fileName, false);

                            string _localPic = GCONST.CachePath + _fileName;

                            PreloadThumbnail(_url, _localPic);

                            PhotoDownloader.PreloadPictures(post, allSize /*, false*/);

                            break;
                        }
                    case "link":
                        {
                            string _url = post.preview.thumbnail_url;

                            string _localPic = GCONST.CachePath + post.post_id + "_previewthumbnail_" + ".jpg";

                            PreloadThumbnail(_url, _localPic);

                            break;
                        }

                    case "rtf":
                        {
                            Attachment _a = null;

                            foreach (Attachment _attachment in post.attachments)
                            {
                                if (_attachment.type == "image")
                                {
                                    _a = _attachment;
                                    break;
                                }
                            }

                            if (_a != null)
                            {
                                string _url = string.Empty;
                                string _fileName = string.Empty;
                                Current.RT.REST.attachments_getRedirectURL_Image(_a, "small", out _url,
                                                                                 out _fileName, false);

                                string _localPic = GCONST.CachePath + _fileName;

                                PreloadThumbnail(_url, _localPic);
                            }

                            break;
                        }

                    case "doc":
                        {
                            Attachment _a = post.attachments[0];

                            if (_a.image != string.Empty)
                            {
                                string _localPic = GCONST.CachePath + _a.object_id + "_thumbnail" + ".jpg";

                                string _url = _a.image;

                                _url = Current.RT.REST.attachments_getRedirectURL_PdfCoverPage(_url);

                                PreloadThumbnail(_url, _localPic);
                            }

                            break;
                        }
                }
            }
        }

        private void PreloadThumbnail(string url, string localPicPath)
        {
            if (!File.Exists(localPicPath))
            {
                ImageItem _item = new ImageItem();
                _item.PostItemType = PostItemType.Thumbnail;
                _item.ThumbnailPath = url;
                _item.LocalFilePath_Origin = localPicPath;

                PhotoDownloader.Add(_item, false);
            }
        }

        private void backgroundWorkerPreloadAllImages_DoWork(object sender, DoWorkEventArgs e)
        {
            if (RT.CurrentGroupPosts != null)
            {
                PrefetchImages(RT.CurrentGroupPosts, false);
            }
        }

        #endregion
    }
}
