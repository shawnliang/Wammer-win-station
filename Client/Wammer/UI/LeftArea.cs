#region

using System;
using System.Drawing;
using System.Windows.Forms;
using CustomControls;
using NLog;
using Waveface.Properties;
using MonthCalendar = CustomControls.MonthCalendar;

#endregion

namespace Waveface
{
    public partial class LeftArea : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        // private FilterManager m_filterManager;
        // private Button m_buttonAddNewFilter;
        private string m_dropAreaMessage;
        private Image m_dropAreaImage;
        private Font m_font = new Font("Arial", 10, FontStyle.Bold);
        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;

        #region Properties

        public int MyWidth
        {
            get
            {
                panelCalendar.Height = monthCalendar.Height + 6;
                return monthCalendar.Width;
            }
        }

        public MonthCalendar MonthCalendar
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

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();
            pbDropArea.AllowDrop = true;

            // taskPaneFilter.UseCustomTheme("panther.dll");
            // taskPaneFilter.UseClassicTheme();

            m_dropAreaImage = new Bitmap(150, 138);
        }

        public void SetNewPostManager()
        {
            NewPostManager.Current.ShowMessage += ShowDragDropMessage;
            NewPostManager.Current.UpdateUI += UpdateDragAndDropUI;
            NewPostManager.Current.UploadDone += UploadDone;
        }

        /*
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
        */

        #region Group

        /*
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

        */

        #endregion

        #region Misc

        public void SetUI(bool flag)
        {
            //taskPaneFilter.Visible = flag;
        }

        private void monthCalendar_DateClicked(object sender, DateEventArgs e)
        {
            Main.Current.ClickCalendar(e.Date);
        }

        #endregion

        #region BatchPost

        public void UpdateDragAndDropUI(int percent, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { UpdateDragAndDropUI(percent, text); }
                           ));
            }
            else
            {
                Main.Current.ShowStatuMessage("", false);

                if ((percent == 0) || (percent == 100))
                {
                    DrawDropArea(Resources.dragNdrop_area1);

                    return;
                }

                if ((percent > 0) && (percent <= 20))
                {
                    DrawDropArea(Resources.dragNdrop_loading0);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }

                if ((percent > 20) && (percent <= 40))
                {
                    DrawDropArea(Resources.dragNdrop_loading1);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }

                if ((percent > 40) && (percent <= 60))
                {
                    DrawDropArea(Resources.dragNdrop_loading2);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }

                if ((percent > 60) && (percent <= 80))
                {
                    DrawDropArea(Resources.dragNdrop_loading3);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }

                if ((percent > 80) && (percent < 100))
                {
                    DrawDropArea(Resources.dragNdrop_loading4);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }
            }
        }

        private void UploadDone(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { UploadDone(text); }
                           ));
            }
            else
            {
                Main.Current.ShowStatuMessage(text, true);
            }
        }

        public void ShowDragDropMessage(string text)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new MethodInvoker(
                               delegate { ShowDragDropMessage(text); }
                               ));
                }
                catch
                {
                }
            }
            else
            {
                m_dropAreaMessage = text;
                DrawDropArea(Resources.dragNdrop_area1);
            }
        }

        private void DrawDropArea(Image bmp)
        {
            using (Graphics _g = Graphics.FromImage(m_dropAreaImage))
            {
                _g.Clear(Color.Transparent);
                _g.DrawImage(bmp, 0, 0);

                Size _size = TextRenderer.MeasureText(m_dropAreaMessage, m_font, pbDropArea.Size, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                _g.DrawString(m_dropAreaMessage, m_font, Brushes.DeepSkyBlue, (bmp.Width - _size.Width) / 2, (bmp.Height - _size.Height) / 2);
            }

            pbDropArea.Image = m_dropAreaImage;
        }

        #endregion

        #region DropArea

        private void DropArea_DragDrop(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Drop(e);
        }

        private void DropArea_DragEnter(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Enter(e);
        }

        private void DropArea_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave();
        }

        private void DropArea_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over(e);
        }

        #endregion

        private void pbDropArea_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            //NewPostManagerUI _newPostManager = new NewPostManagerUI();
            //_newPostManager.ShowDialog();
        }
    }
}