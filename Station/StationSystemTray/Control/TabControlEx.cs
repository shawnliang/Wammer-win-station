using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StationSystemTray
{
	[ToolboxBitmap(typeof(TabControl))]
	public class TabControlEx : TabControl
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
				if (value)
					Multiline = true;
				UpdateStyles();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public new bool Multiline
		{
			get
			{
				if (HideTabs)
					return true;
				return base.Multiline;
			}
			set {
				base.Multiline = HideTabs || value;
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				if (HideTabs)
				{
					return new Rectangle(0, 0, Width, Height);
				}
				Int32 tabStripHeight = default(Int32);
				Int32 itemHeight = default(Int32);

				itemHeight = Alignment <= TabAlignment.Bottom ? ItemSize.Height : ItemSize.Width;

				if (Appearance == TabAppearance.Normal)
				{
					tabStripHeight = 5 + (itemHeight * RowCount);
				}
				else
				{
					tabStripHeight = (3 + itemHeight) * RowCount;
				}
				switch (Alignment)
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