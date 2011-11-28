#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Waveface.Component.MultiPage.Design;

#endregion

namespace Waveface.Component.MultiPage
{
    [Designer(typeof (MultiPanelPageDesigner))]
    public class MultiPanelPage : ContainerControl
    {
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = DockStyle.Fill;
            }
        }

        public MultiPanelPage()
        {
            base.Dock = DockStyle.Fill;
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        #region ControlCollection

        public new class ControlCollection : Control.ControlCollection
        {
            public ControlCollection(Control owner)
                : base(owner)
            {
                if (owner == null)
                    throw new ArgumentNullException("owner", "Tried to create a MultiPanelPage.ControlCollection with a null owner.");
                
                MultiPanelPage _c = owner as MultiPanelPage;
                
                if (_c == null)
                    throw new ArgumentException("Tried to create a MultiPanelPage.ControlCollection with a non-MultiPanelPage owner.", "owner");
            }

            public override void Add(Control value)
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Tried to add a null value to the MultiPanelPage.ControlCollection.");
                
                MultiPanelPage _p = value as MultiPanelPage;
                
                if (_p != null)
                    throw new ArgumentException("Tried to add a MultiPanelPage control to the MultiPanelPage.ControlCollection.", "value");
                
                base.Add(value);
            }
        }

        #endregion
    }
}