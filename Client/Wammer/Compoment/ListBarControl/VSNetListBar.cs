#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.ListBarControl
{

    #region VSNetListBar

    /// <summary>
    /// A class renders like the VS.NET ListBar control.
    /// The primary changes are:
    /// <list type="bullet">To remove the control's border.</list>
    /// <list type="bullet">To place the scroll buttons adjacent to the bar
    /// selection buttons.</list>
    /// <list type="bullet">To provide a method to prevent an item from
    /// begin dragged.</list>
    /// <list type="bullet">To render a popup window showing additional
    /// detail of an item if it is too wide to display.</list>
    /// </summary>
    public class VSNetListBar : ListBar
    {
        /// <summary>
        /// Creates a new instance of the VSNetListBar control.
        /// </summary>
        public VSNetListBar()
        {
            SelectOnMouseDown = true;
        }

        /// <summary>
        /// Gets the collection of <see cref="VSNetListBarGroup"/> objects
        /// associated with the control.
        /// </summary>		
        public new VSNetListBarGroupCollection Groups
        {
            get
            {
                return (VSNetListBarGroupCollection) base.Groups;
            }
        }

        /// <summary>
        /// Returns the currently selected <see cref="VSNetListBarGroup"/>
        /// object in the control.
        /// </summary>
        public new VSNetListBarGroup SelectedGroup
        {
            get
            {
                return (VSNetListBarGroup) base.SelectedGroup;
            }
        }

        /// <summary>
        /// Creates a new instance of a <see cref="ListBarScrollButton"/> used by 
        /// this control to draw the scroll buttons.  Fired during control initialisation
        /// </summary>
        /// <param name="buttonType">The type of scroll button (Up or Down)
        /// to create</param>
        /// <returns>A new <see cref="ListBarScrollButton"/> which is drawn when a 
        /// <see cref="ListBarGroup"/> contains more items than can be 
        /// displayed.</returns>
        protected override ListBarScrollButton CreateListBarScrollButton(
            ListBarScrollButton.ListBarScrollButtonType buttonType)
        {
            return new VSNetListBarScrollButton(this, buttonType);
        }

        /// <summary>
        /// Returns a new instance of the object to hold a collection
        /// of groups.
        /// </summary>
        /// <returns>The new <see cref="VSNetListBarGroupCollection"/></returns>
        protected override ListBarGroupCollection CreateListBarGroupCollection()
        {
            return new VSNetListBarGroupCollection(this);
        }

        /// <summary>
        /// Called by the control's internal sizing mechanism.
        /// Returns the width of a <see cref="ListBarGroup" /> button
        /// rectangle.
        /// </summary>
        /// <param name="group">The <see cref="ListBarGroup"/> to get the size of the 
        /// button for.</param>
        /// <returns>The width of the button.</returns>
        protected override int GetGroupButtonWidth(ListBarGroup group)
        {
            int ret = base.GetGroupButtonWidth(group);
            ListBarGroup selectedGroup = base.SelectedGroup;
            if (group.Equals(selectedGroup))
            {
                ret -= (group.ButtonHeight + 2);
            }
            else
            {
                ListBarGroup nextGroup = GetNextGroup(selectedGroup);
                if (nextGroup != null)
                {
                    if (group.Equals(nextGroup))
                    {
                        ret -= (group.ButtonHeight + 2);
                    }
                }
            }
            return ret;
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
        protected override Rectangle GetScrollButtonRectangle(
            ListBarScrollButton.ListBarScrollButtonType buttonType,
            ListBarGroup selectedGroup,
            int internalGroupHeight
            )
        {
            Point location;
            Size size = new Size(
                selectedGroup.ButtonHeight,
                selectedGroup.ButtonHeight);
            bool rightToLeft = (base.RightToLeft == RightToLeft.Yes);
            int left = (rightToLeft ?
                                        selectedGroup.ButtonLocation.X - size.Width - 2 :
                                                                                            selectedGroup.ButtonLocation.X + selectedGroup.ButtonWidth + 2
                       );

            if (buttonType == ListBarScrollButton.ListBarScrollButtonType.Up)
            {
                location = new Point(
                    left,
                    selectedGroup.ButtonLocation.Y);
            }
            else
            {
                // is selectedGroup the last visible item in the control?				
                ListBarGroup nextGroup = GetNextGroup(selectedGroup);

                location = new Point(
                    left,
                    (nextGroup == null ?
                                           selectedGroup.ButtonLocation.Y + selectedGroup.ButtonHeight + internalGroupHeight - size.Height :
                                                                                                                                               nextGroup.ButtonLocation.Y));
            }

            return new Rectangle(location, size);
        }

        private ListBarGroup GetNextGroup(ListBarGroup group)
        {
            ListBarGroup nextGroup = null;
            for (int i = base.Groups.IndexOf(group) + 1; i < base.Groups.Count; i++)
            {
                if (base.Groups[i].Visible)
                {
                    nextGroup = base.Groups[i];
                    break;
                }
            }
            return nextGroup;
        }

        /// <summary>
        /// Called by the control's internal sizing mechanism.
        /// Returns the client size excluding the border of the
        /// control.
        /// </summary>
        /// <returns>A <c>Rectangle</c> providing the area to 
        /// draw the control into.</returns>
        protected override Rectangle GetClientRectangleExcludingBorder()
        {
            return ClientRectangle;
        }

        /// <summary>
        /// Draw a border around the control.  The default
        /// implementation draws a 1 pixel inset border.
        /// </summary>
        /// <param name="gfx">The graphics object to drawn onto.</param>
        protected override void RenderControlBorder(
            Graphics gfx)
        {
            // intentionally left blank.  This control has no border
        }
    }

    #endregion

    #region VSNetListBarGroupCollection

    /// <summary>
    /// Maintains the collection of groups within the VS.NET ListBar
    /// control.
    /// </summary>
    public class VSNetListBarGroupCollection : ListBarGroupCollection
    {
        /// <summary>
        /// Constructs a new instance of this collection.
        /// </summary>
        /// <param name="listBar">The owning <see cref="VSNetListBar"/> control.</param>
        public VSNetListBarGroupCollection(VSNetListBar listBar) : base(listBar)
        {
            // intentionally blank
        }

        /// <summary>
        /// Gets the <see cref="VSNetListBarGroup" /> at the specified 0-based index.
        /// </summary>
        public new VSNetListBarGroup this[int index]
        {
            get
            {
                return (VSNetListBarGroup) base[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="VSNetListBarGroup" /> with the specified key.
        /// </summary>
        public new VSNetListBarGroup this[string key]
        {
            get
            {
                return (VSNetListBarGroup) base[key];
            }
        }

        /// <summary>
        /// Adds a new group to the control.
        /// </summary>
        /// <param name="group">The <see cref="VSNetListBarGroup"/> to add.</param>
        /// <returns>The 0-based index of the added group.</returns>
        public virtual int Add(VSNetListBarGroup group)
        {
            return base.Add(group);
        }

        /// <summary>
        /// Adds a new group with the specified caption and returns a reference
        /// to it.
        /// </summary>
        /// <param name="caption">The caption for the group to create.</param>
        /// <returns>The newly created <see cref="VSNetListBarGroup"/></returns>
        public new virtual VSNetListBarGroup Add(string caption)
        {
            VSNetListBarGroup newGroup = new VSNetListBarGroup(caption);
            Add(newGroup);
            return newGroup;
        }

        /// <summary>
        /// Adds a range of groups to the collection.
        /// </summary>
        /// <param name="values">An array of <see cref="VSNetListBarGroup"/> objects.</param>
        public virtual void AddRange(VSNetListBarGroup[] values)
        {
            base.AddRange(values);
        }

        /// <summary>
        /// Adds a range of new groups with the specified captions to the collection.
        /// </summary>
        /// <param name="captions">Array of captions for the new groups to create.</param>
        public override void AddRange(string[] captions)
        {
            foreach (string caption in captions)
            {
                VSNetListBarGroup newGroup = new VSNetListBarGroup(caption);
                Add(newGroup);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="ListBarGroup"/> element is contained within 
        /// the control's collection of groups.
        /// </summary>
        /// <param name="group">The ListBarGroup to check if present.</param>
        /// <returns>True if the ListBarGroup is contained within the control's
        /// collection, False otherwise.</returns>
        public virtual bool Contains(VSNetListBarGroup group)
        {
            return base.Contains(group);
        }


        /// <summary>
        /// Gets the 0-based index of the specified <see cref="ListBarGroup"/> within this
        /// collection.
        /// </summary>
        /// <param name="group">The group to find the index for.</param>
        /// <returns>The 0-based index of the group, if found, otherwise - 1.</returns>
        [Description("Gets the 0-based index of the specified ListBarGroup within this collection.")]
        public virtual int IndexOf(VSNetListBarGroup group)
        {
            return base.IndexOf(group);
        }

        /// <summary>
        /// Inserts a group at the specified 0-based index in the collection
        /// of groups.
        /// </summary>
        /// <param name="index">The 0-based index to insert the group at.</param>
        /// <param name="group">The ListBarGroup to add.</param>
        [Description("(Inserts a group at the specified 0-based index in the collection of groups.")]
        public virtual void Insert(int index, VSNetListBarGroup group)
        {
            base.Insert(index, group);
        }

        /// <summary>
        /// Inserts a group immediately before the specified <see cref="VSNetListBarGroup"/>.
        /// </summary>
        /// <param name="groupBefore"><see cref="VSNetListBarGroup"/> to insert before.</param>
        /// <param name="group">Group to insert.</param>
        [Description("(Inserts a group immediately before the specified VSNetListBarGroup object.")]
        public virtual void InsertBefore(VSNetListBarGroup groupBefore, VSNetListBarGroup group)
        {
            base.InsertBefore(groupBefore, group);
        }

        /// <summary>
        /// Inserts a <see cref="VSNetListBarGroup"/> immediately after the specified VSNetListBarGroup.
        /// </summary>
        /// <param name="groupAfter">VSNetListBarGroup to insert after.</param>
        /// <param name="group">Group to insert.</param>
        [Description("(Inserts a group immediately after the specified VSNetListBarGroup object.")]
        public virtual void InsertAfter(VSNetListBarGroup groupAfter, VSNetListBarGroup group)
        {
            base.InsertAfter(groupAfter, group);
        }


        /// <summary>
        /// Removes the specified <see cref="ListBarGroup"/>.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        [Description("(Removes the specified VSNetListBarGroup object.")]
        public virtual void Remove(VSNetListBarGroup group)
        {
            InnerList.Remove(group);
            NotifyOwner(group, true);
        }
    }

    #endregion

    #region VSNetListBarGroup

    /// <summary>
    /// An extended ListBarGroup which renders with the VS.NET
    /// ListBar appearance
    /// </summary>
    public class VSNetListBarGroup : ListBarGroup
    {
        /// <summary>
        /// Constructs a new, blank instance of a ListBarGroup.
        /// </summary>
        public VSNetListBarGroup()
        {
            newDefaults();
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        public VSNetListBarGroup(
            string caption
            ) : base(caption)
        {
            newDefaults();
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption and items.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        /// <param name="subItems">The array of items to add to the group's
        /// collection of items.</param>
        public VSNetListBarGroup(
            string caption,
            ListBarItem[] subItems
            ) : base(caption, subItems)
        {
            newDefaults();
        }

        /// <summary>
        /// Constructs a new instance of a ListBarGroup with the specified
        /// caption and tooltip text.
        /// </summary>
        /// <param name="caption">Caption for the group's control button.</param>
        /// <param name="toolTipText">ToolTip text to show when hovering over
        /// the group.</param>
        public VSNetListBarGroup(
            string caption,
            string toolTipText
            ) : base(caption, toolTipText)
        {
            newDefaults();
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
        public VSNetListBarGroup(
            string caption,
            string toolTipText,
            object tag
            ) : base(caption, toolTipText, tag)
        {
            newDefaults();
        }

        /// <summary>
        /// Draws the items within this group bar onto the control.
        /// </summary>
        /// <param name="gfx">The graphics object to draw onto.</param>
        /// <param name="bounds">The bounding rectangle within which
        /// to draw the items.</param>
        /// <param name="ils">The ImageList object to use to draw
        /// the bar.</param>
        /// <param name="defaultFont">The default font to draw the item with.</param>
        /// <param name="style">The style to draw the ListBar in.</param>
        /// <param name="controlEnabled">Whether the control is enabled or not.</param>
        public override void DrawBar(
            Graphics gfx,
            Rectangle bounds,
            ImageList ils,
            Font defaultFont,
            ListBarDrawStyle style,
            bool controlEnabled
            )
        {
            if (ChildControl != null)
            {
                base.DrawBar(
                    gfx, bounds, ils, defaultFont,
                    style, controlEnabled);
            }
            else
            {
                Items.Draw(
                    gfx, bounds, ils, defaultFont,
                    style, View, controlEnabled,
                    ScrollOffset + 2 + rectangle.Bottom);
            }
        }

        /// <summary>
        /// Draws the button for this group bar onto the control.
        /// </summary>
        /// <param name="gfx">The graphics object to draw onto.</param>
        /// <param name="defaultFont">The default font to use to draw the control.</param>
        /// <param name="controlEnabled">Whether the control is enabled or not.</param>
        public override void DrawButton(
            Graphics gfx,
            Font defaultFont,
            bool controlEnabled
            )
        {
            bool rightToLeft = false;
            Font drawFont = Font;
            if (Owner != null)
            {
                rightToLeft = (Owner.RightToLeft == RightToLeft.Yes);
            }
            if (drawFont == null)
            {
                drawFont = defaultFont;
            }

            // Fill background:
            Brush br = new SolidBrush(BackColor);
            gfx.FillRectangle(br, rectangle);
            br.Dispose();

            // Draw the border:
            CustomBorderColor.DrawBorder(
                gfx, rectangle, BackColor, false, (MouseDown && MouseOver));

            // Draw the text:
            RectangleF textRect = new RectangleF(
                rectangle.Left + 2F, rectangle.Top + 1F,
                rectangle.Width - 4F, rectangle.Height - 2F);
            StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit |
                                                (rightToLeft ? StringFormatFlags.DirectionRightToLeft : 0));
            fmt.Alignment = StringAlignment.Near;
            fmt.LineAlignment = StringAlignment.Center;
            fmt.Trimming = StringTrimming.EllipsisWord;
            if (controlEnabled)
            {
                br = new SolidBrush(ForeColor);
                gfx.DrawString(Caption, drawFont, br, textRect, fmt);
                br.Dispose();
            }
            else
            {
                textRect.Offset(1F, 1F);
                gfx.DrawString(Caption, drawFont, SystemBrushes.ControlLightLight, textRect, fmt);
                textRect.Offset(-1F, -1F);
                gfx.DrawString(Caption, drawFont, SystemBrushes.ControlDark, textRect, fmt);
            }
            fmt.Dispose();
        }

        /// <summary>
        /// Sets the height of the button for this group.
        /// </summary>
        /// <param name="defaultFont">The default <see cref="System.Drawing.Font"/>
        /// to use if this item doesn't already have a font set.</param>
        public override void SetButtonHeight(
            Font defaultFont
            )
        {
            base.SetButtonHeight(defaultFont);
            rectangle.Height -= 3;
        }

        private void newDefaults()
        {
            rectangle.Height = 17;
            View = ListBarGroupView.SmallIcons;
        }
    }

    #endregion

    #region VSNetListBarItem

    /// <summary>
    /// An extended ListBarItem which renders with an alpha-blended shadow
    /// appearance.
    /// </summary>
    public class VSNetListBarItem : ListBarItem
    {
        /// <summary>
        /// Constructs a new, empty instance of a ListBarItem.
        /// </summary>
        public VSNetListBarItem()
        {
            newDefaults();
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption to display.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        public VSNetListBarItem(string caption) : base(caption)
        {
            newDefaults();
        }

        /// <summary>
        ///  Constructs a new instance of a ListBarItem, specifying
        ///  the caption and the index of the icon to display.
        /// </summary>
        /// <param name="caption">The caption for this item.</param>
        /// <param name="iconIndex">The 0-based index of the icon
        /// to display</param>
        public VSNetListBarItem(
            string caption,
            int iconIndex
            ) : base(caption, iconIndex)
        {
            newDefaults();
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
        public VSNetListBarItem(
            string caption,
            int iconIndex,
            string toolTipText
            ) : base(caption, iconIndex, toolTipText)
        {
            newDefaults();
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
        public VSNetListBarItem(
            string caption,
            int iconIndex,
            string toolTipText,
            object tag
            ) : base(caption, iconIndex, toolTipText, tag)
        {
            newDefaults();
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
        public VSNetListBarItem(
            string caption,
            int iconIndex,
            string toolTipText,
            object tag,
            string key
            ) : base(caption, iconIndex, toolTipText, tag, key)
        {
            newDefaults();
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
        public override void SetLocationAndWidth(Point location, int width)
        {
            if (Owner != null)
            {
                //width = this.Owner.Width;
            }
            base.SetLocationAndWidth(location, width);
        }

        /// <summary>
        /// Draws this item into the specified graphics object.
        /// </summary>
        /// <param name="gfx">The graphics object to draw onto.</param>
        /// <param name="ils">The ImageList to source icons from.</param>
        /// <param name="defaultFont">The default font to use to draw the button.</param>
        /// <param name="style">The style (Outlook version) to draw using.</param>
        /// <param name="view">The view (large or small icons) to draw using.</param>
        /// <param name="scrollOffset">The offset of the first item from the 
        /// (0,0) point in the graphics object.</param>
        /// <param name="skipDrawText">Whether to skip drawing text or not
        /// (the item is being edited)</param>
        /// <param name="controlEnabled">Whether the control is enabled or not.</param>
        public override void DrawButton(
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
            Font drawFont = Font;
            if (drawFont == null)
            {
                drawFont = defaultFont;
            }
            Color backColor = Color.FromKnownColor(KnownColor.Control);
            if (Owner != null)
            {
                if (Owner.RightToLeft == RightToLeft.Yes)
                {
                    rightToLeft = true;
                }
                backColor = Owner.BackColor;
            }

            Rectangle drawRect = new Rectangle(Location,
                                               new Size(Width, Height));
            drawRect.Offset(0, scrollOffset);
            if (((Selected) && (!MouseOver)) || (MouseOver && MouseDown))
            {
                // Draw the background:
                VSNetListBarUtility.DrawSelectedItemBackground(gfx, drawRect);
            }

            Rectangle itemRect = drawRect;
            itemRect.Inflate(-1, -1);

            if ((Selected) || (MouseDown && MouseOver))
            {
                itemRect.Offset(1, 1);
            }

            RectangleF textRect;
            // Draw the icon:
            if (rightToLeft)
            {
                iconRectangle = new Rectangle(
                    itemRect.Right - ils.ImageSize.Width - 4,
                    itemRect.Top + (itemRect.Height - ils.ImageSize.Height)/2,
                    ils.ImageSize.Width, ils.ImageSize.Height);
                textRect = new RectangleF(
                    itemRect.Left, itemRect.Top,
                    itemRect.Width - (ils.ImageSize.Width - 6),
                    itemRect.Height);
            }
            else
            {
                iconRectangle = new Rectangle(
                    itemRect.Left + 4,
                    itemRect.Top + (itemRect.Height - ils.ImageSize.Height)/2,
                    ils.ImageSize.Width, ils.ImageSize.Height);
                textRect = new RectangleF(
                    itemRect.Left + ils.ImageSize.Height + 6, itemRect.Top,
                    itemRect.Width - (ils.ImageSize.Width - 6),
                    itemRect.Height);
            }
            if (IconIndex <= ils.Images.Count)
            {
                if (Enabled && controlEnabled)
                {
                    ils.Draw(gfx, iconRectangle.Left, iconRectangle.Top,
                             IconIndex);
                }
                else
                {
                    System.Windows.Forms.ControlPaint.DrawImageDisabled(gfx, ils.Images[IconIndex],
                                                   iconRectangle.Left, iconRectangle.Top, backColor);
                }
            }

            if ((view == ListBarGroupView.SmallIconsOnly) || (view == ListBarGroupView.LargeIconsOnly))
            {
                textRectangle = new Rectangle(0, 0, 0, 0);
                skipDrawText = true;
            }

            if (!skipDrawText)
            {
                // Draw the text:
                StringFormat format = new StringFormat(StringFormatFlags.LineLimit |
                                                       (rightToLeft ? StringFormatFlags.DirectionRightToLeft : 0));
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisWord;
                if (Enabled && controlEnabled)
                {
                    Brush br = new SolidBrush(ForeColor);
                    gfx.DrawString(Caption, drawFont, br, textRect, format);
                    br.Dispose();
                }
                else
                {
                    Brush br = new SolidBrush(CustomBorderColor.ColorLightLight(backColor));
                    textRect.Offset(1F, 1F);
                    gfx.DrawString(Caption, drawFont, br, textRect, format);
                    br.Dispose();
                    textRect.Offset(-1F, -1F);
                    br = new SolidBrush(CustomBorderColor.ColorDark(backColor));
                    gfx.DrawString(Caption, drawFont, br, textRect, format);
                    br.Dispose();
                }
                format.Dispose();

                textRectangle = new Rectangle((int) textRect.Left, (int) textRect.Top,
                                              (int) textRect.Width, (int) textRect.Height);

                // TODO_ If mouse over and text too wide show the popup.
                // Currently a problem with my popup code in that it 
                // foils the Framework's WM_NCACTIVATE processing which 
                // results in spurious focus/mouse over events...
            }


            // Draw the border:
            if (((MouseOver) || (Selected)) && (Enabled))
            {
                CustomBorderColor.DrawBorder(gfx, drawRect,
                                             backColor, true,
                                             ((MouseDown && MouseOver) || (Selected)));
            }
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
        public override void SetSize(
            ListBarGroupView view,
            Font defaultFont,
            Size imageSize)
        {
            base.SetSize(view, defaultFont, imageSize);
            rectangle.Height -= 3;

            rectangle.Height = (view == ListBarGroupView.LargeIcons) ? rectangle.Height - 24 : rectangle.Height;
        }

        /// <summary>
        /// Incomplete method.  To be used to render an item in the 
        /// popup view.
        /// </summary>
        /// <param name="gfx">Graphics object to draw onto</param>
        public virtual void PopupDraw(Graphics gfx)
        {
            if (Owner != null)
            {
                //this.Owner.PopupDraw(gfx, this);
            }
        }

        private void newDefaults()
        {
        }
    }

    #endregion

    #region VSNetListBarScrollButton

    /// <summary>
    /// A subclassed ListBarScrollButton which renders in the style
    /// of the VS.NET scroll buttons in the ListBar.
    /// </summary>
    public class VSNetListBarScrollButton : ListBarScrollButton
    {
        /// <summary>
        /// The owner control for this scroll button
        /// </summary>
        protected VSNetListBar owner;

        /// <summary>
        /// Creates a new instance of this class with the specified
        /// button type (Up or Down)
        /// </summary>
        /// <param name="owner">The owning control.</param>
        /// <param name="buttonType">The scroll button type to create.</param>
        public VSNetListBarScrollButton(
            VSNetListBar owner,
            ListBarScrollButtonType buttonType
            ) : base(buttonType)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Draws the button onto the specified Graphics object.
        /// </summary>
        /// <param name="gfx">The graphics object to draw on.</param>
        /// <param name="defaultBackColor">The default background
        /// colour to use when drawing the button.</param>
        /// <param name="controlEnabled">Whether the control is enabled or not.</param>
        public override void DrawItem(
            Graphics gfx,
            Color defaultBackColor,
            bool controlEnabled
            )
        {
            Color backColor = defaultBackColor;
            if (owner != null)
            {
                VSNetListBarGroup selGroup = owner.SelectedGroup;
                if (selGroup != null)
                {
                    if (ButtonType == ListBarScrollButtonType.Up)
                    {
                        backColor = selGroup.BackColor;
                    }
                    else
                    {
                        VSNetListBarGroup nextGroup = null;
                        for (int i = owner.Groups.IndexOf(selGroup) + 1; i < owner.Groups.Count; i++)
                        {
                            if (owner.Groups[i].Visible)
                            {
                                nextGroup = owner.Groups[i];
                                break;
                            }
                        }
                        if (nextGroup != null)
                        {
                            backColor = nextGroup.BackColor;
                        }
                    }
                }
            }
            Brush br = new SolidBrush(backColor);
            gfx.FillRectangle(br, Rectangle);
            br.Dispose();
            // in VS.NET bar the item is always drawn.  When
            // it's not "Visible" then it is drawn disabled:
            VSNetListBarUtility.DrawScrollButton(
                gfx, Rectangle, backColor,
                (MouseDown && MouseOver),
                (Visible && controlEnabled),
                (ButtonType != ListBarScrollButtonType.Up));
        }
    }

    #endregion

    #region VSNetListBarUtility

    /// <summary>
    /// Utility methods for drawing the <see cref="VSNetListBar"/>
    /// control.
    /// </summary>
    internal class VSNetListBarUtility
    {
        /// <summary>
        /// Private constructor, all methods static
        /// </summary>
        private VSNetListBarUtility()
        {
            // intentionally blank
        }

        public static void DrawSelectedItemBackground(
            Graphics gfx,
            Rectangle rect
            )
        {
            Brush br = new SolidBrush(Color.FromArgb(128, Color.White));
            gfx.FillRectangle(br, rect);
            br.Dispose();
        }


        /// <summary>
        /// Draws a scroll button for the <see cref="VSNetListBar"/> control.
        /// </summary>
        /// <param name="gfx"><see cref="Graphics"/> object to draw onto.</param>
        /// <param name="rect"><see cref="System.Drawing.Rectangle"/> to draw button in.</param>
        /// <param name="color">The <see cref="System.Drawing.Color"/> of the background for the button.</param>
        /// <param name="pressed">Whether the button is pressed or not.</param>
        /// <param name="enabled">Whether the button is enabled or not.</param>
        /// <param name="down">If <c>True</c>, the button is a scroll down button,
        /// otherwise it is a scroll up button.</param>
        public static void DrawScrollButton(
            Graphics gfx,
            Rectangle rect,
            Color color,
            bool pressed,
            bool enabled,
            bool down
            )
        {
            if (!enabled)
            {
                Brush br = new SolidBrush(CustomBorderColor.ColorLightLight(color));
                DrawScrollButtonTriangle(gfx, rect,
                                         br, 1, down);
                br.Dispose();
                br = new SolidBrush(CustomBorderColor.ColorDark(color));
                DrawScrollButtonTriangle(gfx, rect,
                                         br, 0, down);
                br.Dispose();
            }
            else
            {
                DrawScrollButtonTriangle(gfx, rect,
                                         SystemBrushes.WindowText, (pressed ? 1 : 0), down);
            }
            CustomBorderColor.DrawBorder(
                gfx, rect, color, true, pressed);
        }

        private static void DrawScrollButtonTriangle(
            Graphics gfx,
            Rectangle rect,
            Brush brush,
            int offset,
            bool down
            )
        {
            // where to put a 9 x 5 triangle in rect:
            PointF location = new PointF(
                (rect.Width - 2F - 9F)/2F + rect.Left + 1F,
                (rect.Height - 2F - 5F)/2F + rect.Top + 1F);
            location.X += offset;
            location.Y += offset;
            RectangleF triangleRect = new RectangleF(location,
                                                     new SizeF(10F, 6F));

            // draw the triangle:
            GraphicsPath glyph = new GraphicsPath();
            if (down)
            {
                glyph.AddLine(triangleRect.Left, triangleRect.Top,
                              triangleRect.Right - 1, triangleRect.Top);
                glyph.AddLine(triangleRect.Right - 1, triangleRect.Top,
                              triangleRect.Left + 4, triangleRect.Bottom - 1);
                glyph.CloseFigure();
            }
            else
            {
                glyph.AddLine(triangleRect.Left, triangleRect.Bottom - 1,
                              triangleRect.Right - 1, triangleRect.Bottom - 1);
                glyph.AddLine(triangleRect.Right - 1, triangleRect.Bottom - 1,
                              triangleRect.Left + 4, triangleRect.Top);
                glyph.CloseFigure();
            }

            gfx.FillPath(brush, glyph);
            glyph.Dispose();
        }
    }

    #endregion

    #region VSNetListItemPopup

    internal class VSNetListItemPopup : Control
    {
        #region Unmanaged Code

        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;

        [DllImport("user32")]
        private static extern int SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent);

        [DllImport("user32")]
        private static extern int ShowWindow(
            IntPtr hWnd,
            int nCmdShow);

        #endregion

        /// <summary>
        /// The ListBar which owns this popup
        /// </summary>
        private VSNetListBarItem owner;

        /// <summary>
        /// Get the <see cref="System.Windows.Forms.CreateParams"/>
        /// used to create the control.  This override adds the
        /// <code>WS_EX_NOACTIVATE</code>, <code>WS_EX_TOOLWINDOW</code>
        /// and <code>WS_EX_TOPMOST</code> extended styles to make
        /// the Window float on top.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ExStyle |= (WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
                return p;
            }
        }

        /// <summary>
        /// Shows the control as a floating Window child 
        /// of the desktop.  To hide the control again,
        /// use the <see cref="System.Windows.Forms.Control.Visible"/> property.
        /// </summary>
        public void ShowFloating(VSNetListBarItem owner)
        {
            this.owner = owner;
            if (Handle == IntPtr.Zero)
            {
                CreateControl();
            }
            SetParent(Handle, IntPtr.Zero);
            ShowWindow(Handle, 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            owner.PopupDraw(e.Graphics);
        }
    }

    #endregion
}