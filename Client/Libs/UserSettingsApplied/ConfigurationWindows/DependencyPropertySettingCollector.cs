
#region

using System;
using System.Diagnostics;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class DependencyPropertySettingCollector : SettingCollector
    {
        private readonly FrameworkElement m_owner;
        private readonly DependencyProperty m_dependencyProperty;

        public FrameworkElement Owner
        {
            get { return m_owner; }
        }

        public DependencyProperty DependencyProperty
        {
            get { return m_dependencyProperty; }
        }

        public override void Collect()
        {
            Collect(m_owner, true);
        }

        public DependencyPropertySettingCollector(FrameworkElement owner, DependencyProperty dependencyProperty)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (dependencyProperty == null)
            {
                throw new ArgumentNullException("dependencyProperty");
            }

            m_owner = owner;
            m_dependencyProperty = dependencyProperty;
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
                    bool _add = m_dependencyProperty.OwnerType.IsAssignableFrom(_frameworkElement.GetType());

                    if (_add && string.IsNullOrEmpty(_frameworkElement.Name))
                    {
                        _add = false;
                        Debug.WriteLine("DependencyPropertySettingCollector: missing name for framework element " +
                                        _frameworkElement);
                    }

                    if (_add && !OnCollectingSetting(_frameworkElement))
                    {
                        _add = false;
                    }

                    if (_add)
                    {
                        string _settingName = string.Concat(_frameworkElement.Name, ".", m_dependencyProperty.Name);
                        DependencyPropertySetting _dependencyPropertySetting = new DependencyPropertySetting(_settingName, _frameworkElement, m_dependencyProperty);
                        _dependencyPropertySetting.DefaultValue = _dependencyPropertySetting.Value;
                        ApplicationSettings.Settings.Add(_dependencyPropertySetting);
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