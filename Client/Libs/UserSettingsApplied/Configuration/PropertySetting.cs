#region

using System;
using System.Reflection;

#endregion

namespace Waveface.Configuration
{
    public class PropertySetting : ValueSettingBase
    {
        private readonly object m_component;
        private readonly PropertyInfo m_propertyInfo;

        #region Properties

        public PropertyInfo PropertyInfo
        {
            get { return m_propertyInfo; }
        }

        public string PropertyName
        {
            get { return m_propertyInfo.Name; }
        }

        public object Component
        {
            get { return m_component; }
        }

        public override object OriginalValue
        {
            get { return LoadValue(Name, m_propertyInfo.PropertyType, SerializeAs, DefaultValue); }
        }

        public override object Value
        {
            get { return m_propertyInfo.GetValue(m_component, null); }
            set { m_propertyInfo.SetValue(m_component, value, null); }
        }

        #endregion

        public PropertySetting(object component, PropertyInfo propertyInfo) :
            this(propertyInfo.Name, component, propertyInfo)
        {
        }

        public PropertySetting(string m_name, object m_component, PropertyInfo m_propertyInfo) :
            this(m_name, m_component, m_propertyInfo, null)
        {
        }

        public PropertySetting(string name, object component, PropertyInfo propertyInfo, object defaultValue) :
            base(name, defaultValue)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            m_component = component;
            m_propertyInfo = propertyInfo;
        }

        public PropertySetting(object component, string propertyName) :
            this(propertyName, component, propertyName)
        {
        }

        public PropertySetting(string m_name, object m_component, string propertyName) :
            this(m_name, m_component, propertyName, null)
        {
        }

        public PropertySetting(string m_name, object component_, string propertyName, object defaultValue) :
            base(m_name, defaultValue)
        {
            if (component_ == null)
            {
                throw new ArgumentNullException("component_");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            m_component = component_;
            m_propertyInfo = component_.GetType().GetProperty(propertyName,
                                                               BindingFlags.Instance | BindingFlags.Public |
                                                               BindingFlags.NonPublic);
            if (m_propertyInfo == null)
            {
                throw new ArgumentException("missing setting property: " + propertyName);
            }

            if (!m_propertyInfo.CanRead) // no get; accessor
            {
                throw new ArgumentException("setting property '" + propertyName + "' must be readable");
            }

            if (!m_propertyInfo.CanWrite) // no set; accessor
            {
                throw new ArgumentException("setting property '" + propertyName + "' must be writeable");
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

                SaveValue(Name, m_propertyInfo.PropertyType, SerializeAs, _value, DefaultValue);
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