
#region

using System;
using System.Diagnostics;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class PropertySettingCollector : SettingCollector
    {
        private readonly FrameworkElement m_owner;
        private readonly Type m_elementType;
        private readonly string m_propertyName;

        public FrameworkElement Owner
        {
            get { return m_owner; }
        }

        public Type ElementType
        {
            get { return m_elementType; }
        }

        public string PropertyName
        {
            get { return m_propertyName; }
        }

        public PropertySettingCollector(FrameworkElement owner, Type elementType, string propertyName)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            m_owner = owner;
            m_elementType = elementType;
            m_propertyName = propertyName;
        }

        public override void Collect()
        {
            Collect(m_owner, true);
        }

        private void Collect(DependencyObject currentObject, bool recursive)
        {
            foreach (object _child in LogicalTreeHelper.GetChildren(currentObject))
            {
                DependencyObject _dependencyObject = _child as DependencyObject;

                if (_dependencyObject == null)
                {
                    continue;
                }

                FrameworkElement _frameworkElement = _child as FrameworkElement;

                if (_frameworkElement != null)
                {
                    bool _add = m_elementType.IsAssignableFrom(_frameworkElement.GetType());

                    if (_add && string.IsNullOrEmpty(_frameworkElement.Name))
                    {
                        _add = false;
                        Debug.WriteLine("PropertySettingCollector: missing name for framework element " + _frameworkElement);
                    }

                    if (_add && !OnCollectingSetting(_frameworkElement))
                    {
                        _add = false;
                    }

                    if (_add)
                    {
                        string _settingName = string.Concat(_frameworkElement.Name, ".", m_propertyName);
                        PropertySetting _propertySetting = new PropertySetting(_settingName, _frameworkElement, m_propertyName);
                        _propertySetting.DefaultValue = _propertySetting.Value;
                        ApplicationSettings.Settings.Add(_propertySetting);
                    }
                }

                if (recursive)
                {
                    Collect(_dependencyObject, true);
                }
            }
        }
    }
}
