#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace Waveface.Windows.Forms
{
    public class DragDropTabControl : TabControl
    {
        private Container components;
        private TabPage mDragTab;
        private ArrayList mPages;
        private bool mblnAllowDragDrop;
        private bool mblnDragging;
        private bool mblnLoading;

        public DragDropTabControl()
        {
            components = null;
            mblnDragging = false;
            mblnAllowDragDrop = false;
            mblnLoading = false;
            InitializeComponent();
        }

        public override bool AllowDrop
        {
            get
            {
                return mblnAllowDragDrop;
            }
            set
            {
                mblnAllowDragDrop = value;
            }
        }

        public event EventHandler AfterDragDrop;

        protected new void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics graphics = e.Graphics;
            StringFormat stringFormat = new StringFormat();
            try
            {
                base.OnDrawItem(e);
                stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
                graphics.DrawString(TabPages[e.Index].Text, base.Font, SystemBrushes.WindowText, (GetTabRect(e.Index).X + 3), (GetTabRect(e.Index).Y + 3), stringFormat);
            }
            catch
            {
            }
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            foreach (TabPage page in TabPages)
            {
                if (IsMnemonic(charCode, page.Text))
                {
                    SelectedTab = page;
                    Focus();
                    return true;
                }
                return false;
            }
            return false;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (AllowDrop)
            {
                base.OnDragOver(e);
                Point point1 = new Point(e.X, e.Y);
                point1 = PointToClient(point1);
                TabPage tabPage1 = GetTabPageByTab(point1);
                if (tabPage1 == null)
                    e.Effect = DragDropEffects.None;
                else if (e.Data.GetDataPresent("System.Windows.Forms.TabPage"))
                {
                    e.Effect = DragDropEffects.Move;
                    TabPage tabPage2 = (TabPage) e.Data.GetData("System.Windows.Forms.TabPage");
                    mDragTab = (TabPage) e.Data.GetData("System.Windows.Forms.TabPage");
                    int j = FindIndex(tabPage2);
                    int i1 = FindIndex(tabPage1);
                    if (j != i1)
                    {
                        mPages = new ArrayList();
                        mblnDragging = true;
                        int i2 = TabPages.Count - 1;
                        for (int k = 0; k <= i2; k++)
                        {
                            if (k != j)
                                mPages.Add(TabPages[k]);
                        }
                        mPages.Insert(i1, tabPage2);
                    }
                }
            }
        }

        private TabPage GetTabPageByTab(Point pt)
        {
            TabPage tabPage2 = null;
            int j = TabPages.Count - 1;
            for (int i = 0; i <= j; i++)
            {
                if (GetTabRect(i).Contains(pt))
                {
                    tabPage2 = TabPages[i];
                    break;
                }
            }
            TabPage tabPage1 = tabPage2;
            return tabPage1;
        }

        private int FindIndex(TabPage page)
        {
            for (int j = 0; j <= TabPages.Count - 1; j++)
            {
                if (TabPages[j] == page)
                    return j;
            }
            return - 1;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (AllowDrop)
            {
                try
                {
                    if (mblnDragging)
                    {
                        TabPages.Clear();
                        TabPages.AddRange((TabPage[]) mPages.ToArray(typeof (TabPage)));
                        SelectedTab = mDragTab;
                        mblnDragging = false;
                        mDragTab = null;

                        if (AfterDragDrop != null)
                            AfterDragDrop(this, EventArgs.Empty);
                    }
                }
                catch
                {
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!mblnDragging & !mblnLoading)
                base.OnSelectedIndexChanged(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point1 = new Point(e.X, e.Y);
                TabPage tabPage = GetTabPageByTab(point1);
                if (tabPage != null)
                    DoDragDrop(tabPage, DragDropEffects.Move);
            }
            base.OnMouseMove(e);
        }
    }
}