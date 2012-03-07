#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.PopupControl
{
    // Represents a Windows combo box control which can be used in a popup's content control.
    [ToolboxBitmap(typeof (System.Windows.Forms.ComboBox)), ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"), Description("Displays an editable text box with a drop-down list of permitted values.")]
    public partial class ComboBox : System.Windows.Forms.ComboBox
    {
        private static Type s_modalMenuFilter;
        private static MethodInfo s_suspendMenuMode;
        private static MethodInfo s_resumeMenuMode;

        public ComboBox()
        {
            InitializeComponent();
        }

        private static Type modalMenuFilter
        {
            get
            {
                if (s_modalMenuFilter == null)
                {
                    s_modalMenuFilter = Type.GetType("System.Windows.Forms.ToolStripManager+ModalMenuFilter");
                }

                if (s_modalMenuFilter == null)
                {
                    s_modalMenuFilter = new List<Type>(typeof (ToolStripManager).Assembly.GetTypes())
                        .Find(type => type.FullName == "System.Windows.Forms.ToolStripManager+ModalMenuFilter");
                }

                return s_modalMenuFilter;
            }
        }

        private static MethodInfo suspendMenuMode
        {
            get
            {
                if (s_suspendMenuMode == null)
                {
                    Type _t = modalMenuFilter;
                    
                    if (_t != null)
                    {
                        s_suspendMenuMode = _t.GetMethod("SuspendMenuMode", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                }

                return s_suspendMenuMode;
            }
        }

        private static MethodInfo resumeMenuMode
        {
            get
            {
                if (s_resumeMenuMode == null)
                {
                    Type _t = modalMenuFilter;
                    
                    if (_t != null)
                    {
                        s_resumeMenuMode = _t.GetMethod("ResumeMenuMode", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                }

                return s_resumeMenuMode;
            }
        }

        private static void SuspendMenuMode()
        {
            MethodInfo _suspendMenuMode = suspendMenuMode;
            
            if (_suspendMenuMode != null)
            {
                _suspendMenuMode.Invoke(null, null);
            }
        }

        private static void ResumeMenuMode()
        {
            MethodInfo _resumeMenuMode = resumeMenuMode;
            
            if (_resumeMenuMode != null)
            {
                _resumeMenuMode.Invoke(null, null);
            }
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);

            SuspendMenuMode();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            ResumeMenuMode();

            base.OnDropDownClosed(e);
        }
    }
}