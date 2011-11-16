
#region

using System;

#endregion

namespace Waveface.Configuration
{
    public class ValueSetting : ValueSettingBase
    {
        private readonly Type m_valueType;
        private object m_value;

        public override object OriginalValue
        {
            get { return LoadValue(Name, m_valueType, SerializeAs, DefaultValue); }
        }

        public override object Value
        {
            get { return m_value; }
            set { ChangeValue(value); }
        }

        public ValueSetting(string m_name, Type m_valueType) :
            this(m_name, m_valueType, null, null)
        {
        }
        
        public ValueSetting(string m_name, Type m_valueType, object value) :
            this(m_name, m_valueType, value, null)
        {
        }
        
        public ValueSetting(string name, Type valueType, object value, object defaultValue) :
            base(name, defaultValue)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (defaultValue != null && !defaultValue.GetType().Equals(valueType))
            {
                throw new ArgumentException("defaultValue");
            }

            m_valueType = valueType;
            ChangeValue(value);
        }
        
        public override void Load()
        {
            try
            {
                object _originalValue = OriginalValue;
                
                if (_originalValue == null && LoadUndefinedValue == false)
                {
                    return;
                }

                Value = _originalValue;
            }
            catch
            {
                if (ThrowOnErrorLoading)
                {
                    throw;
                }
            }
        }
        
        public override void Save()
        {
            try
            {
                object _toSaveValue = Value;

                if (_toSaveValue == null && SaveUndefinedValue == false)
                {
                    return;
                }

                SaveValue(Name, m_valueType, SerializeAs, _toSaveValue, DefaultValue);
            }
            catch
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
        }

        private void ChangeValue(object newValue)
        {
            if (newValue != null && !newValue.GetType().Equals(m_valueType))
            {
                throw new ArgumentException("newValue");
            }

            m_value = newValue;
        }
    }
}