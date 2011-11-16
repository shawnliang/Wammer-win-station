#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class WindowSettings : ApplicationSettings
    {
        private readonly Window m_window;
        private readonly DependencyPropertySetting m_topSetting;
        private readonly DependencyPropertySetting m_leftSetting;
        private readonly DependencyPropertySetting m_widthSetting;
        private readonly DependencyPropertySetting m_heightSetting;
        private readonly DependencyPropertySetting m_stateSetting;
        private bool? m_saveCondition;
        private bool m_saveOnClose;

        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.RegisterAttached(
                "Settings",
                typeof (string),
                typeof (WindowSettings),
                new FrameworkPropertyMetadata(OnWindowSettingsChanged));

        public static readonly DependencyProperty CollectedSettingProperty =
            DependencyProperty.RegisterAttached(
                "CollectedSetting",
                typeof (DependencyProperty),
                typeof (WindowSettings),
                new FrameworkPropertyMetadata(OnCollectedSettingChanged));

        public static readonly DependencyProperty ExcludeElementProperty = DependencyProperty.RegisterAttached(
            "ExcludeElement",
            typeof (bool),
            typeof (WindowSettings));

        public WindowSettings(Window m_window) :
            this(m_window, m_window.GetType().Name)
        {
        }

        public WindowSettings(Window window, string settingsKey) :
            base(settingsKey)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            m_window = window;
            UseLocation = true;
            UseSize = true;
            UseWindowState = true;
            SaveOnClose = true;

            // settings 
            m_topSetting = CreateSetting("Window.Top", Window.TopProperty);
            m_leftSetting = CreateSetting("Window.Left", Window.LeftProperty);
            m_widthSetting = CreateSetting("Window.Width", FrameworkElement.WidthProperty);
            m_heightSetting = CreateSetting("Window.Height", FrameworkElement.HeightProperty);
            m_stateSetting = CreateSetting("Window.WindowState", Window.WindowStateProperty);

            // subscribe to parent window's events
            m_window.SourceInitialized += WindowSourceInitialized;
            m_window.Initialized += WindowInitialized;
            m_window.Loaded += WindowLoaded;
            m_window.Closing += WindowClosing;
        }

        #region Properties

        public Window Window
        {
            get { return m_window; }
        }

        public ISetting TopSetting
        {
            get { return m_topSetting; }
        }

        public ISetting LeftSetting
        {
            get { return m_leftSetting; }
        }

        public ISetting WidthSetting
        {
            get { return m_widthSetting; }
        }

        public ISetting HeightSetting
        {
            get { return m_heightSetting; }
        }

        public ISetting StateSetting
        {
            get { return m_stateSetting; }
        }

        public bool? SaveCondition
        {
            get { return m_saveCondition; }
            set { m_saveCondition = value; }
        }

        public bool UseLocation { get; set; }
        public bool UseSize { get; set; }
        public bool UseWindowState { get; set; }
        public bool AllowMinimized { get; set; }

        public bool SaveOnClose
        {
            get { return m_saveOnClose; }
            set { m_saveOnClose = value; }
        }

        #endregion

        public static string GetSettings(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SettingsProperty) as string;
        }

        public static void SetSettings(DependencyObject dependencyObject, string settingsKey)
        {
            dependencyObject.SetValue(SettingsProperty, settingsKey);
        }

        public static DependencyProperty GetCollectedSetting(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SettingsProperty) as DependencyProperty;
        }

        public static void SetCollectedSetting(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            dependencyObject.SetValue(SettingsProperty, dependencyProperty);
        }

        public static bool GetExcludeElement(DependencyObject obj)
        {
            return (bool) obj.GetValue(ExcludeElementProperty);
        }

        public static void SetExcludeElement(DependencyObject obj, bool exclude)
        {
            obj.SetValue(ExcludeElementProperty, exclude);
        }

        public override void Save()
        {
            if (m_saveCondition.HasValue && m_saveCondition != m_window.DialogResult)
            {
                return;
            }

            base.Save();
        }

        protected override void OnCollectingSetting(SettingCollectorCancelEventArgs e)
        {
            FrameworkElement _frameworkElement = e.Element as FrameworkElement;

            if (_frameworkElement == null)
            {
                e.Cancel = true;
                return;
            }

            // exclusion
            object _exclude = _frameworkElement.ReadLocalValue(ExcludeElementProperty);
            
            if (_exclude != null && _exclude.GetType() == typeof (bool) && (bool) _exclude)
            {
                e.Cancel = true;
                return;
            }

            base.OnCollectingSetting(e);
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            // By settings the window state here, it allows the window to be
            // maximized to the correct screen in a multi-monitor environment.
            if (UseWindowState)
            {
                Settings.Add(m_stateSetting);
            }
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            if (UseLocation)
            {
                Settings.Add(m_topSetting);
                Settings.Add(m_leftSetting);
            }

            if (UseSize)
            {
                Settings.Add(m_widthSetting);
                Settings.Add(m_heightSetting);
            }

            Load();
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

        private DependencyPropertySetting CreateSetting(string name, DependencyProperty dependencyProperty)
        {
            DependencyPropertySetting _propertySetting = new DependencyPropertySetting(name, m_window, dependencyProperty);
            _propertySetting.ValueSaving += ValueSaving;
            return _propertySetting;
        }

        private void ValueSaving(object sender, SettingValueCancelEventArgs e)
        {
            if (AllowMinimized == false && m_window.WindowState == WindowState.Minimized)
            {
                e.Cancel = true;
            }
        }

        private static void OnWindowSettingsChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs e)
        {
            Window _window = dependencyObject as Window;
           
            if (_window == null)
            {
                Debug.WriteLine("WindowSettings: invalid window");
                return;
            }

            if (_window.GetValue(DependencyPropertySetting.ApplicationSettingsProperty) != null)
            {
                return; // window contains already an application setting
            }

            string _settingsKey = e.NewValue as string;
            
            if (string.IsNullOrEmpty(_settingsKey))
            {
                Debug.WriteLine("WindowSettings: missing window settings key");
                return;
            }

            // create and attach the application settings to the window
            WindowSettings _windowSettings = new WindowSettings(_window, _settingsKey);
            _window.SetValue(DependencyPropertySetting.ApplicationSettingsProperty, _windowSettings);
        }

        private static void OnCollectedSettingChanged(DependencyObject dependencyObject,
                                                      DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement _frameworkElement = dependencyObject as FrameworkElement;
            
            if (_frameworkElement == null)
            {
                Debug.WriteLine("WindowSettings: invalid framework element");
                return;
            }

            DependencyProperty _dependencyProperty = e.NewValue as DependencyProperty;

            if (_dependencyProperty == null)
            {
                Debug.WriteLine("WindowSettings: missing dependency property");
                return;
            }

            WindowSettings _windowSettings =
                _frameworkElement.ReadLocalValue(DependencyPropertySetting.ApplicationSettingsProperty) as WindowSettings;
           
            if (_windowSettings == null)
            {
                Debug.WriteLine("WindowSettings: missing window settings in element " + _frameworkElement);
                return;
            }

            DependencyPropertySettingCollector _collector = new DependencyPropertySettingCollector(_frameworkElement, _dependencyProperty);
            _windowSettings.SettingCollectors.Add(_collector);
        }
    }
}