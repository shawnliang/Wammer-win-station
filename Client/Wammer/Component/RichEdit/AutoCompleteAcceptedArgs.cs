#region

using System;

#endregion

namespace Waveface.Component.RichEdit
{
    public class AutoCompleteAcceptedArgs : EventArgs
    {
        private string m_item;

        public string Item
        {
            get { return m_item; }
        }

        public AutoCompleteAcceptedArgs(string item)
        {
            m_item = item;
        }
    }
}