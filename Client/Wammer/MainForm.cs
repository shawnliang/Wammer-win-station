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
        public static GCONST GCONST = new GCONST();

        #region Fields

        private bool m_canAutoFetchNewestPosts = true;
        private List<string> m_delayPostPicList = new List<string>();

        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
        private DropableNotifyIcon m_dropableNotifyIcon = new DropableNotifyIcon();
        private Popup m_trayIconPopup;
        private TrayIconPanel m_trayIconPanel;
        private UploadOriginPhotosToStation m_uploadOriginPhotosToStation;

        private string m_stationIP;

        //ScreenShot
        private ImageStorage m_imageStorage;

        //Main
        private Bitmap m_offlineImage;
        private Bitmap m_onlineImage;
        private string m_shellContentMenuFilePath = Application.StartupPath + @"\ShellContextMenu.dat";
        private VirtualFolderForm m_virtualFolderForm;
        private MyTaskbarNotifier m_taskbarNotifier;

        private RunTime m_runTime = new RunTime();

        #endregion

        #region Properties

        public RunTime RT
        {
            get { return m_runTime; }
            set { m_runTime = value; }
        }

        #endregion

        public MainForm()
        {
            THIS = this;

            File.Delete(m_shellContentMenuFilePath);

            InitializeComponent();

            postsArea.PostsList.DetailView = detailView;

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            //initVirtualFolderForm();

            InitTaskbarNotifier();
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

            m_uploadOriginPhotosToStation = new UploadOriginPhotosToStation();
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

                RT.REST.IsNetworkAvailable = true;
            }
            else
            {
                connectedImageLabel.Text = " Disconnected";
                connectedImageLabel.Image = m_offlineImage;

                RT.REST.IsNetworkAvailable = false;
            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            try
            {
                if (IsHandleCreated)
                    Invoke(new MethodInvoker(UpdateStatusBar));
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.ToString(), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Windows Size

        private void MainForm_Resize(object sender, EventArgs e)
        {
            panelLeftInfo.Width = leftArea.MyWidth + 8;

            m_dropableNotifyIcon.NotifyIcon.BalloonTipTitle = "Waveface";
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

        #region Login

        public void Reset()
        {
            RT.Reset();

            WService.StationIP = "";

            postsArea.ShowTypeUI(false);
        }

        public bool Login(string email, string password)
        {
            bool _ret = false;

            Cursor.Current = Cursors.WaitCursor;

            MR_auth_login _login = RT.REST.Auth_Login(email, password);

            if (_login != null)
            {
                Reset();

                RT.Login = _login;
                RT.OnlineMode = true;

                CheckStation(RT.Login.stations);

                getGroupAndUser();

                fillUserInformation();

                //預設群組
                RT.CurrentGroupID = RT.Login.groups[0].group_id;

                //bgWorkerGetAllData.RunWorkerAsync();

                // 顯示所有Post
                DoTimelineFilter(FilterHelper.CreateAllPostFilterItem(), true);

                leftArea.SetUI(true);

                _ret = true;
            }
            else //離線模式
            {
                RT.OnlineMode = false;


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

                        WService.StationIP = _ip;

                        //test
                        //m_stationIP = _ip;
                        //panelStation.Visible = true;

                        RT.StationMode = true;

                        return;
                    }
                }
            }

            RT.StationMode = false;
        }

        private void fillUserInformation()
        {
            labelName.Text = RT.Login.user.nickname;

            if (RT.Login.user.avatar_url == string.Empty)
            {
                pictureBoxAvatar.Image = null;
            }
            else
            {
                pictureBoxAvatar.LoadAsync(RT.Login.user.avatar_url);
            }
        }

        private void getGroupAndUser()
        {
            foreach (Group _g in RT.Login.groups)
            {
                MR_groups_get _mrGroupsGet = RT.REST.Groups_Get(_g.group_id);

                if (_mrGroupsGet != null)
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
            if (!RT.LoginOK)
                return;

            if (item != null) //會null是由PostArea的comboBoxType發出
            {
                RT.IsAllPostMode = item.IsAllPost;
                RT.CurrentFilterItem = item;
            }

            RT.CurrentPosts = new List<Post>(); //Reset

            RT.IsTimelineFilter = isTimelineFilter; // 是Timeline才秀Type
            postsArea.ShowTypeUI(RT.IsTimelineFilter);

            //string _title = "[" + RT.CurrentFilterItem.Name + "]";
            //_title += (postsArea.GetPostTypeText() == "All Posts") ? "" : (" - " + postsArea.GetPostTypeText());

            FetchPostsAndShow(true);
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

        private void FetchPostsAndShow(bool firstTime)
        {
            int _offset = GCONST.GetPostOffset;

            if (RT.IsAllPostMode)
            {
                if (RT.CurrentGroupPosts != null)
                    _offset = RT.CurrentGroupPosts.Count;
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

            MR_posts_get _postsGet = RT.REST.Posts_FetchByFilter(_filter);

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

            setCalendarBoldedDates(_posts);

            postsArea.ShowPostInfo(RT.CurrentPostsAllCount, _posts.Count);

            postsArea.PostsList.SetPosts(_posts, refreshCurrentPost, RT.IsFirstTimeGetData);
        }

        public void PostListClick(int clickIndex)
        {
            RT.IsFirstTimeGetData = false;
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
            if (!RT.LoginOK)
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

        #region AfterPostComment

        public void AfterPostComment(string post_id)
        {
            MR_posts_getSingle _singlePost = RT.REST.Posts_GetSingle(post_id);

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

        public void FillTimelineComboBox(List<FilterItem> list)
        {
            postsArea.FillTimelineComboBox(list);
        }

        private void radioButtonStation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCloud.Checked)
            {
                WService.StationIP = WService.CloundIP;
                RT.StationMode = false;
            }
            else
            {
                WService.StationIP = m_stationIP;
                RT.StationMode = true;
            }
        }

        #endregion

        #region GetAllData

        private void bgWorkerGetAllData_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string _firstGetCount = "200"; //須為正
            string _continueGetCount = "-200"; //須為負

            MR_posts_get _postsGet = null;
            Dictionary<string, Post> _allPosts = new Dictionary<string, Post>();
            string _datum = string.Empty;

            // 先取得第一批
            MR_posts_getLatest _getLatest = RT.REST.Posts_getLatest(_firstGetCount);

            if (_getLatest != null)
            {
                foreach (Post _p in _getLatest.posts)
                {
                    _allPosts.Add(_p.post_id, _p);
                    _datum = _p.timestamp;
                }

                // 若未取完
                if (_getLatest.get_count < _getLatest.total_count)
                {
                    // 假設還有很多沒取得
                    int _remainingCount = int.MaxValue;

                    while (_remainingCount > 0)
                    {
                        _datum = DateTimeHelp.ToUniversalTime_ToISO8601(DateTimeHelp.ISO8601ToDateTime(_datum).AddSeconds(1));

                        _postsGet = RT.REST.Posts_get(_continueGetCount, _datum, "");

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

            if (RT.GroupAllPosts.ContainsKey(RT.CurrentGroupID))
                RT.GroupAllPosts[RT.CurrentGroupID] = _tmpPosts;
            else
                RT.GroupAllPosts.Add(RT.CurrentGroupID, _tmpPosts);
        }

        private void bgWorkerGetAllData_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            List<Post> _posts = RT.GroupAllPosts[RT.CurrentGroupID];

            setCalendarBoldedDates(_posts);

            postsArea.PostsList.SetPosts(_posts, false, RT.IsFirstTimeGetData);
        }

        #endregion
    }
}