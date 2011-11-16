
#region

using System;
using System.Configuration;

#endregion

namespace Waveface.Configuration
{
    public abstract class ValueSettingBase : Setting
    {
        private readonly string m_name;

        #region Properties

        public string Name
        {
            get { return m_name; }
        }

        public object DefaultValue { get; set; }

        public SettingsSerializeAs SerializeAs { get; set; }

        public bool LoadUndefinedValue { get; set; }

        public bool SaveUndefinedValue { get; set; }

        public bool HasValue
        {
            get { return Value != null; }
        }

        public override bool HasChanged
        {
            get
            {
                object _originalValue = OriginalValue;
                object _value = Value;

                if (_originalValue == _value)
                {
                    return false;
                }

                return _originalValue == null || !_originalValue.Equals(_value);
            }
        }

        public abstract object OriginalValue { get; }

        public abstract object Value { get; set; }

        #endregion

        protected ValueSettingBase(string m_name) :
            this(m_name, null)
        {
        }

        protected ValueSettingBase(string name, object defaultValue)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            m_name = name;
            DefaultValue = defaultValue;
            SerializeAs = SettingsSerializeAs.String;
        }

        public override string ToString()
        {
            return string.Concat(m_name, "=", Value);
        }
    }
}