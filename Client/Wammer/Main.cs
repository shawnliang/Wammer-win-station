#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component.PopupControl;
using Waveface.Component.DropableNotifyIcon;
using Waveface.Configuration;
using Waveface.FilterUI;
using Waveface.ImageCapture;
using Waveface.Properties;
using Waveface.SettingUI;
using MonthCalendar = CustomControls.MonthCalendar;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

#endregion

namespace Waveface
{
    public partial class Main : Form
    {
        public static Main Current;
        public static GCONST GCONST;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Fields

        //Main
        public SettingForm m_setting;

        private DropableNotifyIcon m_dropableNotifyIcon = new DropableNotifyIcon();
        private VirtualFolderForm m_virtualFolderForm;
        private MyTaskbarNotifier m_taskbarNotifier;
        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
        private Popup m_trayIconPopup;
        private TrayIconPanel m_trayIconPanel;

        private bool m_forceLogout;
        private bool m_manualRefresh;
        private ShowTimelineIndexType m_showTimelineIndexType;

        private List<string> m_delayPostPicList = new List<string>();
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private RunTime m_runTime = new RunTime();

        private string m_stationIP;

        private PostType m_delayPostType;

        private FormSettings m_formSettings;
        private PostForm m_postForm;

        private PhotoDownloader m_photoDownloader;
        private UploadOriginPhotosToStationManager m_uploadOriginPhotosToStationManager;
        private BatchPostManager m_batchPostManager;
        private StationState m_stationState;
        private AppLimit.NetSparkle.Sparkle m_autoUpdator;
        private bool m_getAllDataError;
        private string m_newestUpdateTime;
        private string m_initSessionToken;
        // private BorderlessFormTheme m_borderlessFormTheme = new BorderlessFormTheme();

        #endregion

        #region Properties

        /*
        public BorderlessFormTheme BorderlessFormTheme
        {
            get { return m_borderlessFormTheme; }
        }
        */

        public string LoadingImagePath
        {
            get
            {
                return Path.Combine(GCONST.RunTimeDataPath, "LoadingImage.jpg");
            }
        }

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

        public BatchPostManager BatchPostManager
        {
            get
            {
                if (m_batchPostManager == null)
                {
                    m_batchPostManager = BatchPostManager.Load() ?? new BatchPostManager();
                }

                return m_batchPostManager;
            }
            set { m_batchPostManager = value; }
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

        public DialogResult NewPostThreadErrorDialogResult
        {
            get;
            set;
        }

        public QuitOption QuitOption { get; private set; }

        #endregion

        public Main()
        {
            Init();
        }

        public Main(string initSessionToken)
        {
            Init();
            m_initSessionToken = initSessionToken;
        }

        private void Init()
        {
            QuitOption = QuitOption.QuitProgram;

            Current = this;

            File.Delete(m_shellContentMenuFilePath);

            InitializeComponent();

            Text = "Waveface Stream";

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            //initVirtualFolderForm();

            InitTaskbarNotifier();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = true;
            m_formSettings.UseLocation = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            m_autoUpdator = new AppLimit.NetSparkle.Sparkle(WService.WebURL + "/extensions/windowsUpdate/versioninfo.xml");
            m_autoUpdator.StartLoop(true, TimeSpan.FromHours(5.0));

            bgWorkerGetAllData.WorkerSupportsCancellation = true;

            s_logger.Trace("Constructor: OK");
        }

        #region Init

        private void Form_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(m_initSessionToken))
                LoginWithInitSession();

            postsArea.PostsList.DetailView = detailView;

            if (Environment.GetCommandLineArgs().Length == 1)
            {
                NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

                UpdateNetworkStatus();
            }

            // InitDropableNotifyIcon();

            m_trayIconPopup = new Popup(m_trayIconPanel = new TrayIconPanel());

            //-- Send To
            CreateFileWatcher();

            CreateLoadingImage();

            //@ BorderlessFormTheme.ApplyFormThemeSizable(this, false);

            s_logger.Trace("Form_Load: OK");
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case (Keys.Control | Keys.N):
                    if (RT.Login != null)
                    {
                        Post();
                    }

                    return true;
            }

            return false;
        }

        private void CreateLoadingImage()
        {
            int s = 256;
            int k = 8;

            try
            {
                Bitmap _img = new Bitmap(s, s);
                Graphics _g = Graphics.FromImage(_img);

                _g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, s, s));

                using (Pen _p = new Pen(Color.FromArgb(89, 154, 174)))
                {
                    _p.DashStyle = DashStyle.Dash;

                    _g.DrawRectangle(_p, new Rectangle(k, k, s - (2 * k), s - (2 * k)));
                }

                _img.Save(LoadingImagePath);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "CreateLoadingImage");
            }
        }

        private void InitDropableNotifyIcon()
        {
            m_dropableNotifyIcon.Text = "Waveface: Drop something here to start post!";
            m_dropableNotifyIcon.NotifyIcon.Icon = this.Icon;
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

            return false;
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
                MessageBox.Show(I18n.L.T("Station401Exception"), "Waveface Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                m_forceLogout = true;
                QuitOption = QuitOption.Logout;

                Close();
            }
        }

        public void ForceLogout()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ForceLogout(); }
                           ));
            }
            else
            {
                MessageBox.Show(I18n.L.T("ForceLogout"), "Waveface Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                m_forceLogout = true;
                QuitOption = QuitOption.Logout;

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

            if (!m_forceLogout)
                SetLastReadPos();

            SaveRunTime();
            BatchPostManager.Save();
        }

        public void Logout()
        {
            Program.ShowCrashReporter = false;

            QuitOption = QuitOption.Logout;

            timerPolling.Enabled = false;

            bgWorkerGetAllData.CancelAsync();

            try
            {
                if (BatchPostManager != null)
                {
                    BatchPostManager.AbortThread();
                    BatchPostManager = null;
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

        public void AccountInformation()
        {
            m_setting = new SettingForm(m_autoUpdator);
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
                panelLeftInfo.Width = leftArea.MyWidth;

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

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                SingleInstance.WinApi.ShowToFront(Handle);
            }

            base.WndProc(ref message);
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
            RT.REST.IsNetworkAvailable = true;

            StatusLabelNetwork.Text = I18n.L.T("NetworkConnected");
            StatusLabelNetwork.Image = Resources.network_receive;

            StatusLabelServiceStatus.Visible = true;
        }

        public bool CheckNetworkStatus()
        {
            return true;
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

            m_forceLogout = false;

            WService.StationIP = "";

            panelTitle.showRefreshUI(false);

            s_logger.Trace("Reset.Online" + online.ToString());
        }


        private bool procLoginResponse(MR_auth_login _login)
            {
            if (_login == null)
            {
                s_logger.Trace("Login.Auth_Login: null");
                return false;
            }

            s_logger.Trace("Login.Auth_Login: OK");

            RT.Login = _login;
            GCONST = new GCONST(RT);

            getGroupAndUser();
            fillUserInformation();

            RT.CurrentGroupID = RT.Login.groups[0].group_id;
            RT.LoadGroupLocalRead();

            if (Environment.GetCommandLineArgs().Length == 1)
                StartBgThreads();
            else
            {
                UploadOriginPhotosToStationManager.Start();
                PhotoDownloader.Start();
                BatchPostManager.Start();
            }

            leftArea.SetNewPostManager();

            panelTitle.showRefreshUI(true);

            Cursor = Cursors.Default;

            GetAllDataAsync(ShowTimelineIndexType.GlobalLastRead, false);

            return true;
        }

        private void LoginWithInitSession()
        {
            UpdateNetworkStatus();

            Reset(true);

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                m_stationIP = "http://127.0.0.1:9981";
                WService.StationIP = m_stationIP;
                StationState_ShowStationState(ConnectServiceStateType.Station_LocalIP);
                // radioButtonStation.Checked = true;
                RT.StationMode = true;
            }

			try
			{
				MongoDB.Driver.MongoServer dbServer = MongoDB.Driver.MongoServer.Create("mongodb://localhost:10319/?safe=true");
				BsonDocument doc = dbServer.GetDatabase("wammer").GetCollection("LoginedSession").FindOne(Query.EQ("_id", m_initSessionToken));
				string json = doc.ToJson();

				MR_auth_login _login = JsonConvert.DeserializeObject<MR_auth_login>(json);
				_login.session_token = m_initSessionToken;

				procLoginResponse(_login);
			}
			catch (Exception e)
			{
				MessageBox.Show(I18n.L.T("ForceLogout") + "\r\n" + e.ToString());
				Close();
			}
        }

        public bool Login(string email, string password, out string errorMessage)
        {
            Cursor = Cursors.WaitCursor;

            errorMessage = string.Empty;

            UpdateNetworkStatus();

            Reset(true);

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                m_stationIP = "http://127.0.0.1:9981";
                WService.StationIP = m_stationIP;
                StationState_ShowStationState(ConnectServiceStateType.Station_LocalIP);
                // radioButtonStation.Checked = true;
                RT.StationMode = true;
            }

            MR_auth_login _login = RT.REST.Auth_Login(email, password);
            return procLoginResponse(_login);
        }

        private void StartBgThreads()
        {
            UploadOriginPhotosToStationManager.Start();
            PhotoDownloader.Start();
            BatchPostManager.Start();

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
                        StatusLabelServiceStatus.Text = "Cloud";
                        break;

                    case ConnectServiceStateType.Station_LocalIP:
                    case ConnectServiceStateType.Station_UPnP:
                        StatusLabelServiceStatus.Image = Resources.Station;
                        StatusLabelServiceStatus.Text = "Station";
                        break;
                }

                s_logger.Trace("ConnectServiceStateType:" + type);
            }
        }

        private void fillUserInformation()
        {
        }

        private void getGroupAndUser()
        {
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

            FilterFetchPostsAndShow(true);
        }

        public void FilterReadMorePost()
        {
            FilterFetchPostsAndShow(false);
        }

        private void FilterFetchPostsAndShow(bool firstTime)
        {
            if ((RT.FilterPostsAllCount != 0) && (RT.FilterPostsAllCount == RT.FilterPosts.Count))
                return;

            int _offset = RT.FilterPosts.Count;

            string _filter = RT.CurrentFilterItem.Filter;

            if (RT.CurrentFilterItem.DynamicNow)
                _filter = FilterHelper.GetAllPostFilterStringByPostType(_filter);

            _filter = _filter.Replace("[offset]", _offset.ToString());

            Cursor = Cursors.WaitCursor;

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

            Cursor = Cursors.Default;

            ShowPostToUI(false);
        }

        private void ShowPostToUI(bool refreshCurrentPost)
        {
            List<Post> _posts = RT.FilterPosts;

            setCalendarBoldedDates(_posts);

            postsArea.ShowPostInfor(RT.FilterPostsAllCount, _posts.Count);

            postsArea.PostsList.SetFilterPosts(_posts, refreshCurrentPost, RT.IsFilterFirstTimeGetData);
        }

        #endregion

        #region Newest Post (NOT USED NOW)

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

                    ShowAllTimeline(m_showTimelineIndexType, -1);
                }
                else
                {
                    s_logger.Info("GetLastReadAndShow.getLastScan:" + _lastRead.post_id);

                    RT.SetCurrentGroupLastRead(_lastRead);

                    //@
                    //if (IsLastReadPostInCacheData(_lastRead.post_id))
                    {
                        ShowAllTimeline(m_showTimelineIndexType, -1);
                    }
                    /*else
                    {
                        s_logger.Trace("GetLastReadAndShow: Get more posts");

                        timerReloadAllData.Enabled = true;
                    }*/
                }

                timerPolling.Enabled = true;
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

                Cursor = Cursors.WaitCursor;

                panelTitle.updateRefreshUI(false);

                if (bgWorkerGetAllData.IsBusy)
                    Cursor = Cursors.Default;
                else
                    bgWorkerGetAllData.RunWorkerAsync();
            }
        }

        private void ShowAllTimeline(ShowTimelineIndexType showTimelineIndexType, int index)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowAllTimeline(showTimelineIndexType, index); }
                           ));
            }
            else
            {
                if (!m_manualRefresh)
                    PrefetchImages();

                List<Post> _posts = RT.CurrentGroupPosts;

                setCalendarBoldedDates(_posts);

                postsArea.ShowPostInforPanel(false);
                leftArea.SetUI(true);

                int _index;

                if (index < 0)
                {
                    _index = RT.GetMyTimelinePosition(showTimelineIndexType);
                }
                else
                {
                    if (index < RT.CurrentGroupPosts.Count)
                        _index = index;
                    else
                        _index = RT.CurrentGroupPosts.Count - 1;

                    RT.SetCurrentGroupLocalLastRead(RT.CurrentGroupPosts[_index]);
                }

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

            DoRealPostForm(m_delayPostPicList, m_delayPostType);

            m_delayPostPicList.Clear();

            timerDelayPost.Enabled = true;
        }

        public void Post()
        {
            DoRealPostForm(new List<string>(), PostType.All);
        }

        public void EditPost(Post post)
        {
            if (!RT.LoginOK)
            {
                MessageBox.Show("Please Login first.", "Waveface Stream"); //@! i18n
                return;
            }

            try
            {
                m_postForm = new PostForm(new List<string>(), PostType.All, post, true);
                DialogResult _dr = m_postForm.ShowDialog();

                switch (_dr)
                {
                    case DialogResult.Yes:
                        break;

                    case DialogResult.OK:
                        BatchPostManager.Add(m_postForm.BatchPostItem);

                        if (m_postForm.BatchPostItem.Post != null)
                        {
                            ShowAllTimeline(ShowTimelineIndexType.LocalLastRead, -1);
                        }

                        break;
                }
            }
            catch (Exception _e)
            {
                //MessageBox.Show(I18n.L.T("PostError") + " : " + _e.Message, "Waveface Stream");

                NLogUtility.Exception(s_logger, _e, "Edit Post");
            }

            m_postForm = null;
        }

        public void Post(List<string> pics, PostType postType)
        {
            m_delayPostType = postType;
            m_delayPostPicList = pics;
        }

        private void DoRealPostForm(List<string> pics, PostType postType)
        {
            if (!RT.LoginOK)
            {
                MessageBox.Show("Please Login first.", "Waveface Stream"); //@! i18n
                return;
            }

            try
            {
                m_postForm = new PostForm(pics, postType, null, false);
                DialogResult _dr = m_postForm.ShowDialog();

                switch (_dr)
                {
                    case DialogResult.Yes:
                        break;

                    case DialogResult.OK:
                        BatchPostManager.Add(m_postForm.BatchPostItem);
                        break;
                }
            }
            catch (Exception _e)
            {
                MessageBox.Show(I18n.L.T("PostError") + " : " + _e.Message, "Waveface Stream");

                NLogUtility.Exception(s_logger, _e, "Post");
            }

            m_postForm = null;
        }

        #endregion

        #region PostUpdate

        public string GetPostUpdateTime(Post post)
        {
            string _time = post.timestamp;

            try
            {
                MR_posts_getSingle _singlePost = RT.REST.Posts_GetSingle(post.post_id);

                if ((_singlePost != null) && (_singlePost.post != null))
                {
                    if (_singlePost.post.update_time != null)
                    {
                        _time = _singlePost.post.update_time;
                    }
                }
            }
            catch
            {
            }

            return _time;
        }

        public Post PostUpdate(Post post, Dictionary<string, string> optionalParams, bool refreshUI)
        {

            MR_posts_update _update = null;

            try
            {
                string _time = GetPostUpdateTime(post);

                _update = RT.REST.Posts_update(post.post_id, _time, optionalParams);

                if (_update == null)
                {
                    return null;
                }

                RefreshSinglePost(_update.post, refreshUI);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "PostUpdate");

                MessageBox.Show(I18n.L.T("ErrorAndTry"), "Waveface Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ReloadAllData();

                return null;
            }

            return _update.post;
        }

        public bool ChangePostFavorite(Post post, bool refreshUI)
        {
            try
            {
                if (post.favorite == null)
                    return false;

                int _value = int.Parse(post.favorite);

                _value = (_value == 0) ? 1 : 0;

                string _time = GetPostUpdateTime(post);

                Dictionary<string, string> _params = new Dictionary<string, string>();
                _params.Add("favorite", _value.ToString());

                MR_posts_update _update = RT.REST.Posts_update(post.post_id, _time, _params);

                if (_update == null)
                {
                    return false;
                }

                RefreshSinglePost(_update.post, refreshUI);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "PostUpdate");

                MessageBox.Show(I18n.L.T("ErrorAndTry"), "Waveface Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ReloadAllData();

                return false;
            }

            return true;
        }

        public bool HidePost(string postId)
        {

            Cursor = Cursors.WaitCursor;

            MR_posts_hide_ret _ret = RT.REST.Posts_hide(postId);

            if (_ret != null)
            {
                RemovePostLocalAndRefresh(postId, true);

                MessageBox.Show(I18n.L.T("PostRemoved"), "Waveface Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Cursor = Cursors.Default;
                return true;
            }

            Cursor = Cursors.Default;
            return false;
        }

        private void RemovePostLocalAndRefresh(string postId, bool changeUI)
        {
            int _index = 0;

            foreach (Post _p in RT.CurrentGroupPosts)
            {
                if (_p.post_id == postId)
                {
                    RT.CurrentGroupPosts.Remove(_p);

                    if (changeUI)
                    {
                        ShowAllTimeline(ShowTimelineIndexType.Manual, _index);
                    }
                    else
                    {
                        ShowAllTimeline(ShowTimelineIndexType.LocalLastRead, -1);
                    }

                    break;
                }

                _index++;
            }
        }

        public void RefreshSinglePost_ByID(string post_id)
        {
            MR_posts_getSingle _singlePost = RT.REST.Posts_GetSingle(post_id);

            if ((_singlePost != null) && (_singlePost.post != null))
            {
                ReplacePostInList(_singlePost.post, RT.CurrentGroupPosts);
                //ReplacePostInList(_singlePost.post, RT.FilterPosts);

                ShowAllTimeline(ShowTimelineIndexType.LocalLastRead, -1);
            }
        }

        public void RefreshSinglePost(Post post, bool refreshUI)
        {
            ReplacePostInList(post, RT.CurrentGroupPosts);

            if (refreshUI)
                ShowAllTimeline(ShowTimelineIndexType.LocalLastRead, -1);
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

        private string GetNewestUpdateTimeInPosts(List<Post> posts)
        {
            DateTime _dtNewest = new DateTime(1, 1, 1);
            DateTime _dt;

            string _time = "";

            try
            {
                for (int i = 0; i < posts.Count; i++)
                {
                    _dt = DateTimeHelp.ISO8601ToDateTime(posts[i].update_time);

                    if (_dt > _dtNewest)
                    {
                        _dtNewest = _dt;
                        _time = posts[i].update_time;
                    }
                }
            }
            catch
            {
                return "";
            }

            return _time;
        }

        public bool checkNewPosts()
        {
            if (RT.CurrentGroupPosts.Count == 0)
                return false;

            try
            {
                string _datum = RT.CurrentGroupPosts[0].timestamp;
                _datum = DateTimeHelp.ToUniversalTime_ToISO8601(DateTimeHelp.ISO8601ToDateTime(_datum).AddSeconds(1));

                MR_posts_get _postsGet = RT.REST.Posts_get("100", _datum, "");

                if (_postsGet != null)
                {
                    return (_postsGet.posts.Count > 0);
                }
            }
            catch
            {
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
            if (date == DateTime.Now.Date)
            {
                setCalendarDay(date);
                postsArea.PostsList.ScrollTo(0);
                return;
            }

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
            _calendar.ViewStart = date;
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

                string _pathToSave = Path.Combine(GCONST.TempPath, _filename);

                _img.Save(_pathToSave, ImageFormat.Jpeg);

                Post(new List<string> { _pathToSave }, PostType.Photo);
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

        public void RemovePost()
        {
            postsArea.RemovePost();
        }

        public void SetClock(bool visible, DateTime dateTime)
        {
            detailView.SetClock(visible, dateTime);
        }

        public int GetLeftAreaWidth()
        {
            return leftArea.Width;
        }

        private void timerPolling_Tick(object sender, EventArgs e)
        {
            timerPolling.Enabled = false;

            try
            {
                if (checkNewPosts())
                {
                    ReloadAllData();

                    return;
                }

                string _newestUpdateTime;

                if (string.IsNullOrEmpty(m_newestUpdateTime))
                {
                    _newestUpdateTime = GetNewestUpdateTimeInPosts(RT.CurrentGroupPosts);
                }
                else
                {
                    _newestUpdateTime = m_newestUpdateTime;
                }

                _newestUpdateTime = DateTimeHelp.ToUniversalTime_ToISO8601(DateTimeHelp.ISO8601ToDateTime(_newestUpdateTime).AddSeconds(1));

                MR_usertracks_get _usertracks = RT.REST.usertracks_get(_newestUpdateTime);

                if (_usertracks != null)
                {
                    if (_usertracks.get_count == 0)
                    {
                        timerPolling.Enabled = true;

                        return;
                    }

                    m_newestUpdateTime = _usertracks.latest_timestamp;

                    foreach (UT_UsertrackList _usertrack in _usertracks.usertrack_list)
                    {
                        foreach (UT_Action _action in _usertrack.actions)
                        {
                            if (_action.action == "unhide")
                            {
                                ReloadAllData();

                                return;
                            }

                            if ((_action.action == "hide") && (_usertrack.target_type == "post"))
                            {
                                RemovePostLocalAndRefresh(_usertrack.target_id, false);
                            }
                        }
                    }

                    string _json = JsonConvert.SerializeObject(_usertracks.post_id_list);

                    MR_posts_get _postsGet = RT.REST.Posts_FetchByFilter_2(_json);

                    if (_postsGet != null)
                    {
                        bool _changed = false;

                        foreach (Post _p in _postsGet.posts)
                        {
                            _changed = ReplacePostInList(_p, RT.CurrentGroupPosts);
                        }

                        if (_changed)
                            ShowAllTimeline(ShowTimelineIndexType.LocalLastRead, -1);
                    }
                }

            }
            catch (Exception ex)
            {
                s_logger.WarnException("user track failed", ex);
            }
            finally
            {
                timerPolling.Enabled = true;
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

        /*
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
        */

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
                    panelTitle.ShowStatusText(message);
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
            panelTitle.ShowStatusText("");
        }

        public void ShowFileMissDialog(string text)
        {
            NewPostThreadErrorDialogResult = DialogResult.None;

            MsgBox _msgBox = new MsgBox(string.Format(I18n.L.T("BatchPostManager.FileMiss"), text), "Waveface Stream", MessageBoxIcon.Warning);
            _msgBox.SetButtons(new[] { I18n.L.T("Continue"), I18n.L.T("Retry"), I18n.L.T("Cancel") }, new[] { DialogResult.Yes, DialogResult.Retry, DialogResult.Cancel }, 3);
            DialogResult _dr = _msgBox.ShowDialog();

            NewPostThreadErrorDialogResult = _dr;
        }

        public void OverQuotaMissDialog(string text)
        {
            NewPostThreadErrorDialogResult = DialogResult.None;

            MsgBox _msgBox = new MsgBox(string.Format(I18n.L.T("BatchPostManager.OverQuota"), text), "Waveface Stream", MessageBoxIcon.Warning);
            _msgBox.SetButtons(new[] { I18n.L.T("Retry"), I18n.L.T("Cancel") }, new[] { DialogResult.Retry, DialogResult.Cancel }, 2);
            DialogResult _dr = _msgBox.ShowDialog();

            NewPostThreadErrorDialogResult = _dr;
        }

        #endregion

        #region GetAllData

        public void ReloadAllData()
        {
            GetAllDataAsync(ShowTimelineIndexType.LocalLastRead, true);
        }

        private void bgWorkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
        {
            string _firstGetCount = "100"; //200
            string _continueGetCount = "-100"; //200
            Dictionary<string, Post> _allPosts = new Dictionary<string, Post>();
            string _datum = string.Empty;

            MR_posts_getLatest _getLatest = null;

            try
            {
                _getLatest = RT.REST.Posts_getLatest(_firstGetCount);
            }
            catch
            {
                //Hack: Cloud �R�X���~����

                ForceLogout();
                m_getAllDataError = true;
                return;
            }

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
                        if (bgWorkerGetAllData.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
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
            if (m_getAllDataError)
            {
                m_getAllDataError = false;
            }
            else
            {
                Cursor = Cursors.Default;

                panelTitle.updateRefreshUI(true);

                GetLastReadAndShow();
            }
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

                            string _localPic = Path.Combine(GCONST.ImageCachePath, _fileName);

                            PreloadThumbnail(_url, _localPic);

                            PhotoDownloader.PreloadPictures(post, allSize);

                            break;
                        }
                    case "link":
                        {
                            if (post.preview.thumbnail_url != null)
                            {
                                string _url = post.preview.thumbnail_url;

                                string _localPic = Path.Combine(GCONST.RunTimeDataPath, post.post_id + "_previewthumbnail_" + ".jpg");

                                PreloadThumbnail(_url, _localPic);
                            }

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

                                string _localPic = Path.Combine(GCONST.ImageCachePath, _fileName);

                                PreloadThumbnail(_url, _localPic);
                            }

                            break;
                        }

                    case "doc":
                        {
                            Attachment _a = post.attachments[0];

                            if (_a.image != string.Empty)
                            {
                                string _localPic = Path.Combine(GCONST.ImageCachePath, _a.object_id + "_thumbnail" + ".jpg");

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
                PrefetchImages(RT.CurrentGroupPosts, false); //@
            }
        }

        #endregion

        #region BorderlessForm

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, long lParam, long wParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (!BorderlessFormTheme.HostWindow.WinMaxed)
            {
                if (MouseButtons.ToString() == "Left")
                {
                    ReleaseCapture();

                    uint WM_NCLBUTTONDOWN = 161;
                    int HT_CAPTION = 0x2;

                    SendMessage(Parent.Parent.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            */
        }

        #endregion
    }
}
