
#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.PopupControl
{
    // Represents a Windows combo box control with a custom popup control attached.
    [ToolboxBitmap(typeof (System.Windows.Forms.ComboBox)), ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"), Description("Displays an editable text box with a drop-down list of permitted values.")]
    public partial class PopupComboBox : ComboBox
    {
        private Popup m_dropDown;
        private Control m_dropDownControl;
        private DateTime m_dropDownHideTime;

        public PopupComboBox()
        {
            m_dropDownHideTime = DateTime.UtcNow;
            
            InitializeComponent();
            
            base.DropDownHeight = base.DropDownWidth = 1;
            base.IntegralHeight = false;
        }

        // Gets or sets the drop down control.
        public Control DropDownControl
        {
            get
            {
                return m_dropDownControl;
            }
            set
            {
                if (m_dropDownControl == value)
                {
                    return;
                }

                m_dropDownControl = value;

                if (m_dropDown != null)
                {
                    m_dropDown.Closed -= dropDown_Closed;
                    m_dropDown.Dispose();
                }

                m_dropDown = new Popup(value);
                m_dropDown.Closed += dropDown_Closed;
            }
        }

        // Gets or sets a value indicating whether the combo box is displaying its drop-down portion.
        public new bool DroppedDown
        {
            get
            {
                return m_dropDown.Visible;
            }
            set
            {
                if (DroppedDown)
                {
                    HideDropDown();
                }
                else
                {
                    ShowDropDown();
                }
            }
        }

        private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            m_dropDownHideTime = DateTime.UtcNow;
        }

        // Occurs when the drop-down portion of a System.Windows.Forms.ComboBox is shown.
        public new event EventHandler DropDown;

        public void ShowDropDown()
        {
            if (m_dropDown != null)
            {
                if ((DateTime.UtcNow - m_dropDownHideTime).TotalSeconds > 0.5)
                {
                    if (DropDown != null)
                    {
                        DropDown(this, EventArgs.Empty);
                    }

                    m_dropDown.Show(this);
                }
                else
                {
                    m_dropDownHideTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 1));
                    Focus();
                }
            }
        }

        // Occurs when the drop-down portion of the System.Windows.Forms.ComboBox is no longer visible.
        public new event EventHandler DropDownClosed;

        public void HideDropDown()
        {
            if (m_dropDown != null)
            {
                m_dropDown.Hide();
            
                if (DropDownClosed != null)
                {
                    DropDownClosed(this, EventArgs.Empty);
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (NativeMethods.WM_COMMAND + NativeMethods.WM_REFLECT) && NativeMethods.HIWORD(m.WParam) == NativeMethods.CBN_DROPDOWN)
            {
                BeginInvoke(new MethodInvoker(ShowDropDown));
                return;
            }

            base.WndProc(ref m);
        }

        #region " Unused Properties "

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownWidth
        {
            get
            {
                return base.DropDownWidth;
            }
            set
            {
                base.DropDownWidth = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownHeight
        {
            get
            {
                return base.DropDownHeight;
            }
            set
            {
                base.DropDownHeight = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IntegralHeight
        {
            get
            {
                return base.IntegralHeight;
            }
            set
            {
                base.IntegralHeight = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int ItemHeight
        {
            get
            {
                return base.ItemHeight;
            }
            set
            {
                base.ItemHeight = value;
            }
        }

        #endregion
    }
}