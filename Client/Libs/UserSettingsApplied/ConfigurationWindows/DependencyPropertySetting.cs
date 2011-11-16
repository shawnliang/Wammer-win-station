
#region

using System;
using System.Diagnostics;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class DependencyPropertySetting : ValueSettingBase
    {
        private readonly DependencyObject m_dependencyObject;
        private readonly DependencyProperty m_dependencyProperty;

        public static readonly DependencyProperty ApplicationSettingsProperty =
            DependencyProperty.RegisterAttached(
                "ApplicationSettings",
                typeof(ApplicationSettings),
                typeof(DependencyPropertySetting));

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.RegisterAttached(
                "Property",
                typeof(DependencyProperty),
                typeof(DependencyPropertySetting),
                new FrameworkPropertyMetadata(OnDependencyPropertyChanged));

        public DependencyPropertySetting(DependencyObject dependencyObject, DependencyProperty dependencyProperty) :
            this(dependencyObject, dependencyProperty, null)
        {
        }

        public DependencyPropertySetting(DependencyObject dependencyObject, DependencyProperty dependencyProperty,
                                         object defaultValue) :
            this(
                                             dependencyProperty.Name, dependencyObject, dependencyProperty, defaultValue
                                             )
        {
        }

        public DependencyPropertySetting(string name, DependencyObject m_dependencyObject,
                                         DependencyProperty m_dependencyProperty) :
            this(name, m_dependencyObject, m_dependencyProperty, null)
        {
        }

        public DependencyPropertySetting(string name, DependencyObject dependencyObject,
                                         DependencyProperty dependencyProperty, object defaultValue) :
            base(name, defaultValue)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            if (dependencyProperty == null)
            {
                throw new ArgumentNullException("dependencyProperty");
            }

            m_dependencyObject = dependencyObject;
            m_dependencyProperty = dependencyProperty;
        }

        #region Properties

        public DependencyProperty DependencyProperty
        {
            get { return m_dependencyProperty; }
        }

        public DependencyObject DependencyObject
        {
            get { return m_dependencyObject; }
        }

        public override object OriginalValue
        {
            get { return LoadValue(Name, m_dependencyProperty.PropertyType, SerializeAs, DefaultValue); }
        }

        public override object Value
        {
            get { return m_dependencyObject.GetValue(m_dependencyProperty); }
            set { m_dependencyObject.SetValue(m_dependencyProperty, value); }
        }

        #endregion

        public static DependencyProperty GetProperty(DependencyObject obj)
        {
            return obj.GetValue(PropertyProperty) as DependencyProperty;
        }

        public static void SetProperty(DependencyObject obj, DependencyProperty dependencyProperty)
        {
            obj.SetValue(PropertyProperty, dependencyProperty);
        }

        public static ApplicationSettings GetApplicationSettings(DependencyObject obj)
        {
            return obj.GetValue(PropertyProperty) as ApplicationSettings;
        }

        public static void SetApplicationSettings(DependencyObject obj, ApplicationSettings applicationSettings)
        {
            obj.SetValue(PropertyProperty, applicationSettings);
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

                SaveValue(Name, m_dependencyProperty.PropertyType, SerializeAs, _value, DefaultValue);
            }
            catch
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
        }

        private static void OnDependencyPropertyChanged(DependencyObject dependencyObject,
                                                        DependencyPropertyChangedEventArgs e)
        {
            IFrameworkInputElement _frameworkInputElement = dependencyObject as IFrameworkInputElement;

            if (_frameworkInputElement == null)
            {
                Debug.WriteLine("DependencyPropertySetting: invalid framework element");
                return;
            }

            string _elementName = _frameworkInputElement.Name;

            if (string.IsNullOrEmpty(_elementName))
            {
                Debug.WriteLine("DependencyPropertySetting: missing name for framework element " + _frameworkInputElement);
                return; // name is required
            }

            DependencyProperty _dependencyProperty = e.NewValue as DependencyProperty;

            if (_dependencyProperty == null)
            {
                Debug.WriteLine("DependencyPropertySetting: missing dependency property");
                return;
            }

            // search on the parent-tree for application settings
            ApplicationSettings _applicationSettings = FindParentSettings(dependencyObject);

            if (_applicationSettings == null)
            {
                Debug.WriteLine("DependencyPropertySetting: missing application settings in parent hierarchy");
                return;
            }

            string settingName = string.Concat(_elementName, ".", _dependencyProperty.Name);
            _applicationSettings.Settings.Add(new DependencyPropertySetting(settingName, dependencyObject, _dependencyProperty));
        }

        private static ApplicationSettings FindParentSettings(DependencyObject element)
        {
            while (element != null)
            {
                ApplicationSettings _applicationSettings = element.ReadLocalValue(ApplicationSettingsProperty) as ApplicationSettings;

                if (_applicationSettings != null)
                {
                    return _applicationSettings;
                }

                element = LogicalTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}