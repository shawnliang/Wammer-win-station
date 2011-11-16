#region

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.MozBar
{

    #region Delegates

    public delegate void MozScrollEventHandler(object sender, MozScrollEventArgs e);

    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public int cbSize;
        public int fMask;
        public int nMin;
        public int nMax;
        public int nPage;
        public int nPos;
        public int nTrackPos;
    }

    [ToolboxItem(false)]
    public class ScrollableControlWithScrollEvents : ScrollableControl
    {
        #region Win32 API Constants

        //private const int WS_HSCROLL = 0x100000;

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SIF_RANGE = 0x1;
        private const int SIF_PAGE = 0x2;
        private const int SIF_POS = 0x4;
        private const int SIF_DISABLENOSCROLL = 0x8;
        private const int SIF_TRACKPOS = 0x10;
        private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_DISABLENOSCROLL | SIF_TRACKPOS;

        #endregion

        #region Win32 API Functions

        [DllImport("User32", EntryPoint = "GetScrollInfo")]
        private static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO info);

        #endregion

        #region Events

        [Browsable(true)]
        [Description("Indicates that the control has been scrolled horizontally.")]
        [Category("Panel")]
        public event MozScrollEventHandler HorizontalScroll;


        [Browsable(true)]
        [Description("Indicates that the control has been scrolled vertically.")]
        [Category("Panel")]
        public event MozScrollEventHandler VerticalScroll;

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                //p.Style= p.Style & ~WS_HSCROLL;
                return p; //base.CreateParams;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Let the control process the message
            base.WndProc(ref m);

            // Was this a horizontal scroll message?
            if (m.Msg == WM_HSCROLL)
            {
                if (HorizontalScroll != null)
                {
                    uint _wParam = (uint) m.WParam.ToInt32();
                    SCROLLINFO _si = new SCROLLINFO();
                    _si.cbSize = Marshal.SizeOf(_si);
                    _si.fMask = SIF_ALL;
                    bool _ret = GetScrollInfo(Handle, SB_HORZ, ref _si);
                    HorizontalScroll(this,
                                     new MozScrollEventArgs(
                                         GetEventType(_wParam & 0xffff), (int) (_wParam >> 16), _si));
                }
            } 
                // or a vertical scroll message?
            else if (m.Msg == WM_VSCROLL)
            {
                if (VerticalScroll != null)
                {
                    uint _wParam = (uint) m.WParam.ToInt32();
                    SCROLLINFO _si = new SCROLLINFO();
                    _si.cbSize = Marshal.SizeOf(_si);
                    _si.fMask = SIF_ALL;
                    bool _ret = GetScrollInfo(Handle, SB_VERT, ref _si);
                    VerticalScroll(this,
                                   new MozScrollEventArgs(
                                       GetEventType(_wParam & 0xffff), (int) (_wParam >> 16), _si));
                }
            }
        }

        #endregion

        #region Methods

        // Based on SB_* constants
        private static ScrollEventType[] m_events =
            new[]
                {
                    ScrollEventType.SmallDecrement,
                    ScrollEventType.SmallIncrement,
                    ScrollEventType.LargeDecrement,
                    ScrollEventType.LargeIncrement,
                    ScrollEventType.ThumbPosition,
                    ScrollEventType.ThumbTrack,
                    ScrollEventType.First,
                    ScrollEventType.Last,
                    ScrollEventType.EndScroll
                };

        // Decode the type of scroll message
        private ScrollEventType GetEventType(uint wParam)
        {
            if (wParam < m_events.Length)
                return m_events[wParam];
            else
                return ScrollEventType.EndScroll;
        }

        #endregion
    }

    #region MozScrollEventArgs

    public class MozScrollEventArgs
    {
        #region Class Data

        private SCROLLINFO m_info;
        private int m_newValue;

        // The color that has changed
        private ScrollEventType m_type;

        #endregion

        #region Constructor

        // Initializes a new instance of the MozItemEventArgs class with default settings
        public MozScrollEventArgs(ScrollEventType type, int newValue, SCROLLINFO info)
        {
            m_type = type;
            m_newValue = newValue;
            m_info = info;
        }

        #endregion

        #region Properties

        public SCROLLINFO ScrollInfo
        {
            get
            {
                return m_info;
            }
        }

        public ScrollEventType Type
        {
            get
            {
                return m_type;
            }
        }

        public int NewValue
        {
            get
            {
                return m_newValue;
            }
        }

        #endregion
    }

    #endregion
}