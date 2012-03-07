using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace StationSystemTray.Control
{
    [ToolboxBitmap(typeof(System.Windows.Forms.TabControl))]
    public class TabControlEx : System.Windows.Forms.TabControl
    {


        private bool m_HideTabs;
        [DefaultValue(false), RefreshProperties(RefreshProperties.All)]
        public bool HideTabs
        {
            get { return m_HideTabs; }
            set
            {
                if (m_HideTabs == value)
                    return;
                m_HideTabs = value;
                if (value == true)
                    this.Multiline = true;
                this.UpdateStyles();
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public bool Multiline
        {
            get
            {
                if (this.HideTabs)
                    return true;
                return base.Multiline;
            }
            set
            {
                if (this.HideTabs)
                {
                    base.Multiline = true;
                }
                else
                {
                    base.Multiline = value;
                }
            }
        }



        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                if (this.HideTabs)
                {
                    return new Rectangle(0, 0, Width, Height);
                }
                else
                {
                    Int32 tabStripHeight = default(Int32);
                    Int32 itemHeight = default(Int32);

                    if (this.Alignment <= TabAlignment.Bottom)
                    {
                        itemHeight = this.ItemSize.Height;
                    }
                    else
                    {
                        itemHeight = this.ItemSize.Width;
                    }

                    if (this.Appearance == TabAppearance.Normal)
                    {
                        tabStripHeight = 5 + (itemHeight * this.RowCount);
                    }
                    else
                    {
                        tabStripHeight = (3 + itemHeight) * this.RowCount;
                    }
                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            return new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                        case TabAlignment.Bottom:
                            return new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                        case TabAlignment.Left:
                            return new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                        case TabAlignment.Right:
                            return new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                    }
                    return base.DisplayRectangle;
                }
            }
        }
    }
}
