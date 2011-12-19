#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CustomControls;
using NLog;
using Waveface.API.V2;
using Waveface.Component.ListBarControl;
using Waveface.FilterUI;
using XPExplorerBar;

#endregion

namespace Waveface
{
    public partial class LeftArea : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private NewPostManager m_newPostManager;
        private FilterManager m_filterManager;
        private Button m_buttonAddNewFilter;

        private int m_count;
        private bool m_startUpload;

        #region Properties

        public int MyWidth
        {
            get
            {
                panelCalendar.Height = monthCalendar.Height + 6;
                return monthCalendar.Width;
            }
        }

        public CustomControls.MonthCalendar MonthCalendar
        {
            get
            {
                panelCalendar.Visible = true;

                return monthCalendar;
            }
            set { monthCalendar = value; }
        }

        #endregion

        public LeftArea()
        {
            InitializeComponent();

            //taskPaneFilter.UseCustomTheme("panther.dll");
            taskPaneFilter.UseClassicTheme();

            initBatchPostItems();

            //initTimeline();   
        }

        #region CustomizedFilters

        private void AddNewItem_Click(object sender, EventArgs e)
        {
            m_filterManager = new FilterManager();
            m_filterManager.MyParent = this;
            m_filterManager.ShowDialog();

            FillCustomizedFilters();
        }

        public void FillCustomizedFilters()
        {
            expandoQuicklist.Items.Clear();

            // --
            List<Fetch_Filter> _filters = FilterHelper.GetList();

            if (_filters != null)
            {
                foreach (Fetch_Filter _f in _filters)
                {
                    FilterItem _item = new FilterItem();
                    _item.Name = _f.filter_name;
                    _item.Filter = _f.filter_entity.ToString();

                    TaskItem _taskItem = CreateTaskItem(_item, true);
                    _taskItem.ImageList = imageListCustomFilter;
                    _taskItem.ImageIndex = 1;

                    expandoQuicklist.Items.Add(_taskItem);
                }
            }

            // --
            m_buttonAddNewFilter = new Button();
            m_buttonAddNewFilter.Text = "Add New Filter";
            m_buttonAddNewFilter.Click += AddNewItem_Click;
            m_buttonAddNewFilter.Width = expandoQuicklist.Width - 16;

            expandoQuicklist.Items.Add(m_buttonAddNewFilter);
        }

        private void taskPaneFilter_Resize(object sender, EventArgs e)
        {
            if (m_buttonAddNewFilter != null)
                m_buttonAddNewFilter.Width = expandoQuicklist.Width - 16;
        }

        #endregion

        #region Timeline

        private void resetAllTaskItemForeColor()
        {
            foreach (Control _control in expandoQuicklist.Items)
            {
                if (_control is TaskItem)
                    ((TaskItem)_control).CustomSettings.LinkColor = SystemColors.HotTrack;
            }
        }

        public void initTimeline()
        {
            DateTime _beginDay = new DateTime(2011, 11, 1); //@ 要改以註冊時間
            DateTime _endDay = DateTime.Now;

            IEnumerable<DateTime> _months = MonthsBetween(_beginDay, _endDay).Reverse();

            List<FilterItem> _filterItems = new List<FilterItem>();

            foreach (DateTime _dt in _months)
            {
                string _m = _dt.ToString("y");

                DateTime _from = new DateTime(_dt.Year, _dt.Month, 1, 0, 0, 0);

                DateTime _to;

                if (_dt.Month == 12)
                    _to = new DateTime(_dt.Year + 1, 1, 1, 0, 0, 0);
                else
                    _to = new DateTime(_dt.Year, _dt.Month + 1, 1, 0, 0, 0);

                _to = _to.AddSeconds(-1);

                FilterItem _item = new FilterItem();
                _item.Name = _m;
                //_item.Filter = FilterHelper.GetTimeRangeFilterJson(_from, _to, -10, "[type]", "[offset]"); //@
                _item.Filter = FilterHelper.GetTimeStampFilterJson(_to, -20, "[type]", "[offset]");

                _filterItems.Add(_item);
            }
        }

        private TaskItem CreateTaskItem(FilterItem item, bool isCustom)
        {
            TaskItem _taskItem = new TaskItem();
            _taskItem.CustomSettings.LinkColor = SystemColors.HotTrack;
            _taskItem.Text = item.Name;
            _taskItem.Tag = item;

            if (isCustom)
                _taskItem.Click += CustomFilterTaskItem_Click;
            else
                _taskItem.Click += FilterlinkLabel_Click;

            return _taskItem;
        }

        private void CustomFilterTaskItem_Click(object sender, EventArgs e)
        {
            resetAllTaskItemForeColor();

            TaskItem _taskItem = (TaskItem)sender;
            _taskItem.CustomSettings.LinkColor = Color.DarkOrange;

            FilterItem _item = (FilterItem)_taskItem.Tag;

            Main.Current.DoTimelineFilter(_item, false);
        }

        private void FilterlinkLabel_Click(object sender, EventArgs e)
        {
            resetAllTaskItemForeColor();

            TaskItem _taskItem = (TaskItem)sender;
            _taskItem.CustomSettings.LinkColor = Color.DarkOrange;

            FilterItem _item = (FilterItem)_taskItem.Tag;

            Main.Current.DoTimelineFilter(_item, true);
        }

        private IEnumerable<DateTime> MonthsBetween(DateTime d0, DateTime d1)
        {
            if (d0 > d1)
            {
                DateTime _dt = d0;
                d0 = d1;
                d1 = _dt;
            }

            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
        }

        #endregion

        #region Group

        public void fillGroupAndUser()
        {
            vsNetListBarGroups.Groups.Clear();
            removeImageListLargeIcon();

            Dictionary<string, MR_groups_get> _mrGroups = Main.Current.RT.GroupGetReturnSets;

            int k = 0;
            int _imageIndex; //在這裡是正確的

            foreach (string _groupID in _mrGroups.Keys)
            {
                MR_groups_get _groupsGet = _mrGroups[_groupID];

                VSNetListBarItem[] _subItems = new VSNetListBarItem[_groupsGet.active_members.Count];

                int i = 0;

                foreach (User _user in _groupsGet.active_members)
                {
                    if (_user.avatar_url == "")
                    {
                        _imageIndex = 0;
                    }
                    else
                    {
                        k++;
                        _imageIndex = k;

                        Image _img = ImageUtility.GetAvatarImage(_user.user_id, _user.avatar_url);
                        imageListLarge.Images.Add(_img);
                    }

                    _subItems[i] = new VSNetListBarItem(_user.nickname, _imageIndex);

                    i++;
                }

                VSNetListBarGroup _groupListBar = new VSNetListBarGroup(_groupsGet.group.name, _subItems);
                _groupListBar.Tag = _groupID;
                _groupListBar.View = ListBarGroupView.LargeIcons;

                vsNetListBarGroups.Groups.Add(_groupListBar);
            }
        }

        private void removeImageListLargeIcon()
        {
            while (imageListLarge.Images.Count > 2)
                imageListLarge.Images.RemoveAt(imageListLarge.Images.Count - 1);
        }

        #endregion

        #region Misc

        public void SetUI(bool flag)
        {
            buttonCreatePost.Visible = flag;
            taskPaneFilter.Visible = flag;
        }

        private void monthCalendar_DateClicked(object sender, DateEventArgs e)
        {
            Main.Current.ClickCalendar(e.Date);
        }

        private void buttonCreatePost_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            Main.Current.Post();
        }

        #endregion

        #region BatchPost

        private void initBatchPostItems()
        {
            m_newPostManager = new NewPostManager();
            NewPostManager _newPostManager = m_newPostManager;
            m_newPostManager = _newPostManager;

            ThreadPool.QueueUserWorkItem(state => { BatchPostThreadMethod(); });
        }

        public void AddNewPostItem(NewPostItem item)
        {
            m_newPostManager.Add(item);
            m_newPostManager.Save();
        }

        private void BatchPostThreadMethod()
        {
            NewPostItem _newPost = null;
            m_startUpload = true;

            while (true)
            {
                lock (m_newPostManager)
                {
                    if (m_newPostManager.Items.Count > 0)
                        _newPost = m_newPostManager.Items[m_newPostManager.Items.Count - 1];
                    else
                        _newPost = null;
                }

                if (_newPost != null)
                {
                    if (m_startUpload)
                    {
                        NewPostItem _retItem = BatchPhotoPost(_newPost);

                        if (_retItem.PostOK)
                        {
                            lock (m_newPostManager)
                            {
                                m_newPostManager.Remove(_newPost);

                                m_newPostManager.Save();
                            }
                        }
                        else
                        {
                            lock (m_newPostManager)
                            {
                                m_newPostManager.Save();
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private NewPostItem BatchPhotoPost(NewPostItem newPost)
        {
            s_logger.Trace("BatchPhotoPost:" + newPost.Text + ", Files=" + newPost.Files.Count);

            string _ids = "[";

            while (true)
            {
                if (m_startUpload)
                {
                    string _file = newPost.Files[m_count];

                    if (newPost.UploadedFiles.Keys.Contains(_file))
                    {
                        _ids += "\"" + newPost.UploadedFiles[_file] + "\"" + ",";
                    }
                    else
                    {
                        try
                        {
                            string _text = new FileName(_file).Name;
                            string _resizedImage = ImageUtility.ResizeImage(_file, _text, newPost.ResizeRatio, 100);

                            MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_text, _resizedImage, "", true);

                            if (_uf == null)
                            {
                                newPost.PostOK = false;
                                return newPost;
                            }

                            _ids += "\"" + _uf.object_id + "\"" + ",";

                            newPost.UploadedFiles.Add(_file, _uf.object_id);
                        }
                        catch (Exception _e)
                        {
                            NLogUtility.Exception(s_logger, _e, "BatchPhotoPost:File_UploadFile");
                            newPost.PostOK = false;
                            return newPost;
                        }
                    }

                    m_count++;

                    int _count = newPost.Files.Count;

                    ChangeProgressBarUI(m_count * 100 / _count, m_count + "/" + _count);

                    if (m_count == _count)
                        break;
                }
                else
                {
                    newPost.PostOK = false;
                    return newPost;
                }
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
            _ids += "]";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(newPost.Text, _ids, "", "image");

                if (_np == null)
                {
                    newPost.PostOK = false;
                    return newPost;
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "BatchPhotoPost:File_UploadFile");

                newPost.PostOK = false;
                return newPost;
            }

            SinglePostDone("OK");

            newPost.PostOK = true;
            return newPost;
        }

        public void ChangeProgressBarUI(int percent, string countText)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               ChangeProgressBarUI(percent, countText);
                           }
                           ));
            }
            else
            {
                UpdateDragAndDropUI(percent);
            }
        }

        public void UpdateDragAndDropUI(int percent)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               UpdateDragAndDropUI(percent);
                           }
                           ));
            }
            else
            {
                if ((percent == 0) || (percent == 100))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_area1;

                    return;
                }

                if ((percent > 0) && (percent <= 20))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_loading0;

                    return;
                }

                if ((percent > 20) && (percent <= 40))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_loading1;

                    return;
                }

                if ((percent > 40) && (percent <= 60))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_loading2;

                    return;
                }

                if ((percent > 60) && (percent <= 80))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_loading3;

                    return;
                }

                if ((percent > 80) && (percent < 100))
                {
                    pbDropArea.Image = Properties.Resources.dragNdrop_loading4;

                    return;
                }
            }
        }

        public void SinglePostDone(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               SinglePostDone(text);
                           }
                           ));
            }
            else
            {
                //Main.Current.AfterBatchPostDone();
                //DeleteThis();
            }
        }

        #endregion
    }
}