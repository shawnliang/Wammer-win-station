#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public class CustomDataGridView : DataGridView
    {
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONDBLCLK = 0x0206;

        public event EventHandler<DataGridViewCellContextMenuStripNeededEventArgs> ContextMenuStripNeeded;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // handle mouse right click message, compute row/col index, call event handler.
            switch (m.Msg)
            {
                case WM_RBUTTONDBLCLK:
                case WM_RBUTTONDOWN:
                    if (ContextMenuStrip != null && ContextMenuStripNeeded != null)
                    {
                        // The low-order word specifies the horizontal position of the cursor.
                        // The high-order word specifies the vertical position of the cursor.

                        int _v = m.LParam.ToInt32();
                        int _x = _v & 0xffff; // low-order word.
                        int _y = (_v >> 16) & 0xffff; // high-order word.

                        int _rowIndex = GetRowIndexAt(_y);
                        int _columnIndex = GetColumnIndexAt(_x);

                        DataGridViewCellContextMenuStripNeededEventArgs _e =
                            new DataGridViewCellContextMenuStripNeededEventArgs(_columnIndex, _rowIndex);

                        _e.ContextMenuStrip = ContextMenuStrip; // set ContextMenuStrip property
                        OnContextMenuStripNeeded(_e); // call event methods subscribed.
                    }

                    break;
            }
        }

        protected virtual void OnContextMenuStripNeeded(DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (ContextMenuStrip != null && ContextMenuStripNeeded != null)
            {
                ContextMenuStripNeeded(this, e);
            }
        }

        private int GetColumnIndexAt(int mouseLocation_X)
        {
            if (FirstDisplayedScrollingColumnIndex < 0)
            {
                return -1; // no columns defined.
            }

            if (RowHeadersVisible && mouseLocation_X <= RowHeadersWidth)
            {
                return -1; // at rowheaders.
            }

            int _columnCount = Columns.Count;

            for (int _index = 0; _index < _columnCount; _index++)
            {
                if (Columns[_index].Displayed)
                {
                    Rectangle _rect = GetColumnDisplayRectangle(_index, true); // partial rectangle only.

                    if (_rect.Left <= mouseLocation_X && mouseLocation_X < _rect.Right)
                    {
                        return _index;
                    }
                }
            }

            return -1;
        }

        private int GetRowIndexAt(int mouseLocation_Y)
        {
            if (FirstDisplayedScrollingRowIndex < 0)
            {
                return -1; // no rows defined.
            }

            if (ColumnHeadersVisible && mouseLocation_Y <= ColumnHeadersHeight)
            {
                return -1; // at columnheaders.
            }

            int _index = FirstDisplayedScrollingRowIndex;
            int _displayedCount = DisplayedRowCount(true);

            for (int k = 1; k <= _displayedCount;)
            {
                if (Rows[_index].Visible)
                {
                    Rectangle _rect = GetRowDisplayRectangle(_index, true); // partial rectangle only

                    if (_rect.Top <= mouseLocation_Y && mouseLocation_Y < _rect.Bottom)
                    {
                        return _index;
                    }

                    k++; // add when visible;
                }

                _index++;
            }

            return -1;
        }
    }
}