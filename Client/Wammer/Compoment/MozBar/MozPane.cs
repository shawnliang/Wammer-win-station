
#region

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace Waveface.Component.MozBar
{
    #region Enumerations

    public enum MozPaneStyle
    {
        Vertical = 0,
        Horizontal = 1
    }

    public enum MozSelectButton
    {
        Any = 0,
        Left = 1,
        Middle = 2,
        Right = 3
    }

    public enum MozItemColor
    {
        Background = 0,
        Border = 1,
        Text = 2,
        FocusBackground = 3,
        FocusBorder = 4,
        SelectedBackground = 5,
        SelectedBorder = 6,
        Divider = 7,
        SelectedText = 8,
        FocusText = 9
    }

    #endregion

    #region Delegates

    public delegate void ItemColorChangedEventHandler(object sender, ItemColorChangedEventArgs e);

    public delegate void ItemBorderStyleChangedEventHandler(object sender, ItemBorderStyleChangedEventArgs e);

    #endregion

    #region MozPane

    [DesignerAttribute(typeof (MozPaneDesigner))]
    [ToolboxItem(true)]
    [DefaultEvent("ItemClick")]
    public class MozPane : ScrollableControlWithScrollEvents, ISupportInitialize
    {
        #region Win32 API functions

        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        #endregion

        #region Win32 API constants

        private const int WM_THEMECHANGED = 0x031A;
        private const int WM_KEYDOWN = 0x100;

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_CTL = 2;
        private const int SB_BOTH = 3;

        private const int WM_NCCALCSIZE = 0x83;

        private const int WS_HSCROLL = 0x100000;
        private const int WS_VSCROLL = 0x200000;
        private const int GWL_STYLE = (-16);

        #endregion

        #region EventHandlers

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that an item color has changed.")]
        public event ItemColorChangedEventHandler ItemColorChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that an item borderstyle has changed.")]
        public event ItemBorderStyleChangedEventHandler ItemBorderStyleChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the border color was changed.")]
        public event EventHandler BorderColorChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the Theme setting was changed.")]
        public event EventHandler UseThemeChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the select button was changed.")]
        public event EventHandler SelectButtonChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the imagelist has has been changed.")]
        public event EventHandler ImageListChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the style has been changed.")]
        public event EventHandler PaneStyleChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the toggle property has been changed.")]
        public event EventHandler ToggleChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates the max allowed number of selected items has changed.")]
        public event EventHandler MaxSelectedItemsChanged;

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Indicates that the padding has has been changed.")]
        public event EventHandler PaddingChanged;

        [Category("Panel")]
        [Description("Indicates that an item was added to the panel.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemAdded;

        [Category("Panel")]
        [Description("Indicates that an item was removed from the panel.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemRemoved;

        [Category("Action")]
        [Description("Indicates that an item was selected.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemSelected;

        [Category("Action")]
        [Description("Indicates that an item was unselected.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemDeselected;

        [Category("Action")]
        [Description("Indicates that an item lost focus.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemLostFocus;

        [Category("Action")]
        [Description("Indicates that an item recieved focus.")]
        [Browsable(true)]
        public event MozItemEventHandler ItemGotFocus;

        [Category("Action")]
        [Description("Indicates that an item has been double clicked.")]
        [Browsable(true)]
        public event MozItemClickEventHandler ItemDoubleClick;

        [Category("Action")]
        [Description("Indicates that an item has been clicked.")]
        [Browsable(true)]
        public event MozItemClickEventHandler ItemClick;

        #endregion

        #region private class members

        private int beginUpdateCount;
        private Container components;
        internal bool deserializing;
        internal bool initialising;
        private bool layout;

        private Color m_borderColor;
        private ButtonBorderStyle m_borderStyle;
        private BorderStyleCollection m_borderStyleCollection;
        private ColorCollection m_colorCollection;
        private ImageList m_imageList;
        private int m_maxSelectedItems;
        private MozItem m_mouseOverItem;
        private MozItemCollection m_mozItemCollection;
        private PaddingCollection m_padding;
        private MozSelectButton m_selectButton;
        private int m_selectedItems;
        //private ThemeManager m_themeManager;

        private MozPaneStyle m_style;
        private int m_tabIndex = -1;
        private IntPtr m_theme;
        private bool m_toggle;
        private bool m_useTheme;

        #endregion

        #region Constructor

        public MozPane()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO_ remove the following lines after you know the resource names

            // This call is required by the Windows.Forms Form Designer.
            components = new Container();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ContainerControl, true);

            m_mozItemCollection = new MozItemCollection(this);

            // Enable Autoscroll
            //this.AutoScroll = true;

            m_padding = new PaddingCollection(this);
            m_colorCollection = new ColorCollection(this);
            m_borderStyleCollection = new BorderStyleCollection(this);
            //m_themeManager = new ThemeManager(); 
            m_selectButton = MozSelectButton.Left;
            m_style = MozPaneStyle.Vertical;
            m_toggle = false;
            m_maxSelectedItems = 1;
            m_selectedItems = 0;

            m_useTheme = false;
            m_theme = IntPtr.Zero;

            // Listen for changes to the parent
            ParentChanged += OnParentChanged;


            beginUpdateCount = 0;

            deserializing = false;
            initialising = false;

            m_borderColor = Color.FromArgb(127, 157, 185);
            BackColor = Color.White;
            m_borderStyle = ButtonBorderStyle.Solid;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                m_colorCollection.Dispose();
                m_borderStyleCollection.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region properties

        [DefaultValue(typeof (Color), "White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Category("Behavior")]
        [Description("Mouse button used to select items.")]
        [DefaultValue(typeof (MozSelectButton), "Left")]
        public MozSelectButton SelectButton
        {
            get
            {
                return m_selectButton;
            }
            set
            {
                if (m_selectButton != value)
                {
                    m_selectButton = value;
                    if (SelectButtonChanged != null)
                        SelectButtonChanged(this, new EventArgs());
                }
            }
        }

        [Category("Behavior")]
        [Description("Indicates wether the control should use the current theme.")]
        [DefaultValue(false)]
        public bool Theme
        {
            get
            {
                return m_useTheme;
            }
            set
            {
                if (m_useTheme != value)
                {
                    m_useTheme = value;
                    if (UseThemeChanged != null)
                        UseThemeChanged(this, new EventArgs());
                    if (m_useTheme)
                        GetThemeColors();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Indicates the possibility to toggle items i.e. unselect selected items.")]
        [DefaultValue(false)]
        public bool Toggle
        {
            get
            {
                return m_toggle;
            }
            set
            {
                // Check if new value differs from old one
                if (value != m_toggle)
                {
                    m_toggle = value;
                    if (value == false)
                        MaxSelectedItems = 1;
                    if (ToggleChanged != null)
                        ToggleChanged(this, new EventArgs());
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Max number of selected items.")]
        [DefaultValue(1)]
        public int MaxSelectedItems
        {
            get
            {
                return m_maxSelectedItems;
            }
            set
            {
                if (value != m_maxSelectedItems)
                {
                    if (value < 1)
                        value = 1;
                    if (value > Items.Count)
                        value = Items.Count;

                    m_maxSelectedItems = value;
                    if (MaxSelectedItemsChanged != null)
                        MaxSelectedItemsChanged(this, new EventArgs());
                }
            }
        }

        // Returns number of selected item in the panel
        [Browsable(false)]
        public int SelectedItems
        {
            get
            {
                return m_selectedItems;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Colors for various states.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ColorCollection ItemColors
        {
            get
            {
                return m_colorCollection;
            }
            set
            {
                if (value != null)
                    m_colorCollection = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Various border styles.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BorderStyleCollection ItemBorderStyles
        {
            get
            {
                return m_borderStyleCollection;
            }
            set
            {
                if ((value != null) && (value != m_borderStyleCollection))
                    m_borderStyleCollection = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Padding (Horizontal, Vertical)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PaddingCollection Padding
        {
            get
            {
                return m_padding;
            }
            set
            {
                if (value != m_padding)
                {
                    if (value != null)
                        m_padding = value;
                    DoLayout();
                    Invalidate();
                    if (PaddingChanged != null)
                        PaddingChanged(this, new EventArgs());
                }
            }
        }


        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (MozPaneStyle), "Vertical")]
        [Description("Pane style.")]
        public MozPaneStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                if (m_style != value)
                {
                    m_style = value;

                    // for each control in our list...
                    for (int i = 0; i < Items.Count; i++)
                    {
                        // force layout change
                        Items[i].DoLayout();
                        Invalidate();
                    }
                    DoLayout();
                    Invalidate();
                    if (PaneStyleChanged != null)
                        PaneStyleChanged(this, new EventArgs());
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The imagelist that contains images used by the MozBar.")]
        public ImageList ImageList
        {
            get
            {
                return m_imageList;
            }
            set
            {
                if (m_imageList != value)
                {
                    m_imageList = value;
                    // for each control in our list...
                    for (int i = 0; i < Items.Count; i++)
                    {
                        // reset all images
                        Items[i].Images.NormalImage = null;
                        Items[i].Images.FocusImage = null;
                        Items[i].Images.SelectedImage = null;
                        //redo layout (size might have changed..)
                        Items[i].DoLayout();
                        Invalidate();
                    }

                    if (ImageListChanged != null)
                        ImageListChanged(this, new EventArgs());
                    DoLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (ButtonBorderStyle), "Solid")]
        [Description("Border color for panel.")]
        public ButtonBorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }
            set
            {
                if (m_borderStyle != value)
                {
                    m_borderStyle = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (Color), "127,157,185")]
        [Description("Border color for panel.")]
        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                if (value != m_borderColor)
                {
                    m_borderColor = value;
                    if (BorderColorChanged != null)
                        BorderColorChanged(this, new EventArgs());
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(null)]
        [Description("The Items contained in the Panel."),]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof (MozItemCollectionEditor), typeof (UITypeEditor))]
        public MozItemCollection Items
        {
            get
            {
                return m_mozItemCollection;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get
            {
                return base.Controls;
            }
        }

        // obsolete properties

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ObsoleteAttribute("This property is not supported", true)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ObsoleteAttribute("This property is not supported", true)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ObsoleteAttribute("This property is not supported", true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ObsoleteAttribute("This property is not supported", true)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ObsoleteAttribute("This property is not supported", true)]
        public override bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = true;
            }
        }

        #endregion

        #region Events

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (m_tabIndex == -1)
                m_tabIndex = 0;
            RemoveFocus();
            if (Items.Count >= 1)
                if (Items[m_tabIndex].state != MozItemState.Selected)
                {
                    Items[m_tabIndex].state = MozItemState.Focus;
                    ScrollControlIntoView(Items[m_tabIndex]);
                }
        }


        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            RemoveFocus();
            m_tabIndex = -1;
        }

        //protected override onp

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Enter: // Enter
                case Keys.Space:
                    {
                        if (m_tabIndex != -1)
                            SelectItem(m_tabIndex);
                        break;
                    }
                case Keys.Down:
                case Keys.Right:
                case Keys.Tab:
                    {
                        // Move tabindex one step forward.
                        m_tabIndex++;
                        if ((m_tabIndex < Items.Count) && (m_tabIndex > 0))
                        {
                            RemoveFocus();
                            if (Items[m_tabIndex].state != MozItemState.Selected)
                                Items[m_tabIndex].state = MozItemState.Focus;
                            ScrollControlIntoView(Items[m_tabIndex]);
                        }
                        else
                            SelectNextControl(this, true, true, true, true);

                        break;
                    }
                case Keys.Up:
                case Keys.Left:
                    {
                        // Move tabindex one step backward
                        m_tabIndex--;
                        if ((m_tabIndex >= 0) && (m_tabIndex < Items.Count))
                        {
                            RemoveFocus();
                            if (Items[m_tabIndex].state != MozItemState.Selected)
                                Items[m_tabIndex].state = MozItemState.Focus;
                            ScrollControlIntoView(Items[m_tabIndex]);
                        }
                        else
                            SelectNextControl(this, false, true, true, true);

                        break;
                    }
            }
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            // Check if message is KEY_DOWN
            if (msg.Msg == WM_KEYDOWN)
            {
                Keys keyData = ((Keys) (int) msg.WParam) | ModifierKeys;
                Keys keyCode = ((Keys) (int) msg.WParam);
                // Make sure we handle certain keys
                switch (keyCode)
                {
                        // Keys used to move forward i list
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Tab:
                        {
                            // If not at the end handle message
                            if (m_tabIndex < Items.Count - 1)
                                return false;
                            //Cant go any further backwards , do not handle message;
                            m_tabIndex = -1;
                            break;
                        }
                        // Keys used to move backwards in list
                    case Keys.Up:
                    case Keys.Left:
                        {
                            // If not at the end handle message
                            if (m_tabIndex > 0)
                                return false;
                            //Cant go any further foreward , do not handle message;
                            m_tabIndex = -1;
                            break;
                        }
                    default:
                        break;
                }
            }

            return base.PreProcessMessage(ref msg);
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            if (Theme)
                GetThemeColors();
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_THEMECHANGED:
                    {
                        // Theme has changed , get new colors if Theme = true
                        if (Theme)
                            GetThemeColors();
                        break;
                    }
                case WM_NCCALCSIZE:
                    {
                        if (Style == MozPaneStyle.Vertical)
                        {
                            // hide horizontal scrollbar
                            ShowScrollBar(m.HWnd, SB_HORZ, 0);
                        }
                        else
                        {
                            // hide vertical scrollbar
                            ShowScrollBar(m.HWnd, SB_VERT, 0);
                        }

                        break;
                    }
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            //this.AutoScroll = false; 
            //this.AutoScroll = true;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            ButtonBorderStyle leftStyle = m_borderStyle;
            ButtonBorderStyle topStyle = m_borderStyle;
            ButtonBorderStyle rightStyle = m_borderStyle;
            ButtonBorderStyle bottomStyle = m_borderStyle;

            base.OnPaint(e);

            Rectangle borderRect = new Rectangle();

            borderRect = DisplayRectangle;
            Brush bgBrush = new SolidBrush(BackColor);

            // Draw background and border

            e.Graphics.FillRectangle(bgBrush, DisplayRectangle);
            if (Style == MozPaneStyle.Vertical)
            {
                if (IsVerticalScrollBarVisible())
                {
                    topStyle = ButtonBorderStyle.None;
                    bottomStyle = ButtonBorderStyle.None;
                }
            }
            else
            {
                if (IsHorizontalScrollBarVisible())
                {
                    leftStyle = ButtonBorderStyle.None;
                    rightStyle = ButtonBorderStyle.None;
                }
            }
            System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, borderRect,
                                    m_borderColor, 1, leftStyle, // left
                                    m_borderColor, 1, topStyle, // top
                                    m_borderColor, 1, rightStyle, // right
                                    m_borderColor, 1, bottomStyle); //bottom

            // clean up
            bgBrush.Dispose();
        }

        private void OnParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.VisibleChanged += OnParentVisibleChanged;
            }
        }

        private void OnParentVisibleChanged(object sender, EventArgs e)
        {
            if (sender != Parent)
            {
                ((Control) sender).VisibleChanged -= OnParentVisibleChanged;

                return;
            }

            if (Parent.Visible)
            {
                DoLayout();
            }
        }


        protected override void OnResize(EventArgs e)
        {
            // size has changed , force layout change

            DoLayout();
            Invalidate(true);
        }


        private void MozItem_GotFocus(object sender, MozItemEventArgs e)
        {
            //Check if item is selected
            if (e.MozItem.state != MozItemState.Selected)
            {
                // if not set its state to focus
                e.MozItem.state = MozItemState.Focus;
                m_mouseOverItem = e.MozItem;
                if (ItemGotFocus != null)
                    ItemGotFocus(this, e);
            }
        }

        private void MozItem_LostFocus(object sender, MozItemEventArgs e)
        {
            // check if item is selected
            if (e.MozItem.state != MozItemState.Selected)
            {
                // if not set its state to normal
                e.MozItem.state = MozItemState.Normal;
                m_mouseOverItem = null;
                if (ItemLostFocus != null)
                    ItemLostFocus(this, e);
            }
        }

        private void MozItem_Click(object sender, MozItemClickEventArgs e)
        {
            // Fire click event and then ...
            if (ItemClick != null)
                ItemClick(this, e);
            // Try to select item if the proper button was used.
            if ((e.Button.ToString() == SelectButton.ToString()) ||
                (SelectButton == MozSelectButton.Any))
            {
                m_tabIndex = Items.IndexOf(e.MozItem);
                Focus();
                SelectItem(Items.IndexOf(e.MozItem));
            }
        }

        private void MozItem_DoubleClick(object sender, MozItemClickEventArgs e)
        {
            if (ItemDoubleClick != null)
                ItemDoubleClick(this, e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            // check if control is a MozItem	
            if ((e.Control as MozItem) == null)
            {
                // If not remove and...
                Controls.Remove(e.Control);
                // throw exception
                throw new InvalidCastException("Only MozItem's can be added to the MozPane");
            }

            base.OnControlAdded(e);

            // Check if item exists in collection
            if (!Items.Contains((MozItem) e.Control))
            {
                // if not add it
                Items.Add((MozItem) e.Control);
            }

            // Refresh
            Invalidate();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            // Check if item is in collection
            if (Items.Contains(e.Control))
            {
                // If it is , remove it
                Items.Remove((MozItem) e.Control);
            }

            // Refresh
            Invalidate();
        }

        protected virtual void OnMozItemAdded(MozItemEventArgs e)
        {
            if (!Controls.Contains(e.MozItem))
            {
                Controls.Add(e.MozItem);
            }

            // tell the MozItem who's its daddy...
            e.MozItem.MozPane = this;

            // listen for events
            e.MozItem.ItemGotFocus += MozItem_GotFocus;
            e.MozItem.ItemLostFocus += MozItem_LostFocus;
            e.MozItem.ItemClick += MozItem_Click;
            e.MozItem.ItemDoubleClick += MozItem_DoubleClick;

            // update the layout of the controls

            DoLayout();

            if (ItemAdded != null)
            {
                ItemAdded(this, e);
            }
        }


        protected virtual void OnMozItemRemoved(MozItemEventArgs e)
        {
            if (Controls.Contains(e.MozItem))
            {
                Controls.Remove(e.MozItem);
            }

            // remove event listeners
            e.MozItem.ItemGotFocus -= MozItem_GotFocus;
            e.MozItem.ItemLostFocus -= MozItem_LostFocus;
            e.MozItem.ItemClick -= MozItem_Click;
            e.MozItem.ItemDoubleClick -= MozItem_DoubleClick;


            // update the layout of the controls
            DoLayout();

            if (ItemRemoved != null)
            {
                ItemRemoved(this, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Signals the MozPane that initialization is starting
        /// </summary>
        public void BeginInit()
        {
            initialising = true;
        }


        /// <summary>
        /// Signals the MozPane that initialization is complete
        /// </summary>
        public void EndInit()
        {
            initialising = false;

            //this.DoLayout();
        }

        #region Layout

        public void DoLayout()
        {
            DoLayout(false);
        }

        public void DoLayout(bool performRealLayout)
        {
            if (layout || beginUpdateCount > 0 || deserializing)
            {
                return;
            }

            layout = true;

            SuspendLayout();

            MozItem e;
            Point p;

            switch (m_style)
            {
                case MozPaneStyle.Vertical: // Vertical
                    {
                        // work out how wide to make the controls, and where
                        // the top of the first control should be
                        int y = DisplayRectangle.Y + m_padding.Vertical;
                        int width = ClientRectangle.Width - (2*m_padding.Horizontal);
                        // for each control in our list...
                        for (int i = 0; i < Items.Count; i++)
                        {
                            e = Items[i];
                            // go to the next mozitem if this one is invisible and 
                            // it's parent is visible
                            if (!e.Visible && e.Parent != null && e.Parent.Visible)
                            {
                                continue;
                            }
                            p = new Point(m_padding.Horizontal, y);
                            // set the width and location of the control
                            e.Location = p;
                            e.Width = Width;
                            // update the next starting point
                            y += e.Height + m_padding.Vertical;
                        }
                        break;
                    }
                case MozPaneStyle.Horizontal: // Horizontal
                    {
                        int x = DisplayRectangle.X + m_padding.Horizontal;
                        int height = ClientRectangle.Height - (2*m_padding.Vertical);
                        for (int i = 0; i < Items.Count; i++)
                        {
                            e = Items[i];
                            if (!e.Visible && e.Parent != null && e.Parent.Visible)
                            {
                                continue;
                            }
                            p = new Point(x, m_padding.Vertical);
                            e.Location = p;
                            e.Height = height;
                            x += e.Width + m_padding.Horizontal;
                        }
                        break;
                    }
            }

            // restart the layout engine
            ResumeLayout(performRealLayout);

            layout = false;
        }

        internal bool IsVerticalScrollBarVisible()
        {
            return (GetWindowLong(Handle, GWL_STYLE) & WS_VSCROLL) != 0;
        }

        internal bool IsHorizontalScrollBarVisible()
        {
            return (GetWindowLong(Handle, GWL_STYLE) & WS_HSCROLL) != 0;
        }

        internal void UpdateMozItems()
        {
            if (Items.Count == Controls.Count)
            {
                MatchControlCollToMozItemsColl();

                return;
            }

            if (Items.Count > Controls.Count)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (!Controls.Contains(Items[i]))
                    {
                        OnMozItemAdded(new MozItemEventArgs(Items[i]));
                    }
                }
            }
            else
            {
                int i = 0;
                MozItem mozItem;

                while (i < Controls.Count)
                {
                    mozItem = (MozItem) Controls[i];

                    if (!Items.Contains(mozItem))
                    {
                        OnMozItemRemoved(new MozItemEventArgs(mozItem));
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        internal void MatchControlCollToMozItemsColl()
        {
            SuspendLayout();

            for (int i = 0; i < Items.Count; i++)
                Controls.SetChildIndex(Items[i], i);

            ResumeLayout(false);

            DoLayout(true);

            Invalidate(true);
        }

        #endregion

        private void RemoveFocus()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if ((Items[i].state == MozItemState.Focus) && (Items[i] != m_mouseOverItem))
                {
                    Items[i].state = MozItemState.Normal;
                }
            }
        }

        private void GetThemeColors()
        {
            int EPB_HEADERBACKGROUND = 1;
            int EPB_NORMALGROUPBACKGROUND = 5;

            int TMT_GRADIENTCOLOR1 = 3810;
            int TMT_GRADIENTCOLOR2 = 3811;

            Color selectColor = new Color();
            Color focusColor = new Color();
            Color borderColor = new Color();
            bool useSystemColors = false;

            {
                useSystemColors = true;
            }

            if (useSystemColors)
            {
                // Get System colors
                selectColor = SystemColors.ActiveCaption;
                focusColor = System.Windows.Forms.ControlPaint.Light(selectColor);
                borderColor = SystemColors.ActiveBorder;
            }

            // apply colors..
            ItemColors.SelectedBorder = System.Windows.Forms.ControlPaint.Light(System.Windows.Forms.ControlPaint.Dark(selectColor));
            BorderColor = borderColor;
            ItemColors.Divider = borderColor;

            ItemColors.SelectedBackground = selectColor;
            ItemColors.FocusBackground = focusColor;
            ItemColors.FocusBorder = selectColor;

            Invalidate();
        }

        public void SelectItem(int index)
        {
            // Check if index is valid
            if (index >= 0 && index <= Items.Count - 1)
            {
                // Check if item is selected
                if (Items[index].state != MozItemState.Selected)
                {
                    // Is it a divider ?
                    if (Items[index].ItemStyle != MozItemStyle.Divider)
                    {
                        // Check if toggle is enabled
                        if (!Toggle)
                        {
                            // for each control in our list...
                            for (int i = 0; i < Items.Count; i++)
                            {
                                // set all items to not selected
                                if ((Items[i] != m_mouseOverItem) || (Items[i].state == MozItemState.Selected))
                                    Items[i].state = MozItemState.Normal;
                            }
                            // No item is selected
                            m_selectedItems = 0;
                        }
                        // Check that the allowed number of selected items isnt exceeded
                        if (m_maxSelectedItems >= m_selectedItems + 1)
                        {
                            // set our item to selected
                            Items[index].state = MozItemState.Selected;
                            m_selectedItems++;
                            // Scroll selected item into view
                            ScrollControlIntoView(Items[index]);
                            if (ItemSelected != null)
                                ItemSelected(this, new MozItemEventArgs(Items[index]));
                        }
                    }
                }
                else
                {
                    if (Toggle)
                    {
                        //unselect selected item by setting its state to Focus
                        Items[index].state = MozItemState.Focus;
                        m_selectedItems--;
                        if (ItemDeselected != null)
                            ItemDeselected(this, new MozItemEventArgs(Items[index]));
                    }
                }
            }
        }

        public void SelectItem(string tag)
        {
            // loop through item collection
            for (int i = 0; i < Items.Count; i++)
            {
                // if matching tag is found try to select item
                if (Items[i].Tag.ToString() == tag)
                    SelectItem(i);
            }
        }

        public void SelectByName(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                    SelectItem(i);
            }
        }

        public void SelectByText(string text)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Text == text)
                    SelectItem(i);
            }
        }

        #endregion

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        #region MozItemsCollection

        /// <summary>
        /// Represents a collection of MozItem objects
        /// </summary>
        public class MozItemCollection : CollectionBase
        {
            #region Class Data

            /// <summary>
            /// The MozPane that owns this MozItemsCollection
            /// </summary>
            private MozPane owner;

            #endregion

            #region Constructor

            public MozItemCollection(MozPane owner)
            {
                if (owner == null)
                {
                    throw new ArgumentNullException("owner");
                }

                this.owner = owner;
            }

            public MozItemCollection(MozPane owner, MozItemCollection mozItems) : this(owner)
            {
                Add(mozItems);
            }

            #endregion

            #region Methods

            public void Add(MozItem value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                List.Add(value);

                if (owner != null && !owner.deserializing)
                {
                    owner.Controls.Add(value);

                    owner.OnMozItemAdded(new MozItemEventArgs(value));
                }
            }

            public void AddRange(MozItem[] mozItems)
            {
                if (mozItems == null)
                {
                    throw new ArgumentNullException("mozItems");
                }

                for (int i = 0; i < mozItems.Length; i++)
                    Add(mozItems[i]);
            }

            public void Add(MozItemCollection mozItems)
            {
                if (mozItems == null)
                {
                    throw new ArgumentNullException("mozItems");
                }

                for (int i = 0; i < mozItems.Count; i++)
                    Add(mozItems[i]);
            }


            public new void Clear()
            {
                while (Count > 0)
                    RemoveAt(0);
            }


            public bool Contains(MozItem mozItem)
            {
                if (mozItem == null)
                {
                    throw new ArgumentNullException("mozItem");
                }

                return (IndexOf(mozItem) != -1);
            }

            public bool Contains(Control control)
            {
                if (!(control is MozItem))
                {
                    return false;
                }

                return Contains((MozItem) control);
            }

            public int IndexOf(MozItem mozItem)
            {
                if (mozItem == null)
                {
                    throw new ArgumentNullException("mozItem");
                }

                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == mozItem)
                    {
                        return i;
                    }
                }

                return -1;
            }

            public void Remove(MozItem value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                List.Remove(value);

                if (owner != null && !owner.deserializing)
                {
                    owner.Controls.Remove(value);

                    owner.OnMozItemRemoved(new MozItemEventArgs(value));
                }
            }

            public new void RemoveAt(int index)
            {
                Remove(this[index]);
            }


            public void Move(MozItem value, int index)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (index < 0)
                {
                    index = 0;
                }
                else if (index > Count)
                {
                    index = Count;
                }

                if (!Contains(value) || IndexOf(value) == index)
                {
                    return;
                }

                List.Remove(value);

                if (index > Count)
                {
                    List.Add(value);
                }
                else
                {
                    List.Insert(index, value);
                }

                if (owner != null && !owner.deserializing)
                {
                    owner.MatchControlCollToMozItemsColl();
                }
            }

            public void MoveToTop(MozItem value)
            {
                Move(value, 0);
            }


            public void MoveToBottom(MozItem value)
            {
                Move(value, Count);
            }

            #endregion

            #region Properties

            public virtual MozItem this[int index]
            {
                get
                {
                    return List[index] as MozItem;
                }
            }

            #endregion
        }

        #endregion

        #region MozItemCollectionEditor

        /// <summary>
        /// A custom CollectionEditor for editing MozItemCollections
        /// </summary>
        internal class MozItemCollectionEditor : CollectionEditor
        {
            public MozItemCollectionEditor(Type type) : base(type)
            {
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider isp, object value)
            {
                MozPane originalControl = (MozPane) context.Instance;

                object returnObject = base.EditValue(context, isp, value);

                originalControl.UpdateMozItems();

                return returnObject;
            }


            protected override object CreateInstance(Type itemType)
            {
                object mozItem = base.CreateInstance(itemType);

                ((MozItem) mozItem).Name = "MozItem";

                return mozItem;
            }
        }

        #endregion

        #region  PaddingCollection

        [TypeConverter(typeof (PaddingCollectionTypeConverter))]
        public class PaddingCollection
        {
            private int m_horizontal;
            private MozPane m_pane;
            private int m_vertical;

            public PaddingCollection(MozPane pane)
            {
                // set the control to which the collection belong
                m_pane = pane;
                // Default values
                m_horizontal = 2;
                m_vertical = 2;
            }

            [RefreshProperties(RefreshProperties.All)]
            [Description("Horizontal padding.")]
            [DefaultValue(2)]
            public int Horizontal
            {
                get
                {
                    return m_horizontal;
                }
                set
                {
                    if (m_horizontal != value)
                    {
                        m_horizontal = value;
                        if (m_pane != null)
                        {
                            // padding has changed , force DoLayout
                            m_pane.DoLayout();
                            m_pane.Invalidate();
                            if (m_pane.PaddingChanged != null)
                                m_pane.PaddingChanged(this, new EventArgs());
                        }
                    }
                }
            }

            [RefreshProperties(RefreshProperties.All)]
            [Description("Vertical padding.")]
            [DefaultValue(2)]
            public int Vertical
            {
                get
                {
                    return m_vertical;
                }
                set
                {
                    if (m_vertical != value)
                    {
                        m_vertical = value;
                        if (m_pane != null)
                        {
                            m_pane.DoLayout();
                            m_pane.Invalidate();
                            if (m_pane.PaddingChanged != null)
                                m_pane.PaddingChanged(this, new EventArgs());
                        }
                    }
                }
            }
        }

        #endregion

        #region PaddingCollectionTypeConverter

        public class PaddingCollectionTypeConverter : ExpandableObjectConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof (string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof (string))
                    return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value.GetType() == typeof (string))
                {
                    // Parse property string
                    string[] ss = value.ToString().Split(new[]
                                                             {
                                                                 ';'
                                                             }, 2);
                    if (ss.Length == 2)
                    {
                        // Create new PaddingCollection
                        PaddingCollection item = new PaddingCollection((MozPane) context.Instance);
                        // Set properties
                        item.Horizontal = int.Parse(ss[0]);
                        item.Vertical = int.Parse(ss[1]);
                        return item;
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof (string) && (value is PaddingCollection))
                {
                    // cast value to paddingCollection
                    PaddingCollection dest = (PaddingCollection) value;
                    // create property string
                    return dest.Horizontal.ToString() + "; " + dest.Vertical.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion

        #region  ColorCollection

        [TypeConverter(typeof (ItemCollectionTypeConverter))]
        public class ColorCollection
        {
            private Color m_back;
            private Color m_border;
            private Color m_divider;
            private Color m_focus;
            private Color m_focusBorder;
            private Color m_focusText;

            public MozPane m_pane;
            private Color m_selected;
            private Color m_selectedBorder;
            private Color m_selectedText;
            private Color m_text;

            public ColorCollection(MozPane pane)
            {
                m_pane = pane;
                // Default values
                m_selected = Color.FromArgb(193, 210, 238);
                m_selectedBorder = Color.FromArgb(49, 106, 197);
                m_focus = Color.FromArgb(224, 232, 246);
                m_focusBorder = Color.FromArgb(152, 180, 226);
                m_back = Color.White;
                m_border = Color.Black;
                m_text = Color.Black;
                m_selectedText = Color.Black;
                m_focusText = Color.Black;
                m_divider = Color.FromArgb(127, 157, 185);
            }

            [Description("Color used for item text.")]
            [DefaultValue(typeof (Color), "Black")]
            public Color Text
            {
                get
                {
                    return m_text;
                }
                set
                {
                    if (m_text != value)
                    {
                        m_text = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.Text));
                    }
                }
            }

            [Description("Text color when item is selected.")]
            [DefaultValue(typeof (Color), "Black")]
            public Color SelectedText
            {
                get
                {
                    return m_selectedText;
                }
                set
                {
                    if (m_selectedText != value)
                    {
                        m_selectedText = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.SelectedText));
                    }
                }
            }

            [Description("Text color when item has focus.")]
            [DefaultValue(typeof (Color), "Black")]
            public Color FocusText
            {
                get
                {
                    return m_focusText;
                }
                set
                {
                    if (m_focusText != value)
                    {
                        m_focusText = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.FocusText));
                    }
                }
            }


            [Description("Background color when item is not selected or has focus.")]
            [DefaultValue(typeof (Color), "White")]
            public Color Background
            {
                get
                {
                    return m_back;
                }
                set
                {
                    if (m_back != value)
                    {
                        m_back = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.Background));
                    }
                }
            }

            [Description("Border color when item is not selected or has focus.")]
            [DefaultValue(typeof (Color), "Black")]
            public Color Border
            {
                get
                {
                    return m_border;
                }
                set
                {
                    if (m_border != value)
                    {
                        m_border = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.Border));
                    }
                }
            }

            [Description("Color used when item style is set to divider.")]
            [DefaultValue(typeof (Color), "127,157,185")]
            public Color Divider
            {
                get
                {
                    return m_divider;
                }
                set
                {
                    if (m_divider != value)
                    {
                        m_divider = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.Divider));
                    }
                }
            }

            [Description("Background color when item is selected.")]
            [DefaultValue(typeof (Color), "193,210,238")]
            public Color SelectedBackground
            {
                get
                {
                    return m_selected;
                }
                set
                {
                    if (m_selected != value)
                    {
                        m_selected = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.SelectedBackground));
                    }
                }
            }

            [Description("Border color when item is selected.")]
            [DefaultValue(typeof (Color), "49,106,197")]
            public Color SelectedBorder
            {
                get
                {
                    return m_selectedBorder;
                }
                set
                {
                    if (m_selectedBorder != value)
                    {
                        m_selectedBorder = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.SelectedBorder));
                    }
                }
            }

            [Description("Background color when item has focus.")]
            [DefaultValue(typeof (Color), "224,232,246")]
            public Color FocusBackground
            {
                get
                {
                    return m_focus;
                }
                set
                {
                    if (m_focus != value)
                    {
                        m_focus = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.FocusBackground));
                    }
                }
            }

            [Description("Border color when item has focus.")]
            [DefaultValue(typeof (Color), "152,180,226")]
            public Color FocusBorder
            {
                get
                {
                    return m_focusBorder;
                }
                set
                {
                    if (m_focusBorder != value)
                    {
                        m_focusBorder = value;
                        UpdateItems();
                        if (m_pane.ItemColorChanged != null)
                            m_pane.ItemColorChanged(this, new ItemColorChangedEventArgs(MozItemColor.FocusBorder));
                    }
                }
            }

            public void Dispose()
            {
            }

            private void UpdateItems()
            {
                // for each item contained in the panel
                for (int i = 0; i < m_pane.Items.Count; i++)
                {
                    // Refresh item
                    m_pane.Items[i].Invalidate();
                }
            }
        }

        #endregion

        #region  BorderStyleCollection

        [TypeConverter(typeof (ItemCollectionTypeConverter))]
        public class BorderStyleCollection
        {
            private ButtonBorderStyle m_borderStyle;
            private ButtonBorderStyle m_focusBorderStyle;
            public MozPane m_pane;
            private ButtonBorderStyle m_selectedBorderStyle;


            public BorderStyleCollection(MozPane pane)
            {
                m_pane = pane;
                m_borderStyle = ButtonBorderStyle.None;
                m_focusBorderStyle = ButtonBorderStyle.Solid;
                m_selectedBorderStyle = ButtonBorderStyle.Solid;
            }

            [Description("Border style when item has no focus.")]
            [DefaultValue(typeof (ButtonBorderStyle), "None")]
            public ButtonBorderStyle Normal
            {
                get
                {
                    return m_borderStyle;
                }
                set
                {
                    if (m_borderStyle != value)
                    {
                        m_borderStyle = value;
                        UpdateItems();
                        if (m_pane.ItemBorderStyleChanged != null)
                            m_pane.ItemBorderStyleChanged(this, new ItemBorderStyleChangedEventArgs(MozItemState.Normal));
                    }
                }
            }

            [Description("Border style when item has focus.")]
            [DefaultValue(typeof (ButtonBorderStyle), "Solid")]
            public ButtonBorderStyle Focus
            {
                get
                {
                    return m_focusBorderStyle;
                }
                set
                {
                    if (m_focusBorderStyle != value)
                    {
                        m_focusBorderStyle = value;
                        UpdateItems();
                        if (m_pane.ItemBorderStyleChanged != null)
                            m_pane.ItemBorderStyleChanged(this, new ItemBorderStyleChangedEventArgs(MozItemState.Focus));
                    }
                }
            }

            [Description("Border style when item is selected.")]
            [DefaultValue(typeof (ButtonBorderStyle), "Solid")]
            public ButtonBorderStyle Selected
            {
                get
                {
                    return m_selectedBorderStyle;
                }
                set
                {
                    if (m_selectedBorderStyle != value)
                    {
                        m_selectedBorderStyle = value;
                        UpdateItems();
                        if (m_pane.ItemBorderStyleChanged != null)
                            m_pane.ItemBorderStyleChanged(this, new ItemBorderStyleChangedEventArgs(MozItemState.Selected));
                    }
                }
            }

            public void Dispose()
            {
            }

            private void UpdateItems()
            {
                // for each item contained in the panel
                for (int i = 0; i < m_pane.Items.Count; i++)
                {
                    // Refresh item
                    m_pane.Items[i].Invalidate();
                }
            }
        }

        #endregion
    }

    #endregion

    #region Designer

    // ControlDesigner

    public class MozPaneDesigner : ScrollableControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                // Remove all manual resizing of the control
                SelectionRules selectionRules = base.SelectionRules;
                selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
                return selectionRules;
            }
        }

        public override void OnSetComponentDefaults()
        {
            base.OnSetComponentDefaults();
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // Remove obsolete properties
            properties.Remove("BackgroundImage");
            properties.Remove("ForeColor");
            properties.Remove("Text");
            properties.Remove("RightToLeft");
            properties.Remove("ImeMode");
            properties.Remove("AutoScroll");
        }
    }

    #endregion

    #region ColorChangedEventArgs

    public class ItemColorChangedEventArgs : EventArgs
    {
        #region Class Data

        /// <summary>
        /// The color that has changed
        /// </summary>
        private MozItemColor m_color;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MozItemEventArgs class with default settings
        /// </summary>
        public ItemColorChangedEventArgs()
        {
            m_color = 0;
        }


        public ItemColorChangedEventArgs(MozItemColor color)
        {
            m_color = color;
        }

        #endregion

        #region Properties

        public MozItemColor Color
        {
            get
            {
                return m_color;
            }
        }

        #endregion
    }

    #endregion

    #region ItemBorderStyleChangedEventArgs

    public class ItemBorderStyleChangedEventArgs : EventArgs
    {
        #region Class Data

        /// <summary>
        /// The MozItem that generated the event
        /// </summary>
        private MozItemState m_state;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MozItemEventArgs class with default settings
        /// </summary>
        public ItemBorderStyleChangedEventArgs()
        {
            m_state = 0;
        }


        public ItemBorderStyleChangedEventArgs(MozItemState state)
        {
            m_state = state;
        }

        #endregion

        #region Properties

        public MozItemState State
        {
            get
            {
                return m_state;
            }
        }

        #endregion
    }

    #endregion
}