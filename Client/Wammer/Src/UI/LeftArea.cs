#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using CustomControls;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.FilterUI;
using Waveface.Properties;
using MonthCalendar = CustomControls.MonthCalendar;

#endregion

namespace Waveface
{
    public partial class LeftArea : UserControl
    {
        #region Private Static Var
        private static Logger s_logger = LogManager.GetCurrentClassLogger(); 
        #endregion


        #region Var
        private FilterManager _filterManager;
        private Button m_buttonAddNewFilter;
        private string m_dropAreaMessage;
        private Image _dropAreaImage;
        private Font _font;
        private DragDrop_Clipboard_Helper _dragDropClipboardHelper; 
        #endregion

        #region Private Property
        private Font m_Font
        {
            get
            {
                return _font ?? (_font = new Font(Resources.DEFAULT_FONT, 9 * getDPIRatio(), FontStyle.Bold));
            }
        }

        private DragDrop_Clipboard_Helper m_DragDropClipboardHelper
        {
            get
            {
                return _dragDropClipboardHelper ?? (_dragDropClipboardHelper = new DragDrop_Clipboard_Helper());
            }
        }

        private Image m_DropAreaImage
        {
            get
            {
                return _dropAreaImage ?? (_dropAreaImage = new Bitmap(150, 138));
            }
            set
            {
                if (_dropAreaImage == value)
                    return;

                if (_dropAreaImage != null)
                {
                    _dropAreaImage.Dispose();
                    _dropAreaImage = null;
                }

                _dropAreaImage = value;
            }
        }

        private FilterManager m_FilterManager
        {
            get
            {
                return _filterManager ?? (_filterManager = new FilterManager() 
                {
                    MyParent = this
                });
            }
            set
            {
                if (_filterManager == value)
                    return;

                if (_filterManager != null)
                {
                    if (!_filterManager.IsDisposed)
                        _filterManager.Dispose();
                    _filterManager = null;
                }

                _filterManager = value;
            }
        }
        #endregion


        #region Public Property
        public int MyWidth
        {
            get
            {
                //panelCalendar.Height = monthCalendar.Height + 4;

                return monthCalendar.Width + 4;
            }
        }

        public MonthCalendar MonthCalendar
        {
            get
            {
                panelCalendar.Visible = true;

                if (monthCalendar.Font.Size != 8)
                    monthCalendar.Font = new Font("Tahoma", 8);

                return monthCalendar;
            }
        }

        #endregion

        #region Constructor
        public LeftArea()
        {
            InitializeComponent();

            pbDropArea.AllowDrop = true;

            InitDefaultFilters();

            InitAddNewButton();
        } 
        #endregion


        #region Private Method

        private float getDPIRatio()
        {
            try
            {
                using (Graphics _g = CreateGraphics())
                {
                    if (_g.DpiX == 120)
                        return 0.85f;
                }
            }
            catch
            {
            }

            return 1;
        } 

        #endregion

        #region CustomizedFilters

        private void AddNewItem_Click(object sender, EventArgs e)
        {
            m_FilterManager = null;
            m_FilterManager.ShowDialog();

            FillCustomizedFilters();
        }

        public void FillCustomizedFilters()
        {
            List<Fetch_Filter> _filters = FilterHelper.GetList();

            if (_filters != null)
            {
                tvCustomFilter.SuspendLayout();

                tvCustomFilter.Nodes.Clear();

                foreach (Fetch_Filter _f in _filters)
                {
                    FilterItem _item = new FilterItem();
                    _item.Name = _f.filter_name;
                    _item.Filter = _f.filter_entity.ToString();

                    TreeNode _treeNode = new TreeNode();
                    _treeNode.Text = _item.Name;
                    _treeNode.Tag = _item;

                    _treeNode.ImageIndex = 0;
                    _treeNode.SelectedImageIndex = 0;

                    tvCustomFilter.Nodes.Add(_treeNode);
                }

                ResumeLayout();
            }
        }

        private void InitAddNewButton()
        {
            m_buttonAddNewFilter = new Button();
            m_buttonAddNewFilter.Text = "Add New Filter";
            m_buttonAddNewFilter.Click += AddNewItem_Click;
            m_buttonAddNewFilter.Width = panelCustomFilter.Width - 16;

            panelCustomFilter.Controls.Add(m_buttonAddNewFilter);
        }

        #endregion

        #region Timeline

        private void InitDefaultFilters()
        {
            InitAllAndPostTypeFilters();
            InitMonthFilters();
        }

        private void InitAllAndPostTypeFilters()
        {
            // "all", "text", "image", "link", "rtf", "doc"

            AddTimelineTreeNode(FilterHelper.CreateAllPostFilterItemByPostType("全部", "all"));
            AddTimelineTreeNode(FilterHelper.CreateAllPostFilterItemByPostType("文字", "text"));
            AddTimelineTreeNode(FilterHelper.CreateAllPostFilterItemByPostType("照片", "image"));
            AddTimelineTreeNode(FilterHelper.CreateAllPostFilterItemByPostType("網頁", "link"));
        }

        private void InitMonthFilters()
        {
            DateTime _beginDay = new DateTime(2012, 1, 1); //@ 要改以註冊時間
            DateTime _endDay = DateTime.Now;

            IEnumerable<DateTime> _months = MonthsBetween(_beginDay, _endDay).Reverse();

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
                _item.Filter = FilterHelper.GetTimeRangeFilterJson(_from, _to, -100, "all", "[offset]");
                //_item.Filter = FilterHelper.GetTimeStampFilterJson(_to, -20, "all", "[offset]");

                AddTimelineTreeNode(_item);
            }
        }

        private void AddTimelineTreeNode(FilterItem _item)
        {
            TreeNode _treeNode = new TreeNode();
            _treeNode.Text = _item.Name;
            _treeNode.Tag = _item;

            _treeNode.ImageIndex = 1;
            _treeNode.SelectedImageIndex = 1;

            tvTimeline.Nodes.Add(_treeNode);
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
            panelFilter.Visible = flag;

            FillCustomizedFilters();
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

                if (percent == int.MinValue)
                {
                    DrawDropArea(Resources.dragNdrop_area1, percent);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }

                if ((percent == 0) || (percent == 100))
                {
                    DrawDropArea(Resources.dragNdrop_area1, percent);

                    return;
                }

                if ((percent > 0) && (percent < 100))
                {
                    DrawDropArea(Resources.dragNdrop_loading0, percent);
                    Main.Current.ShowStatuMessage(text, false);

                    return;
                }
            }
        }

        void ShowFileMissDialog(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { ShowFileMissDialog(text); }
                           ));
            }
            else
            {
                Main.Current.ShowFileMissDialog(text);
            }
        }

        void OverQuotaMissDialog(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { OverQuotaMissDialog(text); }
                           ));
            }
            else
            {
                Main.Current.OverQuotaMissDialog(text);
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

                Main.Current.ReloadAllData();
            }
        }

        private void EditUpdateDone(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { EditUpdateDone(text); }
                           ));
            }
            else
            {
                Main.Current.ShowStatuMessage(text, true);
                Main.Current.ReloadAllData();
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
                DrawDropArea(Resources.dragNdrop_area1, -1);
            }
        }

        private void DrawDropArea(Image bmp, int percent)
        {
            using (Graphics _g = Graphics.FromImage(m_DropAreaImage))
            {
                _g.TextRenderingHint = TextRenderingHint.AntiAlias;
                _g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _g.SmoothingMode = SmoothingMode.HighQuality;

                int _off = -3;

                _g.Clear(Color.Transparent);
                _g.DrawImage(bmp, _off, 0, bmp.Width, bmp.Height);

                if ((percent > 0) && (percent < 100))
                {
                    int _y = 128;
                    int _dy = 9;
                    int _sx = 20;
                    int _ex = 140;

                    int _dx = ((_ex - _sx) * percent) / 100;

                    _g.FillRectangle(Brushes.PaleTurquoise, _sx, _y, _dx, _dy);
                }

                Size _size = TextRenderer.MeasureText(m_dropAreaMessage, m_Font, pbDropArea.Size, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                using (Brush _brush = new SolidBrush(Color.FromArgb(33, 69, 99)))
                {
                    _g.DrawString(m_dropAreaMessage, m_Font, _brush, ((bmp.Width - _size.Width) / 2) + _off - 1, bmp.Height - _size.Height - 6);
                }
            }

            pbDropArea.Image = m_DropAreaImage;
        }

        #endregion

        #region DropArea

        private void DropArea_DragDrop(object sender, DragEventArgs e)
        {
            m_DragDropClipboardHelper.Drag_Drop(e);

            FlashWindow.Stop(Main.Current);
        }

        private void DropArea_DragEnter(object sender, DragEventArgs e)
        {
            FlashWindow.Start(Main.Current);

            m_DragDropClipboardHelper.Drag_Enter(e, false);
        }

        private void DropArea_DragLeave(object sender, EventArgs e)
        {
            m_DragDropClipboardHelper.Drag_Leave();

            FlashWindow.Stop(Main.Current);
        }

        private void DropArea_DragOver(object sender, DragEventArgs e)
        {
            m_DragDropClipboardHelper.Drag_Over(e, false);
        }

        private void pbDropArea_Click(object sender, EventArgs e)
        {
            using (var dialog = new DropAreaInforForm())
            {
                dialog.ShowDialog();
            }
        }

        #endregion

        private void LeftArea_Resize(object sender, EventArgs e)
        {
            if (m_buttonAddNewFilter != null)
                m_buttonAddNewFilter.Width = Width - 8;

            btnNewPost.Left = (Width - btnNewPost.Width) / 2;
			btnImport.Left = btnNewPost.Left;
            btnToday.Left = (Width - btnToday.Width) / 2;
        }

        private void tvTimeline_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse)
            {
                FilterItem _item = (FilterItem)e.Node.Tag;

                // Main.Current.DoTimelineFilter(_item, true);
            }
        }

        private void tvCustomFilter_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse)
            {
                FilterItem _item = (FilterItem)e.Node.Tag;

                // Main.Current.DoTimelineFilter(_item, false);
            }
        }

        private void btnNewPost_Click(object sender, EventArgs e)
        {
            Main.Current.Post();
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            Main.Current.ClickCalendar(DateTime.Now.Date);
        }

		private void btnImport_Click(object sender, EventArgs e)
		{
			Main.Current.AutoImport();
		}
    }
}