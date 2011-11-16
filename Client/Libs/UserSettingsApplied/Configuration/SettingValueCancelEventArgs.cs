#region

using System;
using System.ComponentModel;

#endregion

namespace Waveface.Configuration
{
    public delegate void SettingValueCancelEventHandler(object sender, SettingValueCancelEventArgs e);

    public class SettingValueCancelEventArgs : CancelEventArgs
    {
        private readonly ISetting m_setting;
        private readonly string m_name;
        private readonly object m_value;
        private object m_targetValue;

        #region Properties

        public ISetting Setting
        {
            get { return m_setting; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public bool HasValue
        {
            get { return m_value != null; }
        }

        public object Value
        {
            get { return m_value; }
        }

        public object TargetValue
        {
            get { return m_targetValue; }
            set { m_targetValue = value; }
        }

        #endregion

        public SettingValueCancelEventArgs(ISetting m_setting, string m_name, object m_value) :
            this(m_setting, m_name, m_value, false)
        {
        }

        public SettingValueCancelEventArgs(ISetting setting, string name, object value, bool cancel) :
            base(cancel)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_setting = setting;
            m_name = name;
            m_value = value;
            m_targetValue = value;
        }
    }
}