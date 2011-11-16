#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component.ListBarControl;
using XPExplorerBar;

#endregion

namespace Waveface
{
    public partial class LeftArea : UserControl
    {
        private List<NewPostItem> m_batchPostItems = new List<NewPostItem>();
        private AdvSelection m_selection;
        private int m_antOffset;

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
                monthCalendar.Visible = true;
                taskPaneFilter.Visible = true;

                return monthCalendar;
            }
        }

        public LeftArea()
        {
            InitializeComponent();

            taskPaneFilter.UseCustomTheme("panther.dll");
            //taskPaneFilter.UseClassicTheme();

            initAnt();
            initBatchPostItems();

            AntTimer.Start();
        }

        #region BatchPostItems

        private void initBatchPostItems()
        {
            //
            //從檔案讀入
            //

            foreach (NewPostItem _item in m_batchPostItems)
            {
                AddToExplorerBar(_item);
            }
        }

        public void AddNewPostItem(NewPostItem newPostItem)
        {
            m_batchPostItems.Add(newPostItem);

            AddToExplorerBar(newPostItem);
        }

        private void AddToExplorerBar(NewPostItem item)
        {
            Expando _expando = new Expando();
            _expando.Text = item.OrgPostTime.ToString("MM/dd HH:mm:ss");
            _expando.Animate = true;

            BatchPostItemUI _batchPostItemUi = new BatchPostItemUI(this, _expando, item);
            _batchPostItemUi.Dock = DockStyle.Fill;
            _expando.Controls.Add(_batchPostItemUi);

            taskPaneBatchPost.Expandos.Add(_expando);
        }

        public void DeletePostItem(BatchPostItemUI batchPostItemUi, Expando expando, NewPostItem newPostItem)
        {
            m_batchPostItems.Remove(newPostItem);
            taskPaneBatchPost.Expandos.Remove(expando);
        }

        #endregion

        public void fillGroupAndUser()
        {
            vsNetListBarGroups.Groups.Clear();
            removeImageListLargeIcon();

            Dictionary<string, MR_groups_get> _mrGroups = MainForm.THIS.RT.GroupSets;

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
            {
                imageListLarge.Images.RemoveAt(imageListLarge.Images.Count - 1);
            }
        }

        private void pictureBoxHintDrop_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(pbHintDrop.Image, 0, 0);

            Pen _pen = new Pen(Color.White, 2);
            _pen.DashStyle = DashStyle.Dash;
            _pen.DashPattern = new float[] { 3, 3 };
            _pen.DashOffset = m_antOffset;

            Bitmap _ant = new Bitmap(pbHintDrop.Width, pbHintDrop.Height);
            Graphics _g = Graphics.FromImage(_ant);
            _g.Clear(Color.Magenta);
            _g.DrawPath(new Pen(Color.Black), m_selection.Line());
            _g.DrawPath(_pen, m_selection.Line());
            _g.FillRegion(new SolidBrush(Color.Magenta), m_selection.Selected);

            _ant.MakeTransparent(Color.Magenta);
            e.Graphics.DrawImageUnscaled(_ant, 0, 0);

            TextRenderer.DrawText(e.Graphics, "Drag && drop the file to start the sharing", new Font("Tahoma", 11),
                                              new Rectangle(8, 8, pbHintDrop.Width - 16, pbHintDrop.Height - 16),
                                              SystemColors.ControlDarkDark,
                                              TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void AntTimer_Tick(object sender, EventArgs e)
        {
            m_antOffset++;
            m_antOffset %= 6;

            pbHintDrop.Refresh();
        }

        private void initAnt()
        {
            pbHintDrop.Image = new Bitmap(pbHintDrop.Width, pbHintDrop.Height);

            m_selection = new AdvSelection();
            m_selection.AddRectangle(new Rectangle(4, 4, pbHintDrop.Width - 8, pbHintDrop.Height - 8));
        }

        private void LeftArea_Resize(object sender, EventArgs e)
        {
            initAnt();
        }

        private void pbHintDrop_Click(object sender, EventArgs e)
        {
        }

        private void vsNetListBarGroups_SelectedGroupChanged(object sender, EventArgs e)
        {
            MainForm.THIS.showGroupPosts(vsNetListBarGroups.SelectedGroup.Tag.ToString());
        }

        private void monthCalendar_DateClicked(object sender, CustomControls.DateEventArgs e)
        {
            MainForm.THIS.ClickCalendar(e.Date);
        }
    }

    #region AdvSelection

    internal class AdvSelection
    {
        private GraphicsPath m_gp;
        private Region m_region;

        public Region Selected
        {
            get { return m_region; }
        }

        public GraphicsPath Outline
        {
            get { return m_gp; }
        }

        public AdvSelection()
        {
            m_region = new Region();
            m_gp = new GraphicsPath();

            Clear();
        }

        public void AddRectangle(Rectangle rectangle)
        {
            m_gp.AddRectangle(rectangle);
            m_region.Union(m_gp);
        }

        public void AddCircle(Rectangle rectangle)
        {
            m_gp.AddEllipse(rectangle);
            m_region.Union(m_gp);
        }

        public void AddFreeform(Point[] points)
        {
            m_gp.AddPolygon(points);
            m_region.Union(m_gp);
        }

        public void RemoveRectangle(Rectangle rectangle)
        {
            m_gp.AddRectangle(rectangle);
            m_region.Exclude(m_gp);
        }

        public void RemoveCircle(Rectangle rectangle)
        {
            m_gp.AddEllipse(rectangle);
            m_region.Exclude(m_gp);
        }

        public void RemoveFreeform(Point[] points)
        {
            m_gp.AddPolygon(points);
            m_region.Exclude(m_gp);
        }

        public void Clear()
        {
            m_region.MakeEmpty();
            m_gp.Reset();
        }

        public GraphicsPath Line()
        {
            GraphicsPath _gp = new GraphicsPath();

            if (m_gp.PointCount > 0)
            {
                _gp.AddPath(m_gp, false);
                _gp.Widen(new Pen(Color.White, 2));
            }

            return _gp;
        }
    }

    #endregion
}