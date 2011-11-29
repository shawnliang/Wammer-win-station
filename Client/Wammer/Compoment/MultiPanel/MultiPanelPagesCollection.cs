#region

using System;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.MultiPage
{
    public class MultiPanelPagesCollection : Control.ControlCollection
    {
        private MultiPanel m_owner;

        public MultiPanelPagesCollection(Control owner)
            : base(owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner", "Tried to create a MultiPanelPagesCollection with a null owner.");

            m_owner = owner as MultiPanel;

            if (m_owner == null)
                throw new ArgumentException("Tried to create a MultiPanelPagesCollection with a non-MultiPanel owner.", "owner");
        }

        public override void Add(Control value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Tried to add a null value to the MultiPanelPagesCollection.");

            MultiPanelPage _p = value as MultiPanelPage;

            if (_p == null)
                throw new ArgumentException("Tried to add a non-MultiPanelPage control to the MultiPanelPagesCollection", "value");

            _p.SendToBack();

            base.Add(_p);
        }

        public override void AddRange(Control[] controls)
        {
            foreach (MultiPanelPage _p in controls)
                Add(_p);
        }

        public override int IndexOfKey(string key)
        {
            Control _ctrl = base[key];

            return GetChildIndex(_ctrl);
        }
    }
}