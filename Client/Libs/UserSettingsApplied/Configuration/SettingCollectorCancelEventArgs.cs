
#region

using System;
using System.ComponentModel;

#endregion

namespace Waveface.Configuration
{
    public delegate void SettingCollectorCancelEventHandler(object sender, SettingCollectorCancelEventArgs e);

    public class SettingCollectorCancelEventArgs : CancelEventArgs
    {
        private readonly object m_element;        
        
        public object Element
        {
            get { return m_element; }
        }

        public SettingCollectorCancelEventArgs(object m_element) :
            this(m_element, false)
        {
        }

        public SettingCollectorCancelEventArgs(object element, bool cancel) :
            base(cancel)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            m_element = element;
        }
    }
}