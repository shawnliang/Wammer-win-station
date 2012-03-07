
#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using VS = System.Windows.Forms.VisualStyles;

#endregion

namespace Waveface.Component.PopupControl
{
    [CLSCompliant(true), ToolboxItem(false)]
    public partial class Popup : ToolStripDropDown
    {
        #region " Fields & Properties "

        private Popup m_childPopup;
        private ToolStripControlHost m_host;
        private bool m_isChildPopupOpened;
        private bool m_nonInteractive;
        private Control m_opener;
        private Popup m_ownerPopup;
        private bool m_resizable;
        private bool m_resizableLeft;
        private bool m_resizableTop;

        public Control Content { get; private set; }

        public PopupAnimations ShowingAnimation { get; set; }

        public PopupAnimations HidingAnimation { get; set; }

        public int AnimationDuration { get; set; }
        
        // Gets or sets a value indicating whether the content should receive the focus after the pop-up has been opened.
        public bool FocusOnOpen { get; set; }

        public bool AcceptAlt { get; set; }

        public bool Resizable
        {
            get
            {
                return m_resizable && !m_isChildPopupOpened;
            }
            set
            {
                m_resizable = value;
            }
        }

        public bool NonInteractive
        {
            get
            {
                return m_nonInteractive;
            }
            set
            {
                if (value != m_nonInteractive)
                {
                    m_nonInteractive = value;
                    
                    if (IsHandleCreated)
                        RecreateHandle();
                }
            }
        }

        public new Size MinimumSize { get; set; }

        public new Size MaximumSize { get; set; }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams _cp = base.CreateParams;
                _cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                
                if (NonInteractive)
                    _cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TOOLWINDOW;
                
                return _cp;
            }
        }

        #endregion

        public Popup(Control content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            Content = content;
            FocusOnOpen = true;
            AcceptAlt = true;
            ShowingAnimation = PopupAnimations.SystemDefault;
            HidingAnimation = PopupAnimations.None;
            AnimationDuration = 100;
            InitializeComponent();
            AutoSize = false;
            DoubleBuffered = true;
            ResizeRedraw = true;
            m_host = new ToolStripControlHost(content);
            Padding = Margin = m_host.Padding = m_host.Margin = Padding.Empty;
           
            if (NativeMethods.IsRunningOnMono)
                content.Margin = Padding.Empty;

            MinimumSize = content.MinimumSize;
            content.MinimumSize = content.Size;
            MaximumSize = content.MaximumSize;
            content.MaximumSize = content.Size;
            Size = content.Size;

            if (NativeMethods.IsRunningOnMono)
                m_host.Size = content.Size;

            TabStop = content.TabStop = true;
            content.Location = Point.Empty;
            Items.Add(m_host);
            content.Disposed += (sender, e) =>
                                    {
                                        content = null;
                                        Dispose(true);
                                    };
            content.RegionChanged += (sender, e) => UpdateRegion();
            content.Paint += (sender, e) => PaintSizeGrip(e);
            UpdateRegion();
        }

        #region " Methods "

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (NativeMethods.IsRunningOnMono)
                return; // in case of non-Windows
            
            if ((Visible && ShowingAnimation == PopupAnimations.None) || (!Visible && HidingAnimation == PopupAnimations.None))
            {
                return;
            }
            
            NativeMethods.AnimationFlags _animationFlags = Visible ? NativeMethods.AnimationFlags.Roll : NativeMethods.AnimationFlags.Hide;
            PopupAnimations _flags = Visible ? ShowingAnimation : HidingAnimation;
           
            if (_flags == PopupAnimations.SystemDefault)
            {
                if (SystemInformation.IsMenuAnimationEnabled)
                {
                    if (SystemInformation.IsMenuFadeEnabled)
                    {
                        _flags = PopupAnimations.Blend;
                    }
                    else
                    {
                        _flags = PopupAnimations.Slide | (Visible ? PopupAnimations.TopToBottom : PopupAnimations.BottomToTop);
                    }
                }
                else
                {
                    _flags = PopupAnimations.None;
                }
            }

            if ((_flags & (PopupAnimations.Blend | PopupAnimations.Center | PopupAnimations.Roll | PopupAnimations.Slide)) == PopupAnimations.None)
            {
                return;
            }

            if (m_resizableTop) // popup is “inverted”, so the animation must be
            {
                if ((_flags & PopupAnimations.BottomToTop) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.BottomToTop) | PopupAnimations.TopToBottom;
                }
                else if ((_flags & PopupAnimations.TopToBottom) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.TopToBottom) | PopupAnimations.BottomToTop;
                }
            }

            if (m_resizableLeft) // popup is “inverted”, so the animation must be
            {
                if ((_flags & PopupAnimations.RightToLeft) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.RightToLeft) | PopupAnimations.LeftToRight;
                }
                else if ((_flags & PopupAnimations.LeftToRight) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.LeftToRight) | PopupAnimations.RightToLeft;
                }
            }

            _animationFlags = _animationFlags | (NativeMethods.AnimationFlags.Mask & (NativeMethods.AnimationFlags) (int) _flags);
            NativeMethods.SetTopMost(this);
            NativeMethods.AnimateWindow(this, AnimationDuration, _animationFlags);
        }

        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (AcceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
                else
                {
                    Close();
                }
            }

            bool _processed = base.ProcessDialogKey(keyData);
           
            if (!_processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
            {
                bool _backward = (keyData & Keys.Shift) == Keys.Shift;
                Content.SelectNextControl(null, !_backward, true, true, true);
            }

            return _processed;
        }

        protected void UpdateRegion()
        {
            if (Region != null)
            {
                Region.Dispose();
                Region = null;
            }

            if (Content.Region != null)
            {
                Region = Content.Region.Clone();
            }
        }

        public void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            Show(control, control.ClientRectangle);
        }

        public void Show(Rectangle area)
        {
            m_resizableTop = m_resizableLeft = false;
            Point _location = new Point(area.Left, area.Top + area.Height);
            Rectangle _screen = Screen.FromControl(this).WorkingArea;
            
            if (_location.X + Size.Width > (_screen.Left + _screen.Width))
            {
                m_resizableLeft = true;
                _location.X = (_screen.Left + _screen.Width) - Size.Width;
            }

            if (_location.Y + Size.Height > (_screen.Top + _screen.Height))
            {
                m_resizableTop = true;
                _location.Y -= Size.Height + area.Height;
            }

            Show(_location, ToolStripDropDownDirection.BelowRight);
        }

        public void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            SetOwnerItem(control);

            m_resizableTop = m_resizableLeft = false;
            Point _location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
            Rectangle _screen = Screen.FromControl(control).WorkingArea;
           
            if (_location.X + Size.Width > (_screen.Left + _screen.Width))
            {
                m_resizableLeft = true;
                _location.X = (_screen.Left + _screen.Width) - Size.Width;
            }

            if (_location.Y + Size.Height > (_screen.Top + _screen.Height))
            {
                m_resizableTop = true;
                _location.Y -= Size.Height + area.Height;
            }

            _location = control.PointToClient(_location);
            Show(control, _location, ToolStripDropDownDirection.BelowRight);
        }

        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control is Popup)
            {
                Popup _popupControl = control as Popup;
                m_ownerPopup = _popupControl;
                m_ownerPopup.m_childPopup = this;
                OwnerItem = _popupControl.Items[0];
                return;
            }
            else if (m_opener == null)
            {
                m_opener = control;
            }

            if (control.Parent != null)
            {
                SetOwnerItem(control.Parent);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (Content != null)
            {
                Content.MinimumSize = Size;
                Content.MaximumSize = Size;
                Content.Size = Size;
                Content.Location = Point.Empty;
            }

            base.OnSizeChanged(e);
        }

        
        protected override void OnLayout(LayoutEventArgs e)
        {
            if (!NativeMethods.IsRunningOnMono)
            {
                base.OnLayout(e);
                return;
            }

            Size _suggestedSize = GetPreferredSize(Size.Empty);
            
            if (AutoSize && _suggestedSize != Size)
            {
                Size = _suggestedSize;
            }

            SetDisplayedItems();
            OnLayoutCompleted(EventArgs.Empty);
            Invalidate();
        }

        
        protected override void OnOpening(CancelEventArgs e)
        {
            if (Content.IsDisposed || Content.Disposing)
            {
                e.Cancel = true;
                return;
            }

            UpdateRegion();
            base.OnOpening(e);
        }

        protected override void OnOpened(EventArgs e)
        {
            if (m_ownerPopup != null)
            {
                m_ownerPopup.m_isChildPopupOpened = true;
            }

            if (FocusOnOpen)
            {
                Content.Focus();
            }

            base.OnOpened(e);
        }

        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            m_opener = null;

            if (m_ownerPopup != null)
            {
                m_ownerPopup.m_isChildPopupOpened = false;
            }

            base.OnClosed(e);
        }

        #endregion

        #region " Resizing Support "

        private VisualStyleRenderer _sizeGripRenderer;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (InternalProcessResizing(ref m, false))
            {
                return;
            }

            base.WndProc(ref m);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ProcessResizing(ref Message m)
        {
            return InternalProcessResizing(ref m, true);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m, bool contentControl)
        {
            if (m.Msg == NativeMethods.WM_NCACTIVATE && m.WParam != IntPtr.Zero && m_childPopup != null && m_childPopup.Visible)
            {
                m_childPopup.Hide();
            }

            if (!Resizable && !NonInteractive)
            {
                return false;
            }

            if (m.Msg == NativeMethods.WM_NCHITTEST)
            {
                return OnNcHitTest(ref m, contentControl);
            }

            else if (m.Msg == NativeMethods.WM_GETMINMAXINFO)
            {
                return OnGetMinMaxInfo(ref m);
            }

            return false;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            NativeMethods.MINMAXINFO _minmax = (NativeMethods.MINMAXINFO) Marshal.PtrToStructure(m.LParam, typeof (NativeMethods.MINMAXINFO));
            
            if (!MaximumSize.IsEmpty)
            {
                _minmax.maxTrackSize = MaximumSize;
            }

            _minmax.minTrackSize = MinimumSize;
            Marshal.StructureToPtr(_minmax, m.LParam, false);
            return true;
        }

        private bool OnNcHitTest(ref Message m, bool contentControl)
        {
            if (NonInteractive)
            {
                m.Result = (IntPtr) NativeMethods.HTTRANSPARENT;
                return true;
            }

            int _x = Cursor.Position.X; // NativeMethods.LOWORD(m.LParam);
            int _y = Cursor.Position.Y; // NativeMethods.HIWORD(m.LParam);
            Point _clientLocation = PointToClient(new Point(_x, _y));

            GripBounds _gripBouns = new GripBounds(contentControl ? Content.ClientRectangle : ClientRectangle);
            IntPtr _transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

            if (m_resizableTop)
            {
                if (m_resizableLeft && _gripBouns.TopLeft.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTTOPLEFT;
                    return true;
                }

                if (!m_resizableLeft && _gripBouns.TopRight.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTTOPRIGHT;
                    return true;
                }

                if (_gripBouns.Top.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTTOP;
                    return true;
                }
            }
            else
            {
                if (m_resizableLeft && _gripBouns.BottomLeft.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTBOTTOMLEFT;
                    return true;
                }

                if (!m_resizableLeft && _gripBouns.BottomRight.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTBOTTOMRIGHT;
                    return true;
                }

                if (_gripBouns.Bottom.Contains(_clientLocation))
                {
                    m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTBOTTOM;
                    return true;
                }
            }

            if (m_resizableLeft && _gripBouns.Left.Contains(_clientLocation))
            {
                m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTLEFT;
                return true;
            }

            if (!m_resizableLeft && _gripBouns.Right.Contains(_clientLocation))
            {
                m.Result = contentControl ? _transparent : (IntPtr) NativeMethods.HTRIGHT;
                return true;
            }

            return false;
        }

        public void PaintSizeGrip(PaintEventArgs e)
        {
            if (e == null || e.Graphics == null || !m_resizable)
            {
                return;
            }

            Size _clientSize = Content.ClientSize;

            using (Bitmap _gripImage = new Bitmap(0x10, 0x10))
            {
                using (Graphics g = Graphics.FromImage(_gripImage))
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        if (_sizeGripRenderer == null)
                        {
                            _sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                        }

                        _sizeGripRenderer.DrawBackground(g, new Rectangle(0, 0, 0x10, 0x10));
                    }
                    else
                    {
                        System.Windows.Forms.ControlPaint.DrawSizeGrip(g, Content.BackColor, 0, 0, 0x10, 0x10);
                    }
                }

                GraphicsState _gs = e.Graphics.Save();
                e.Graphics.ResetTransform();

                if (m_resizableTop)
                {
                    if (m_resizableLeft)
                    {
                        e.Graphics.RotateTransform(180);
                        e.Graphics.TranslateTransform(-_clientSize.Width, -_clientSize.Height);
                    }
                    else
                    {
                        e.Graphics.ScaleTransform(1, -1);
                        e.Graphics.TranslateTransform(0, -_clientSize.Height);
                    }
                }
                else if (m_resizableLeft)
                {
                    e.Graphics.ScaleTransform(-1, 1);
                    e.Graphics.TranslateTransform(-_clientSize.Width, 0);
                }

                e.Graphics.DrawImage(_gripImage, _clientSize.Width - 0x10, _clientSize.Height - 0x10 + 1, 0x10, 0x10);
                e.Graphics.Restore(_gs);
            }
        }

        #endregion
    }
}