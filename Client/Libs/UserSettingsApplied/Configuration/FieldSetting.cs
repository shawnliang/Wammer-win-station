#region

using System;
using System.Reflection;

#endregion

namespace Waveface.Configuration
{
    public class FieldSetting : ValueSettingBase
    {
        private readonly object m_component;
        private readonly FieldInfo m_fieldInfo;

        #region Properties

        public FieldInfo FieldInfo
        {
            get { return m_fieldInfo; }
        }

        public string FieldName
        {
            get { return m_fieldInfo.Name; }
        }

        public object Component
        {
            get { return m_component; }
        }

        public override object OriginalValue
        {
            get { return LoadValue(Name, m_fieldInfo.FieldType, SerializeAs, DefaultValue); }
        }

        public override object Value
        {
            get { return m_fieldInfo.GetValue(m_component); }
            set { m_fieldInfo.SetValue(m_component, value); }
        }

        #endregion

        public FieldSetting(object component, FieldInfo fieldInfo) :
            this(fieldInfo.Name, component, fieldInfo)
        {
        }

        public FieldSetting(string m_name, object m_component, FieldInfo m_fieldInfo) :
            this(m_name, m_component, m_fieldInfo, null)
        {
        }

        public FieldSetting(string name, object component, FieldInfo fieldInfo, object defaultValue) :
            base(name, defaultValue)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            m_component = component;
            m_fieldInfo = fieldInfo;
        }

        public FieldSetting(object component, string fieldName) :
            this(fieldName, component, fieldName)
        {
        }

        public FieldSetting(string m_name, object m_component, string fieldName) :
            this(m_name, m_component, fieldName, null)
        {
        }

        public FieldSetting(string m_name, object m_component, string fieldName, object defaultValue) :
            base(m_name, defaultValue)
        {
            if (m_component == null)
            {
                throw new ArgumentNullException("m_component");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            this.m_component = m_component;
            m_fieldInfo = m_component.GetType().GetField(fieldName,
                                                     BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.NonPublic);
            if (m_fieldInfo == null)
            {
                throw new ArgumentException("missing setting field: " + fieldName);
            }

            if (m_fieldInfo.IsInitOnly) // readonly field
            {
                throw new ArgumentException("setting field '" + fieldName + "' is readonly");
            }
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
                object _value = Value;
                
                if (_value == null && SaveUndefinedValue == false)
                {
                    return;
                }

                SaveValue(Name, m_fieldInfo.FieldType, SerializeAs, _value, DefaultValue);
            }
            catch
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
        }
    }
}