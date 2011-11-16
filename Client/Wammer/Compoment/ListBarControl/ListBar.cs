#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.ListBarControl
{

    #region ListBar Control class

    /// <summary>
    /// An implementation of a Microsoft Outlook Style ListBar control.
    /// The control provides all the features needed to implement a replica
    /// of the Outlook style control and is also designed to allow the same
    /// functionality to be used in overriden controls in which the
    /// individual sizing and appearance of each of the UI components can be
    /// customised.
    /// 
    /// The <c>ListBar</c> control is modelled as an extension to
    /// the <c>System.Windows.Forms.UserControl</c> class.  Bars
    /// are configured using <see cref="ListBarGroup" /> objects which are
    /// collected in the <see cref="ListBarGroupCollection" /> object
    /// accessible through the control's <see cref="Groups" /> accessor.
    /// Each <see cref="ListBarGroup" /> in turn contains a 
    /// <see cref="ListBarItemCollection" /> of <see cref="ListBarItem" /> objects 
    /// which represent the buttons within a group.
    /// </summary>	
    public class ListBar : UserControl
    {
        #region Member Variables

        /// <summary>
        /// Contains a reference to the active scroll button when one is pressed
        /// and the mouse is over it.
        /// </summary>
        private ListBarScrollButton activeButton;

        /// <summary>
        /// Whether groups can be dragged or not
        /// </summary>
        private bool allowDragGroups = true;

        /// <summary>
        /// Whether items can be dragged or not
        /// </summary>
        private bool allowDragItems = true;

        /// <summary>
        /// Down scroll buttons reference.
        /// </summary>
        protected ListBarScrollButton btnDown;

        /// <summary>
        /// Up scroll button reference.
        /// </summary>
        protected ListBarScrollButton btnUp;

        /// <summary>
        /// A timer for controlling scrolling when the scroll buttons are held
        /// down.
        /// </summary>
        private Timer buttonPressed = new Timer();

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components;

        /// <summary>
        /// The object that was last hovered over during
        /// drag-drop, if any:
        /// </summary>
        private IMouseObject dragHoverOver;

        /// <summary>
        /// The time at which hovering started over the object
        /// which is currently being hovered over:
        /// </summary>
        private DateTime dragHoverOverStartTime = DateTime.Now;

        /// <summary>
        /// During drag-drop, the insert point, if any.
        /// </summary>
        private ListBarDragDropInsertPoint dragInsertPoint;

        /// <summary>
        /// Drawing style fo the control.
        /// </summary>
        private ListBarDrawStyle drawStyle = ListBarDrawStyle.ListBarDrawStyleOfficeXP;

        /// <summary>
        /// The ListBarGroup currently being edited, if any
        /// </summary>
        private ListBarGroup editGroup;

        /// <summary>
        /// The ListBarItem currently being edited, if any
        /// </summary>
        private ListBarItem editItem;

        /// <summary>
        /// Reference to the collection of groups contained within the ListBar control.
        /// </summary>
        private ListBarGroupCollection groups;

        /// <summary>
        /// The index of the group which is currently selected
        /// when scrolling a new group into view:
        /// </summary>
        protected int indexCurrent = -1;

        /// <summary>
        /// The index of the newly selected group which will replace
        /// the selected index when scrolling a new group into view:
        /// </summary>
        protected int indexNew = -1;

        /// <summary>
        /// Reference to an external Image List for drawing the large icon view.
        /// </summary>
        private ImageList largeImageList;

        /// <summary>
        /// Last height the control was drawn at.  Used to control resizing.
        /// </summary>
        private int lastHeight;

        /// <summary>
        /// The last time a scroll occurred during a drag-drop operation.  Used
        /// to control the speed of scrolling during drag-drop.
        /// </summary>
        private DateTime lastScrollTime = DateTime.Now;

        /// <summary>
        /// Last width the control was drawn at.  Used to control resizing.
        /// </summary>
        private int lastWidth;

        /// <summary>
        /// The object that the mouse is currently down on, if any.
        /// </summary>
        private IMouseObject mouseDown;

        /// <summary>
        /// The object that the mouse is currently over, if any.
        /// </summary>
        private IMouseObject mouseTrack;

        /// <summary>
        /// A class to determine when the TextBox used for
        /// editing should be cancelled:
        /// </summary>
        private PopupCancelNotifier popupCancel;

        /// <summary>
        /// The rectangle containing the "ListView" portion of the control.
        /// </summary>
        private Rectangle rcListView;

        /// <summary>
        /// Flag to control whether redrawing occurs or not
        /// during updating:
        /// </summary>
        private bool redraw = true;

        /// <summary>
        /// Are we scrolling a new group into view or not?
        /// </summary>
        private bool scrollingGroup;

        /// <summary>
        /// Whether items are selected on MouseDown or
        /// MouseUp.
        /// </summary>
        private bool selectOnMouseDown;

        /// <summary>
        /// Reference to an external Image List for drawing the small icon view.
        /// </summary>
        private ImageList smallImageList;

        /// <summary>
        /// Reference to an external ToolTip object.
        /// </summary>
        private ToolTip toolTip;

        /// <summary>
        /// The Text Box used for editing an item's caption.
        /// </summary>
        private TextBox txtEdit;

        #endregion

        #region Events

        /// <summary>
        /// Raised before the selected group in the ListBar control is changed. Allows
        /// the group selection to be cancelled.
        /// </summary>
        [Description("Raised before the selected group in the ListBar control is changed.")]
        public event BeforeGroupChangedEventHandler BeforeSelectedGroupChanged;

        /// <summary>
        /// Raised when the selected group in a ListBar control has been
        /// changed.
        /// </summary>
        [Description("Raised once the selected group in the ListBar control has been changed.")]
        public event EventHandler SelectedGroupChanged;

        /// <summary>
        /// Raised before an item in a ListBar control is clicked.  Allows
        /// the item selection to be cancelled.
        /// </summary>
        [Description("Raised before an item in the ListBar control is clicked.")]
        public event BeforeItemClickedEventHandler BeforeItemClicked;

        /// <summary>
        /// Raised when an item has been clicked in the ListBar control.
        /// </summary>
        [Description("Raised once an item in the ListBar control has been clicked.")]
        public event ItemClickedEventHandler ItemClicked;

        /// <summary>
        /// Raised when an item has been double clicked in the ListBar control.
        /// </summary>
        [Description("Raised when an item has been double clicked in the ListBar control.")]
        public event ItemClickedEventHandler ItemDoubleClicked;

        /// <summary>
        /// Raised when a group has been clicked in the ListBar control.
        /// </summary>
        [Description("Raised when a group has been clicked in the ListBar control.")]
        public event GroupClickedEventHandler GroupClicked;

        /// <summary>
        /// Raised before an item's label is about to be edited in the ListBar
        /// control.  Allows the label edit to be cancelled.
        /// </summary>
        [Description("Raised before an item's label is about to be edited in the ListBar control.")]
        public event ListBarLabelEditEventHandler BeforeLabelEdit;

        /// <summary>
        /// Raised after an item's label has been edited in the ListBar control.
        /// Allows the new caption to be checked and the edit cancelled.
        /// </summary>
        [Description("Raised after an item's label has been edited but before the change is committed.")]
        public event ListBarLabelEditEventHandler AfterLabelEdit;

        #endregion

        #region Constructor and Dispose/Finalise

        /// <summary>
        /// Creates a new instance of a ListBar control.
        /// </summary>
        public ListBar()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Set up the control:
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            // Initialisation:
            groups = CreateListBarGroupCollection();
            btnUp = CreateListBarScrollButton(ListBarScrollButton.ListBarScrollButtonType.Up);
            btnDown = CreateListBarScrollButton(ListBarScrollButton.ListBarScrollButtonType.Down);

            // Scroll timer:
            buttonPressed.Interval = 350;
            buttonPressed.Enabled = false;
            buttonPressed.Tick += buttonPressed_Tick;

            // Text box:
            txtEdit.KeyDown += txtEdit_KeyDown;

            popupCancel = new PopupCancelNotifier();
            popupCancel.PopupCancel += popupCancel_PopupCancel;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Responding to events

        /// <summary>
        /// Controls scrolling when the mouse is over and down on a scroll
        /// bar button.
        /// </summary>
        /// <param name="sender">The object which raised this event.</param>
        /// <param name="e">Arguments associated with this event.</param>
        private void buttonPressed_Tick(object sender, EventArgs e)
        {
            // check if the mouse is still over a scroll button
            // that's been pressed:
            if (activeButton != null)
            {
                // shorten the interval for the next scroll down
                // to 75ms:
                buttonPressed.Interval = 75;
                // Check if mouse in button:
                Point pos = Cursor.Position;
                pos = PointToClient(pos);
                if (activeButton.HitTest(pos))
                {
                    // perform the scrolling:
                    Scroll(activeButton, true);
                }
            }
        }

        /// <summary>
        /// Scroll the control for the selected button.
        /// </summary>
        /// <param name="button">Button to scroll for.</param>
        /// <param name="fromTimer">Whether request to scroll from a 
        /// scroll button timer event.</param>
        private void Scroll(ListBarScrollButton button, bool fromTimer)
        {
            int direction =
                (button.ButtonType == ListBarScrollButton.ListBarScrollButtonType.Up ? 1 : -1);
            Scroll(direction, fromTimer);
        }

        /// <summary>
        /// Scroll the control for the selected button.
        /// </summary>
        /// <param name="button">Button to scroll for.</param>
        private void Scroll(ListBarScrollButton button)
        {
            Scroll(button, false);
        }


        /// <summary>
        /// Scroll the control in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move in.  Note_ that this follows
        /// the direction of movement of an item: +1 scrolls up, -1 scrolls down.</param>
        private void Scroll(int direction)
        {
            Scroll(direction, false);
        }

        /// <summary>
        /// Scroll the control in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move in.  Note_ that this follows
        /// the direction of movement of an item: +1 scrolls up, -1 scrolls down.</param>
        /// <param name="fromTimer">Whether request to scroll from a 
        /// scroll button timer event.</param>
        private void Scroll(int direction, bool fromTimer)
        {
            // get the distance we must scroll to move one entire
            // item:
            ListBarGroup selGroup = SelectedGroup;
            int endScrollOffset = selGroup.ScrollOffset +
                                  (direction*selGroup.Items[0].Height);
            if (endScrollOffset > 0)
            {
                endScrollOffset = 0;
            }

            // Get the invalidation rectangle:
            Rectangle rcInvalid = new Rectangle(
                new Point(1, selGroup.ButtonLocation.X + selGroup.ButtonHeight),
                new Size(Width - 2,
                         ((groups.IndexOf(selGroup) == groups.Count - 1) ?
                                                                             Height - (selGroup.ButtonLocation.Y + selGroup.ButtonHeight) :
                                                                                                                                              groups[groups.IndexOf(selGroup) + 1].ButtonLocation.Y)));

            // Starting from the current point, scroll the selected
            // bar to the new point in ever increasing steps:
            int step = direction;
            if (fromTimer)
            {
                step *= selGroup.Items[0].Height/4;
            }
            while (selGroup.ScrollOffset != endScrollOffset)
            {
                // determine the new scroll offset:
                int newOffset = selGroup.ScrollOffset + step;
                if (direction < 0)
                {
                    if (newOffset < endScrollOffset)
                    {
                        newOffset = endScrollOffset;
                    }
                }
                else
                {
                    if (newOffset > endScrollOffset)
                    {
                        newOffset = endScrollOffset;
                    }
                }
                selGroup.ScrollOffset = newOffset;

                // refresh the display:
                Invalidate();
                Update();

                // Make the next step larger.
                step *= 2;
            }

            // Ensure that everything is shown in the right place
            DoResize();
        }

        /// <summary>
        /// Raises the Resize event and performs internal
        /// sizing of the objects in the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            DoResize();
            base.OnResize(e);
        }

        /// <summary>
        /// Raises the SizeChanged event for this control
        /// and internally sizes the display.
        /// </summary>
        /// <param name="e">Event arguments associated with this
        /// event.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            DoResize();
            Invalidate();

            base.OnSizeChanged(e);
        }

        private ListBarGroup ensureSelection()
        {
            ListBarGroup selectedGroup = SelectedGroup;

            if ((selectedGroup == null) || (!selectedGroup.Visible))
            {
                selectedGroup = null;
                if (groups.Count > 0)
                {
                    for (int i = 0; i < groups.Count; i++)
                    {
                        if ((groups[i].Visible) && (selectedGroup == null))
                        {
                            groups[i].Selected = true;
                            selectedGroup = groups[i];
                        }
                        else
                        {
                            if (groups[i].Selected)
                            {
                                groups[i].Selected = false;
                            }
                        }
                    }
                }
            }
            return selectedGroup;
        }

        /// <summary>
        /// Called by the control's internal sizing mechanism.
        /// Returns the client size excluding the border of the
        /// control.
        /// </summary>
        /// <returns>A <c>Rectangle</c> providing the area to 
        /// draw the control into.</returns>
        protected virtual Rectangle GetClientRectangleExcludingBorder()
        {
            Rectangle rcClient = new Rectangle(
                ClientRectangle.Left + 1,
                ClientRectangle.Top + 1,
                ClientRectangle.Width - 2,
                ClientRectangle.Height - 2);
            return rcClient;
        }

        /// <summary>
        /// Called by the control's internal sizing mechanism.
        /// Returns the rectangle for a scroll button.
        /// </summary>
        /// <param name="buttonType">The scroll button to
        /// get the rectangle for.</param>
        /// <param name="selectedGroup">The Selected Group in the control.</param>
        /// <param name="internalGroupHeight">The internal height of the
        /// selected group</param>
        /// <returns>The Rectangle for the scroll button.</returns>
        protected virtual Rectangle GetScrollButtonRectangle(
            ListBarScrollButton.ListBarScrollButtonType buttonType,
            ListBarGroup selectedGroup,
            int internalGroupHeight
            )
        {
            Rectangle buttonRect;
            if (buttonType == ListBarScrollButton.ListBarScrollButtonType.Up)
            {
                buttonRect = new Rectangle(
                    new Point(
                        ((RightToLeft == RightToLeft.Yes) ?
                                                              2 :
                                                                    Width - 2 - btnUp.Rectangle.Width),
                        selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight + 2),
                    btnUp.Rectangle.Size);
            }
            else
            {
                buttonRect = new Rectangle(
                    new Point(
                        ((RightToLeft == RightToLeft.Yes) ?
                                                              2 :
                                                                    Width - 2 - btnUp.Rectangle.Width),
                        selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight +
                        internalGroupHeight - 2 - btnDown.Rectangle.Height),
                    btnDown.Rectangle.Size);
            }
            return buttonRect;
        }


        private void DoResize()
        {
            if (redraw)
            {
                if (groups.Count > 0)
                {
                    ListBarGroup selectedGroup = ensureSelection();
                    if (selectedGroup != null)
                    {
                        Rectangle rcClient = GetClientRectangleExcludingBorder();
                        rcListView = new Rectangle(rcClient.Location, rcClient.Size);

                        int lastVisibleGroup = 0;
                        int firstVisibleGroup = groups.Count - 1;
                        int nextVisibleGroup = firstVisibleGroup;

                        for (int i = 0; i <= groups.IndexOf(selectedGroup); i++)
                        {
                            ListBarGroup group = groups[i];

                            if (group.Visible)
                            {
                                int buttonWidth = GetGroupButtonWidth(group);
                                group.SetLocationAndWidth(
                                    new Point(rcClient.Left, rcListView.Top),
                                    buttonWidth);
                                rcListView.Y += group.ButtonHeight;
                                rcListView.Height -= group.ButtonHeight;

                                if (i > lastVisibleGroup)
                                {
                                    lastVisibleGroup = i;
                                }
                                if (i < firstVisibleGroup)
                                {
                                    firstVisibleGroup = i;
                                }
                            }
                        }

                        int bottom = rcClient.Bottom;
                        for (int i = groups.Count - 1; i > groups.IndexOf(selectedGroup); i--)
                        {
                            ListBarGroup group = groups[i];
                            if (group.Visible)
                            {
                                int buttonWidth = GetGroupButtonWidth(group);
                                bottom -= group.ButtonHeight;
                                rcListView.Height -= group.ButtonHeight;
                                group.SetLocationAndWidth(
                                    new Point(rcClient.Left, bottom),
                                    buttonWidth);

                                if (i > lastVisibleGroup)
                                {
                                    lastVisibleGroup = i;
                                }
                                if (i < nextVisibleGroup)
                                {
                                    nextVisibleGroup = i;
                                }
                            }
                        }

                        int size = selectedGroup.Items.Height;
                        int height = selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight;
                        if (groups.IndexOf(selectedGroup) == lastVisibleGroup)
                        {
                            height = ClientRectangle.Height - height;
                        }
                        else
                        {
                            height = groups[nextVisibleGroup].ButtonLocation.Y -
                                     height;
                        }

                        bool needUp = false;
                        bool needDown = false;

                        needUp = (selectedGroup.ScrollOffset < 0);
                        needDown = ((size + selectedGroup.ScrollOffset) > height);

                        Rectangle btnUpRect = GetScrollButtonRectangle(
                            ListBarScrollButton.ListBarScrollButtonType.Up,
                            selectedGroup,
                            height);
                        btnUp.SetRectangle(btnUpRect);
                        btnUp.Visible = needUp;
                        if (!needUp)
                        {
                            if (activeButton != null)
                            {
                                if (activeButton.Equals(btnUp))
                                {
                                    buttonPressed.Enabled = false;
                                }
                            }
                        }

                        Rectangle btnDownRect = GetScrollButtonRectangle(
                            ListBarScrollButton.ListBarScrollButtonType.Down,
                            selectedGroup, height);
                        btnDown.SetRectangle(btnDownRect);
                        btnDown.Visible = needDown;
                        if (!needDown)
                        {
                            if (activeButton != null)
                            {
                                if (activeButton.Equals(btnDown))
                                {
                                    buttonPressed.Enabled = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        btnUp.Visible = false;
                        btnDown.Visible = false;
                    }

                    if (Width != lastWidth)
                    {
                        lastWidth = Width;
                    }

                    if (Height != lastHeight)
                    {
                        lastHeight = Height;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the Paint event and performs internal drawing of the
        /// control.	
        /// </summary>
        /// <param name="e">A PaintEventArgs object with details about the 
        /// paint event that must be performed.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (scrollingGroup)
            {
                RenderScrollNewGroup(e);
            }
            else
            {
                Render(e);
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// Raises the double click event and performs internal double-click
        /// processing for the control.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> associated with this
        /// double-click event.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            Point pt = PointToClient(Cursor.Position);

            IMouseObject obj = HitTest(pt, false);
            if (obj != null)
            {
                if (typeof (ListBarItem).IsAssignableFrom(obj.GetType()))
                {
                    ListBarItem item = (ListBarItem) obj;
                    MouseButtons button = MouseButtons.Left; // TODO_ should use GetAsyncKeyState or whatever the Framework equivalent is
                    ItemClickedEventArgs ice = new ItemClickedEventArgs(
                        item, pt, button);
                    OnItemDoubleClicked(ice);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ItemDoubleClicked"/> event for an item.
        /// </summary>
        /// <param name="e">The <see cref="ItemClickedEventArgs"/> details
        /// associated with the double click event.</param>
        protected virtual void OnItemDoubleClicked(ItemClickedEventArgs e)
        {
            if (ItemDoubleClicked != null)
            {
                ItemDoubleClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseDown event and performs internal mouse-down
        /// processing for the control.
        /// </summary>
        /// <param name="e">A MouseEventArgs object with details about the
        /// mouse event that has occurred.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (mouseTrack != null)
                {
                    mouseDown = mouseTrack;
                    mouseDown.MouseDown = true;
                    mouseDown.MouseDownPoint = new Point(e.X, e.Y);

                    // Check whether a scroll button has been pressed.
                    // If it has, then start a timer to auto-scroll
                    // more.
                    if (typeof (ListBarScrollButton).IsAssignableFrom(mouseTrack.GetType()))
                    {
                        // Set the active scrolling button:
                        activeButton = (ListBarScrollButton) mouseTrack;
                        // perform the initial scroll:
                        Scroll(activeButton);
                        // initialise the timer:
                        buttonPressed.Interval = 350;
                        buttonPressed.Enabled = true;
                    }
                    else if (typeof (ListBarItem).IsAssignableFrom(mouseTrack.GetType()))
                    {
                        if (selectOnMouseDown)
                        {
                            MouseSelectItem((ListBarItem) mouseTrack, e);
                        }
                    }

                    // Redraw the control:
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the MouseMove event and performs mouse move processing
        /// for the control.
        /// </summary>
        /// <param name="e">A MouseEventArgs object describing the mouse
        /// move event that has occurred.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // no motion during item editing
            if (editItem != null)
            {
                return;
            }

            // detect if the mouse is over anything:
            IMouseObject newMouseOver = HitTest(new Point(e.X, e.Y));

            if (newMouseOver == null)
            {
                if (mouseTrack != null)
                {
                    mouseTrack.MouseOver = false;
                    mouseTrack = null;
                    Cursor = Cursors.Default;
                    Invalidate();
                }
                if (toolTip != null)
                {
                    toolTip.SetToolTip(this, "");
                }
            }
            else
            {
                bool noChange = false;
                if (mouseTrack != null)
                {
                    if (mouseTrack == newMouseOver)
                    {
                        // We're not tracking a new item.
                        noChange = true;

                        // However, if we mouse-downed on an item, then we 
                        // should check if the new mouse position is sufficiently
                        // far from the original position that a drag operation
                        // is in order:
                        if (allowDragItems)
                        {
                            if (typeof (ListBarItem).IsAssignableFrom(mouseTrack.GetType()))
                            {
                                if (mouseTrack.MouseDown)
                                {
                                    int hysteresis = (SelectedGroup.View == ListBarGroupView.LargeIcons ? 4 : 2);
                                    if ((Math.Abs(mouseTrack.MouseDownPoint.X - e.X) > hysteresis) ||
                                        (Math.Abs(mouseTrack.MouseDownPoint.Y - e.Y) > hysteresis))
                                    {
                                        // time to start dragging:
                                        ListBarItem dragItem = (ListBarItem) mouseTrack;
                                        DoDragDrop(dragItem, DragDropEffects.Move);
                                        InternalDragDropComplete(dragItem, true);
                                        EnsureItemVisible(dragItem);
                                        return;
                                    }
                                }
                            }
                        }
                        if (allowDragGroups)
                        {
                            if (typeof (ListBarGroup).IsAssignableFrom(mouseTrack.GetType()))
                            {
                                if (mouseTrack.MouseDown)
                                {
                                    if ((Math.Abs(mouseTrack.MouseDownPoint.X - e.X) > 4) ||
                                        (Math.Abs(mouseTrack.MouseDownPoint.Y - e.Y) > 4))
                                    {
                                        // time to start dragging:
                                        ListBarGroup dragGroup = (ListBarGroup) mouseTrack;
                                        DoDragDrop(dragGroup, DragDropEffects.Move);
                                        //InternalDragDropComplete(dragGroup);
                                        dragGroup.MouseOver = false;
                                        dragGroup.MouseDown = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        mouseTrack.MouseOver = false;
                    }
                }
                if (!noChange)
                {
                    mouseTrack = newMouseOver;
                    if (toolTip != null)
                    {
                        toolTip.SetToolTip(this, mouseTrack.ToolTipText);
                    }
                    mouseTrack.MouseOver = true;
                    if (typeof (ListBarGroup).IsAssignableFrom(mouseTrack.GetType()))
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                    Invalidate();
                }
            }
        }


        /// <summary>
        /// Raises the MouseUp event and performs mouse up processing
        /// for the control.
        /// </summary>
        /// <param name="e">A MouseEventArgs object describing the mouse
        /// move event that has occurred.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                if (mouseTrack != null)
                {
                    if (mouseTrack.Equals(mouseDown))
                    {
                        if (typeof (ListBarGroup).IsAssignableFrom(mouseTrack.GetType()))
                        {
                            BeforeGroupChangedEventArgs bgc = new BeforeGroupChangedEventArgs(
                                (ListBarGroup) mouseTrack);
                            OnBeforeGroupChanged(ref bgc);
                            if (!bgc.Cancel)
                            {
                                // group clicked.  Select the new group:
                                SelectGroup((ListBarGroup) mouseTrack);
                                OnSelectedGroupChanged(new EventArgs());
                                GroupClickedEventArgs gce = new GroupClickedEventArgs(
                                    (ListBarGroup) mouseTrack,
                                    new Point(e.X, e.Y),
                                    e.Button);
                                OnGroupClicked(gce);
                            }
                        }
                        else if (typeof (ListBarScrollButton).IsAssignableFrom(mouseTrack.GetType()))
                        {
                            // don't need to do anything here, except be sure
                            // we reset the active scroll button & timer later
                        }
                        else
                        {
                            if (activeButton == null)
                            {
                                if (!selectOnMouseDown)
                                {
                                    MouseSelectItem((ListBarItem) mouseTrack, e);
                                }
                            }
                        }
                    }
                }

                // no more scrolling
                activeButton = null;
                buttonPressed.Enabled = false;

                if (mouseDown != null)
                {
                    mouseDown.MouseDown = false;
                    mouseDown.MouseOver = false;
                }
                if (mouseTrack != null)
                {
                    mouseTrack.MouseOver = false;
                }
                Invalidate();
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (mouseTrack != null)
                {
                    // Right click?
                    if (typeof (ListBarGroup).IsAssignableFrom(mouseTrack.GetType()))
                    {
                        GroupClickedEventArgs gce = new GroupClickedEventArgs(
                            (ListBarGroup) mouseTrack,
                            new Point(e.X, e.Y),
                            e.Button);
                        OnGroupClicked(gce);
                    }
                    else if (typeof (ListBarItem).IsAssignableFrom(mouseTrack.GetType()))
                    {
                        ItemClickedEventArgs ic = new ItemClickedEventArgs(
                            (ListBarItem) mouseTrack,
                            new Point(e.X, e.Y),
                            e.Button);
                        OnItemClicked(ic);
                    }
                    else
                    {
                        // no action currently
                    }
                }
                else
                {
                    // group right click:
                    GroupClickedEventArgs gce = new GroupClickedEventArgs(
                        SelectedGroup,
                        new Point(e.X, e.Y),
                        e.Button);
                    OnGroupClicked(gce);
                }
            }
        }

        /// <summary>
        /// Raises the MouseLeave event and performs internal mouse
        /// track processing for the control.
        /// </summary>
        /// <param name="e">Event arguments associated with this event.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (mouseTrack != null)
            {
                mouseTrack.MouseOver = false;
                mouseTrack = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the MouseWheel event and performs mouse wheel 
        /// processing for the control.
        /// </summary>
        /// <param name="e">A MouseEventArgs object describing the mouse
        /// move event that has occurred.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if ((e.Delta > 0) && (btnUp.Visible))
            {
                Scroll(1);
            }
            else if ((e.Delta < 0) && (btnDown.Visible))
            {
                Scroll(-1);
            }
        }

        private object GetBestDragDropFormat(DragEventArgs e)
        {
            object ret = null;
            object defaultFormat = null;
            foreach (string format in e.Data.GetFormats())
            {
                object thisFormatData = e.Data.GetData(format);
                if (defaultFormat == null)
                {
                    defaultFormat = thisFormatData;
                }

                if (typeof (ListBarItem).IsAssignableFrom(thisFormatData.GetType()))
                {
                    ret = thisFormatData;
                    break;
                }
                else if (typeof (ListBarItem).IsAssignableFrom(thisFormatData.GetType()))
                {
                    ret = thisFormatData;
                    break;
                }
            }

            if (ret == null)
            {
                ret = defaultFormat;
            }

            return ret;
        }

        private object GetTypeOrSubClassFromData(DragEventArgs e, Type dataType)
        {
            object ret = null;
            foreach (string format in e.Data.GetFormats())
            {
                if (dataType.IsAssignableFrom(e.Data.GetData(format).GetType()))
                {
                    ret = e.Data.GetData(format);
                    break;
                }
            }
            return ret;
        }

        private bool PerformAutoDrag(DragEventArgs e)
        {
            bool ret = false;
            if ((allowDragItems) || (allowDragGroups))
            {
                foreach (string format in e.Data.GetFormats())
                {
                    Type dataType = e.Data.GetData(format).GetType();
                    if (typeof (ListBarItem).IsAssignableFrom(dataType))
                    {
                        ret = true;
                        break;
                    }
                    else if (typeof (ListBarGroup).IsAssignableFrom(dataType))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Raises the DragOver event and performs internal processing of 
        /// drag-drop to show the insertion point and navigate through
        /// the items in the control.
        /// </summary>
        /// <param name="e">A DragEventArgs object describing the drag
        /// over being performed.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            // perform the base operation:
            base.OnDragOver(e);

            if (groups.Count > 0)
            {
                if (e.Effect != DragDropEffects.None)
                {
                    InternalDragOverProcess(e, true);
                }
                else if (PerformAutoDrag(e))
                {
                    InternalDragOverProcess(e, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            // perform the base operation:
            base.OnDragDrop(e);

            if (groups.Count > 0)
            {
                object obj = GetBestDragDropFormat(e);

                if (e.Effect != DragDropEffects.None)
                {
                    bool move = (e.Effect == DragDropEffects.Move);
                    InternalDragDropComplete(obj, move);
                }
                else if (PerformAutoDrag(e))
                {
                    InternalDragDropComplete(obj, true);
                }
            }
        }

        /// <summary>
        /// Raises the BeforeSelectedGroupChanged event.  This event enables
        /// the user to prevent a group selection.
        /// </summary>
        /// <param name="e">The BeforeGroupChangedEventArgs object associated
        /// with this event.</param>
        protected virtual void OnBeforeGroupChanged(ref BeforeGroupChangedEventArgs e)
        {
            if (BeforeSelectedGroupChanged != null)
            {
                BeforeSelectedGroupChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeItemClicked event.  This event enables
        /// the user to prevent an item from being selected.
        /// </summary>
        /// <param name="e">The BeforeItemClickedEventArgs object associated
        /// with this event.</param>
        protected virtual void OnBeforeItemClicked(ref BeforeItemClickedEventArgs e)
        {
            e.Cancel = (!e.Item.Enabled);
            if (BeforeItemClicked != null)
            {
                BeforeItemClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the <c>ItemClicked</c> event. 
        /// </summary>
        /// <param name="e">The <c>ItemClickedEventArgs</c> object associated 
        /// with this event.</param>
        protected virtual void OnItemClicked(ItemClickedEventArgs e)
        {
            if (ItemClicked != null)
            {
                ItemClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the <c>GroupClicked</c> event.
        /// </summary>
        /// <param name="e">The <c>GroupClickedEventArgs</c> object
        /// associated with this event.</param>
        protected virtual void OnGroupClicked(GroupClickedEventArgs e)
        {
            if (GroupClicked != null)
            {
                GroupClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeLabelEdit event for an item in the control.
        /// </summary>
        /// <param name="e">The LabelEditEventArgs describing the item
        /// that is about to be edited and allowing the edit action
        /// to be cancelled.</param>
        protected virtual void OnBeforeLabelEdit(ListBarLabelEditEventArgs e)
        {
            if (BeforeLabelEdit != null)
            {
                BeforeLabelEdit(this, e);
            }
        }

        /// <summary>
        /// Raises the AfterLabelEdit event for an item in the control.
        /// </summary>
        /// <param name="e">The AfterEditEventArgs describing the item
        /// that has just been edited and allowing the edit action
        /// to be cancelled or the new caption to be changed.</param>
        protected virtual void OnAfterLabelEdit(ListBarLabelEditEventArgs e)
        {
            if (AfterLabelEdit != null)
            {
                AfterLabelEdit(this, e);
            }
        }

        /// <summary>
        /// Raises the <c>SelectedGroupChanged</c> event.
        /// </summary>
        /// <param name="e">An EventArgs object associated with the event.</param>
        protected virtual void OnSelectedGroupChanged(EventArgs e)
        {
            if (SelectedGroupChanged != null)
            {
                SelectedGroupChanged(this, e);
            }
        }

        private void txtEdit_TextChanged(object sender, EventArgs e)
        {
            if (editItem != null)
            {
                setTextBoxSize(editItem);
            }
        }

        private void txtEdit_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Return:
                    // end editing:
                    EndTextEdit(true);
                    break;

                case Keys.Escape:
                    // cancel editing:
                    EndTextEdit(false);
                    break;
            }
        }

        private void popupCancel_PopupCancel(object sender, EventArgs e)
        {
            EndTextEdit(true);
        }

        #endregion

        #region Internal implementation

        private void EndTextEdit(bool commit)
        {
            if (editItem != null)
            {
                ListBarItem editedItem = editItem;
                editItem = null;

                if ((commit) && (editedItem != null))
                {
                    ListBarGroup selectedGroup = SelectedGroup;

                    ListBarLabelEditEventArgs lea = new ListBarLabelEditEventArgs(
                        selectedGroup.Items.IndexOf(editItem), txtEdit.Text, editedItem);
                    OnAfterLabelEdit(lea);

                    if (!lea.CancelEdit)
                    {
                        if (editedItem != null) // may be shutting down...
                        {
                            editedItem.Caption = lea.Label;
                        }
                    }
                }
            }
            else if (editGroup != null)
            {
                ListBarGroup editedGroup = editGroup;
                editGroup = null;

                if ((commit) && (editedGroup != null))
                {
                    ListBarLabelEditEventArgs lea = new ListBarLabelEditEventArgs(
                        Groups.IndexOf(editedGroup), txtEdit.Text, editedGroup);
                    OnAfterLabelEdit(lea);

                    if (!lea.CancelEdit)
                    {
                        if (editedGroup != null)
                        {
                            editedGroup.Caption = lea.Label;
                        }
                    }
                }
            }

            txtEdit.Visible = false;
            Invalidate();
        }

        private void InternalDragDropComplete(
            object dragItem,
            bool move
            )
        {
            ListBarItem listBarDragItem = null;

            if (typeof (ListBarItem).IsAssignableFrom(dragItem.GetType()))
            {
                listBarDragItem = (ListBarItem) dragItem;
                listBarDragItem.MouseOver = false;
                listBarDragItem.MouseDown = false;
            }

            if (dragInsertPoint != null)
            {
                ListBarGroup groupTo = SelectedGroup;
                if (groupTo != null) // cannot happen...
                {
                    ListBarGroup groupFrom = null;

                    if (listBarDragItem != null)
                    {
                        // Check which bar we've come from
                        // (it may be none, we may have come
                        // from another control):

                        foreach (ListBarGroup group in groups)
                        {
                            if (group.Items.Contains(listBarDragItem))
                            {
                                groupFrom = group;
                                break;
                            }
                        }
                    }

                    if (groupFrom != null) // Dragged from this control
                    {
                        // moving to a new group: 
                        if (move)
                        {
                            if (dragInsertPoint.ItemAfter != null)
                            {
                                if (dragInsertPoint.ItemAfter.Equals(listBarDragItem))
                                {
                                    listBarDragItem = null;
                                }
                            }
                            else if (dragInsertPoint.ItemBefore != null)
                            {
                                if (dragInsertPoint.ItemBefore.Equals(listBarDragItem))
                                {
                                    listBarDragItem = null;
                                }
                            }
                            if (listBarDragItem != null)
                            {
                                groupFrom.Items.Remove(listBarDragItem);
                            }
                        }
                        else
                        {
                            // Clone a new item to add:
                            ListBarItem newItem = new ListBarItem(
                                listBarDragItem.Caption, listBarDragItem.IconIndex, listBarDragItem.ToolTipText,
                                listBarDragItem.Tag);
                            listBarDragItem = newItem;
                        }
                    }
                    else
                    {
                        // add a new item which represents what's been dragged
                        if (listBarDragItem != null)
                        {
                            // there's an issue with which image to pick here
                            listBarDragItem = new ListBarItem(
                                listBarDragItem.Caption, listBarDragItem.IconIndex,
                                listBarDragItem.ToolTipText, listBarDragItem.Tag);
                        }
                        else
                        {
                            // Create a new item
                            listBarDragItem = new ListBarItem(
                                dragItem.ToString());
                            ((ListBarItem) dragItem).Tag = dragItem;
                        }
                    }

                    if (listBarDragItem != null)
                    {
                        if (dragInsertPoint.ItemAfter != null)
                        {
                            groupTo.Items.InsertAfter(dragInsertPoint.ItemAfter, listBarDragItem);
                        }
                        else
                        {
                            groupTo.Items.InsertAfter(dragInsertPoint.ItemAfter, listBarDragItem);
                        }
                    }
                }
            }

            dragInsertPoint = null;
            Invalidate();
        }

        private void SelectGroup(ListBarGroup group)
        {
            // first work out the scrolling logic:

            ListBarGroup selGroup = SelectedGroup;
            if (selGroup != group)
            {
                // Which groups are we moving between?
                indexNew = groups.IndexOf(group);
                indexCurrent = groups.IndexOf(selGroup);

                // Scrolling the new group into view:
                if (redraw)
                {
                    scrollingGroup = true;

                    if (indexNew > indexCurrent)
                    {
                        // the new index is below the current one.					
                        // Scroll buttons from indexCurrent + 1 to indexNew
                        // upwards
                        int newIndexTargetPos = selGroup.ButtonLocation.Y + selGroup.ButtonHeight;
                        for (int i = indexCurrent + 1; i <= indexNew - 1; i++)
                        {
                            if (groups[i].Visible)
                            {
                                newIndexTargetPos += groups[i].ButtonHeight;
                            }
                        }

                        bool finished = false;
                        int currentPos = group.ButtonLocation.Y;
                        int step = -1;
                        while (!finished)
                        {
                            currentPos += step;
                            if (currentPos <= newIndexTargetPos)
                            {
                                step += (newIndexTargetPos - currentPos);
                                currentPos = newIndexTargetPos;
                                finished = true;
                            }

                            for (int i = indexCurrent + 1; i <= indexNew; i++)
                            {
                                ListBarGroup workGroup = groups[i];
                                if (workGroup.Visible)
                                {
                                    Point newLocation = workGroup.ButtonLocation;
                                    newLocation.Y += step;
                                    workGroup.SetLocationAndWidth(
                                        newLocation, workGroup.ButtonWidth);
                                }
                            }

                            Invalidate();
                            Update();

                            step *= 2;
                        }
                    }
                    else
                    {
                        // the new index is above the current one.
                        // scroll buttons from indexNew + 1 to indexCurrent
                        // downwards
                        int lastIndex = indexCurrent;
                        int nextIndex = Groups.Count - 1;
                        for (int i = indexCurrent + 1; i < Groups.Count; i++)
                        {
                            if (i > lastIndex)
                            {
                                lastIndex = i;
                            }
                            if (i < nextIndex)
                            {
                                nextIndex = i;
                            }
                        }
                        int currentTargetPos = (indexCurrent == lastIndex ?
                                                                              ClientRectangle.Height :
                                                                                                         groups[nextIndex].ButtonLocation.Y);

                        bool finished = false;
                        int currentPos = selGroup.ButtonLocation.Y;
                        int step = 1;
                        while (!finished)
                        {
                            currentPos += step;
                            if (currentPos >= currentTargetPos)
                            {
                                step -= (currentPos - currentTargetPos);
                                currentPos = currentTargetPos;
                                finished = true;
                            }

                            for (int i = indexNew + 1; i <= indexCurrent; i++)
                            {
                                ListBarGroup workGroup = groups[i];
                                if (workGroup.Visible)
                                {
                                    Point newLocation = workGroup.ButtonLocation;
                                    newLocation.Y += step;
                                    workGroup.SetLocationAndWidth(newLocation, workGroup.ButtonWidth);
                                }
                            }

                            Invalidate();
                            Update();

                            step *= 2;
                        }
                    }

                    scrollingGroup = false;
                }

                selGroup.Selected = false;
                group.Selected = true;
                DoResize();
            }
        }

        /// <summary>
        /// Selects an item in response to a mouse event.
        /// </summary>
        /// <param name="item">Item to be selected.</param>
        /// <param name="e"><see cref="System.Windows.Forms.MouseEventArgs"/> 
        /// details associated with the mouse event.</param>
        private void MouseSelectItem(ListBarItem item, MouseEventArgs e)
        {
            BeforeItemClickedEventArgs bic = new BeforeItemClickedEventArgs(
                (ListBarItem) mouseTrack);
            OnBeforeItemClicked(ref bic);
            if (!bic.Cancel)
            {
                // item clicked:
                SelectItem((ListBarItem) mouseTrack);
                ItemClickedEventArgs ic = new ItemClickedEventArgs(
                    (ListBarItem) mouseTrack,
                    new Point(e.X, e.Y),
                    e.Button);
                OnItemClicked(ic);
            }
        }

        /// <summary>
        /// Selects an item in the selected bar and makes
        /// it visible.
        /// </summary>
        /// <param name="item">The item to select.</param>
        private void SelectItem(ListBarItem item)
        {
            BeginUpdate();
            foreach (ListBarItem otherItem in SelectedGroup.Items)
                otherItem.Selected = false;
            item.Selected = true;
            EndUpdate();
            EnsureItemVisible(item);
            Invalidate();
        }

        /// <summary>
        /// Starts editing the specified ListBarGroup.  Note_ this
        /// method is called from the StartEdit method of a ListBarGroup.
        /// </summary>
        /// <param name="group">The group to start editing.</param>
        protected internal void StartGroupEdit(ListBarGroup group)
        {
            // Fire the BeforeLabelEdit event:
            ListBarLabelEditEventArgs e = new ListBarLabelEditEventArgs(
                groups.IndexOf(group), group.Caption, group);
            OnBeforeLabelEdit(e);

            if (!e.CancelEdit)
            {
                editGroup = group;

                // Focus the control:
                Focus();

                // Set the edit text:
                txtEdit.Text = group.Caption;
                txtEdit.Font = (group.Font ?? Font);
                txtEdit.Location = group.ButtonLocation;
                txtEdit.Size = new Size(group.ButtonWidth, group.ButtonHeight);
                txtEdit.Visible = true;
                txtEdit.BringToFront();
                txtEdit.Focus();

                popupCancel.StartTracking(txtEdit);
            }
        }

        /// <summary>
        /// Starts editing the specified <c>ListBarItem</c>.  Note_ this
        /// method is called from the <c>StartEdit</c> method of a 
        /// <c>ListBarItem</c>.
        /// <seealso cref="ListBarItem.StartEdit"/>
        /// </summary>
        /// <param name="item">The item to start editing.</param>
        protected internal void StartItemEdit(ListBarItem item)
        {
            // Get rectangle of item relative to control:
            ListBarGroup selectedGroup = SelectedGroup;

            // Check whether item is part of the selected
            // control:
            if (selectedGroup.Items.Contains(item))
            {
                // Fire the BeforeLabelEdit event:
                ListBarLabelEditEventArgs e = new ListBarLabelEditEventArgs(
                    selectedGroup.Items.IndexOf(item), item.Caption, item);
                OnBeforeLabelEdit(e);
                if (!e.CancelEdit)
                {
                    editItem = item;

                    // Make sure we can see it:
                    EnsureItemVisible(item);

                    // Focus the control:
                    Focus();

                    // Set the edit text:
                    txtEdit.Text = item.Caption;
                    txtEdit.Font = (item.Font ?? Font);
                    setTextBoxSize(editItem);
                    int top = item.TextRectangle.Top;
                    txtEdit.Top = top;
                    txtEdit.Visible = true;
                    txtEdit.BringToFront();
                    txtEdit.Focus();

                    popupCancel.StartTracking(txtEdit);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Editing is only possible on items belonging to the SelectedGroup in the control.");
            }
        }

        private void setTextBoxSize(ListBarItem editItem)
        {
            ListBarGroup selectedGroup = SelectedGroup;
            if (selectedGroup != null)
            {
                string text = txtEdit.Text;
                if (text.Length == 0)
                {
                    text = "Xg";
                }

                int maxWidth = 0;
                if (selectedGroup.View == ListBarGroupView.SmallIcons)
                {
                    maxWidth = ClientRectangle.Width - editItem.TextRectangle.Left - 1;
                }
                else
                {
                    maxWidth = ClientRectangle.Width - 2;
                }

                Graphics gfx = Graphics.FromHwnd(txtEdit.Handle);
                StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit |
                                                    (txtEdit.RightToLeft == RightToLeft.Yes ? StringFormatFlags.DirectionRightToLeft : 0));
                fmt.Alignment = StringAlignment.Center;
                SizeF textSize = gfx.MeasureString(text, txtEdit.Font, maxWidth - 6, fmt);
                fmt.Dispose();
                gfx.Dispose();

                if (textSize.Width < 24)
                {
                    textSize.Width = 24;
                }
                textSize.Height += 2.0F;

                txtEdit.Size = new Size((int) textSize.Width + 6, (int) textSize.Height + 4);
                if (selectedGroup.View == ListBarGroupView.SmallIcons)
                {
                    txtEdit.Left = editItem.TextRectangle.Left + 1;
                }
                else
                {
                    txtEdit.Left = 1 + (maxWidth - (int) textSize.Width)/2;
                }
            }
        }

        /// <summary>
        /// Brings the specified <c>ListBarItem</c> into view if it is not already
        /// visible.  The <c>ListBarItem</c> must be in the selected group.
        /// <seealso cref="ListBarItem"/>
        /// <seealso cref="ListBar.SelectedGroup"/>
        /// </summary>
        /// <param name="item">Item to bring into view.</param>
        protected internal void EnsureItemVisible(ListBarItem item)
        {
            // Get rectangle of item relative to control:
            ListBarGroup selectedGroup = SelectedGroup;

            // Check whether item is part of the selected
            // group:
            if (selectedGroup.Items.Contains(item))
            {
                Rectangle rcVisible = new Rectangle(
                    selectedGroup.ButtonLocation,
                    new Size(ClientRectangle.Width, 0)
                    );

                ListBarGroup nextGroup = null;
                for (int i = groups.IndexOf(selectedGroup) + 1; i < groups.Count; i++)
                {
                    if (groups[i].Visible)
                    {
                        nextGroup = groups[i];
                        break;
                    }
                }

                if (nextGroup == null)
                {
                    rcVisible.Height = ClientRectangle.Height -
                                       (selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight);
                }
                else
                {
                    rcVisible.Height = nextGroup.ButtonLocation.Y - rcVisible.Top;
                }

                bool invisible = true;
                bool notFirstTime = false;
                while (invisible)
                {
                    Rectangle rcItem = new Rectangle(item.Location,
                                                     new Size(ClientRectangle.Width, item.Height));
                    rcItem.Offset(0,
                                  selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight +
                                  selectedGroup.ScrollOffset);

                    // Check if the item is too low:
                    if (rcItem.Bottom > rcVisible.Bottom)
                    {
                        // need to scroll down until it can be seen:
                        Scroll(-1, notFirstTime);
                    }
                    else if (rcItem.Top < rcVisible.Top)
                    {
                        // need to scroll up until it can be seen:
                        Scroll(1, notFirstTime);
                    }
                    else
                    {
                        invisible = false;
                    }
                    notFirstTime = true;
                }
            }
        }

        /// <summary>
        /// Checks if there is an object which interacts with
        /// the mouse in the control under the specified point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>If there is a mouse object under the point 
        /// then its IMouseObject interface, otherwise null.</returns>
        private IMouseObject HitTest(Point pt)
        {
            return HitTest(pt, false);
        }

        /// <summary>
        /// Checks if there is an object which interacts with
        /// the mouse in the control under the specified point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>If there is a mouse object under the point 
        /// then its IMouseObject interface, otherwise null.</returns>
        /// <param name="forDragDrop">Whether the hit testing is
        /// being performed for a drag-drop operation or not.  During
        /// drag-drop, the hittest rectangle is relaxed so it includes
        /// the entire rectangle and not just the icon and text.
        /// </param>
        private IMouseObject HitTest(Point pt, bool forDragDrop)
        {
            // Default return value:
            IMouseObject mouseObject = null;
            ListBarGroup selectedGroup = SelectedGroup;

            // Over a scroll button?
            if (btnUp.HitTest(pt))
            {
                // over the scroll up button:
                mouseObject = btnUp;
            }
            else if (btnDown.HitTest(pt))
            {
                // over the scroll down button:
                mouseObject = btnDown;
            }
            else
            {
                if ((forDragDrop) && (selectedGroup != null))
                {
                    // we test for any point with 6 pixels of
                    // the scroll bars if the scroll buttons are on:
                    if (btnUp.Visible)
                    {
                        Rectangle scrollTest = new Rectangle(
                            selectedGroup.ButtonLocation.X, selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight,
                            ClientRectangle.Width, 6);
                        if (scrollTest.Contains(pt))
                        {
                            mouseObject = btnUp;
                        }
                    }
                    if (btnDown.Visible)
                    {
                        ListBarGroup nextGroup = null;
                        for (int i = groups.IndexOf(selectedGroup) + 1; i < groups.Count; i++)
                        {
                            if (groups[i].Visible)
                            {
                                nextGroup = groups[i];
                                break;
                            }
                        }
                        if (nextGroup != null)
                        {
                            Rectangle scrollTest = new Rectangle(
                                nextGroup.ButtonLocation.X, nextGroup.ButtonLocation.Y - 6,
                                ClientRectangle.Width, 6);
                            if (scrollTest.Contains(pt))
                            {
                                mouseObject = btnDown;
                            }
                        }
                    }
                }
            }

            // Check whether we're over any group buttons:
            if (mouseObject == null)
            {
                foreach (ListBarGroup group in groups)
                {
                    if (group.Visible)
                    {
                        Rectangle buttonRectangle = new Rectangle(
                            group.ButtonLocation, new Size(group.ButtonWidth, group.ButtonHeight));
                        if (buttonRectangle.Contains(pt))
                        {
                            // over a group:
                            mouseObject = group;
                            break;
                        }
                    }
                }
            }

            // Otherwise check whether we're over any list bar buttons:
            if (mouseObject == null)
            {
                // Is there a selected ListBar Group?
                if (selectedGroup != null)
                {
                    // Check each item in this group:
                    foreach (ListBarItem item in selectedGroup.Items)
                    {
                        Rectangle rcTest;
                        if (forDragDrop)
                        {
                            // For drag drop the entire rectangle of the item
                            // is taken into account:
                            rcTest = new Rectangle(item.Location,
                                                   new Size(item.Width, item.Height));
                            rcTest.Offset(0, selectedGroup.ScrollOffset + selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight);
                            if (rcTest.Contains(pt))
                            {
                                mouseObject = item;
                                break;
                            }
                        }
                        else
                        {
                            // Get the icon rectangle of the item within the group:
                            rcTest = item.IconRectangle;
                            // Check if the point is there:
                            if (rcTest.Contains(pt))
                            {
                                // We're over an item:
                                mouseObject = item;
                                break;
                            }
                            // Otherwise try the text rectangle:
                            rcTest = item.TextRectangle;
                            if (rcTest.Contains(pt))
                            {
                                // We're over an item:
                                mouseObject = item;
                                break;
                            }
                        }
                    }
                }
            }

            // Return the object the mouse is over if any
            return mouseObject;
        }


        /// <summary>
        /// Internal notification from a ListBarGroup that it has 
        /// been changed.
        /// </summary>
        /// <param name="group">The ListBarGroup which has been
        /// changed, or null the group has been removed.</param>
        /// <param name="addRemove">Whether the effect of the
        /// change will require the control to re-measured.</param>
        protected internal void GroupChanged(ListBarGroup group, bool addRemove)
        {
            // if we have changed the number of groups,
            // we need to redraw the entire control,
            // otherwise we just redraw this group
            if (addRemove)
            {
                if (group != null)
                {
                    group.SetButtonHeight(Font);
                }
                DoResize();
                PostResizeBarChanged();
            }
            Invalidate();
        }

        /// <summary>
        /// Internal notification from a ListBarItem that it has been
        /// changed.
        /// </summary>
        /// <param name="item">The ListBarItem which has been changed, 
        /// or null if the item has been removed.</param>
        /// <param name="addRemove">Whether the effect of the control
        /// will require the bar's contents to be remeasured.</param>
        protected internal void ItemChanged(ListBarItem item, bool addRemove)
        {
            ListBarGroup selGroup = SelectedGroup;
            ListBarGroup owningGroup = null;
            if (item != null)
            {
                // Which bar does it belong to
                foreach (ListBarGroup group in groups)
                {
                    if (group.Items.Contains(item))
                    {
                        owningGroup = group;
                        break;
                    }
                }

                if (owningGroup != null)
                {
                    Size imageSize = new Size(32, 32);
                    if ((owningGroup.View == ListBarGroupView.LargeIcons) || (owningGroup.View == ListBarGroupView.LargeIconsOnly))
                    {
                        if (largeImageList != null)
                        {
                            imageSize = largeImageList.ImageSize;
                        }
                    }
                    else
                    {
                        if (smallImageList != null)
                        {
                            imageSize = smallImageList.ImageSize;
                        }
                        else
                        {
                            imageSize = new Size(16, 16);
                        }
                    }

                    // Tell the item to size itself
                    item.SetSize(owningGroup.View, base.Font, imageSize);
                }
                else
                {
                    selGroup.SetLocationAndWidth(selGroup.ButtonLocation, selGroup.ButtonWidth);
                }
            }

            if (selGroup != null)
            {
                if (item == null)
                {
                    // need to assume it does
                    if (addRemove)
                    {
                        DoResize();
                        PostResizeBarChanged();
                    }
                    Invalidate();
                }
                else
                {
                    if (selGroup.Items.Contains(item))
                    {
                        // yes it does.  We need to modify the 
                        // display:
                        if (addRemove)
                        {
                            DoResize();
                            PostResizeBarChanged();
                        }
                        Invalidate();
                    }
                    else
                    {
                        if (owningGroup == null)
                        {
                            if (addRemove)
                            {
                                DoResize();
                                PostResizeBarChanged();
                            }
                            Invalidate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ensures the scroll bar isn't irrelevantly 
        /// begin displayed.
        /// </summary>
        private void PostResizeBarChanged()
        {
            // if the selected bar is scrolled,then we need 
            // to check in the new arrangement that there isn't
            // an unused space below the last item in the bar.

            // if there is we should check if it is possible
            // to scroll up by one or more items whilst still
            // ensuring the last item currently visible in the
            // view does not become any less visible.

            ListBarGroup selectedGroup = SelectedGroup;

            if (selectedGroup != null)
            {
                if (selectedGroup.Items.Count > 0)
                {
                    if (selectedGroup.ScrollOffset != 0)
                    {
                        bool finished = false;
                        ListBarGroup nextGroup = null;
                        for (int i = groups.IndexOf(selectedGroup) + 1; i < groups.Count; i++)
                        {
                            if (groups[i].Visible)
                            {
                                nextGroup = groups[i];
                                break;
                            }
                        }

                        while (!finished)
                        {
                            ListBarItem lastItem = selectedGroup.Items[selectedGroup.Items.Count - 1];
                            Rectangle rcItemLast = new Rectangle(lastItem.Location,
                                                                 new Size(ClientRectangle.Width, lastItem.Height));
                            rcItemLast.Offset(0, selectedGroup.ScrollOffset + selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight);

                            Rectangle rcView = new Rectangle(
                                selectedGroup.ButtonLocation.X, selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight,
                                ClientRectangle.Width, 0);
                            if (nextGroup == null)
                            {
                                rcView.Height = ClientRectangle.Height - rcView.Top;
                            }
                            else
                            {
                                rcView.Height = nextGroup.ButtonLocation.Y - rcView.Top;
                            }

                            if (rcItemLast.Bottom < rcView.Bottom + rcItemLast.Height)
                            {
                                // we can scroll up:
                                Scroll(1);
                            }
                            else
                            {
                                finished = true;
                            }

                            if (selectedGroup.ScrollOffset == 0)
                            {
                                finished = true;
                            }
                        }
                    }
                }
            }
        }

        private void InternalDragOverProcess(DragEventArgs e, bool assumeItem)
        {
            // TODO_ this method requires refactoring

            ListBarItem itemBefore = null;
            ListBarItem itemAfter = null;
            bool overEmptyBar = false;
            IMouseObject newDragHoverOver = null;
            bool overGroup = false;

            // see if the drag drop data contains a ListBarItem:		
            if ((GetTypeOrSubClassFromData(e, typeof (ListBarItem)) != null)
                || (assumeItem && (GetTypeOrSubClassFromData(e, typeof (ListBarGroup)) == null)))
            {
                // check if we're over an item:
                Point pt = new Point(e.X, e.Y);
                pt = PointToClient(pt);
                IMouseObject obj = HitTest(pt, true);
                newDragHoverOver = obj;

                if (obj != null)
                {
                    // Do scrolling checks on this bar.  Scrolling
                    // is rate limited to once per 250ms to assist
                    // with usability
                    TimeSpan diff = DateTime.Now.Subtract(lastScrollTime);
                    if (diff.Milliseconds > 250)
                    {
                        // Firstly, check if we're over an actual scroll button:
                        if (typeof (ListBarScrollButton).IsAssignableFrom(obj.GetType()))
                        {
                            // scroll in the appropriate direction:
                            ListBarScrollButton scrollButton = (ListBarScrollButton) obj;
                            Scroll(scrollButton, true);
                            lastScrollTime = DateTime.Now;
                        }
                        else
                        {
                            // Otherwise, we may be within the boundary of the
                            // scroll buttons:
                            if (btnUp.Visible)
                            {
                                Rectangle rcBtnUp = btnUp.Rectangle;
                                rcBtnUp.X = 0;
                                rcBtnUp.Width = ClientRectangle.Width;
                                if (rcBtnUp.Contains(pt))
                                {
                                    Scroll(1, true);
                                    lastScrollTime = DateTime.Now;
                                }
                            }
                            if (btnDown.Visible)
                            {
                                Rectangle rcBtnDown = btnDown.Rectangle;
                                rcBtnDown.X = 0;
                                rcBtnDown.Width = ClientRectangle.Width;
                                if (rcBtnDown.Contains(pt))
                                {
                                    Scroll(-1, true);
                                    lastScrollTime = DateTime.Now;
                                }
                            }
                        }
                    }


                    // Now check for being over an item or an empty bar:
                    if (typeof (ListBarItem).IsAssignableFrom(obj.GetType()))
                    {
                        ListBarItem item = (ListBarItem) obj;
                        object objDragItem = GetTypeOrSubClassFromData(e, typeof (ListBarItem));
                        bool itemEqualsDragItem = false;
                        ListBarItem dragItem = null;
                        if (objDragItem != null)
                        {
                            dragItem = (ListBarItem) objDragItem;
                            itemEqualsDragItem = item.Equals(dragItem);
                        }

                        if (!itemEqualsDragItem)
                        {
                            // we're over an item.

                            // Get the rectangle relative to the bar:
                            Rectangle rc = new Rectangle(item.Location, new Size(item.Width, item.Height));
                            // Adjust the rectangle so it's relative to the control:
                            ListBarGroup selectedGroup = SelectedGroup;
                            rc.Offset(0, selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight + selectedGroup.ScrollOffset);

                            // The commented section here is an 8 pixel
                            // margin from the top or bottom of an item,
                            // as per the Outlook bar.  I found the control
                            // more usable with a ListView style drag-drop
                            // approach, where the before/after decision
                            // is made depending on where you are relative
                            // to the centre of the item.

                            /* 
								** BEGIN: Outlook style drag-drop logic
							if (((pt.Y - rc.Top) > -8) && ((pt.Y - rc.Top) < 8))
							{
								itemBefore = item;
								// we can't go before the item which follows
								// the one we're dragging:
								if ((selectedGroup.Items.IndexOf(itemBefore) - 1) == 
									selectedGroup.Items.IndexOf(dragItem))
								{
									itemBefore = null;
								}

							}

							if (((rc.Bottom - pt.Y) > -8) && ((rc.Bottom - pt.Y) < 16))
							{
								itemAfter = item;
								// we can't go after the item which is before
								// the one we're dragging:
								if ((selectedGroup.Items.IndexOf(itemAfter) + 1) == 
									selectedGroup.Items.IndexOf(dragItem))
								{
									itemAfter = null;
								}
							}
								** END: Outlook style drag drop logic
								*/

                            // ListView style drag insert point logic.
                            if ((selectedGroup.View == ListBarGroupView.SmallIconsOnly) || (selectedGroup.View == ListBarGroupView.LargeIconsOnly))
                            {
                                int distRight = Math.Abs(rc.Right - pt.X);
                                int distLeft = Math.Abs(pt.X - rc.Left);
                                if (distRight < distLeft)
                                {
                                    itemAfter = item;
                                }
                                else
                                {
                                    itemBefore = item;
                                }
                            }
                            else
                            {
                                int distBottom = Math.Abs(rc.Bottom - pt.Y);
                                int distTop = Math.Abs(pt.Y - rc.Top);
                                if (distBottom < distTop)
                                {
                                    itemAfter = item;
                                }
                                else
                                {
                                    itemBefore = item;
                                }
                            }

                            if (itemAfter != null)
                            {
                                // we can't go after the item which is before
                                // the one we're dragging:
                                /*
								if ((selectedGroup.Items.IndexOf(itemAfter) + 1) == 
									selectedGroup.Items.IndexOf(dragItem))
								{
									itemAfter = null;
								}
								*/
                                if (itemAfter != null)
                                {
                                    // check there isn't an appropriate item before:
                                    if (selectedGroup.Items.IndexOf(itemAfter) < selectedGroup.Items.Count - 1)
                                    {
                                        itemBefore = selectedGroup.Items[selectedGroup.Items.IndexOf(itemAfter) + 1];
                                    }
                                }
                            }
                            else if (itemBefore != null)
                            {
                                // we can't go before the item which follows
                                // the one we're dragging:
                                /*
								if ((selectedGroup.Items.IndexOf(itemBefore) - 1) == 
									selectedGroup.Items.IndexOf(dragItem))
								{
									itemBefore = null;
								}
								*/
                                if (itemBefore != null)
                                {
                                    // check there isn't an appropriate item after:
                                    if (selectedGroup.Items.IndexOf(itemBefore) > 0)
                                    {
                                        itemAfter = selectedGroup.Items[selectedGroup.Items.IndexOf(itemBefore) - 1];
                                    }
                                }
                            }
                        }
                    }
                    else if (typeof (ListBarGroup).IsAssignableFrom(obj.GetType()))
                    {
                        overGroup = true;

                        // over a group
                        if (dragHoverOver != null)
                        {
                            if (dragHoverOver.Equals(obj))
                            {
                                TimeSpan overTime = DateTime.Now.Subtract(dragHoverOverStartTime);
                                if (overTime.Milliseconds > 350)
                                {
                                    // we should select this group:
                                    dragHoverOver = null;
                                    SelectGroup((ListBarGroup) obj);
                                    // Prevent the control from scrolling for a little bit.
                                    // TODO_
                                    // Actually what we really want to do here is to say
                                    // that unless the mouse moves > 4 pixels from this
                                    // spot scrolling will not occur in the new bar
                                    lastScrollTime = DateTime.Now.AddMilliseconds(500);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // we may be over the bar section:
                    ListBarGroup selectedGroup = SelectedGroup;

                    if (selectedGroup != null)
                    {
                        if (selectedGroup.Items.Count > 0)
                        {
                            // we're not over an item.  Check if we're 
                            // within the bar:
                            if (obj == null)
                            {
                                // Check if the selected group is the last group in
                                // the control:
                                ListBarGroup nextGroup = null;
                                for (int i = groups.IndexOf(selectedGroup) + 1; i < groups.Count; i++)
                                {
                                    if (groups[i].Visible)
                                    {
                                        nextGroup = groups[i];
                                        break;
                                    }
                                }

                                if (nextGroup == null)
                                {
                                    // If so the bar area extends from the bottom
                                    // of the button rectangle to the bottom of the 
                                    // control:
                                    if ((pt.Y > (selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight)) &&
                                        (pt.Y < ClientRectangle.Height))
                                    {
                                        itemAfter = selectedGroup.Items[selectedGroup.Items.Count - 1];
                                    }
                                }
                                else
                                {
                                    // Otherwise the bar area extends from the bottom
                                    // of the button rectangle of this control to the
                                    // top of the button rectangle of the next control:
                                    if ((pt.Y > (selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight)) &&
                                        (pt.Y < nextGroup.ButtonLocation.Y))
                                    {
                                        itemAfter = selectedGroup.Items[selectedGroup.Items.Count - 1];
                                    }
                                }
                            }
                        }
                        else
                        {
                            overEmptyBar = true;
                        }
                    }
                }
            }
            else if (GetTypeOrSubClassFromData(e, typeof (ListBarGroup)) != null)
            {
                itemAfter = null;
                itemBefore = null;

                // Here we check if we should drag the list bar
                // into a new position
                // check if we're over an item:
                Point pt = new Point(e.X, e.Y);
                pt = PointToClient(pt);
                IMouseObject obj = HitTest(pt, true);
                newDragHoverOver = obj;

                if (obj != null)
                {
                    if (typeof (ListBarGroup).IsAssignableFrom(obj.GetType()))
                    {
                        overGroup = true;

                        ListBarGroup dragGroup = (ListBarGroup) GetTypeOrSubClassFromData(e, typeof (ListBarGroup));

                        ListBarGroup dragOverGroup = (ListBarGroup) obj;
                        if (!dragOverGroup.Equals(dragGroup))
                        {
                            bool reSelect = false;
                            if (dragGroup.Selected)
                            {
                                reSelect = true;
                            }
                            bool isLastGroup = true;
                            for (int i = groups.IndexOf(dragOverGroup) + 1; i < groups.Count; i++)
                            {
                                if (groups[i].Visible)
                                {
                                    isLastGroup = false;
                                }
                            }
                            groups.Remove(dragGroup);
                            if (isLastGroup)
                            {
                                groups.Add(dragGroup);
                            }
                            else
                            {
                                groups.Insert(groups.IndexOf(dragOverGroup), dragGroup);
                            }
                            if (reSelect)
                            {
                                for (int i = 0; i < groups.Count; i++)
                                    groups[i].Selected = false;
                                dragGroup.Selected = true;
                            }
                            DoResize();
                            Invalidate();
                        }
                    }
                }
            }

            // Now check if we have any drag/drop insert position:
            if ((itemBefore != null) || (itemAfter != null) || (overEmptyBar))
            {
                e.Effect = DragDropEffects.Move;

                ListBarDragDropInsertPoint newInsertPoint =
                    new ListBarDragDropInsertPoint(itemBefore, itemAfter, overEmptyBar);

                // do we currently have an insert point?
                if (dragInsertPoint != null)
                {
                    if (dragInsertPoint.CompareTo(newInsertPoint) == 0)
                    {
                        // we have nothing to do
                        newInsertPoint = null;
                    }
                }

                if (newInsertPoint != null)
                {
                    Trace.WriteLine("Drag Insert Point has changed");
                    dragInsertPoint = newInsertPoint;
                    Invalidate();
                }
            }
            else
            {
                if (overGroup)
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }

                // Clear the drag insert point if it's set:
                if (dragInsertPoint != null)
                {
                    dragInsertPoint = null;
                    // redraw the control:
                    Invalidate();
                }
            }

            if ((newDragHoverOver != null) && (dragHoverOver != null))
            {
                if (!newDragHoverOver.Equals(dragHoverOver))
                {
                    dragHoverOver = newDragHoverOver;
                    dragHoverOverStartTime = DateTime.Now;
                }
                // else we keep the drag hover over time.
            }
            else
            {
                dragHoverOver = newDragHoverOver;
                dragHoverOverStartTime = DateTime.Now;
            }
        }

        #endregion

        #region API

        /// <summary>
        /// Gets/sets the default <see cref="System.Drawing.Font"/> used to 
        /// render text in the control.
        /// </summary>
        [Description("Gets/sets the Font used to render the text in this control.")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;

                // Force all of the ListBar items to be measured
                foreach (ListBarGroup group in groups)
                    GroupChanged(group, true);
            }
        }

        /// <summary>
        /// Gets/sets whether items are selected on MouseDown,
        /// rather than on MouseUp, which is the default.	
        /// </summary>
        [Description("Gets/sets whether items are selected on MouseDown, rather than on MouseUp.")]
        public bool SelectOnMouseDown
        {
            get
            {
                return selectOnMouseDown;
            }
            set
            {
                selectOnMouseDown = value;
            }
        }

        /// <summary>
        /// Gets/sets whether items will be dragged 
        /// in the control automatically.  The default
        /// is <c>True</c>.
        /// </summary>
        [Description("Gets/sets whether items will be dragged in the control automatically.  The default is True.")]
        public bool AllowDragItems
        {
            get
            {
                return allowDragItems;
            }
            set
            {
                allowDragItems = value;
            }
        }

        /// <summary>
        /// Gets/sets whether groups will be dragged 
        /// in the control automatically.  The default
        /// is <c>True</c>. (Note_ in MS Outlook
        /// Groups cannot be reordered by dragging, but
        /// they can in VS.NET).
        /// </summary>
        [Description("Gets/sets whether groups can be dragged automatically in the control.  The default is True.")]
        public bool AllowDragGroups
        {
            get
            {
                return allowDragGroups;
            }
            set
            {
                allowDragGroups = value;
            }
        }

        /// <summary>
        /// Gets the item which is currently being edited, if any,
        /// otherwise returns null.
        /// </summary>
        [Description("Gets the item which is currently being edited, if any, otherwise returns null.")]
        public ListBarItem EditItem
        {
            get
            {
                return editItem;
            }
        }

        /// <summary>
        /// Gets/sets how the ListBar control will be drawn.
        /// </summary>
        [Description("Gets/sets the style used to draw the ListBar control.")]
        public ListBarDrawStyle DrawStyle
        {
            get
            {
                return drawStyle;
            }
            set
            {
                drawStyle = value;
            }
        }

        /// <summary>
        /// Gets the collection of groups in the ListBar.
        /// </summary>
        [Description("Gets the collection of groups in the ListBar.")]
        public ListBarGroupCollection Groups
        {
            get
            {
                return groups;
            }
        }

        /// <summary>
        /// Gets/sets the tooltip object associated with this control.
        /// The control does not generate its own internal Tooltips
        /// and instead relies on an external ToolTip object to
        /// display tooltips.
        /// </summary>
        [Description("Gets/sets the tooltip object which will be used to show tooltips for groups and items in the control.")]
        public ToolTip ToolTip
        {
            get
            {
                return toolTip;
            }
            set
            {
                toolTip = value;
            }
        }

        /// <summary>
        /// Gets/sets the large icon ImageList to be used for items 
        /// with the <see cref="ListBarGroupView.LargeIcons"/> and 
        /// <see cref="ListBarGroupView.LargeIconsOnly"/> view.
        /// </summary>
        [Description("Gets/sets the large icon ImageList to be used for items in groups with the LargeIcons or LargeIconsOnly view.")]
        public ImageList LargeImageList
        {
            get
            {
                return largeImageList;
            }
            set
            {
                largeImageList = value;
            }
        }

        /// <summary>
        /// Gets/sets the small icon ImageList to be used for ListBar groups
        /// using the <see cref="ListBarGroupView.SmallIcons"/> or 
        /// <see cref="ListBarGroupView.SmallIconsOnly "/> view.
        /// </summary>
        [Description("Gets/sets the small icon ImageList to be used for items in groups with the SmallIcons or SmallIconsOnly view.")]
        public ImageList SmallImageList
        {
            get
            {
                return smallImageList;
            }
            set
            {
                smallImageList = value;
            }
        }

        /// <summary>
        /// Returns the currently selected group in the ListBar control,
        /// if any.
        /// </summary>
        [Description("Gets the currently selected group in the ListBar control.")]
        public virtual ListBarGroup SelectedGroup
        {
            get
            {
                ListBarGroup selGroup = null;
                if (groups.Count > 0)
                {
                    foreach (ListBarGroup group in groups)
                    {
                        if (group.Selected)
                        {
                            selGroup = group;
                            break;
                        }
                    }
                }
                return selGroup;
            }
        }

        /// <summary>
        /// Called by the control's internal sizing mechanism.
        /// Returns the size of a <see cref="ListBarGroup"/> button
        /// rectangle.
        /// </summary>
        /// <param name="group">The <see cref="ListBarGroup"/> to get the 
        /// button width for.</param>
        /// <returns>The width of the button.</returns>
        protected virtual int GetGroupButtonWidth(ListBarGroup group)
        {
            return ClientRectangle.Width - 2;
        }

        /// <summary>
        /// Sets the groups object associated with this control
        /// to a new group collection.
        /// </summary>
        /// <param name="groups">The <see cref="ListBarGroupCollection"/> object holding
        /// the new collection of groups to associate with this control.</param>
        [Description("Sets the internal collection holding the Groups associated with this control to a new object.")]
        public void SetGroups(ListBarGroupCollection groups)
        {
            BeginUpdate();
            groups.SetOwner(this);
            EndUpdate();
            this.groups = groups;
            DoResize();
            Invalidate();
        }

        /// <summary>
        /// Prevents the control from drawing until the 
        /// <see cref="EndUpdate"/> method is called.
        /// </summary>
        [Description("Prevents the control from drawing until the EndUpdate method is called.")]
        public void BeginUpdate()
        {
            redraw = false;
        }

        /// <summary>
        /// Resumes drawing of the control after drawing was suspended by the 
        /// <see cref="BeginUpdate"/> method.		
        /// </summary>
        [Description("Resumes drawing of the control after drawing was suspended by the BeginUpdate method.")]
        public void EndUpdate()
        {
            redraw = true;
            DoResize();
            Invalidate();
        }

        /// <summary>
        /// Renders the control when a new group is being scrolled
        /// into view.
        /// </summary>
        /// <param name="pe">The arguments associated with the paint
        /// event.</param>
        [Description("Renders the control as a new group is being scrolled into view")]
        protected virtual void RenderScrollNewGroup(PaintEventArgs pe)
        {
            int lastBar = 0;
            int currentNext = groups.Count - 1;
            int newNext = groups.Count - 1;
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Visible)
                {
                    if (i > lastBar)
                    {
                        lastBar = i;
                    }
                    if ((i > indexCurrent) && (i < currentNext))
                    {
                        currentNext = i;
                    }
                    if ((i > indexNew) && (i < newNext))
                    {
                        newNext = i;
                    }
                }
            }

            ListBarGroup currentBar = groups[indexCurrent];
            ListBarGroup newBar = groups[indexNew];

            // get the rectangle for currentBar:
            Rectangle currentBarBounds = new Rectangle(
                currentBar.ButtonLocation,
                new Size(currentBar.ButtonWidth, 0));
            // the height of the current bar rect is the height of the control
            // or the top of the rectangle of the next button along:
            if (indexCurrent == lastBar)
            {
                currentBarBounds.Height = ClientRectangle.Height -
                                          currentBarBounds.Top;
            }
            else
            {
                currentBarBounds.Height = groups[currentNext].ButtonLocation.Y -
                                          currentBarBounds.Top;
            }

            // get the rectangle for newBar:
            Rectangle newBarBounds = new Rectangle(
                newBar.ButtonLocation,
                new Size(newBar.ButtonWidth, 0));
            // the height of the new bar is the height of the control or
            // the top of the rectangle of the next bar along:
            if (indexNew == lastBar)
            {
                newBarBounds.Height = ClientRectangle.Height -
                                      newBarBounds.Top;
            }
            else
            {
                newBarBounds.Height = groups[newNext].ButtonLocation.Y -
                                      newBarBounds.Top;
            }

            // Draw the current bar contents:
            currentBar.DrawBar(
                pe.Graphics, currentBarBounds,
                (currentBar.View == ListBarGroupView.LargeIcons ? largeImageList : smallImageList),
                Font,
                drawStyle,
                Enabled);

            // Draw the new bar contents:
            newBar.DrawBar(
                pe.Graphics, newBarBounds,
                (newBar.View == ListBarGroupView.LargeIcons ? largeImageList : smallImageList),
                Font,
                drawStyle,
                Enabled);

            // Draw the buttons:
            foreach (ListBarGroup group in groups)
                group.DrawButton(pe.Graphics, Font, Enabled);

            // Draw the border:
            RenderControlBorder(pe.Graphics);
        }

        /// <summary>
        /// Renders the control given the object passed to a Paint event.
        /// </summary>
        /// <param name="pe">The arguments associated with the paint
        /// event.</param>
        protected virtual void Render(PaintEventArgs pe)
        {
            if (redraw)
            {
                // background - does not need to be painted
                // as the control does it itself

                // draw the control elements:
                if (groups.Count > 0)
                {
                    // First draw the items in the selected group,
                    // if any:
                    ListBarGroup selectedGroup = SelectedGroup;

                    if ((selectedGroup != null) && (selectedGroup.Visible))
                    {
                        // Draw the items in the group:
                        selectedGroup.DrawBar(pe.Graphics, rcListView,
                                              ((selectedGroup.View == ListBarGroupView.LargeIcons ||
                                                selectedGroup.View == ListBarGroupView.LargeIconsOnly)
                                                   ? largeImageList : smallImageList),
                                              Font,
                                              drawStyle,
                                              Enabled);

                        // Render the drag-drop insertion point, if any:
                        RenderDragInsertPoint(pe.Graphics,
                                              selectedGroup);
                    }

                    // draw the scroll buttons:
                    Color defaultBackColor = Color.FromKnownColor(KnownColor.Control);
                    if (drawStyle == ListBarDrawStyle.ListBarDrawStyleNormal)
                    {
                        if (!BackColor.Equals(Color.FromKnownColor(KnownColor.ControlDark)))
                        {
                            defaultBackColor = BackColor;
                        }
                    }
                    else if (DrawStyle == ListBarDrawStyle.ListBarDrawStyleOfficeXP)
                    {
                        defaultBackColor = BackColor;
                    }

                    btnUp.DrawItem(pe.Graphics, defaultBackColor, Enabled);
                    btnDown.DrawItem(pe.Graphics, defaultBackColor, Enabled);

                    // Now draw the group buttons, if any:
                    foreach (ListBarGroup group in groups)
                    {
                        if (group.Visible)
                        {
                            group.DrawButton(pe.Graphics, Font, Enabled);
                        }
                    }
                }

                // border:
                RenderControlBorder(pe.Graphics);
            }
        }

        /// <summary>
        /// Draw a border around the control.  The default
        /// implementation draws a 1 pixel inset border.
        /// </summary>
        /// <param name="gfx">The graphics object to drawn onto.</param>
        protected virtual void RenderControlBorder(
            Graphics gfx)
        {
            // draw the control's border
            Pen darkPen = new Pen(CustomBorderColor.ColorDark(BackColor));
            Pen lightPen = new Pen(CustomBorderColor.ColorLightLight(BackColor));
            gfx.DrawLine(darkPen,
                         ClientRectangle.Left, ClientRectangle.Bottom - 2,
                         ClientRectangle.Left, ClientRectangle.Top);
            gfx.DrawLine(darkPen,
                         ClientRectangle.Left, ClientRectangle.Top,
                         ClientRectangle.Right - 2, ClientRectangle.Top);
            gfx.DrawLine(lightPen,
                         ClientRectangle.Right - 1, ClientRectangle.Top,
                         ClientRectangle.Right - 1, ClientRectangle.Bottom - 1);
            gfx.DrawLine(lightPen,
                         ClientRectangle.Right - 1, ClientRectangle.Bottom - 1,
                         ClientRectangle.Left, ClientRectangle.Bottom - 1);
            darkPen.Dispose();
            lightPen.Dispose();
        }

        /// <summary>
        /// Draws the drag insert point, if any.  The drag insert point is
        /// drawn using the same style as the Windows XP ListView drag
        /// insert point.
        /// 
        /// Note_ that the Outlook ListBar draws a single pixel drag insert
        /// point rather than a double width one.  I preferred the ListView 
        /// XP style so went with this.  The code can be overridden to
        /// use a single pixel border instead if desired. 
        /// </summary>
        /// <param name="gfx">The graphics object to draw onto.</param>
        /// <param name="selectedGroup">The currently selected ListBarGroup.</param>
        protected virtual void RenderDragInsertPoint(
            Graphics gfx,
            ListBarGroup selectedGroup
            )
        {
            if (dragInsertPoint != null)
            {
                ListBarItem itemAfter = dragInsertPoint.ItemAfter;
                ListBarItem itemBefore = dragInsertPoint.ItemBefore;

                int offset = (selectedGroup.View == ListBarGroupView.LargeIcons ? 2 : 0);

                if (itemAfter != null)
                {
                    // Get the bounding rectangle of the item after:
                    Rectangle rcItem = new Rectangle(itemAfter.Location, new Size(itemAfter.Width, itemAfter.Height));
                    // adjust the rectangle so it corresponds to the display:
                    rcItem.Offset(0,
                                  selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight +
                                  selectedGroup.ScrollOffset);

                    // Draw the insertion point line:
                    if ((selectedGroup.View == ListBarGroupView.SmallIconsOnly) || (selectedGroup.View == ListBarGroupView.LargeIconsOnly))
                    {
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Right, rcItem.Top + 2,
                                     rcItem.Right, rcItem.Bottom - 1);
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Right + 1, rcItem.Top + 2,
                                     rcItem.Right + 1, rcItem.Bottom - 1);

                        // Draw triangles:
                        if (itemBefore != null)
                        {
                            // left triangles:
                            ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                                  new Point(rcItem.Right + 1, rcItem.Top + 2), 5, 5);
                            ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                                  new Point(rcItem.Right + 1, rcItem.Bottom), 5, -6);
                        }

                        // right triangles:
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Right, rcItem.Top + 2), -4, 4);
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Right, rcItem.Bottom), -5, -6);
                    }
                    else
                    {
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 7, rcItem.Bottom + offset,
                                     rcItem.Right - 7, rcItem.Bottom + offset);
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 7, rcItem.Bottom + offset + 1,
                                     rcItem.Right - 7, rcItem.Bottom + offset + 1);

                        // Draw the triangles:
                        if (itemBefore != null)
                        {
                            // below triangles:
                            ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                                  new Point(rcItem.Left + 7, rcItem.Bottom + offset + 1), 10, 5);
                            ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                                  new Point(rcItem.Right - 6, rcItem.Bottom + offset + 1), -10, 5);
                        }

                        // above triangles
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Left + 7, rcItem.Bottom + offset), 10, -5);
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Right - 6, rcItem.Bottom + offset), -10, -5);
                    }
                }
                else
                {
                    // before the first item:

                    // Get the bounding rectangle of the item after:
                    Rectangle rcItem;
                    if (itemBefore != null)
                    {
                        rcItem = new Rectangle(itemBefore.Location, new Size(Width, itemBefore.Height));
                        // adjust the rectangle so it corresponds to the display:
                        rcItem.Offset(0,
                                      selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight +
                                      selectedGroup.ScrollOffset);
                    }
                    else
                    {
                        rcItem = new Rectangle(selectedGroup.ButtonLocation,
                                               new Size(selectedGroup.ButtonWidth, selectedGroup.ButtonHeight));
                        rcItem.Offset(0, selectedGroup.ButtonHeight);
                    }

                    // draw the insertion point line:
                    if ((selectedGroup.View == ListBarGroupView.SmallIconsOnly) || (selectedGroup.View == ListBarGroupView.LargeIconsOnly))
                    {
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 1, rcItem.Top + 2,
                                     rcItem.Left + 1, rcItem.Bottom - 1);
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 2, rcItem.Top + 2,
                                     rcItem.Left + 2, rcItem.Bottom - 1);

                        // left triangles:
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Left + 2, rcItem.Top + 2), 5, 5);
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Left + 2, rcItem.Bottom), 5, -6);
                    }
                    else
                    {
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 7, rcItem.Top + offset,
                                     rcItem.Right - 7, rcItem.Top + offset);
                        gfx.DrawLine(SystemPens.WindowText,
                                     rcItem.Left + 7, rcItem.Top + offset + 1,
                                     rcItem.Right - 7, rcItem.Top + offset + 1);

                        // Now draw the triangles:
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Left + 7, rcItem.Top + offset + 1), 10, 5);
                        ListBarUtility.FillRightAngleTriangle(gfx, SystemBrushes.WindowText,
                                                              new Point(rcItem.Right - 6, rcItem.Top + offset + 1), -10, 5);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the ListBarGroupCollection used by this
        /// control to store the ListBarGroups.  Fired during control 
        /// initialisation.
        /// </summary>
        /// <returns>A new instance of the ListBarGroupCollection to be used
        /// by the control to store ListBarGroups.</returns>
        protected virtual ListBarGroupCollection CreateListBarGroupCollection()
        {
            return new ListBarGroupCollection(this);
        }

        /// <summary>
        /// Creates a new instance of a ListBarScrollButton used by this control
        /// to draw the scroll buttons.  Fired during control initialisation
        /// </summary>
        /// <param name="buttonType">The type of scroll button (Up or Down)
        /// to create</param>
        /// <returns>A new ListBarScrollButton which is drawn when a ListBar
        /// contains more items than can be displayed.</returns>
        protected virtual ListBarScrollButton CreateListBarScrollButton(ListBarScrollButton.ListBarScrollButtonType buttonType)
        {
            return new ListBarScrollButton(buttonType);
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtEdit
            // 
            this.txtEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEdit.Location = new System.Drawing.Point(60, 92);
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.Size = new System.Drawing.Size(80, 44);
            this.txtEdit.TabIndex = 0;
            this.txtEdit.Text = "";
            this.txtEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtEdit.Visible = false;
            this.txtEdit.TextChanged += new System.EventHandler(this.txtEdit_TextChanged);
            // 
            // ListBar
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                                       {
                                           this.txtEdit
                                       });
            this.Name = "ListBar";
            this.ResumeLayout(false);
        }

        #endregion
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// Enumeration specifying the view to use for the items within
    /// a <see cref="ListBarGroup"/>.
    /// </summary>
    [Description("Enumeration specifying the view to use for the items within a group.")]
    public enum ListBarGroupView
    {
        /// <summary>
        /// The ListBar will display using large icons, with the text underneath.
        /// </summary>
        [Description("The ListBar will display using large icons, with the text underneath.")] LargeIcons,

        /// <summary>
        /// The ListBar will display using small icons, with text to the left.
        /// </summary>
        [Description("The ListBar will display using small icons, with text to the left.")] SmallIcons,

        /// <summary>
        /// The ListBar will display using large icons with no text.
        /// </summary>
        [Description("The ListBar will display large icons with no text.")] LargeIconsOnly,

        /// <summary>
        /// The ListBar will display using small icons with no text.
        /// </summary>
        [Description("The ListBar will display small icons with no text.")] SmallIconsOnly
    }

    /// <summary>
    /// Enumeration specifying how the <see cref="ListBar"/> control will draw.
    /// </summary>
    [Description("Enumeration specifying the ListBar control drawing style.")]
    public enum ListBarDrawStyle
    {
        /// <summary>
        /// The ListBar will draw using the style of the original Office
        /// releases.
        /// </summary>
        [Description("The ListBar will draw using the style of the original Office releases.")] ListBarDrawStyleNormal,

        /// <summary>
        /// The ListBar will draw using the Office XP style.
        /// </summary>
        [Description("The ListBar will draw using the Office XP style.")] ListBarDrawStyleOfficeXP,

        /// <summary>
        /// The ListBar will draw using the Office 2003 style
        /// (not implemented yet).
        /// </summary>
        [Description("The ListBar will draw using the Office 2003 style (not implemented yet).")] ListBarDrawStyleOffice2003
    }

    #endregion

    #region Event argument classes

    /// <summary>
    /// Provides details about an item which will undergo
    /// an edit operation.
    /// </summary>
    public class ListBarLabelEditEventArgs : LabelEditEventArgs
    {
        private object labelEditObject;

        /// <summary>
        /// Constructs a new instance of this object
        /// given the item, label and object.
        /// </summary>
        /// <param name="item">The index of the item being edited.</param>
        /// <param name="label">The label of the item being edited.</param>
        /// <param name="labelEditObject">The object being edited.</param>
        [Description("Constructs a new instance of this object.")]
        public ListBarLabelEditEventArgs(
            int item,
            string label,
            object labelEditObject
            ) : base(item, label)
        {
            this.labelEditObject = labelEditObject;
        }

        /// <summary>
        /// Returns the object for which label editing has
        /// been requested.  Can either be a <see cref="ListBarItem"/> or
        /// a <see cref="ListBarGroup"/> (or a subclass of either).
        /// </summary>
        [Description("Gets the object for which label editing has been requested.  Either a ListBarItem or a ListBarGroup (or a subclass)")]
        public object LabelEditObject
        {
            get
            {
                return labelEditObject;
            }
        }
    }

    /// <summary>
    /// Provides event arguments for the BeforeSelectedGroupChanged event
    /// raised by the control.  This object contains the group that
    /// would be selected and provides the opportunity to cancel the 
    /// group selection.
    /// </summary>
    public class BeforeGroupChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to cancel the operation or not.
        /// </summary>
        private bool cancel;

        /// <summary>
        /// The ListBarGroup that would be selected.
        /// </summary>
        private ListBarGroup group;

        /// <summary>
        /// Constructs a new instance of this object.
        /// Called
        /// by the <see cref="ListBar"/> control before firing a 
        /// <c>BeforeSelectedGroupChanged</c> event.
        /// </summary>
        /// <param name="group">The group that will be selected</param>
        [Description("Constructs a new instance of this object.")]
        public BeforeGroupChangedEventArgs(
            ListBarGroup group
            )
        {
            this.group = group;
        }

        /// <summary>
        /// Gets the group that will be selected.
        /// </summary>
        [Description("Gets the group that will be selected.")]
        public ListBarGroup Group
        {
            get
            {
                return @group;
            }
        }

        /// <summary>
        /// Gets/sets whether the group selection should be cancelled
        /// or not. By default the group selection is not cancelled.
        /// </summary>
        [Description("Gets/sets whether the group selection should be cancelled.")]
        public bool Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
            }
        }
    }

    /// <summary>
    /// This class is used with the BeforeItemClicked event and provides
    /// the item which is about to be clicked and the option to prevent
    /// the item being clicked by setting the Cancel property.
    /// </summary>
    public class BeforeItemClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Whether the click should be cancelled or not.
        /// </summary>
        private bool cancel;

        /// <summary>
        /// The ListBarItem which is about to be clicked.
        /// </summary>
        private ListBarItem item;

        /// <summary>
        /// Constructor for this object. Called
        /// by the <see cref="ListBar"/> control before firing a 
        /// <see cref="BeforeItemClickedEventHandler"/> event.
        /// </summary>
        /// <param name="item">The item that's about to be clicked.</param>
        [Description("Constructs a new instance of this object.")]
        public BeforeItemClickedEventArgs(
            ListBarItem item
            )
        {
            this.item = item;
        }

        /// <summary>
        /// Gets/sets whether the click should be cancelled or not.
        /// </summary>
        [Description("Gets/sets whether the click should be cancelled or not.")]
        public bool Cancel
        {
            get
            {
                return cancel;
            }
            set
            {
                cancel = value;
            }
        }

        /// <summary>
        /// Gets the ListBarItem that is about to be clicked.
        /// </summary>
        [Description("Gets the ListBarItem that is about to be clicked.")]
        public ListBarItem Item
        {
            get
            {
                return item;
            }
        }
    }

    /// <summary>
    /// This class is provides details of which item has been clicked
    /// and the mouse details of the click when the <c>ItemClicked</c> event
    /// is raised from a <c>ListBar</c>.
    /// <seealso cref="ListBar.ItemClicked"/>
    /// </summary>
    public class ItemClickedEventArgs : ObjectClickedEventArgs
    {
        /// <summary>
        /// The ListBarIem that has been clicked.
        /// </summary>
        private ListBarItem item;

        /// <summary>
        /// Constructs a new instance of this object.  Called by the <see cref="ListBar"/>
        /// control when firing an <c>ItemClicked</c> event.
        /// </summary>
        /// <param name="item">The item that has been clicked</param>
        /// <param name="location">The mouse location relative to the 
        /// control for the click.</param>
        /// <param name="mouseButton">The mouse button used to click
        /// the item.</param>
        [Description("Constructs a new instance of this object")]
        public ItemClickedEventArgs(
            ListBarItem item,
            Point location,
            MouseButtons mouseButton
            ) : base(location, mouseButton)
        {
            this.item = item;
        }

        /// <summary>
        /// Gets the <see cref="ListBarItem"/> that has been clicked.
        /// </summary>
        [Description("Gets the ListBarItem that has been clicked.")]
        public ListBarItem Item
        {
            get
            {
                return item;
            }
        }
    }

    /// <summary>
    /// This class is provides details of which item has been clicked
    /// and the mouse details of the click when the <c>GroupClicked</c> event
    /// is raised from a <see cref="ListBar" /> control.
    /// </summary>
    public class GroupClickedEventArgs : ObjectClickedEventArgs
    {
        /// <summary>
        /// The ListBarGroup that has been clicked.
        /// </summary>
        private ListBarGroup group;

        /// <summary>
        /// Constructs a new instance of this object.  Called by the <see cref="ListBar"/>
        /// control when firing a <c>GroupClicked</c> event.
        /// </summary>
        /// <param name="group">The <see cref="ListBarGroup"/> that has been clicked</param>
        /// <param name="location">The mouse location relative to the 
        /// control for the click.</param>
        /// <param name="mouseButton">The mouse button used to click
        /// the item.</param>
        [Description("Constructs a new instance of this object.")]
        public GroupClickedEventArgs(
            ListBarGroup group,
            Point location,
            MouseButtons mouseButton
            ) : base(location, mouseButton)
        {
            this.group = group;
        }

        /// <summary>
        /// Gets the <see cref="ListBarGroup"/> that has been clicked.
        /// </summary>
        [Description("Gets the ListBarGroup that has been clicked.")]
        public ListBarGroup Group
        {
            get
            {
                return @group;
            }
        }
    }

    /// <summary>
    /// An abstract class used as the bases for the <c>ItemClicked</c>
    /// and <c>GroupClicked</c> events of the <see cref="ListBar"/> control.
    /// This class stores details of the mouse location and button.
    /// </summary>
    public abstract class ObjectClickedEventArgs : EventArgs
    {
        /// <summary>
        /// The location of the mouse when the item was clicked.
        /// </summary>
        private Point location;

        /// <summary>
        /// The mouse button that was used.
        /// </summary>
        private MouseButtons mouseButton = MouseButtons.Left;

        /// <summary>
        /// When used in a subclass, constructs a new instance of the class with the specified
        /// mouse location and button.
        /// </summary>
        /// <param name="location">The location of the mouse.</param>
        /// <param name="mouseButton">The button which was pressed.</param>
        [Description("When used in a subclass, constructs a new instance of this class.")]
        public ObjectClickedEventArgs(
            Point location,
            MouseButtons mouseButton
            )
        {
            this.location = location;
            this.mouseButton = mouseButton;
        }

        /// <summary>
        /// The Location of the mouse, relative to the control,
        /// when the item was clicked.
        /// </summary>
        [Description("The location of the mouse relative to the control when the item was clicked.")]
        public Point Location
        {
            get
            {
                return location;
            }
        }


        /// <summary>
        /// The MouseButton used to click the item.
        /// </summary>
        [Description("The mouse button used to click this item.")]
        public MouseButtons MouseButton
        {
            get
            {
                return mouseButton;
            }
        }
    }

    #endregion

    #region Event delegates

    /// <summary>
    /// Represents the method that handles the BeforeSelectedGroupChanged event
    /// of a ListBar control.
    /// </summary>
    public delegate void BeforeGroupChangedEventHandler(
        object sender,
        BeforeGroupChangedEventArgs e);

    /// <summary>
    /// Represents the method that handles the BeforeItemClicked event
    /// of a ListBar control.
    /// </summary>
    public delegate void BeforeItemClickedEventHandler(
        object sender,
        BeforeItemClickedEventArgs e);

    /// <summary>
    /// Represents the method that handles the ItemClicked event of a
    /// ListBar control.
    /// </summary>
    public delegate void ItemClickedEventHandler(
        object sender,
        ItemClickedEventArgs e);

    /// <summary>
    /// Represents the method that handles the GroupClicked event of a
    /// ListBar control.
    /// </summary>
    public delegate void GroupClickedEventHandler(
        object sender,
        GroupClickedEventArgs e);

    /// <summary>
    /// Represents the method that handles the BeforeLabelEdit and AfterLabelEdit
    /// events of a ListBar control.
    /// </summary>
    public delegate void ListBarLabelEditEventHandler(
        object sender,
        ListBarLabelEditEventArgs e);

    #endregion

    #region ListBarGroupCollection class

    /// <summary>
    /// A class to hold the collection of groups in the ListBar control.
    /// </summary>
    [SerializableAttribute]
    public class ListBarGroupCollection : CollectionBase, ISerializable
    {
        /// <summary>
        /// The ListBar which owns this collection
        /// </summary>
        private ListBar owner;

        /// <summary>
        /// 
        /// TODO_ This method has not been implemented yet.
        /// 
        /// Constructs this object from a serialized representation.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// containing the serialized data to build this object from.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public ListBarGroupCollection(
            SerializationInfo info,
            StreamingContext context)
        {
        }

        /// <summary>
        /// Creates a new instance of the ListBarGroup collection and associates
        /// it with the control which owns it.
        /// </summary>
        /// <param name="owner">The owning ListBar control.</param>
        public ListBarGroupCollection(ListBar owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets the <see cref="ListBarGroup"/> at the specified 0-based index.
        /// </summary>
        [Description("(Gets the ListBarGroup at the specified 0-based index.")]
        public ListBarGroup this[int index]
        {
            get
            {
                return (ListBarGroup) InnerList[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="ListBarGroup"/> with the specified string key.
        /// </summary>
        [Description("(Gets the ListBarGroup with the specified string key.")]
        public ListBarGroup this[string key]
        {
            get
            {
                ListBarGroup ret = null;
                foreach (ListBarGroup group in InnerList)
                {
                    if (group.Key.Equals(key))
                    {
                        ret = group;
                        break;
                    }
                }
                return ret;
            }
        }

        #region ISerializable Members

        /// <summary>
        /// 
        /// TODO_ This method has not been implemented yet.
        /// 
        /// Populates a System.Runtime.Serialization.SerializationInfo object with the 
        /// data needed to serialize this object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// to populate with data.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
        }

        #endregion

        /// <summary>
        /// Adds a new <see cref="ListBarGroup"/> to the control.
        /// </summary>
        /// <param name="group">The group to add to the control</param>
        /// <returns>The index at which the group was added.</returns>
        [Description("Adds a new ListBarGroup to the control.")]
        public virtual int Add(ListBarGroup group)
        {
            int ret = InnerList.Add(group);
            group.SetOwner(owner);
            return ret;
        }

        /// <summary>
        /// Adds a new <see cref="ListBarGroup"/> with the specified caption to
        /// the control and returns a reference to it.
        /// </summary>
        /// <param name="caption">The caption for the new ListBarGroup.</param>
        /// <returns>The ListBarGroup added to the control.</returns>
        [Description("Adds a new ListBarGroup with the specified caption to the control and returns a reference to it.")]
        public virtual ListBarGroup Add(
            string caption
            )
        {
            ListBarGroup group = new ListBarGroup(caption);
            InnerList.Add(group);
            group.SetOwner(owner);
            return group;
        }

        /// <summary>
        /// Adds a series of <see cref="ListBarGroup"/> objectss based on the supplied captions.
        /// </summary>
        /// <param name="captions">The array of captions to use when creating
        /// the <see cref="ListBarGroup"/> objects.</param>
        [Description("Adds a series of ListBarGroups with the specified captions to the control.")]
        public virtual void AddRange(
            string[] captions
            )
        {
            foreach (string caption in captions)
                Add(caption);
        }

        /// <summary>
        /// Adds a range of previously defined <see cref="ListBarGroup" /> objects.
        /// </summary>
        /// <param name="values">The array of ListBarGroups to add
        /// to the control.</param>
        [Description("Adds a range of previously defined ListBarGroup objects.")]
        public virtual void AddRange(
            ListBarGroup[] values
            )
        {
            foreach (ListBarGroup group in values)
            {
                InnerList.Add(group);
                group.SetOwner(owner);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="ListBarGroup"/> element is contained within 
        /// the control's collection of groups.
        /// </summary>
        /// <param name="group">The ListBarGroup to check if present.</param>
        /// <returns>True if the ListBarGroup is contained within the control's
        /// collection, False otherwise.</returns>
        [Description("Determins whether a ListBarGroup element is contained within the control's collection of groups.")]
        public virtual bool Contains(ListBarGroup group)
        {
            return InnerList.Contains(group);
        }

        /// <summary>
        /// Gets the 0-based index of the specified <see cref="ListBarGroup"/> within this
        /// collection.
        /// </summary>
        /// <param name="group">The group to find the index for.</param>
        /// <returns>The 0-based index of the group, if found, otherwise - 1.</returns>
        [Description("Gets the 0-based index of the specified ListBarGroup within this collection.")]
        public virtual int IndexOf(ListBarGroup group)
        {
            return InnerList.IndexOf(group);
        }

        /// <summary>
        /// Inserts a group at the specified 0-based index in the collection
        /// of groups.
        /// </summary>
        /// <param name="index">The 0-based index to insert the group at.</param>
        /// <param name="group">The ListBarGroup to add.</param>
        [Description("(Inserts a group at the specified 0-based index in the collection of groups.")]
        public virtual void Insert(int index, ListBarGroup group)
        {
            InnerList.Insert(index, group);
            group.SetOwner(owner);
        }

        /// <summary>
        /// Inserts a group immediately before the specified <see cref="ListBarGroup"/>.
        /// </summary>
        /// <param name="groupBefore">ListBarGroup to insert before.</param>
        /// <param name="group">Group to insert.</param>
        [Description("(Inserts a group immediately before the specified ListBarGroup object.")]
        public virtual void InsertBefore(ListBarGroup groupBefore, ListBarGroup group)
        {
            InnerList.Insert(InnerList.IndexOf(groupBefore), group);
            group.SetOwner(owner);
        }

        /// <summary>
        /// Inserts a <see cref="ListBarGroup"/> immediately after the specified ListBarGroup.
        /// </summary>
        /// <param name="groupAfter">ListBarGroup to insert after.</param>
        /// <param name="group">Group to insert.</param>
        [Description("(Inserts a group immediately after the specified ListBarGroup object.")]
        public virtual void InsertAfter(ListBarGroup groupAfter, ListBarGroup group)
        {
            int index = InnerList.IndexOf(groupAfter);
            if (index == InnerList.Count - 1)
            {
                Add(group);
            }
            else
            {
                Insert(index + 1, group);
            }
        }


        /// <summary>
        /// Removes the specified <see cref="ListBarGroup"/>.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        [Description("(Removes the specified ListBarGroup object.")]
        public virtual void Remove(ListBarGroup group)
        {
            InnerList.Remove(group);
            NotifyOwner(group, true);
        }

        /// <summary>
        /// Notifies the owning ListBar control of any changes to a group.
        /// </summary>
        /// <param name="group">The Group which has changed.</param>
        /// <param name="addRemove">Whether the control should resize
        /// all groups associated with the ListBar.</param>
        protected virtual void NotifyOwner(ListBarGroup group, bool addRemove)
        {
            if (owner != null)
            {
                owner.GroupChanged(group, addRemove);
            }
        }

        /// <summary>
        /// Notifies the control after clearing all groups.
        /// </summary>
        protected override void OnClearComplete()
        {
            NotifyOwner(null, true);
        }

        /// <summary>
        /// Notifies the control after inserting a new ListBarGroup.
        /// </summary>
        protected override void OnInsertComplete(Int32 index, Object value)
        {
            NotifyOwner((ListBarGroup) value, true);
        }

        /// <summary>
        /// Notifies the control after removing a new ListBarGroup.
        /// </summary>
        protected override void OnRemoveComplete(Int32 index, Object value)
        {
            NotifyOwner(null, true);
        }

        /// <summary>
        /// Notifies the control after setting a ListBarGroup to another ListBarGroup.
        /// </summary>
        protected override void OnSetComplete(Int32 index, Object oldValue, Object newValue)
        {
            NotifyOwner((ListBarGroup) newValue, false);
        }

        /// <summary>
        /// Enables a deserialized object graph to be associated with a ListBar
        /// control.
        /// </summary>
        /// <param name="owner">The ListBar control which will own
        /// this collection of items.</param>
        public virtual void SetOwner(ListBar owner)
        {
            this.owner = owner;
            foreach (ListBarGroup group in InnerList)
                group.SetOwner(owner);
        }
    }

    #endregion

    #region ListBarGroup class

    /// <summary>
    /// A <c>ListBarGroup</c> is a bar within a <see cref="ListBar"/> control.
    /// A bar can either contain items or it can contain a Windows
    /// Forms control.
    /// </summary>
    [SerializableAttribute]
    public class ListBarGroup : IMouseObject, ISerializable
    {
        /// <summary>
        /// BackColor to render this group with.
        /// </summary>
        private Color backColor = Color.FromKnownColor(KnownColor.Control);

        /// <summary>
        /// The caption of the group.
        /// </summary>
        private string caption = "";

        /// <summary>
        /// A child control to display in this bar instead
        /// of the child items.
        /// </summary>
        private Control childControl;

        /// <summary>
        /// Font to render this group with.
        /// </summary>
        private Font font;

        /// <summary>
        /// ForeColor to render this group with.
        /// </summary>
        private Color foreColor = Color.FromKnownColor(KnownColor.WindowText);

        /// <summary>
        /// The view (LargeIcons or SmallIcons) to use when drawing the items 
        /// in the bar.
        /// </summary>
        private ListBarGroupView iconSize = ListBarGroupView.LargeIcons;

        /// <summary>
        /// The collection of items associated with this 
        /// group.
        /// </summary>
        private ListBarItemCollection items;

        /// <summary>
        /// The string key to associate with this item.
        /// </summary>
        private string key = "";

        /// <summary>
        /// Whether the mouse is down on the button or not.
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// The point at which the mouse was clicked on the group
        /// button.
        /// </summary>
        private Point mouseDownPoint = new Point(0, 0);

        /// <summary>
        /// Whether the mouse is over the button or not.
        /// </summary>
        private bool mouseOver;

        /// <summary>
        /// The owning control.
        /// </summary>
        private ListBar owner;

        /// <summary>
        /// Bounding rectangle for this group's button.  The height
        /// is managed by this object but the other members are typically
        /// adjusted by the owning control through the <see cref="SetLocationAndWidth"/>
        /// and the <see cref="SetButtonHeight"/> methods.
        /// </summary>
        protected Rectangle rectangle = new Rectangle(0, 0, 0, 24);

        /// <summary>
        /// The scroll 
        /// </summary>
        private int scrollOffset;

        /// <summary>
        /// Whether the item is selected or not.
        /// </summary>
        private bool selected;

        /// <summary>
        /// Temporary array to hold the subitems to add to
        /// this group once it's owner has been assigned.
        /// </summary>
        private ListBarItem[] subItems;

        /// <summary>
        /// User-defined data to associate with this item.
        /// </summary>
        private object tag;

        /// <summary>
        /// The tooltip text for this group.
        /// </summary>
        private string toolTipText = "";

        /// <summary>
        /// Whether the group is visible or not.
        /// </summary>
        private bool visible = true;

        /// <summary>
        /// Constructs this object from a serialized representation.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// containing the serialized data to build this object from.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public ListBarGroup(
            SerializationInfo info,
            StreamingContext context)
        {
            font = (Font) info.GetValue("Font", typeof (Font));
            toolTipText = info.GetString("ToolTipText");
            caption = info.GetString("Caption");
            foreColor = (Color) info.GetValue("ForeColor", typeof (Color));
            backColor = (Color) info.GetValue("BackColor", typeof (Color));
            tag = info.GetValue("Tag", typeof (object));
            key = info.GetString("Key");
            rectangle = (Rectangle) info.GetValue("Rectangle", typeof (Rectangle));
            View = (ListBarGroupView) info.GetInt32("View");
            selected = info.GetBoolean("Selected");

            items = CreateListBarItemCollection(info, context);
        }


        /// <summary>
        /// Constructs a new, blank instance of a ListBarGroup.
        /// </summary>
        public ListBarGroup()
        {
            // intentionally empty
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        public ListBarGroup(
            string caption
            ) : this()
        {
            this.caption = caption;
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption and items.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        /// <param name="subItems">The array of items to add to the group's
        /// collection of items.</param>
        public ListBarGroup(
            string caption,
            ListBarItem[] subItems
            ) : this(caption)
        {
            this.subItems = subItems;
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption and tooltip text.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        /// <param name="toolTipText">ToolTip text to show when hovering over
        /// the group.</param>
        public ListBarGroup(
            string caption,
            string toolTipText
            ) : this(caption)
        {
            this.toolTipText = toolTipText;
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption, tooltip text and user-defined data.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        /// <param name="toolTipText">ToolTip text to show when hovering over
        /// the group.</param>
        /// <param name="tag">User-defined object data which is associated with
        /// the group.</param>
        public ListBarGroup(
            string caption,
            string toolTipText,
            object tag
            ) : this(caption, toolTipText)
        {
            this.tag = tag;
        }

        /// <summary>
        /// Returns the selected <see cref="ListBarItem"/> in this Group, if any, otherwise null.
        /// </summary>
        [Description("Returns the selected ListBarItem in this Group, if any, otherwise null.")]
        public ListBarItem SelectedItem
        {
            get
            {
                ListBarItem ret = null;
                foreach (ListBarItem item in items)
                {
                    if (item.Selected)
                    {
                        ret = item;
                        break;
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Gets/sets a <see cref="System.Windows.Forms.Control"/>
        /// which can be displayed within this group.
        /// </summary>
        /// <remarks>
        /// Do not set the child control until this group has
        /// been added to the control.
        /// </remarks>
        [Description("Gets/sets a Control which is displayed in this group rather than items.")]
        public Control ChildControl
        {
            get
            {
                return childControl;
            }
            set
            {
                value.Visible = false;
                childControl = value;
                childControl.Parent = owner;
                NotifyOwner(true);
            }
        }

        /// <summary>
        /// Internal member holding the negative scrolled 
        /// offset of this bar from the top of the client area
        /// </summary>
        protected internal int ScrollOffset
        {
            get
            {
                return scrollOffset;
            }
            set
            {
                scrollOffset = value;
            }
        }

        /// <summary>
        /// Gets/sets the which view to show the items within this bar.
        /// </summary>
        [Description("Gets/sets the which view to show the items within this bar.")]
        public ListBarGroupView View
        {
            get
            {
                return iconSize;
            }
            set
            {
                if (iconSize != value)
                {
                    iconSize = value;
                    SetLocationAndWidth(rectangle.Location, rectangle.Width);
                    NotifyOwner(true);
                }
            }
        }


        /// <summary>
        /// Returns the location of the button
        /// which activates this group relative
        /// to the owning control.
        /// </summary>
        [Description("Returns the location of the button which activates this group relative to the owning control.")]
        public virtual Point ButtonLocation
        {
            get
            {
                return rectangle.Location;
            }
        }

        /// <summary>
        /// Returns the width of the button
        /// which activates this group.
        /// </summary>
        [Description("Returns the width of the button which activates this group.")]
        public virtual int ButtonWidth
        {
            get
            {
                return rectangle.Width;
            }
        }

        /// <summary>
        /// Returns the height of the button
        /// which activates this group.
        /// </summary>
        [Description("Returns the height of the button which activates this group.")]
        public virtual int ButtonHeight
        {
            get
            {
                return rectangle.Height;
            }
        }

        /// <summary>
        /// Returns the collection of items belonging to this <see cref="ListBarGroup" />.
        /// group.
        /// </summary>
        [Description("Returns the collection of items belonging to this ListBarGroup")]
        public virtual ListBarItemCollection Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets/sets whether this group is visible in the control 
        /// or not.
        /// </summary>
        [Description("Gets/sets whether this group is visible in the control or not.")]
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                NotifyOwner(true);
            }
        }


        /// <summary>
        /// Gets/sets the <see cref="System.Drawing.Font"/> to draw the caption for this group.
        /// </summary>
        [Description("Returns the Font used to draw the caption for this group.")]
        public virtual Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                NotifyOwner(true);
            }
        }

        /// <summary>
        /// Gets/sets the foreground colour to use when drawing
        /// the button for this group.
        /// </summary>
        [Description("Gets/sets the foreground colour to use when drawing the button for this group.")]
        public virtual Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the background colour to use when drawing the button for this group.
        /// </summary>
        [Description("Gets/sets the background colour to use when drawing the button for this group.")]
        public virtual Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the caption displayed for this group.
        /// </summary>
        [Description("Gets/sets the caption displayed for this group.")]
        public virtual string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the string key associated with this group.
        /// </summary>
        [Description("Gets/sets a string key associated with this group.")]
        public virtual string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        /// <summary>
        /// Gets/sets whether this group is selected or not.
        /// </summary>
        [Description("Gets/sets whether this group is selected or not.")]
        public virtual bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    if (childControl != null)
                    {
                        childControl.Visible = value;
                    }
                    NotifyOwner(false);
                }
            }
        }

        /// <summary>
        /// Gets/sets a user-defined object associated with this group.
        /// </summary>
        [Description("Gets/sets a user-defined object associated with this group.")]
        public virtual object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// Gets the owning ListBar control for this item.
        /// </summary>
        protected internal ListBar Owner
        {
            get
            {
                return owner;
            }
        }

        #region IMouseObject Members

        /// <summary>
        /// Gets/sets the point at which the mouse was clicked on the group
        /// button.
        /// </summary>
        [Description("Gets/sets the point at which the mouse was clicked on the group button.")]
        public Point MouseDownPoint
        {
            get
            {
                return mouseDownPoint;
            }
            set
            {
                mouseDownPoint = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is over the group button.
        /// </summary>
        [Description("Gets/sets whether the mouse is over the group button.")]
        public bool MouseOver
        {
            get
            {
                return mouseOver;
            }
            set
            {
                mouseOver = (value & visible);
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is down over the group button.
        /// </summary>
        [Description("Gets/sets whether the mouse is down over the group button.")]
        public bool MouseDown
        {
            get
            {
                return mouseDown;
            }
            set
            {
                mouseDown = (value & visible);
            }
        }

        /// <summary>
        /// Gets/sets the tooltip that will be displayed when the user
        /// hovers over this group's button.
        /// </summary>
        [Description("Gets/sets the tooltip text that will be displayed when the user hovers over this group's button.")]
        public virtual string ToolTipText
        {
            get
            {
                return toolTipText;
            }
            set
            {
                toolTipText = value;
            }
        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo object with the 
        /// data needed to serialize this object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// to populate with data.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("Font", font);
            info.AddValue("ToolTipText", toolTipText);
            info.AddValue("Caption", caption);
            info.AddValue("ForeColor", foreColor);
            info.AddValue("BackColor", backColor);
            info.AddValue("Tag", tag);
            info.AddValue("Key", key);
            info.AddValue("Rectangle", rectangle);
            info.AddValue("View", ((int) iconSize));
            info.AddValue("Selected", selected);

            info.AddValue("Items", items);
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this <see cref="ListBarGroup"/>.
        /// </summary>
        /// <returns>A string containing the class name, caption, rectangle
        /// and item count for this group.</returns>
        [Description("Returns a string representation of this ListBarGroup.")]
        public override string ToString()
        {
            return String.Format("{0} Caption={1} Location={2} Height={3} ItemCount={4}",
                                 GetType().FullName, caption, ButtonLocation, ButtonHeight, items.Count);
        }

        /// <summary>
        /// Called to create a new item collection for this ListBarGroup.
        /// </summary>
        /// <returns>The ListBarItemCollection that will be used for this
        /// ListBarGroup</returns>
        protected virtual ListBarItemCollection CreateListBarItemCollection()
        {
            return new ListBarItemCollection(owner);
        }

        /// <summary>
        /// Called to create a new item collection for this ListBarGroup
        /// when the data is being deserialized
        /// </summary>
        /// <returns>The ListBarItemCollection that will be used for this
        /// ListBarGroup</returns>
        protected virtual ListBarItemCollection CreateListBarItemCollection(
            SerializationInfo info,
            StreamingContext context)
        {
            return new ListBarItemCollection(info, context);
        }

        private void SetItemSize()
        {
            if (items != null)
            {
                foreach (ListBarItem item in items)
                    owner.ItemChanged(item, false);
                NotifyOwner(true);
            }
        }

        /// <summary>
        /// Called to set the height of this group's button by the owning control.
        /// </summary>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/>
        /// to use when this item does not have a specific font set.</param>
        [Description("Called to set the height of this group's button by the owning control.")]
        public virtual void SetButtonHeight(
            Font defaultFont
            )
        {
            // Select the font we're going to use
            Font drawFont = defaultFont;
            if (Font != null)
            {
                drawFont = Font;
            }

            // Get the string to measure to determine
            // the item's height
            string measureString = "Xg";
            // Measure the height of an item 
            Bitmap measureBitmap = new Bitmap(30, 30);
            Graphics graphics = Graphics.FromImage(measureBitmap);
            SizeF textSize = graphics.MeasureString(measureString, drawFont);
            graphics.Dispose();
            measureBitmap.Dispose();

            rectangle.Height = (int) textSize.Height + 8;
        }

        /// <summary>
        /// Sets the location and width of the button which
        /// activates this <see cref="ListBarGroup"/>.  This method
        /// is called by internally by the <see cref="ListBar"/> 
        /// which owns this item.
        /// </summary>
        /// <remarks>
        /// This member is not intended to be called from client code.
        /// If you do use it, it is likely that a subsequent operation
        /// on the control or group will replace the values.  If you
        /// need more control over placement, override this class
        /// and build the logic into the override for this method
        /// instead.
        /// </remarks>
        /// <param name="location">The new location for the item.</param>
        /// <param name="width">The new width of the item.</param>
        [Description("Sets the location and width of the button which activates this group.  This method is called internally by the owning ListBar control.")]
        public virtual void SetLocationAndWidth(Point location, int width)
        {
            rectangle.Location = location;
            rectangle.Width = width;
            if (items != null)
            {
                Point itemLocation = new Point(location.X, 0);

                Font defaultFont = Font;
                if (defaultFont == null)
                {
                    if (owner == null)
                    {
                        defaultFont = SystemInformation.MenuFont;
                    }
                    else
                    {
                        defaultFont = owner.Font;
                    }
                }

                Size imageSize = new Size(32, 32);
                if ((iconSize == ListBarGroupView.LargeIcons) || (iconSize == ListBarGroupView.LargeIconsOnly))
                {
                    if (owner != null)
                    {
                        if (owner.LargeImageList != null)
                        {
                            imageSize = owner.LargeImageList.ImageSize;
                        }
                    }
                }
                else
                {
                    imageSize.Width = 16;
                    imageSize.Height = 16;
                    if (owner != null)
                    {
                        if (owner.SmallImageList != null)
                        {
                            imageSize = owner.SmallImageList.ImageSize;
                        }
                    }
                }

                if ((View == ListBarGroupView.SmallIconsOnly) || (View == ListBarGroupView.LargeIconsOnly))
                {
                    int itemWidth = imageSize.Width + 16;
                    for (int i = 0; i < items.Count; i++)
                    {
                        ListBarItem item = items[i];
                        item.SetSize(View, defaultFont, imageSize);
                        item.SetLocationAndWidth(itemLocation, itemWidth);
                        itemLocation.X += item.Width;
                        if (i < items.Count - 1)
                        {
                            if ((item.Location.X + items[i + 1].Width) > width)
                            {
                                itemLocation.X = location.X;
                                itemLocation.Y += item.Height;
                            }
                        }
                    }
                }
                else
                {
                    if (Owner != null)
                    {
                        width = Owner.Width;
                    }

                    foreach (ListBarItem item in items)
                    {
                        item.SetSize(iconSize, defaultFont, imageSize);
                        item.SetLocationAndWidth(itemLocation, width);
                        itemLocation.Y += item.Height;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the items within this <see cref="ListBarGroup"/> onto the control.
        /// </summary>
        /// <param name="gfx">The <see cref="System.Drawing.Graphics"/> object to draw onto.</param>
        /// <param name="bounds">The bounding <see cref="System.Drawing.Rectangle"/> within which
        /// to draw the items.</param>
        /// <param name="ils">The <see cref="System.Windows.Forms.ImageList"/> object to use to draw
        /// the bar.</param>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/> to use.</param>
        /// <param name="style">The style to draw the ListBar in.</param>
        /// <param name="enabled">Whether the ListBar control is enabled or not.</param>
        [Description("Draws the items within this group bar onto the ListBar control.  Called internally by the owning ListBar control.")]
        public virtual void DrawBar(
            Graphics gfx,
            Rectangle bounds,
            ImageList ils,
            Font defaultFont,
            ListBarDrawStyle style,
            bool enabled
            )
        {
            if (childControl != null)
            {
                childControl.Location = bounds.Location;
                childControl.Size = bounds.Size;
            }
            else
            {
                Items.Draw(
                    gfx, bounds, ils, defaultFont,
                    style, View, enabled,
                    scrollOffset + rectangle.Bottom);
            }
        }

        /// <summary>
        /// Draws the button for this group onto the control.
        /// </summary>
        /// <param name="gfx">The <see cref="System.Drawing.Graphics"/> object to draw onto.</param>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/> to 
        /// draw with.</param>
        /// <param name="enabled">Whether this control is enabled or not.</param>
        [Description("Draws the button for this group onto the control.")]
        public virtual void DrawButton(
            Graphics gfx,
            Font defaultFont,
            bool enabled
            )
        {
            if (visible)
            {
                // Get the font to draw with:
                Font drawFont = font;
                if (drawFont == null)
                {
                    drawFont = defaultFont;
                }

                // Fill the item:
                Brush br = new SolidBrush(backColor);
                gfx.FillRectangle(br, rectangle);
                br.Dispose();

                // Draw the text:
                StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = HotkeyPrefix.Show;
                RectangleF rectF = new RectangleF(rectangle.X, rectangle.Y,
                                                  rectangle.Width, rectangle.Height);
                if (enabled)
                {
                    br = new SolidBrush(foreColor);
                    gfx.DrawString(caption, drawFont, br, rectF, format);
                    br.Dispose();
                }
                else
                {
                    rectF.Offset(1F, 1F);
                    br = new SolidBrush(CustomBorderColor.ColorLightLight(backColor));
                    gfx.DrawString(caption, drawFont, br, rectF, format);
                    br.Dispose();
                    rectF.Offset(-1F, -1F);
                    br = new SolidBrush(CustomBorderColor.ColorDark(backColor));
                    gfx.DrawString(caption, drawFont, br, rectF, format);
                    br.Dispose();
                }
                format.Dispose();

                // Draw the border:
                Pen darkDarkPen = new Pen(CustomBorderColor.ColorDarkDark(BackColor));
                Pen darkPen = new Pen(CustomBorderColor.ColorDark(BackColor));
                Pen lightPen = new Pen(CustomBorderColor.ColorLight(BackColor));
                Pen lightLightPen = new Pen(CustomBorderColor.ColorLightLight(BackColor));

                if (mouseDown && mouseOver)
                {
                    gfx.DrawLine(darkDarkPen,
                                 rectangle.Left, rectangle.Bottom - 2,
                                 rectangle.Left, rectangle.Top);
                    gfx.DrawLine(darkDarkPen,
                                 rectangle.Left, rectangle.Top,
                                 rectangle.Right - 2, rectangle.Top);
                    gfx.DrawLine(darkPen,
                                 rectangle.Left + 1, rectangle.Bottom - 3,
                                 rectangle.Left + 1, rectangle.Top + 1);
                    gfx.DrawLine(darkPen,
                                 rectangle.Left + 1, rectangle.Top + 1,
                                 rectangle.Right - 3, rectangle.Top + 1);

                    gfx.DrawLine(lightLightPen,
                                 rectangle.Right - 1, rectangle.Top,
                                 rectangle.Right - 1, rectangle.Bottom - 1);
                    gfx.DrawLine(lightLightPen,
                                 rectangle.Right - 1, rectangle.Bottom - 1,
                                 rectangle.Left, rectangle.Bottom - 1);
                    gfx.DrawLine(lightPen,
                                 rectangle.Right - 2, rectangle.Top + 1,
                                 rectangle.Right - 2, rectangle.Bottom - 2);
                    gfx.DrawLine(lightPen,
                                 rectangle.Right - 2, rectangle.Bottom - 2,
                                 rectangle.Left + 1, rectangle.Bottom - 2);
                }
                else if (MouseOver || mouseDown)
                {
                    gfx.DrawLine(lightLightPen,
                                 rectangle.Left, rectangle.Bottom - 2,
                                 rectangle.Left, rectangle.Top);
                    gfx.DrawLine(lightLightPen,
                                 rectangle.Left, rectangle.Top,
                                 rectangle.Right - 2, rectangle.Top);
                    gfx.DrawLine(lightPen,
                                 rectangle.Left + 1, rectangle.Bottom - 3,
                                 rectangle.Left + 1, rectangle.Top + 1);
                    gfx.DrawLine(lightPen,
                                 rectangle.Left + 1, rectangle.Top + 1,
                                 rectangle.Right - 3, rectangle.Top + 1);

                    gfx.DrawLine(darkDarkPen,
                                 rectangle.Right - 1, rectangle.Top,
                                 rectangle.Right - 1, rectangle.Bottom - 1);
                    gfx.DrawLine(darkDarkPen,
                                 rectangle.Right - 1, rectangle.Bottom - 1,
                                 rectangle.Left, rectangle.Bottom - 1);
                    gfx.DrawLine(darkPen,
                                 rectangle.Right - 2, rectangle.Top + 1,
                                 rectangle.Right - 2, rectangle.Bottom - 2);
                    gfx.DrawLine(darkPen,
                                 rectangle.Right - 2, rectangle.Bottom - 2,
                                 rectangle.Left + 1, rectangle.Bottom - 2);
                }
                else
                {
                    gfx.DrawLine(lightLightPen,
                                 rectangle.Left, rectangle.Bottom - 2,
                                 rectangle.Left, rectangle.Top + 1);
                    gfx.DrawLine(lightLightPen,
                                 rectangle.Left, rectangle.Top + 1,
                                 rectangle.Right - 2, rectangle.Top + 1);
                    gfx.DrawLine(darkPen,
                                 rectangle.Right - 1, rectangle.Top + 1,
                                 rectangle.Right - 1, rectangle.Bottom - 1);
                    gfx.DrawLine(darkPen,
                                 rectangle.Right - 1, rectangle.Bottom - 1,
                                 rectangle.Left, rectangle.Bottom - 1);
                }

                lightLightPen.Dispose();
                lightPen.Dispose();
                darkPen.Dispose();
                darkDarkPen.Dispose();
            }
        }

        /// <summary>
        /// Starts editing this item.  The <c>BeforeLabelEdit</c> event will
        /// be fired prior to the text box being made visible.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item is not
        /// part of a ListBar control.</exception>
        [Description("Starts editing this item and fires the BeforeLabelEdit event.")]
        public virtual void StartEdit()
        {
            if (owner != null)
            {
                owner.StartGroupEdit(this);
            }
            else
            {
                throw new InvalidOperationException("Owner of this ListBarGroup has not been set.");
            }
        }

        /// <summary>
        /// Notifies the owning ListBar control of any changes to a group.
        /// </summary>
        /// <param name="addRemove">Whether the control should resize
        /// all groups associated with the ListBar.</param>
        protected virtual void NotifyOwner(bool addRemove)
        {
            if (owner != null)
            {
                owner.GroupChanged(this, addRemove);
            }
        }

        /// <summary>
        /// Sets the owning control for this Group.  Called automatically
        /// whenever a group is added to the group collection associated with
        /// a ListBar control.
        /// </summary>
        /// <param name="owner">The ListBar control which owns this group.</param>
        protected internal void SetOwner(ListBar owner)
        {
            this.owner = owner;
            if (items == null)
            {
                items = CreateListBarItemCollection();
            }
            if (subItems != null)
            {
                items.AddRange(subItems);
                subItems = null;
            }
            // Set the size of any items which belong
            // to this bar:
            SetItemSize();

            NotifyOwner(true);
        }
    }

    #endregion

    #region ListBarItemCollection class

    /// <summary>
    /// This class manages a collection of items within a ListBarGroup.
    /// </summary>
    [SerializableAttribute]
    public class ListBarItemCollection : CollectionBase, ISerializable
    {
        /// <summary>
        /// The owning ListBar control.
        /// </summary>
        private ListBar owner;

        /// <summary>
        /// 
        /// TODO_ This method has not been implemented yet.
        /// 
        /// Constructs this object from a serialized representation.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// containing the serialized data to build this object from.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public ListBarItemCollection(
            SerializationInfo info,
            StreamingContext context)
        {
            // 
            // TODO_ This method has not been implemented yet.
            // 
        }

        /// <summary>
        /// Constructs a new instance of this collection and sets
        /// the owner.  Typically this is performed by the owning ListBar
        /// control.
        /// </summary>
        /// <param name="owner">The ListBar which owns this collection</param>
        public ListBarItemCollection(ListBar owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets the height of all the items within this collection.
        /// </summary>
        [Description("Gets the overall height of all the items in the collection.")]
        public virtual int Height
        {
            get
            {
                int maxHeight = 0;
                foreach (ListBarItem item in InnerList)
                {
                    int itemBottom = item.Location.Y + item.Height;
                    if (itemBottom > maxHeight)
                    {
                        maxHeight = itemBottom;
                    }
                }
                return maxHeight;
            }
        }

        /// <summary>
        /// Gets the <see cref="ListBarItem"/> at the specified 0-based index.
        /// </summary>
        [Description("Gets the ListBarItem at the specified 0-based index.")]
        public ListBarItem this[int index]
        {
            get
            {
                return (ListBarItem) InnerList[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="ListBarItem"/> with the specified key.
        /// </summary>
        [Description("Gets the ListBarItem at the specified key.")]
        public ListBarItem this[string key]
        {
            get
            {
                ListBarItem ret = null;
                foreach (ListBarItem item in InnerList)
                {
                    if (item.Key.Equals(key))
                    {
                        ret = item;
                        break;
                    }
                }
                return ret;
            }
        }

        #region ISerializable Members

        /// <summary>
        /// 
        /// TODO_ This method has not been implemented yet.
        /// 
        /// Populates a System.Runtime.Serialization.SerializationInfo object with the 
        /// data needed to serialize this object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// to populate with data.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            //
            // TODO_ This method has not been implemented yet.
            // 
        }

        #endregion

        /// <summary>
        /// Sorts the items in this collection using the specified
        /// comparer.
        /// </summary>
        /// <param name="comparer">IComparer implementation specifying
        /// how to sort the objects.</param>
        [Description("Sorts the items in this collection using the specified comparer")]
        public virtual void Sort(IComparer comparer)
        {
            InnerList.Sort(comparer);
            owner.ItemChanged(null, true);
        }

        /// <summary>
        /// Sorts the items in this collection using the default comparison
        /// operation (alphabetic).
        /// </summary>
        [Description("Sorts the items in this collection alphabetically.")]
        public virtual void Sort()
        {
            InnerList.Sort();
            owner.ItemChanged(null, true);
        }

        /// <summary>
        /// Draws the items within this collection.
        /// </summary>
        /// <param name="gfx">The graphics object to draw onto.</param>
        /// <param name="bounds">The bounding rectangle within which
        /// to draw the items.</param>
        /// <param name="ils">The ImageList to use when drawing the icons.</param>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/> to use.</param>
        /// <param name="style">The Style to draw the items using.</param>
        /// <param name="view">The view to use when drawing the items.</param>
        /// <param name="enabled">Whether the owning group is enabled or not.</param>
        /// <param name="scrollOffset">The scrolled offset at which to start
        /// drawing the items.</param>				
        public virtual void Draw(
            Graphics gfx,
            Rectangle bounds,
            ImageList ils,
            Font defaultFont,
            ListBarDrawStyle style,
            ListBarGroupView view,
            bool enabled,
            int scrollOffset
            )
        {
            bool skipDraw = false;
            ListBarItem editItem = owner.EditItem;

            gfx.SetClip(bounds);
            foreach (ListBarItem item in InnerList)
            {
                skipDraw = false;
                if (editItem != null)
                {
                    skipDraw = editItem.Equals(item);
                }
                int itemTop = item.Location.Y;
                itemTop += scrollOffset;
                if (
                    ((itemTop >= bounds.Top) && (itemTop <= bounds.Bottom)) ||
                    (((itemTop + item.Height) <= bounds.Bottom) && ((itemTop + item.Height) > bounds.Top))
                    )
                {
                    item.DrawButton(gfx, ils, defaultFont,
                                    style, view, scrollOffset, enabled, skipDraw);
                }
            }
            gfx.ResetClip();
        }

        /// <summary>
        /// Adds a <see cref="ListBarItem"/> object to the group.
        /// </summary>
        /// <param name="item">The ListBarItem to add.</param>
        [Description("Adds a ListBarItem object to the items in the group.")]
        public virtual void Add(
            ListBarItem item
            )
        {
            InnerList.Add(item);
            item.SetOwner(owner);
            EnsureSingleSelection(item);
            NotifyOwner(item, true);
        }

        /// <summary>
        /// Constructs a new <see cref="ListBarItem"/> object using the specified
        /// caption, adds it to the bar and returns it.
        /// </summary>
        /// <param name="caption">The caption to use for the ListBarItem.</param>
        /// <returns>The newly added ListBarItem object.</returns>
        [Description("Constructs a new ListBarItem object and adds it to the group.")]
        public virtual ListBarItem Add(
            string caption
            )
        {
            ListBarItem item = new ListBarItem(caption);
            InnerList.Add(item);
            item.SetOwner(owner);
            NotifyOwner(item, true);
            return item;
        }

        /// <summary>
        /// Constructs a new ListBarItem object using the specified
        /// caption and icon, adds it to the bar and returns it.
        /// </summary>
        /// <param name="caption">The caption to use for the ListBarItem.</param>
        /// <param name="iconIndex">The 0-based index of the icon for the ListBarItem
        /// within an ImageList</param>
        /// <returns>The newly added ListBarItem object.</returns>
        [Description("Constructs a new ListBarItem object and adds it to the group.")]
        public virtual ListBarItem Add(
            string caption,
            int iconIndex
            )
        {
            ListBarItem item = new ListBarItem(caption, iconIndex);
            InnerList.Add(item);
            item.SetOwner(owner);
            NotifyOwner(item, true);
            return item;
        }

        /// <summary>
        /// Adds a range of <see cref="ListBarItem"/> objects to the bar from an array.
        /// </summary>
        /// <param name="values">The array of ListBarItem objects to
        /// add.</param>
        [Description("Adds of range of ListBarItem objects to the bar.")]
        public virtual void AddRange(
            ListBarItem[] values
            )
        {
            foreach (ListBarItem item in values)
            {
                InnerList.Add(item);
                item.SetOwner(owner);
            }
            EnsureSingleSelection(this[0]);
            NotifyOwner(values[0], true);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified <see cref="ListBarItem "/> is contained
        /// within this collection, otherwise <c>false</c>.
        /// </summary>
        /// <param name="item">The ListBarItem to check.</param>
        /// <returns>True if the specified ListBarItem is contained
        /// within this collection, False otherwise.</returns>
        [Description("Returns true if the specified ListBarItem is found in this collection, otherwise false.")]
        public virtual bool Contains(ListBarItem item)
        {
            return InnerList.Contains(item);
        }

        /// <summary>
        /// Returns the 0-based index of the specified item in the
        /// collection if present, -1 otherwise.
        /// </summary>
        /// <param name="item">The ListBarItem to check.</param>
        /// <returns>The 0-based index of the specified item in the
        /// collection if present, -1 otherwise.</returns>
        [Description("Returns the 0-based index of the specified item in the collection")]
        public virtual int IndexOf(ListBarItem item)
        {
            return InnerList.IndexOf(item);
        }

        /// <summary>
        /// Inserts a <see cref="ListBarItem"/> at the specified index in the bar.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The ListBarItem to insert.</param>
        [Description("Inserts a ListBarItem at the specified index in the bar.")]
        public virtual void Insert(int index, ListBarItem item)
        {
            InnerList.Insert(index, item);
            item.SetOwner(owner);
            EnsureSingleSelection(item);
            NotifyOwner(item, true);
        }

        /// <summary>
        /// Inserts a <see cref="ListBarItem"/> immediately before the specified ListBarItem.
        /// </summary>
        /// <param name="itemBefore">ListBarItem to insert before.</param>
        /// <param name="item">Item to insert.</param>
        [Description("Inserts a ListBarItem immediately before the specified ListBarItem.")]
        public virtual void InsertBefore(ListBarItem itemBefore, ListBarItem item)
        {
            InnerList.Insert(InnerList.IndexOf(itemBefore), item);
            EnsureSingleSelection(item);
            NotifyOwner(item, true);
        }

        /// <summary>
        /// Inserts a <see cref="ListBarItem"/> immediately after the specified ListBarItem.
        /// </summary>
        /// <param name="itemAfter">ListBarItem to insert after.</param>
        /// <param name="item">Item to insert.</param>
        [Description("Inserts a ListBarItem immediately after the specified ListBarItem.")]
        public virtual void InsertAfter(ListBarItem itemAfter, ListBarItem item)
        {
            int index = InnerList.IndexOf(itemAfter);
            if (index == InnerList.Count - 1)
            {
                Add(item);
            }
            else
            {
                Insert(index + 1, item);
            }
            NotifyOwner(item, true);
        }

        /// <summary>
        /// Removes the specified <see cref="ListBarItem"/> from the collection.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        [Description("Removes the specified ListBarItem from the collection.")]
        public virtual void Remove(ListBarItem item)
        {
            InnerList.Remove(item);
            NotifyOwner(item, true);
        }

        private void EnsureSingleSelection(ListBarItem newItem)
        {
            bool foundSelectedItem = false;
            if (newItem.Selected)
            {
                foundSelectedItem = true;
            }

            foreach (ListBarItem item in InnerList)
            {
                if (!item.Equals(newItem))
                {
                    if (item.Selected)
                    {
                        if (foundSelectedItem)
                        {
                            item.Selected = false;
                        }
                        else
                        {
                            foundSelectedItem = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Notifies the owner control that the items have been
        /// cleared.
        /// </summary>
        protected override void OnClearComplete()
        {
            NotifyOwner(null, true);
        }

        /// <summary>
        /// Notifies the owner control after an item has been inserted.
        /// </summary>
        /// <param name="index">Index of inserting item</param>
        /// <param name="value">Item which has been inserted.</param>
        protected override void OnInsertComplete(Int32 index, Object value)
        {
            NotifyOwner((ListBarItem) value, true);
        }

        /// <summary>
        /// Notifies the owner control after an item has been removed.
        /// </summary>
        /// <param name="index">Index of inserting item</param>
        /// <param name="value">Item which has been inserted.</param>
        protected override void OnRemoveComplete(Int32 index, Object value)
        {
            NotifyOwner((ListBarItem) value, true);
        }

        /// <summary>
        /// Notifies the owner control after an item has been changed using set.
        /// </summary>
        /// <param name="index">Index of inserting item</param>
        /// <param name="oldValue">Old item which was there.</param>
        /// <param name="newValue">New Item which has been set.</param>
        protected override void OnSetComplete(Int32 index, Object oldValue, Object newValue)
        {
            NotifyOwner((ListBarItem) newValue, true);
        }

        /// <summary>
        /// Notifies the owning control of a change in this item.
        /// </summary>
        /// <param name="addRemove">Set to true if the change
        /// that has been made requires the size of the display
        /// to be recalculated.</param>
        /// <param name="item">The Item which has been changed
        /// (or null if the item itm is invalid)</param>
        protected virtual void NotifyOwner(ListBarItem item, bool addRemove)
        {
            if (owner != null)
            {
                owner.ItemChanged(item, addRemove);
            }
        }

        /// <summary>
        /// Enables a deserialized object graph to be associated with a ListBar
        /// control.
        /// </summary>
        /// <param name="owner">The ListBar control which will own
        /// this collection of items.</param>
        public virtual void SetOwner(ListBar owner)
        {
            this.owner = owner;
            foreach (ListBarItem item in InnerList)
                item.SetOwner(owner);
        }
    }

    #endregion

    #region ListBarItem class

    /// <summary>
    /// A class containing the information describing an Item in the ListBar
    /// control.
    /// </summary>
    [SerializableAttribute]
    public class ListBarItem : IComparable, IMouseObject, ISerializable
    {
        private string caption = "";
        private bool enabled = true;
        private Font font;
        private Color foreColor = Color.FromKnownColor(KnownColor.WindowText);
        private int iconIndex;

        /// <summary>
        /// The rectangle containing the icon for this item.  Set this 
        /// when overriding the standard drawing mode for an item;
        /// the owning ListBar control uses it for hit-testing.
        /// </summary>
        protected Rectangle iconRectangle;

        private string key = "";

        private bool mouseDown;
        private Point mouseDownPoint = new Point(0, 0);
        private bool mouseOver;
        private ListBar owner;

        /// <summary>
        /// Bounding rectangle for this item, relative to its owning
        /// group.  The members of this are typically adjusted by the 
        /// owning control through the <see cref="SetLocationAndWidth"/>
        /// and the <see cref="SetSize"/> methods.
        /// </summary>
        protected Rectangle rectangle = new Rectangle(0, 0, 0, 72);

        private bool selected;
        private object tag = "";

        /// <summary>
        /// The rectangle containing the text for this item.  Set this
        /// when overriding the standard drawing mode for an item; 
        /// the owning ListBar control uses it for hit-testing.
        /// </summary>
        protected Rectangle textRectangle;

        private string toolTipText = "";

        /// <summary>
        /// Constructs this object from a serialized representation.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// containing the serialized data to build this object from.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public ListBarItem(
            SerializationInfo info,
            StreamingContext context)
        {
            font = (Font) info.GetValue("Font", typeof (Font));
            toolTipText = info.GetString("ToolTipText");
            caption = info.GetString("Caption");
            foreColor = (Color) info.GetValue("ForeColor", typeof (Color));
            tag = info.GetString("Tag");
            key = info.GetString("Key");
            iconIndex = info.GetInt32("IconIndex");
            rectangle = (Rectangle) info.GetValue("Rectangle", typeof (Rectangle));
        }

        /// <summary>
        /// Constructs a new, empty instance of a ListBarItem.
        /// </summary>
        public ListBarItem()
        {
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption to display.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        public ListBarItem(string caption) : this()
        {
            this.caption = caption;
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption and the index of the icon to display.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        /// <param name="iconIndex">The 0-based index of the icon
        /// to display</param>
        public ListBarItem(
            string caption,
            int iconIndex
            ) : this(caption)
        {
            this.iconIndex = iconIndex;
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption, the index of the icon and the 
        ///  tooltip text.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        /// <param name="iconIndex">The 0-based index of the icon
        /// to display</param>
        /// <param name="toolTipText">The tooltip text to show
        /// when the mouse hovers over this item.</param>
        public ListBarItem(
            string caption,
            int iconIndex,
            string toolTipText
            ) : this(caption, iconIndex)
        {
            this.toolTipText = toolTipText;
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption, the index of the icon, the 
        ///  tooltip text and the tag.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        /// <param name="iconIndex">The 0-based index of the icon
        /// to display</param>
        /// <param name="toolTipText">The tooltip text to show
        /// when the mouse hovers over this item.</param>
        /// <param name="tag">An object which can be used to 
        /// associate user-defined data with the item.</param>
        public ListBarItem(
            string caption,
            int iconIndex,
            string toolTipText,
            object tag
            ) : this(caption, iconIndex, toolTipText)
        {
            this.tag = tag;
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption, the index of the icon, the 
        ///  tooltip text, the tag and the key.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        /// <param name="iconIndex">The 0-based index of the icon
        /// to display</param>
        /// <param name="toolTipText">The tooltip text to show
        /// when the mouse hovers over this item.</param>
        /// <param name="tag">An object which can be used to 
        /// associate user-defined data with the item.</param>
        /// <param name="key">A user-defined string which is 
        /// associated with the item.</param>
        public ListBarItem(
            string caption,
            int iconIndex,
            string toolTipText,
            object tag,
            string key
            ) : this(caption, iconIndex, toolTipText, tag)
        {
            this.key = key;
        }

        /// <summary>
        /// Gets/sets whether this item is enabled.
        /// </summary>
        [Description("Gets/sets whether this item is enabled.")]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the foreground colour for this item.
        /// </summary>
        [Description("Gets/sets the foreground colour for this item.")]
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the font used for this object.  The default
        /// font is null which means the item renders using the
        /// font of the parent control.
        /// </summary>
        [Description("Gets/sets the font for this item.")]
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets the location for this item in the control.
        /// </summary>
        /// <remarks>
        /// The location is relative to the group the 
        /// item belongs to.  Therefore to find the position
        /// relative to the control you need to add the 
        /// bottom position of the button rectangle for the group
        /// and the scroll offset of the item. 
        /// </remarks>
        [Description("Gets the location of this item in the control.")]
        public virtual Point Location
        {
            get
            {
                return rectangle.Location;
            }
        }

        /// <summary>
        /// Gets the height of this item.
        /// </summary>
        [Description("Gets the height of this item in the control.")]
        public virtual int Height
        {
            get
            {
                return rectangle.Height;
            }
        }

        /// <summary>
        /// Gets the width of this item.
        /// </summary>
        [Description("Gets the width of this item in the control.")]
        public virtual int Width
        {
            get
            {
                return rectangle.Width;
            }
        }

        /// <summary>
        /// Returns the rectangle in which the icon is drawn for
        /// this item, relative to the control.
        /// </summary>
        [Description("Returns the rectangle in which the icon is drawn for this item, relative to the control.")]
        public virtual Rectangle IconRectangle
        {
            get
            {
                return iconRectangle;
            }
        }

        /// <summary>
        /// Returns the rectangle in which the text is drawn for
        /// this item, relative to the control.
        /// </summary>
        [Description("Returns the rectangle in which the text is drawn for this item, relative to the control.")]
        public virtual Rectangle TextRectangle
        {
            get
            {
                return textRectangle;
            }
        }

        /// <summary>
        /// Gets/sets whether this item is "selected" or not.
        /// Only one item in the ListBar control can be selected
        /// at a time.
        /// </summary>
        [Description("Gets/sets whether this item is selected or not.")]
        public virtual bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyOwner(false);
                }
            }
        }

        /// <summary>
        /// Gets/sets a user-defined string value which can be used
        /// to look up the item in the collection which owns it.
        /// </summary>
        [Description("Gets/sets a user-defined string value which can be used to look up the item in the collection which owns it.")]
        public virtual string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        /// <summary>
        /// Gets/sets the caption displayed for this item.
        /// </summary>
        [Description("Gets/sets the caption displayed for this item.")]
        public virtual string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets the 0-based index of an icon in an <see cref="System.Windows.Forms.ImageList"/>
        /// displayed with this item.
        /// </summary>
        [Description("Gets/sets the 0-based index of an icon in an ImageList displayed with this item.")]
        public virtual int IconIndex
        {
            get
            {
                return iconIndex;
            }
            set
            {
                iconIndex = value;
                NotifyOwner(false);
            }
        }

        /// <summary>
        /// Gets/sets an object which can be used to associate
        /// user-defined data with this item.
        /// </summary>
        [Description("Gets/sets an object which can be used to associate user-defined data with this item.")]
        public virtual object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// Gets the owning ListBar control for this item.
        /// </summary>
        protected internal ListBar Owner
        {
            get
            {
                return owner;
            }
        }

        #region IComparable Members

        /// <summary>
        /// Compares this object with another object of the same type.
        /// The implementation compares the captions of the items to
        /// allow items to be sorted alphabetically.
        /// </summary>
        /// <param name="obj">Another ListBarItem object</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the comparands.  
        /// The return value has these meanings: 
        /// &lt; 0: This instance is less than obj.  
        /// 0: This instance is equal to obj.  
        /// &gt; 0: This instance is greater than obj. </returns>
        [Description("Compares this object with another object of the same type.")]
        public virtual Int32 CompareTo(Object obj)
        {
            return caption.CompareTo(((ListBarItem) obj).Caption);
        }

        #endregion

        #region IMouseObject Members

        /// <summary>
        /// Gets/sets the point at which the mouse was pressed
        /// on this object.
        /// </summary>
        [Description("Gets/sets the point at which the mouse was pressed on this object.")]
        public Point MouseDownPoint
        {
            get
            {
                return mouseDownPoint;
            }
            set
            {
                mouseDownPoint = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is over this item.
        /// </summary>
        [Description("Gets/sets whether the mouse is over this item.")]
        public bool MouseOver
        {
            get
            {
                return mouseOver;
            }
            set
            {
                mouseOver = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is down on this item.
        /// </summary>
        [Description("Gets/sets whether the mouse is down on this item.")]
        public bool MouseDown
        {
            get
            {
                return mouseDown;
            }
            set
            {
                mouseDown = (value & enabled);
            }
        }

        /// <summary>
        /// Gets/sets the tooltip text that will be displayed when
        /// the user hovers over this item.
        /// </summary>
        [Description("Gets/sets the tooltip text that will be displayed when the user hovers over this item.")]
        public virtual string ToolTipText
        {
            get
            {
                return toolTipText;
            }
            set
            {
                toolTipText = value;
            }
        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo object with the 
        /// data needed to serialize this object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo 
        /// to populate with data.</param>
        /// <param name="context">The destination (see 
        /// System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("Font", font);
            info.AddValue("ToolTipText", toolTipText);
            info.AddValue("Caption", caption);
            info.AddValue("ForeColor", foreColor);
            info.AddValue("Tag", tag);
            info.AddValue("Key", key);
            info.AddValue("IconIndex", iconIndex);
            info.AddValue("Rectangle", rectangle);
            info.AddValue("Selected", selected);
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this <see cref="ListBarItem"/>.
        /// </summary>
        /// <returns>A string containing the class name, caption, icon index,
        /// enabled state and rectangle for this item.</returns>
        [Description("Returns a string representation of this ListBarItem")]
        public override string ToString()
        {
            return String.Format("{0} Caption={1} IconIndex={2} Enabled={3} Location={4} Height={5}",
                                 GetType().FullName, caption, iconIndex, enabled, Location, Height);
        }

        /// <summary>
        /// Draws this item into the specified graphics object.
        /// </summary>
        /// <param name="gfx">The <see cref="System.Drawing.Graphics"/> object to draw onto.</param>
        /// <param name="ils">The <see cref="System.Windows.Forms.ImageList"/>to source icons from.</param>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/> to use to render
        /// the item.</param>
        /// <param name="style">The style (Outlook version) to draw using.</param>
        /// <param name="view">The view (large or small icons) to draw using.</param>
        /// <param name="scrollOffset">The offset of the first item from the 
        /// (0,0) point in the graphics object.</param>
        /// <param name="controlEnabled">Whether the control is enabled or not.</param>
        /// <param name="skipDrawText">Whether to skip drawing text or not
        /// (the item is being edited)</param> 
        [Description("Draws this item into the specified graphics object")]
        public virtual void DrawButton(
            Graphics gfx,
            ImageList ils,
            Font defaultFont,
            ListBarDrawStyle style,
            ListBarGroupView view,
            int scrollOffset,
            bool controlEnabled,
            bool skipDrawText
            )
        {
            bool rightToLeft = false;
            Color backColor = Color.FromKnownColor(KnownColor.Control);
            if (owner != null)
            {
                backColor = owner.BackColor;
                if (owner.RightToLeft == RightToLeft.Yes)
                {
                    rightToLeft = true;
                }
            }

            // Work out the icon & text rectangles:			
            textRectangle = new Rectangle(rectangle.Location, rectangle.Size);
            textRectangle.Offset(0, scrollOffset);
            if (view == ListBarGroupView.SmallIcons)
            {
                textRectangle.Y += 1;
                textRectangle.Height -= 1;
            }
            iconRectangle = new Rectangle(textRectangle.Location, textRectangle.Size);

            if (view == ListBarGroupView.SmallIcons)
            {
                if (ils != null)
                {
                    if (rightToLeft)
                    {
                        iconRectangle.X = iconRectangle.Right - ils.ImageSize.Width - 4;
                        iconRectangle.Width = ils.ImageSize.Width;
                    }
                    else
                    {
                        iconRectangle.X += 4;
                        iconRectangle.Width = ils.ImageSize.Width;
                        textRectangle.X += ils.ImageSize.Width + 8;
                    }
                    textRectangle.Width -= (iconRectangle.Width + 8);
                    iconRectangle.Height = ils.ImageSize.Height;
                    iconRectangle.Y += (rectangle.Height - iconRectangle.Height)/2;
                }
                else
                {
                    textRectangle.Inflate(-2, -2);
                }
            }
            else
            {
                if (ils != null)
                {
                    iconRectangle.Y += 7;
                    iconRectangle.Height = ils.ImageSize.Height;
                    iconRectangle.Width = ils.ImageSize.Width;
                    iconRectangle.X = iconRectangle.Left + (rectangle.Width - iconRectangle.Width)/2;

                    textRectangle.Y += ils.ImageSize.Height + 11;
                    textRectangle.Height -= (ils.ImageSize.Height + 11);
                }
                else
                {
                    textRectangle.Inflate(-2, -2);
                }
            }

            // If we're drawing using XP style and the button is
            // hot or down then we draw the background:
            Rectangle rcHighlight = new Rectangle(iconRectangle.Location, iconRectangle.Size);
            rcHighlight.Inflate(2, 2);
            if (style == ListBarDrawStyle.ListBarDrawStyleOfficeXP)
            {
                if ((enabled && controlEnabled) && (MouseOver || mouseDown))
                {
                    Color highlightColor;
                    if (mouseDown && mouseOver)
                    {
                        highlightColor = ListBarUtility.BlendColor(
                            Color.FromKnownColor(KnownColor.Highlight),
                            Color.FromKnownColor(KnownColor.Window),
                            224);
                    }
                    else
                    {
                        highlightColor = ListBarUtility.BlendColor(
                            Color.FromKnownColor(KnownColor.Highlight),
                            Color.FromKnownColor(KnownColor.Window),
                            128);
                    }
                    SolidBrush highlight = new SolidBrush(Color.FromArgb(128, highlightColor));
                    gfx.FillRectangle(highlight, rcHighlight);
                    highlight.Dispose();
                    gfx.DrawRectangle(SystemPens.Highlight, rcHighlight);
                }
            }


            // Draw the icon if necessary:
            if (ils != null)
            {
                if (iconIndex >= 0 && iconIndex <= ils.Images.Count)
                {
                    int iconX = iconRectangle.X;
                    int iconY = iconRectangle.Y;


                    if (mouseDown && mouseOver)
                    {
                        iconX++;
                        iconY++;
                    }
                    if (enabled && controlEnabled)
                    {
                        ils.Draw(gfx, iconRectangle.X + 1, iconRectangle.Y + 1, iconIndex);
                    }
                    else
                    {
                        System.Windows.Forms.ControlPaint.DrawImageDisabled(gfx, ils.Images[iconIndex], iconX, iconY, Color.FromArgb(0, 0, 0, 0));
                    }
                }
                else
                {
                    // We don't want an exception in a paint event
                    Trace.WriteLine(
                        String.Format("Icon {0} doesn't exist in ImageList {1}",
                                      iconIndex,
                                      ils));
                }
            }
            // We do this to make the hit testing more usable:
            iconRectangle.Inflate(4, 4);

            if (skipDrawText)
            {
                return;
            }

            if ((view == ListBarGroupView.SmallIconsOnly) || (view == ListBarGroupView.LargeIconsOnly))
            {
                textRectangle = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                // Draw the text:
                // Get the font to draw with:
                Font drawFont = font;
                if (drawFont == null)
                {
                    if (owner != null)
                    {
                        drawFont = owner.Font;
                    }
                }
                if (drawFont == null)
                {
                    drawFont = SystemInformation.MenuFont;
                }
                // Set up format:
                StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
                format.Trimming = StringTrimming.EllipsisCharacter;
                if (view == ListBarGroupView.SmallIcons)
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    format.FormatFlags = format.FormatFlags | StringFormatFlags.NoWrap;
                }
                else
                {
                    format.Alignment = StringAlignment.Center;
                }
                format.LineAlignment = StringAlignment.Near;
                format.HotkeyPrefix = HotkeyPrefix.Show;
                // Bounding rectangle:
                RectangleF rectF = new RectangleF(textRectangle.X, textRectangle.Y,
                                                  textRectangle.Width, textRectangle.Height);
                if (view == ListBarGroupView.SmallIcons)
                {
                    SizeF textSize = gfx.MeasureString(caption, drawFont, textRectangle.Width, format);
                    rectF.Y += (rectF.Height - textSize.Height)/2;
                    textRectangle.Y += (int) ((rectF.Height - textSize.Height)/2);
                    textRectangle.Height = (int) textSize.Height;
                }
                // Color:
                SolidBrush br = new SolidBrush(foreColor);
                // Finally...
                if (enabled && controlEnabled)
                {
                    gfx.DrawString(caption, drawFont, br, rectF, format);
                }
                else
                {
                    Brush lightBrush = new SolidBrush(CustomBorderColor.ColorLightLight(backColor));
                    Brush darkBrush = new SolidBrush(CustomBorderColor.ColorDark(backColor));
                    rectF.Offset(1F, 1F);
                    gfx.DrawString(caption, drawFont, lightBrush, rectF, format);
                    rectF.Offset(-1F, -1F);
                    gfx.DrawString(caption, drawFont, darkBrush, rectF, format);
                    darkBrush.Dispose();
                    lightBrush.Dispose();
                    /*	
					ControlPaint.DrawStringDisabled(gfx, 
						this.caption, drawFont, 
						Color.FromKnownColor(KnownColor.Control), 
						rectF, format);
					*/
                }
                br.Dispose();
                format.Dispose();
            }

            // The border around the item if required:
            if (owner.DrawStyle == ListBarDrawStyle.ListBarDrawStyleNormal)
            {
                if (enabled && controlEnabled)
                {
                    Pen penTopLeft = null;
                    Pen penBottomRight = null;
                    if ((mouseDown) && (mouseDown))
                    {
                        // inset 3d border:
                        penTopLeft = SystemPens.ControlDarkDark;
                        penBottomRight = SystemPens.ControlLight;
                    }
                    else if ((mouseOver) || (mouseDown))
                    {
                        // raised 3d border:
                        penTopLeft = SystemPens.ControlLight;
                        penBottomRight = SystemPens.ControlDarkDark;
                    }
                    if (penTopLeft != null)
                    {
                        gfx.DrawLine(penTopLeft, rcHighlight.Left, rcHighlight.Bottom - 2,
                                     rcHighlight.Left, rcHighlight.Top);
                        gfx.DrawLine(penTopLeft, rcHighlight.Left, rcHighlight.Top,
                                     rcHighlight.Right - 2, rcHighlight.Top);
                        gfx.DrawLine(penBottomRight, rcHighlight.Right - 1, rcHighlight.Top,
                                     rcHighlight.Right - 1, rcHighlight.Bottom - 1);
                        gfx.DrawLine(penBottomRight, rcHighlight.Right - 1, rcHighlight.Bottom - 1,
                                     rcHighlight.Left, rcHighlight.Bottom - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the location and width of this item.  This method
        /// is called by internally by the <see cref="ListBar"/> or
        /// the <see cref="ListBarGroup"/> which owns this item.
        /// </summary>
        /// <remarks>
        /// This member is not intended to be called from client code.
        /// If you do use it, it is likely that a subsequent operation
        /// on the control or group will replace the values.  If you
        /// need more control over placement, override this class
        /// and build the logic into the override for this method
        /// instead.
        /// </remarks>
        /// <param name="location">The new location for the item.</param>
        /// <param name="width">The new width of the item.</param>
        [Description("Sets the location and width of this item in the control. Called internally by the owning ListBar or group")]
        public virtual void SetLocationAndWidth(Point location, int width)
        {
            rectangle.Location = location;
            rectangle.Width = width;
        }

        /// <summary>
        /// Called to set the height of the item by the owning control.
        /// </summary>
        /// <param name="view">The <see cref="ListBarGroupView"/> in which this
        /// item is being shown.</param>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/>
        /// to use when this item does not have a specific font set.</param>
        /// <param name="imageSize">The size of the images in the ImageList
        /// used to render this view.</param>		
        [Description("Called to set the height of an item by the owning control.")]
        public virtual void SetSize(
            ListBarGroupView view,
            Font defaultFont,
            Size imageSize
            )
        {
            // Select the font we're going to use
            Font drawFont = defaultFont;
            if (Font != null)
            {
                drawFont = Font;
            }

            // Get the string to measure to determine
            // the item's height
            string measureString = "Xg";
            if (view == ListBarGroupView.LargeIcons)
            {
                // by default we allow for two lines:
                measureString += "\r\nXg";
            }

            // Measure the height of an item 
            Bitmap measureBitmap = new Bitmap(30, 30);
            Graphics graphics = Graphics.FromImage(measureBitmap);
            SizeF textSize = graphics.MeasureString(measureString, drawFont);
            graphics.Dispose();
            measureBitmap.Dispose();

            // Set the height using the text size & the image size
            int height = imageSize.Height;
            if (view == ListBarGroupView.LargeIcons)
            {
                height += (int) textSize.Height;
                height += 12;
            }
            else
            {
                if (textSize.Height > height)
                {
                    height = (int) textSize.Height;
                }
                height += 8;
            }
            rectangle.Height = height;
        }

        /// <summary>
        /// Ensures that this item can be seen in the owner
        /// control.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item is not
        /// part of a ListBarGroup.</exception>
        [Description("Ensures that this item can be seen in the owning control.")]
        public virtual void EnsureVisible()
        {
            if (owner != null)
            {
                owner.EnsureItemVisible(this);
            }
            else
            {
                throw new InvalidOperationException("Owner of this ListBarItem has not been set.");
            }
        }

        /// <summary>
        /// Starts editing this item.  The <c>BeforeLabelEdit</c> event will
        /// be fired prior to the text box being made visible.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item is not
        /// part of a ListBarGroup or not part of the selected group
        /// in the control.</exception>
        [Description("Starts editing this item.  The BeforeLabelEdit event will be fired prior to editing commencing.")]
        public virtual void StartEdit()
        {
            if (owner != null)
            {
                owner.StartItemEdit(this);
            }
            else
            {
                throw new InvalidOperationException("Owner of this ListBarItem has not been set.");
            }
        }

        /// <summary>
        /// Notifies the owning control of a change in this item.
        /// </summary>
        /// <param name="addRemove">Set to true if the change
        /// that has been made requires the size of the display
        /// to be recalculated.</param>
        protected virtual void NotifyOwner(bool addRemove)
        {
            if (owner != null)
            {
                owner.ItemChanged(this, addRemove);
            }
        }

        /// <summary>
        /// Sets the owning ListBar control for this item.
        /// </summary>
        /// <param name="owner">The owning ListBar control for this item.</param>
        protected internal void SetOwner(ListBar owner)
        {
            this.owner = owner;
            NotifyOwner(true);
        }
    }

    #endregion

    #region ListBarScrollButton class

    /// <summary>
    /// A class which manages the behaviour and data associated with
    /// a scrolling button in the ListBar control.  This class can
    /// be overridden to provide (for example) an alternative rendering
    /// of the button.
    /// </summary>
    public class ListBarScrollButton : IMouseObject
    {
        #region ListBarScrollButtonType enum

        /// <summary>
        /// Enumeration of available scroll button types 
        /// for this control.
        /// </summary>
        public enum ListBarScrollButtonType
        {
            /// <summary>
            /// The scroll button is an up button.
            /// </summary>
            Up,

            /// <summary>
            /// The scroll button is a down button.
            /// </summary>
            Down
        }

        #endregion

        /// <summary>
        /// The type of scroll button.
        /// </summary>
        private ListBarScrollButtonType buttonType = ListBarScrollButtonType.Up;

        /// <summary>
        /// Whether the mouse is down on the button or not
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// The point at which the mouse was pressed on this button.
        /// </summary>
        private Point mouseDownPoint = new Point(0, 0);

        /// <summary>
        /// Whether the mouse is over this button or not
        /// </summary>
        private bool mouseOver;

        /// <summary>
        /// The bounding rectangle for this button
        /// </summary>
        private Rectangle rectangle = new Rectangle(0, 0, SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);

        /// <summary>
        /// ToolTip Text to display.
        /// </summary>
        private string toolTipText = "";

        /// <summary>
        /// Whether this button is visible or not.
        /// </summary>
        private bool visible;

        /// <summary>
        /// Creates a new instance of this class with the specified
        /// button type (Up or Down)
        /// </summary>
        /// <param name="buttonType">The scroll button type to create.</param>
        public ListBarScrollButton(ListBarScrollButtonType buttonType)
        {
            this.buttonType = buttonType;
        }

        /// <summary>
        /// Gets/sets whether this object is visible or not.
        /// </summary>
        [Description("Gets/sets whether this object is visible or not.")]
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (!value)
                {
                    mouseDown = false;
                    mouseOver = false;
                }
            }
        }

        /// <summary>
        /// Gets which type of scroll button this is (Up or Down)
        /// </summary>
        [Description("Gets which type of scroll button this is (Up or Down)")]
        public ListBarScrollButtonType ButtonType
        {
            get
            {
                return buttonType;
            }
        }

        /// <summary>
        /// Gets the bounding rectangle for this button.
        /// </summary>
        [Description("Gets the bounding rectangle for this button.")]
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
        }

        #region IMouseObject Members

        /// <summary>
        /// Gets/sets the tooltip text to display for this button.
        /// </summary>
        [Description("Gets/sets the tooltip text to display for this button.")]
        public string ToolTipText
        {
            get
            {
                return toolTipText;
            }
            set
            {
                toolTipText = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is down on this object or not.
        /// </summary>
        [Description("Gets/sets whether the mouse is down on this object or not.")]
        public bool MouseDown
        {
            get
            {
                return mouseDown;
            }
            set
            {
                mouseDown = value;
            }
        }

        /// <summary>
        /// Gets/sets whether the mouse is over this object or not.
        /// </summary>
        [Description("Gets/sets whether the mouse is over this object or not.")]
        public bool MouseOver
        {
            get
            {
                return mouseOver;
            }
            set
            {
                mouseOver = value;
            }
        }

        /// <summary>
        /// Gets/sets the point at which the mouse was pressed on
        /// this object.
        /// </summary>
        [Description("Gets/sets the point at which the mouse was pressed on this object.")]
        public Point MouseDownPoint
        {
            get
            {
                return mouseDownPoint;
            }
            set
            {
                mouseDownPoint = value;
            }
        }

        #endregion

        /// <summary>
        /// Determines whether the specified point is within the control.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>True if the point is over the button and the button
        /// is visible, false otherwise.</returns>
        public bool HitTest(Point pt)
        {
            bool hitTest = false;
            if (visible)
            {
                hitTest = rectangle.Contains(pt);
            }
            return hitTest;
        }

        /// <summary>
        /// Draws the button onto the specified <see cref="System.Drawing.Graphics" /> 
        /// object.
        /// </summary>
        /// <remarks>
        /// Note_ that this method is called by the owning bar even if the 
        /// the button's <see cref="Visible"/> property is set to <c>False</c>.
        /// In subclasses of this object this enables the button to 		
        /// be shown disabled when it isn't needed, rather than the default
        /// behaviour which is to remove it entirely.
        /// </remarks>
        /// <param name="gfx">The <see cref="System.Drawing.Graphics"/> object 
        /// to draw on.</param>
        /// <param name="defaultBackColor">The default background
        /// <see cref="System.Drawing.Color"/> to use when drawing
        /// the button.</param>
        /// <param name="controlEnabled">Whether the owning control is enabled
        /// or not.</param>
        public virtual void DrawItem(
            Graphics gfx,
            Color defaultBackColor,
            bool controlEnabled
            )
        {
            if (visible)
            {
                if (defaultBackColor.Equals(Color.FromKnownColor(KnownColor.Control)))
                {
                    // Use the default mechanism:
                    ButtonState buttonState = ButtonState.Normal;
                    if (controlEnabled)
                    {
                        buttonState = ((mouseDown && mouseOver) ? ButtonState.Pushed : ButtonState.Normal);
                    }
                    else
                    {
                        buttonState = ButtonState.Inactive;
                    }
                    System.Windows.Forms.ControlPaint.DrawScrollButton(gfx,
                                                  rectangle,
                                                  (buttonType == ListBarScrollButtonType.Up ? ScrollButton.Up : ScrollButton.Down),
                                                  buttonState);
                }
                else
                {
                    // Not as easy when using custom border colours:

                    // Fill background:
                    Brush br = new SolidBrush(defaultBackColor);
                    gfx.FillRectangle(br, rectangle);
                    br.Dispose();

                    // Draw the glyph:
                    Point centrePoint = new Point(
                        (rectangle.Width/2),
                        (rectangle.Height/2)
                        );
                    centrePoint.Offset(rectangle.Left + 1, rectangle.Top);
                    if (mouseDown && mouseOver)
                    {
                        centrePoint.Offset(1, 1);
                    }
                    int opposite = 0;
                    if (ButtonType == ListBarScrollButtonType.Up)
                    {
                        opposite = -4;
                        centrePoint.Offset(0, 2);
                    }
                    else
                    {
                        opposite = 4;
                        centrePoint.Offset(0, -1);
                    }

                    if (!controlEnabled)
                    {
                        br = new SolidBrush(CustomBorderColor.ColorLightLight(defaultBackColor));
                        centrePoint.Offset(1, 1);
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, br, centrePoint, 4, opposite);
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, br, centrePoint, -4, opposite);
                        br.Dispose();
                        centrePoint.Offset(-1, -1);
                        br = new SolidBrush(CustomBorderColor.ColorDark(defaultBackColor));
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, br, centrePoint, 4, opposite);
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, br, centrePoint, -4, opposite);
                        br.Dispose();
                    }
                    else
                    {
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, SystemBrushes.WindowText, centrePoint, 4, opposite);
                        ListBarUtility.FillRightAngleTriangle(
                            gfx, SystemBrushes.WindowText, centrePoint, -4, opposite);
                    }

                    // Draw the border:
                    CustomBorderColor.DrawBorder(
                        gfx, rectangle, defaultBackColor, true,
                        (mouseDown && mouseOver));
                }
            }
        }

        /// <summary>
        /// Sets the bounding rectangle for this button.
        /// </summary>
        /// <param name="rect"></param>
        protected internal virtual void SetRectangle(Rectangle rect)
        {
            rectangle = rect;
        }
    }

    #endregion

    #region IMouseObject interface

    /// <summary>
    /// An internal interface specifying the properties and methods which must
    /// be supported by an object in the control which interacts with the
    /// mouse.
    /// TODO_ think of a better name for this interface
    /// </summary>
    internal interface IMouseObject
    {
        /// <summary>
        /// Gets/sets the point at which the mouse button was
        /// pressed.
        /// </summary>
        Point MouseDownPoint { get; set; }

        /// <summary>
        /// Gets/sets the tooltip text for this object.
        /// </summary>
        string ToolTipText { get; set; }

        /// <summary>
        /// Gets/sets whether the mouse is over the object or not.
        /// </summary>
        bool MouseOver { get; set; }

        /// <summary>
        /// Gets/sets whether the mouse was pressed on the object or not.
        /// </summary>
        bool MouseDown { get; set; }
    }

    #endregion

    #region ListBarDragDropInsertPoint class

    /// <summary>
    /// An internal class to manage the drag-drop insert point
    /// within the control.
    /// </summary>
    internal class ListBarDragDropInsertPoint : IComparable
    {
        /// <summary>
        /// The item after the drag-drop insert point, if any 
        /// </summary>
        private ListBarItem itemAfter;

        /// <summary>
        /// The item before the drag-drop insert point, if any
        /// </summary>
        private ListBarItem itemBefore;

        /// <summary>
        /// If we're over an empty bar.
        /// </summary>
        private bool overEmptyBar;

        /// <summary>
        ///  Constructs a new instance of this class, setting the items
        ///  before and after the drag-drop insertion point.
        /// </summary>
        /// <param name="itemBefore">Item before the drag-drop insertion
        /// point, or null if no item before.</param>
        /// <param name="itemAfter">Item after the drag-drop insertion
        /// point, or null if no item after.</param>
        /// <param name="overEmptyBar">Whether the drag-drop insertion
        /// point should be displayed in an empty bar.</param>
        public ListBarDragDropInsertPoint(
            ListBarItem itemBefore,
            ListBarItem itemAfter,
            bool overEmptyBar
            )
        {
            this.itemBefore = itemBefore;
            this.itemAfter = itemAfter;
            this.overEmptyBar = overEmptyBar;
        }


        /// <summary>
        /// Returns the item before the drag-drop point, if any.  At least one
        /// of the properties ItemBefore or ItemAfter will return an item.
        /// </summary>
        public ListBarItem ItemBefore
        {
            get
            {
                return itemBefore;
            }
        }

        /// <summary>
        /// Returns the item after the drag-drop point, if any.  At least one
        /// of the properties ItemBefore or ItemAfter will return an item.
        /// </summary>
        public ListBarItem ItemAfter
        {
            get
            {
                return itemAfter;
            }
        }

        /// <summary>
        /// Returns whether the drag point is over an empty bar
        /// or not.
        /// </summary>
        public bool OverEmptyBar
        {
            get
            {
                return overEmptyBar;
            }
        }

        #region IComparable Members

        /// <summary>
        /// Compares this object with another object of the same type.
        /// This implementation is only really useful for testing equality
        /// </summary>
        /// <param name="obj">Another ListBarDragDropInsertPoint object</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the comparands.  
        /// The return value has these meanings: 
        /// &lt; 0: This instance is less than obj.  
        /// 0: This instance is equal to obj.  
        /// &gt; 0: This instance is greater than obj. </returns>
        public virtual Int32 CompareTo(Object obj)
        {
            int ret = 1;
            ListBarDragDropInsertPoint compare = (ListBarDragDropInsertPoint) obj;
            if (compare.ItemBefore == ItemBefore)
            {
                if (compare.ItemAfter == ItemAfter)
                {
                    if (compare.OverEmptyBar == OverEmptyBar)
                    {
                        ret = 0;
                    }
                }
            }
            return ret;
        }

        #endregion
    }

    #endregion

    #region Utility class (static methods)

    /// <summary>
    /// An internal class holding static utility methods for the ListBar
    /// control.
    /// </summary>
    internal class ListBarUtility
    {
        /// <summary>
        /// Private constructor - all methods are intended to be static
        /// so you shouldn't be able to create an instance of the class.
        /// </summary>
        private ListBarUtility()
        {
            // intentionally blank
        }

        /// <summary>
        /// Fills a right-angled triangle using the specified brush.  The
        /// origin of the triangle is taken to be the right-angle corner.
        /// </summary>
        /// <param name="gfx">Graphics object to draw onto.</param>
        /// <param name="brush">Brush to fill the right-angled triangle with.</param>
        /// <param name="origin">Location of the right-angle corner of the triangle.</param>
        /// <param name="adjacent">The length of the adjacent side of the triangle.</param>
        /// <param name="opposite">The length of the opposite side of the triangle.</param>
        public static void FillRightAngleTriangle(
            Graphics gfx,
            Brush brush,
            Point origin,
            int adjacent,
            int opposite
            )
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(origin.X, origin.Y, origin.X + adjacent, origin.Y);
            path.AddLine(origin.X + adjacent, origin.Y, origin.X, origin.Y + opposite);
            path.CloseFigure();
            gfx.FillPath(brush, path);
            path.Dispose();
        }

        /// <summary>
        /// Blends two colours together using the specified alpha amount.
        /// </summary>
        /// <param name="colorFrom">Base colour</param>
        /// <param name="colorTo">Colour to blend with the base colour.</param>
        /// <param name="alpha">Alpha amount to use when blending the colours.</param>
        /// <returns>The blended colour.</returns>
        public static Color BlendColor(
            Color colorFrom,
            Color colorTo,
            int alpha
            )
        {
            Color retColor = Color.FromArgb(
                ((colorFrom.R*alpha)/255) + ((colorTo.R*(255 - alpha))/255),
                ((colorFrom.G*alpha)/255) + ((colorTo.G*(255 - alpha))/255),
                ((colorFrom.B*alpha)/255) + ((colorTo.B*(255 - alpha))/255)
                );
            return retColor;
        }
    }

    #endregion
}