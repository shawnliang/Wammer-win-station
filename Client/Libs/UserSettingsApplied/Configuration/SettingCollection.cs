
using System;
using System.Collections;
using System.Reflection;

namespace Waveface.Configuration
{
    public sealed class SettingCollection : IEnumerable
    {
        private readonly ArrayList m_settings = new ArrayList();
        private readonly ApplicationSettings m_applicationSettings;

        #region Properties

        public ApplicationSettings ApplicationSettings
        {
            get { return m_applicationSettings; }
        }

        public int Count
        {
            get { return m_settings.Count; }
        }

        public bool HasChanges
        {
            get
            {
                foreach (ISetting _setting in m_settings)
                {
                    if (_setting.HasChanged)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        public SettingCollection(ApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException("applicationSettings");
            }

            m_applicationSettings = applicationSettings;
        }

        public IEnumerator GetEnumerator()
        {
            return m_settings.GetEnumerator();
        }

        public bool Contains(ISetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            return m_settings.Contains(setting);
        }

        public void Add(ISetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            setting.ApplicationSettings = m_applicationSettings;
            m_settings.Add(setting);
        }

        public void AddAll(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            // field settings
            FieldInfo[] _fieldInfos = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in _fieldInfos)
            {
                FieldSettingAttribute[] _settingAttributes = (FieldSettingAttribute[])fieldInfo.GetCustomAttributes(typeof(FieldSettingAttribute), true);

                if (_settingAttributes.Length <= 0)
                {
                    continue;
                }

                FieldSettingAttribute _settingAttribute = _settingAttributes[0];
                string _settingName = _settingAttribute.Name;

                if (string.IsNullOrEmpty(_settingName))
                {
                    _settingName = fieldInfo.Name;
                }

                object _defaultValue = _settingAttribute.DefaultValue;
                FieldSetting _fieldSetting = new FieldSetting(_settingName, obj, fieldInfo.Name, _defaultValue);
                Add(_fieldSetting);
            }

            // property settings
            PropertyInfo[] _propertyInfos = obj.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo _propertyInfo in _propertyInfos)
            {
                PropertySettingAttribute[] _settingAttributes = (PropertySettingAttribute[])_propertyInfo.GetCustomAttributes(typeof(PropertySettingAttribute), true);

                if (_settingAttributes.Length <= 0)
                {
                    continue;
                }

                PropertySettingAttribute _settingAttribute = _settingAttributes[0];
                string _settingName = _settingAttribute.Name;

                if (string.IsNullOrEmpty(_settingName))
                {
                    _settingName = _propertyInfo.Name;
                }

                object _defaultValue = _settingAttribute.DefaultValue;
                PropertySetting _propertySetting = new PropertySetting(_settingName, obj, _propertyInfo.Name, _defaultValue);
                Add(_propertySetting);
            }
        }

        public void Remove(ISetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            m_settings.Remove(setting);
        }

        public void Clear()
        {
            m_settings.Clear();
        }
    }
}