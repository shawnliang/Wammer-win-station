﻿#region

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
using System.Linq;
using AppLimit.NetSparkle;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component.DropableNotifyIcon;
using Waveface.Configuration;
using Waveface.ImageCapture;
using Waveface.Properties;
using MonthCalendar = CustomControls.MonthCalendar;
using System.Diagnostics;
using Waveface.Common;

#endregion

namespace Waveface
{
    public partial class Main : Form
    {
        public static Main Current;
        public static GCONST GCONST;

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Fields

        //// Main
        private DropableNotifyIcon m_dropableNotifyIcon = new DropableNotifyIcon();
        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;

        private List<string> m_delayPostPicList = new List<string>();
        private RunTime m_runTime;
        private PostType m_delayPostType;
        private FormSettings m_formSettings;
        private PostForm _postForm;
        private PhotoDownloader m_photoDownloader;
        private UploadOriginPhotosToStationManager m_uploadOriginPhotosToStationManager;
        private Waveface.Upload.AttachmentUploadFacade m_uploader;
        private StationState m_stationState;
        private AutoUpdate m_autoUpdator;

        private bool m_getAllDataError;
        private int m_InForceAutoUpdate;
        private string m_stationIP;
        private int m_next_seq_num;
        private string m_initSessionToken;
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private bool m_isPrimaryStation;
        private string m_displayType;
        private string m_delayPostText;

        private CustomWindow _messageReceiver;
        private WService _service;

        #endregion

        #region Private Property
        private WService m_Service
        {
            get
            {
                return _service ?? (_service = new WService());
            }
        }

        private CustomWindow m_MessageReceiver
        {
            get
            {
                return _messageReceiver ?? (_messageReceiver = new CustomWindow("WindowsClientMessageReceiver", "WindowsClientMessageReceiver"));
            }
        }

        private PostForm m_PostForm
        {
            get
            {
                return _postForm;
            }
            set
            {
                if (_postForm == value)
                    return;

                if (_postForm != null)
                {
                    if (!_postForm.IsDisposed)
                        _postForm.Dispose();
                    _postForm = null;
                }

                _postForm = value;
            }
        }
        #endregion


        #region Public Property
        public bool IsPrimaryStation
        {
            get { return m_isPrimaryStation; }
            set { m_isPrimaryStation = value; }
        }

        public string LoadingImagePath
        {
            get { return Path.Combine(GCONST.RunTimeDataPath, "LoadingImage.jpg"); }
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

        public Waveface.Upload.AttachmentUploadFacade Uploader
        {
            get
            {
                if (m_uploader == null)
                    throw new InvalidOperationException("logic error");

                return m_uploader;
            }
        }

        public UploadOriginPhotosToStationManager UploadOriginPhotosToStationManager
        {
            get
            {
                if (m_uploadOriginPhotosToStationManager == null)
                {
                    m_uploadOriginPhotosToStationManager = UploadOriginPhotosToStationManager.Load() ??
                                                           new UploadOriginPhotosToStationManager();
                }

                return m_uploadOriginPhotosToStationManager;
            }
            set { m_uploadOriginPhotosToStationManager = value; }
        }

        public PhotoDownloader PhotoDownloader
        {
            get { return m_photoDownloader ?? (m_photoDownloader = new PhotoDownloader()); }
            set { m_photoDownloader = value; }
        }

        public RunTime RT
        {
            get 
            {
                return m_runTime ?? (m_runTime = new RunTime());
            }
        }

        public DialogResult NewPostThreadErrorDialogResult { get; set; }

        public QuitOption QuitOption { get; private set; }

        #endregion

        public Main()
        {
            InitializeComponent();

            Current = this;
        }

        public Main(string initSessionToken)
        {
            InitializeComponent();

            Current = this;

            m_initSessionToken = initSessionToken;
        }

        private void Init()
        {
            QuitOption = QuitOption.QuitProgram;

            File.Delete(m_shellContentMenuFilePath);

            HttpHelp.EnableUnsafeHeaderParsing();

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            // InitVirtualFolderForm();
            // InitTaskbarNotifier();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = true;
            m_formSettings.UseLocation = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            m_autoUpdator = new AutoUpdate(false);
            m_autoUpdator.StartLoop();

            bgWorkerGetAllData.WorkerSupportsCancellation = true;

            m_MessageReceiver.WndProc += new EventHandler<MessageEventArgs>(m_MessageReceiver_WndProc);
        }



        #region Init

        private static void CheckRefreshStatus()
        {
            var receiver = FindWindow("SystemTrayMessageReceiver", null);
            if (receiver != null)
            {
                int ret;
                SendMessageTimeout(receiver, 0x404, IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);
            }
        }

        void panelTitle_AccountInfoClosed(object sender, EventArgs e)
        {
            AdjustAccountInfoButton();
        }

        private void AdjustAccountInfoButton()
        {
            try
            {
                var userInfo = UserInfo.Instance;

                var accessTokenExpired = false;

                if ((userInfo.SNS1 != null && userInfo.SNS2 != null))
                {
                    var facebook = (from item1 in userInfo.SNS1
                                    where item1 != null && item1.type == "facebook"
                                    from item2 in userInfo.SNS2
                                    where item2 != null && item2.type == "facebook"
                                select new
                                {
                                    Enabled = item1.enabled,
                                    SnsID = item2.snsid,
                                        Status = item2.status,
                                        Status2 = item1.status,
                                        LastSync = item1.lastSync
                                }).FirstOrDefault();

                    accessTokenExpired = (facebook == null) ? false : facebook.Status.Contains("disconnected");
                }

                panelTitle._btnAccount.ImageDisable = accessTokenExpired ? Resources.account_badge : Resources.FBT_account;
                panelTitle._btnAccount.Image = accessTokenExpired ? Resources.account_badge : Resources.FBT_account;
                panelTitle._btnAccount.ImageHover = accessTokenExpired ? Resources.account_badge_hl : Resources.FBT_account_hl;
            }
            catch (Exception)
            {
            }
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
            int s = 512;
            int k = 16;

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

        /*
        private void InitDropableNotifyIcon()
        {
            m_dropableNotifyIcon.Text = "Waveface: Drop something here to start post!";
            m_dropableNotifyIcon.NotifyIcon.Icon = Icon;
            m_dropableNotifyIcon.NotifyIcon.ContextMenuStrip = mnuTray;
            m_dropableNotifyIcon.NotifyIcon.Visible = true;
            //m_dropableNotifyIcon.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            m_dropableNotifyIcon.InitDrop();
            m_dropableNotifyIcon.DragEnter += DropableNotifyIcon_DragEnter;
        }

        private void initVirtualFolderForm()
        {
            m_virtualFolderForm = new VirtualFolderForm();
            m_virtualFolderForm.Top = Screen.PrimaryScreen.Bounds.Height - m_virtualFolderForm.Height*3;
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
            // m_taskbarNotifier.ContentClick += taskbarNotifier_ContentClick;
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
        
        */

        private void SaveRunTime()
        {
            RT.SaveJSON();
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
				MessageBox.Show(Resources.EXCEPTION_401, "Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

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
				MessageBox.Show(Resources.FORCE_LOGOUT, Resources.SYSTEM_ERROR_TITLE, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                QuitOption = QuitOption.Logout;

                Close();
            }
        }

        #endregion

        #region Event

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dropableNotifyIcon.Dispose();

			CancelAllThreads();
            SaveRunTime();
            
        }

        public void Logout()
        {
            Program.ShowCrashReporter = false;
            QuitOption = QuitOption.Logout;
            Close();
        }

        private void CancelAllThreads()
        {
            timerPolling.Enabled = false;

            bgWorkerGetAllData.CancelAsync();

            try
            {
                m_uploader.Stop();

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
                }
            }
            catch
            {
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            int flags,
            uint timeout,
            out int result);

        public void AccountInformation()
        {
            var receiver = FindWindow("SystemTrayMessageReceiver", null);
            if (receiver != null)
            {
                int ret;
                SendMessageTimeout(receiver, 0x403, IntPtr.Zero, IntPtr.Zero, 2, 500, out ret);
            }
            //m_setting = new SettingForm(m_autoUpdator);
            //m_setting.ShowDialog();
            //m_setting = null;
        }

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        private void Main_Activated(object sender, EventArgs e)
        {
            if (m_PostForm != null)
            {
                BringWindowToTop(m_PostForm.Handle);
                ShowWindow(m_PostForm.Handle, 5); //SW_SHOW
                m_PostForm.Activate();
            }

            //if (m_setting != null)
            //{
            //    BringWindowToTop(m_setting.Handle);
            //    ShowWindow(m_setting.Handle, 5); //SW_SHOW
            //    m_setting.Activate();
            //}
        }

        #endregion

        #region Windows Size

        //private void Main_SizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        panelLeftInfo.Width = leftArea.MyWidth;
        //    }
        //    catch (Exception _e)
        //    {
        //        NLogUtility.Exception(s_logger, _e, "Main_SizeChanged");
        //    }
        //}

        protected override bool ShowWithoutActivation // stops the window from stealing focus
        {
            get { return true; }
        }

        //private void splitterRight_SplitterMoving(object sender, SplitterEventArgs e)
        //{
        //    if (e.SplitX < (panelLeftInfo.Width + postsArea.MinimumSize.Width + 8))
        //        e.SplitX = (panelLeftInfo.Width + postsArea.MinimumSize.Width + 8);
        //}

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
            if (m_PostForm != null)
                return;

            FlashWindow.Start(Current);

            m_dragDropClipboardHelper.Drag_Enter(e, false);
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Drop(e);

            FlashWindow.Stop(Current);
        }

        private void Form_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave();

            FlashWindow.Stop(Current);
        }

        private void Form_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over(e, false);
        }

        /*
        private void DropableNotifyIcon_DragEnter(object sender, DragEventArgs e)
        {
            if (!m_trayIconPopup.Visible)
                m_trayIconPopup.Show(m_dropableNotifyIcon.GetLocation());
        }
        */

        #endregion

        #region Network Status

        public void UpdateNetworkStatus()
        {
            RT.REST.IsNetworkAvailable = true;
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



            Text = "Stream - [" + _login.user.email + "]"; // this has to be sync with SystemStry.Main.CLIENT_TITLE for finding client form

            RT.CurrentGroupID = RT.Login.groups[0].group_id;
            // RT.LoadGroupLocalRead();

            UploadOriginPhotosToStationManager.Start();
            PhotoDownloader.Start();
            //BatchPostManager.Start();
            m_uploader = new Upload.AttachmentUploadFacade(GCONST.RunTimeDataPath, _login.user.user_id);

            if (Environment.GetCommandLineArgs().Length == 1)
            {
                StationState.Start();
            }

            panelTitle.showRefreshUI(true);

            Cursor = Cursors.Default;

            //var rt = RT.LoadJSON();

            //RT.CurrentGroupPosts = rt.CurrentGroupPosts;

            GetAllDataAsync();

            PrefetchImages();

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
                RT.StationMode = true;
            }

            try
            {
                MongoServer _dbServer = MongoServer.Create("mongodb://localhost:10319/?safe=true");
                BsonDocument _doc =
                    _dbServer.GetDatabase("wammer").GetCollection("LoginedSession").FindOne(Query.EQ("_id",
                                                                                                     m_initSessionToken));

				if (_doc == null)
					s_logger.Warn("Cannot found the specified logined data from DB");

				string _json = _doc.ToJson();

                MR_auth_login _login = JsonConvert.DeserializeObject<MR_auth_login>(_json);

				if (_login == null)
					s_logger.Warn("Cannot deserialize to MR_auth_login");
				
				_login.session_token = m_initSessionToken;

                IsPrimaryStation = isPrimaryStation(_dbServer, _login);

                Debug.WriteLine("_login.session_token: " + _login.session_token);
                procLoginResponse(_login);
            }
            catch (Exception e)
            {
                s_logger.Error("Cannot login: " + e);
				MessageBox.Show(Resources.FORCE_LOGOUT, Resources.SYSTEM_ERROR_TITLE);
                QuitOption = QuitOption.Logout;
                Close();
            }
        }

        private bool isPrimaryStation(MongoServer dbServer, MR_auth_login _login)
        {
            BsonDocument _userInfo =
                dbServer.GetDatabase("wammer").GetCollection("drivers").FindOne(Query.EQ("_id", _login.user.user_id));

            BsonElement _isPrimaryElem = _userInfo.GetElement("isPrimaryStation");
            bool _isPrimaryStation = false;

            if (_isPrimaryElem != null && _isPrimaryElem.Value.AsBoolean)
                _isPrimaryStation = true;

            return _isPrimaryStation;
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
                RT.StationMode = true;
            }

            MR_auth_login _login = RT.REST.Auth_Login(email, password);
            return procLoginResponse(_login);
        }


 

        #endregion

        #region Filter (NOT USED NOW)

        /*
        public void DoTimelineFilter(FilterItem item, bool isFilterTimelineMode)
        {
            if (!RT.LoginOK)
                return;

            RT.FilterMode = true;

            if (item != null)
            {
                RT.CurrentFilterItem = item;
            }

            RT.FilterPosts = new List<Post>(); // Reset

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
        */

        #endregion

        #region Helper

        private void ShowTimelineUI()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowTimelineUI(); }
                           ));
            }
            else
            {
                ShowPostInTimeline();

                timerPolling.Enabled = true;
            }
        }

        public void GetAllDataAsync(object parameter = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { GetAllDataAsync(parameter); }));
            }
            else
            {
                Cursor = Cursors.WaitCursor;

                panelTitle.updateRefreshUI(false);

                if (bgWorkerGetAllData.IsBusy)
                    Cursor = Cursors.Default;
                else
                    bgWorkerGetAllData.RunWorkerAsync(parameter);
            }
        }

        private void ShowPostInTimeline()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowPostInTimeline(); }
                           ));
            }
            else
            {
                List<Post> _posts = RT.CurrentGroupPosts;

                _posts = filterPost(_posts);

                Dictionary<DateTime, string> _firstPostInADay = setCalendarBoldedDates(_posts);

                leftArea.SetUI(true);

                lock (postsArea.PostsList)
                {
                    postsArea.PostsList.SetPosts(_posts, _firstPostInADay);
                }
            }
        }

        private List<Post> filterPost(List<Post> posts)
        {
            if (string.IsNullOrEmpty(m_displayType))
                return posts;

            List<Post> _posts = new List<Post>();

            foreach (Post _p in posts)
            {
                if (_p.type == m_displayType)
                    _posts.Add(_p);
            }

            return _posts;
        }

        public void DisplayFilter(string displayType)
        {
            m_displayType = displayType;

            ShowPostInTimeline();
        }

        public void PrefetchImages()
        {
            backgroundWorkerPreloadAllImages.RunWorkerAsync();
        }

        #endregion

        #region Post

        private void timerDelayPost_Tick(object sender, EventArgs e)
        {
            if (m_delayPostPicList.Count == 0)
                return;

            timerDelayPost.Enabled = false;

            DoRealPostForm(m_delayPostPicList, m_delayPostType, m_delayPostText);

            m_delayPostPicList.Clear();
            m_delayPostText = "";

            timerDelayPost.Enabled = true;
        }

        public void Post()
        {
            DoRealPostForm(new List<string>(), PostType.All, "");
        }

        public void EditPost(Post post, List<string> existPostAddPhotos, int existPostAddPhotosIndex)
        {
            try
            {
                m_PostForm = new PostForm("", new List<string>(), PostType.All, post, true, existPostAddPhotos, existPostAddPhotosIndex);
                DialogResult _dr = m_PostForm.ShowDialog();

                if (_dr == DialogResult.Yes)
                {
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Edit Post");
            }

            m_PostForm = null;
        }

        public void Post(List<string> pics, PostType postType, string postText)
        {
            m_delayPostType = postType;
            m_delayPostPicList = pics;
            m_delayPostText = postText;
        }

        private void DoRealPostForm(List<string> pics, PostType postType, string delayPostText)
        {
            try
            {
                m_PostForm = new PostForm(delayPostText, pics, postType, null, false, null, -1);
                DialogResult _dr = m_PostForm.ShowDialog();

                if (_dr == DialogResult.OK)
                {
                }
            }
            catch (Exception _e)
            {
                MessageBox.Show(Resources.UNABLE_POST + " : " + _e.Message, "Stream");

                NLogUtility.Exception(s_logger, _e, "Post");
            }

            m_PostForm = null;
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

        public Post PostUpdate(Post post, Dictionary<string, string> optionalParams)
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

                RefreshSinglePost(_update.post);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "PostUpdate");

				MessageBox.Show(Resources.ERROR_AND_RETRY, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ReloadAllData();

                return null;
            }

            return _update.post;
        }

        public bool ChangePostFavorite(Post post)
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

                RefreshSinglePost(_update.post);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "PostUpdate");

				MessageBox.Show(Resources.ERROR_AND_RETRY, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                detailView.ResetUI();

                RemovePostLocalAndRefresh(postId);

                Cursor = Cursors.Default;
                return true;
            }

            Cursor = Cursors.Default;
            return false;
        }

        private void RemovePostLocalAndRefresh(string postId)
        {
            foreach (Post _p in RT.CurrentGroupPosts)
            {
                if (_p.post_id == postId)
                {
                    RT.CurrentGroupPosts.Remove(_p);

                    ShowPostInTimeline();

                    break;
                }
            }
        }

        public void RefreshSinglePost_ByID(string post_id)
        {
            MR_posts_getSingle _singlePost = RT.REST.Posts_GetSingle(post_id);

            if ((_singlePost != null) && (_singlePost.post != null))
            {
                ReplacePostInList(_singlePost.post, RT.CurrentGroupPosts);

                ShowPostInTimeline();
            }
        }

        public void RefreshSinglePost(Post post)
        {
            ReplacePostInList(post, RT.CurrentGroupPosts);

            ShowPostInTimeline();
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

        #endregion

        #region Calendar

        private Dictionary<DateTime, string> setCalendarBoldedDates(List<Post> posts)
        {
            Dictionary<DateTime, string> _firstPostInADay = new Dictionary<DateTime, string>();

            MonthCalendar _mc = leftArea.MonthCalendar;

            _mc.SuspendLayout();

            _mc.BoldedDates.Clear();

            foreach (Post _p in posts)
            {
                DateTime _dt = DateTimeHelp.ISO8601ToDateTime(_p.timestamp);

                if (!_mc.BoldedDates.Contains(_dt.Date))
                    _mc.BoldedDates.Add(_dt.Date);

                if (!_firstPostInADay.ContainsKey(_dt.Date))
                    _firstPostInADay.Add(_dt.Date, _p.post_id);
            }

            _mc.ResumeLayout();

            _mc.Invalidate();

            return _firstPostInADay;
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
                        m_delayPostText = "";
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

        //private void capture(ShotType shotType)
        //{
        //    try
        //    {
        //        CaptureForm _captureForm = new CaptureForm(shotType);

        //        if ((_captureForm.ShowDialog() != DialogResult.OK) || (_captureForm.Image == null))
        //        {
        //            return;
        //        }

        //        string _filename =
        //            string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmssff"), ImageFormat.Jpeg).ToLower();

        //        Image _img = _captureForm.Image;

        //        string _pathToSave = Path.Combine(GCONST.TempPath, _filename);

        //        _img.Save(_pathToSave, ImageFormat.Jpeg);

        //        Post(new List<string> { _pathToSave }, PostType.Photo, "");
        //    }
        //    catch (Exception _e)
        //    {
        //        MessageBox.Show(_e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void regionMenuItem_Click(object sender, EventArgs e)
        //{
        //    capture(ShotType.Region);
        //}

        //private void windowsMenuItem_Click(object sender, EventArgs e)
        //{
        //    capture(ShotType.Window);
        //}

        //private void screenMenuItem_Click(object sender, EventArgs e)
        //{
        //    capture(ShotType.Screen);
        //}

        #endregion

        #region Misc

        public void RefreshTimelineUI()
        {
            postsArea.RefreshTimelineUI();
        }

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
                int _next_seq_num;

                if (m_next_seq_num <= 0)
                {
                    if (RT.CurrentGroupPosts.Count == 0)
                        _next_seq_num = 1;
                    else
                        _next_seq_num = RT.CurrentGroupPosts.Max(x => x.seq_num) + 1;
                }
                else
                {
                    _next_seq_num = m_next_seq_num;
                }

                MR_changelogs_get _usertracks = RT.REST.changelogs_get(_next_seq_num);

                if (_usertracks != null)
                {
                    if (_usertracks.changelog_list == null || _usertracks.post_list == null)
                    {
                        timerPolling.Enabled = true;
                        return;
                    }

                    m_next_seq_num = _usertracks.next_seq_num;

                    foreach (UT_UsertrackList _usertrack in _usertracks.changelog_list)
                    {
                        foreach (UT_Action _action in _usertrack.actions)
                        {
                            if (_action.target_type == "post")
                            {
                                switch (_action.action)
                                {
                                    case "unhide":
                                    case "create":
                                        ReloadAllData(); //ToDo
                                        return;

                                    case "hide":
                                        RemovePostLocalAndRefresh(_usertrack.target_id);
                                        break;
                                }
                            }
                        }
                    }

                    if (_usertracks.post_list.Count > 0)
                    {
                        string _json = JsonConvert.SerializeObject(_usertracks.post_list.Select(x => x.post_id).ToList());

                        MR_posts_get _postsGet = RT.REST.Posts_FetchByFilter_2(_json);

                        if (_postsGet != null)
                        {
                            bool _changed = false;

                            foreach (Post _post in _postsGet.posts)
                            {
                                _changed = ReplacePostInList(_post, RT.CurrentGroupPosts);
                            }

                            if (_changed)
                                ShowPostInTimeline();
                        }
                    }
                }
            }
            catch (ChangeLogsPurgedException)
            {
                ReloadAllData();
            }
            catch (VersionNotSupportedException)
            {
                handleVersionNotSupportedError();
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

        /*
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
                }
                else
                {
                }
            }
        }

        private void timerShowStatuMessage_Tick(object sender, EventArgs e)
        {
            timerShowStatuMessage.Enabled = false;
        }

        public void ShowFileMissDialog(string text)
        {
            NewPostThreadErrorDialogResult = DialogResult.None;

            MsgBox _msgBox = new MsgBox(string.Format(Resources.FILE_MISS_PATTERN, text), "Stream",
                                        MessageBoxIcon.Warning);
            _msgBox.SetButtons(new[] { Resources.CONTINUE, Resources.RETRY, Resources.CANCEL },
                               new[] { DialogResult.Yes, DialogResult.Retry, DialogResult.Cancel }, 3);
            DialogResult _dr = _msgBox.ShowDialog();

            NewPostThreadErrorDialogResult = _dr;
        }

        public void OverQuotaMissDialog(string text)
        {
            NewPostThreadErrorDialogResult = DialogResult.None;

            MsgBox _msgBox = new MsgBox(string.Format(Resources.OVER_QUOTA, text), "Stream",
                                        MessageBoxIcon.Warning);
            _msgBox.SetButtons(new[] { Resources.RETRY, Resources.CANCEL },
                               new[] { DialogResult.Retry, DialogResult.Cancel }, 2);
            DialogResult _dr = _msgBox.ShowDialog();

            NewPostThreadErrorDialogResult = _dr;
        }

        #endregion

        #region GetAllData

        public void ReloadAllData(object parameter = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { ReloadAllData(parameter); }));
            }
            else
            {
                GetAllDataAsync(parameter);
            }
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
                m_getAllDataError = true;
                throw;
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


            var dirtyPost = e.Argument as PhotoPostInfo;

            foreach (Post _post in _allPosts.Values)
            {

                if (dirtyPost != null &&
                    dirtyPost.post_id == _post.post_id)
                {
                    _post.Sources = dirtyPost.sources;
                }

                foreach (var oldPost in RT.CurrentGroupPosts)
                {
                    if (oldPost.post_id != _post.post_id)
                        continue;
                    _post.Sources = oldPost.Sources;
                    break;
                }

                _tmpPosts.Add(_post);
            }

            RT.CurrentGroupPosts = _tmpPosts;

            s_logger.Info("bgWorkerGetAllData_DoWork. Get Post Count:" + _tmpPosts.Count);
        }

        private void bgWorkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;

            if (e.Error is VersionNotSupportedException)
            {
                handleVersionNotSupportedError();
            }

            if (m_getAllDataError)
            {
                m_getAllDataError = false;
                ForceLogout();
            }
            else
            {
                panelTitle.updateRefreshUI(true);

                ShowTimelineUI();
            }
        }

        private void handleVersionNotSupportedError()
        {
            int original = System.Threading.Interlocked.Exchange(ref m_InForceAutoUpdate, 1);

            if (original == 1)
                return;

			MessageBox.Show(Resources.NEED_UPGRADE, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AutoUpdate update = new AutoUpdate(true);
            if (update.IsUpdateRequired())
                update.ShowUpdateNeededUI();


            System.Threading.Interlocked.Exchange(ref m_InForceAutoUpdate, 0);
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
                            if (post.attachment_id_array == null || post.attachment_id_array.Count == 0)
                                break;

                            var coverId = post.getCoverImageId();

                            string _url = Current.RT.REST.attachments_getImageURL(coverId, "small");
                            string _localPic = Current.RT.REST.attachments_getThumbnailFilePath(coverId, "small");

                            PreloadThumbnail(_url, _localPic);

                            PhotoDownloader.PreloadPictures(post, allSize);

                            break;
                        }
                    case "link":
                        {
                            if (post.preview.thumbnail_url != null)
                            {
                                string _url = post.preview.thumbnail_url;

                                int _hashCode = Math.Abs(post.preview.thumbnail_url.GetHashCode());

                                string _localPic = Path.Combine(GCONST.RunTimeDataPath,
                                                                post.post_id + "_previewthumbnail_" + _hashCode + ".jpg");

                                PreloadThumbnail(_url, _localPic);
                            }

                            break;
                        }

                    case "rtf":
                        {
                            Attachment _a = null;

                            //
                            // TODO: revise this when implementing rtf
                            //

                            //foreach (Attachment _attachment in post.attachments)
                            //{
                            //    if (_attachment.type == "image")
                            //    {
                            //        _a = _attachment;
                            //        break;
                            //    }
                            //}

                            //if (_a != null)
                            //{
                            //    string _url = string.Empty;
                            //    string _fileName = string.Empty;
                            //    Current.RT.REST.attachments_getRedirectURL_Image(_a, "small", out _url,
                            //                                                     out _fileName, false);

                            //    string _localPic = Path.Combine(GCONST.ImageCachePath, _fileName);

                            //    PreloadThumbnail(_url, _localPic);
                            //}

                            break;
                        }

                    case "doc":
                        {
                            //
                            // TODO: revise this when implementing rtf
                            //

                            //Attachment _a = post.attachments[0];

                            //if (_a.image != string.Empty)
                            //{
                            //    string _localPic = Path.Combine(GCONST.ImageCachePath,
                            //                                    _a.object_id + "_thumbnail" + ".jpg");

                            //    string _url = _a.image;

                            //    _url = Current.RT.REST.attachments_getRedirectURL_PdfCoverPage(_url);

                            //    PreloadThumbnail(_url, _localPic);
                            //}

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
                PrefetchImages(RT.CurrentGroupPosts, true);
            }
        }

        #endregion


        void m_MessageReceiver_WndProc(object sender, MessageEventArgs e)
        {
            switch (e.Message)
            {
                case 0x401:
                    if (this.WindowState == FormWindowState.Minimized)
                        WindowState = FormWindowState.Normal;

                    if (!this.Visible)
                        this.Show();

                    this.TopMost = true;
                    BringWindowToTop(this.Handle);
                    this.TopMost = false;
                    break;

                case 0x402:
                    panelTitle.StartRefreshing();
                    break;

                case 0x403:
                    panelTitle.StopRefreshing();
                    break;
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            Init();

            if (!string.IsNullOrEmpty(m_initSessionToken))
                LoginWithInitSession();

            postsArea.PostsList.DetailView = detailView;

            if (Environment.GetCommandLineArgs().Length == 1)
            {
                NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

                UpdateNetworkStatus();
            }

            /*
            InitDropableNotifyIcon();
            
            m_trayIconPopup = new Popup(m_trayIconPanel = new TrayIconPanel());
            
            // Send To
            CreateFileWatcher();
            */

            CreateLoadingImage();

            panelTitle.AccountInfoClosed += new EventHandler(panelTitle_AccountInfoClosed);

            AdjustAccountInfoButton();

            CheckRefreshStatus();

            var timer = new Timer()
            {
                Interval = 15000
            };

            timer.Tick += (s, ex) =>
            {
                CheckRefreshStatus();
            };

            timer.Start();
        }
    }
}