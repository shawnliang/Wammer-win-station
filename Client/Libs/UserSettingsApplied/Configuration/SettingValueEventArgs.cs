
#region

using System;

#endregion

namespace Waveface.Configuration
{
    public delegate void SettingValueEventHandler(object sender, SettingValueEventArgs e);

    public class SettingValueEventArgs : EventArgs
    {
        private readonly ISetting m_setting;
        private readonly string m_name;
        private readonly object m_value;
        private object m_targetValue;

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

        public SettingValueEventArgs(ISetting setting, string name, object value)
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

        public ISetting Setting
        {
            get { return m_setting; }
        }
    }
}