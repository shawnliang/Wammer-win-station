
#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class FrameworkElementSettings : ApplicationSettings
    {
        private readonly FrameworkElement m_frameworkElement;
        private bool m_saveOnClose = true;

        #region Properties

        public FrameworkElement FrameworkElement
        {
            get { return m_frameworkElement; }
        }

        public bool SaveOnClose
        {
            get { return m_saveOnClose; }
            set { m_saveOnClose = value; }
        }

        private Window ParentWindow
        {
            get
            {
                DependencyObject _control = m_frameworkElement;

                while (_control != null)
                {
                    if (_control is Window)
                    {
                        return _control as Window;
                    }

                    _control = LogicalTreeHelper.GetParent(_control);
                }

                return null;
            }
        }

        #endregion

        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.RegisterAttached(
                "Settings",
                typeof (string),
                typeof (FrameworkElementSettings),
                new FrameworkPropertyMetadata(OnFrameworkElementSettingsChanged));

        public static readonly DependencyProperty CollectedSettingProperty =
            DependencyProperty.RegisterAttached(
                "CollectedSetting",
                typeof (DependencyProperty),
                typeof (FrameworkElementSettings),
                new FrameworkPropertyMetadata(OnCollectedSettingChanged));

        public static readonly DependencyProperty ExcludeElementProperty = DependencyProperty.RegisterAttached(
            "ExcludeElement",
            typeof (bool),
            typeof (FrameworkElementSettings));

        public FrameworkElementSettings(FrameworkElement m_frameworkElement) :
            this(m_frameworkElement, m_frameworkElement.GetType().Name)
        {
        }

        public FrameworkElementSettings(FrameworkElement frameworkElement, string settingsKey) :
            base(settingsKey)
        {
            if (frameworkElement == null)
            {
                throw new ArgumentNullException("frameworkElement");
            }

            m_frameworkElement = frameworkElement;
            m_frameworkElement.Initialized += FrameworkElementInitialized;
        }

        public static string GetSettings(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SettingsProperty) as string;
        }

        public static void SetSettings(DependencyObject dependencyObject, string settingsKey)
        {
            dependencyObject.SetValue(SettingsProperty, settingsKey);
        }

        private void FrameworkElementInitialized(object sender, EventArgs e)
        {
            Window _window = ParentWindow;
           
            if (_window == null)
            {
                throw new InvalidOperationException();
            }

            // subscribe to the parent window events
            _window.Loaded += WindowLoaded;
            _window.Closing += WindowClosing;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (m_saveOnClose == false)
            {
                return;
            }

            Save();
        }

        private static void OnFrameworkElementSettingsChanged(DependencyObject dependencyObject,
                                                              DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement _frameworkElement = dependencyObject as FrameworkElement;
           
            if (_frameworkElement == null)
            {
                Debug.WriteLine("FrameworkElementSettings: invalid framework element");
                return;
            }

            if (_frameworkElement.GetValue(DependencyPropertySetting.ApplicationSettingsProperty) != null)
            {
                return; // framework-element contains already an application setting
            }

            string _settingsKey = e.NewValue as string;

            if (string.IsNullOrEmpty(_settingsKey))
            {
                Debug.WriteLine("FrameworkElementSettings: missing framework element settings key");
                return;
            }

            // create and attach the application settings to the framework-element
            FrameworkElementSettings _frameworkElementSettings = new FrameworkElementSettings(_frameworkElement, _settingsKey);
            _frameworkElement.SetValue(DependencyPropertySetting.ApplicationSettingsProperty, _frameworkElementSettings);
        }

        private static void OnCollectedSettingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement _frameworkElement = dependencyObject as FrameworkElement;

            if (_frameworkElement == null)
            {
                Debug.WriteLine("FrameworkElementSettings: invalid framework element");
                return;
            }

            DependencyProperty _dependencyProperty = e.NewValue as DependencyProperty;
           
            if (_dependencyProperty == null)
            {
                Debug.WriteLine("FrameworkElementSettings: missing dependency property");
                return;
            }

            // search the framework element settings
            FrameworkElementSettings _frameworkElementSettings =
                _frameworkElement.ReadLocalValue(DependencyPropertySetting.ApplicationSettingsProperty) as
                FrameworkElementSettings;
          
            if (_frameworkElementSettings == null)
            {
                Debug.WriteLine("FrameworkElementSettings: missing framework element settings in element " +
                                _frameworkElement);
                return;
            }

            DependencyPropertySettingCollector _collector = new DependencyPropertySettingCollector(_frameworkElement, _dependencyProperty);
            _frameworkElementSettings.SettingCollectors.Add(_collector);
        }

    }
}