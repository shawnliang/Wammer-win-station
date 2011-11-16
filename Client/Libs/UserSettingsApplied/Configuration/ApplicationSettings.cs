#region

using System.Configuration;

#endregion

namespace Waveface.Configuration
{
    public class ApplicationSettings : ApplicationSettingsBase
    {
        public event SettingValueCancelEventHandler SettingSaving;
        public event SettingValueCancelEventHandler SettingLoading;
        public event SettingCollectorCancelEventHandler CollectingSetting;

        public const string UpgradeSettingsKey = "UpgradeSettings";

        private readonly LocalFileSettingsProvider m_defaultProvider = new LocalFileSettingsProvider();
        private readonly SettingCollection m_settings;
        private SettingCollectorCollection m_settingCollectors;
        private readonly ValueSetting m_upgradeSettings;

        #region Properties

        public SettingsProvider DefaultProvider
        {
            get { return m_defaultProvider; }
        }

        public SettingCollection Settings
        {
            get { return m_settings; }
        }

        public SettingCollectorCollection SettingCollectors
        {
            get
            {
                if (m_settingCollectors == null)
                {
                    m_settingCollectors = new SettingCollectorCollection(this);
                    m_settingCollectors.CollectingSetting += SettingCollectorsCollectingSetting;
                }

                return m_settingCollectors;
            }
        }

        public bool UseAutoUpgrade
        {
            get { return m_settings.Contains(m_upgradeSettings); }

            set
            {
                if (UseAutoUpgrade)
                {
                    return;
                }

                m_settings.Add(m_upgradeSettings);
            }
        }

        public static string UserConfigurationFilePath
        {
            get
            {
                System.Configuration.Configuration _config =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                return _config.FilePath;
            }
        }

        #endregion

        public ApplicationSettings(object obj) :
            this(obj.GetType().Name, obj)
        {
        }

        public ApplicationSettings(string settingsKey) :
            this(settingsKey, null)
        {
        }

        public ApplicationSettings(string settingsKey, object obj) :
            base(settingsKey)
        {
            m_settings = new SettingCollection(this);

            // provider
            m_defaultProvider = new LocalFileSettingsProvider();
            m_defaultProvider.Initialize("LocalFileSettingsProvider", null);
            base.Providers.Add(m_defaultProvider);

            // upgrade
            m_upgradeSettings = new ValueSetting(UpgradeSettingsKey, typeof (bool), true);
            UseAutoUpgrade = true;

            if (obj != null)
            {
                Settings.AddAll(obj);
            }
        }

        public void Load()
        {
            Load(true);
        }

        public virtual void Load(bool upgrade)
        {
            CollectSettings();
            LoadSettings();

            if (!upgrade)
            {
                return;
            }

            if ((bool) m_upgradeSettings.Value)
            {
                Upgrade();
                m_upgradeSettings.Value = false;
            }

            LoadSettings();
        }

        public new virtual void Reload()
        {
            base.Reload();

            LoadSettings();
        }

        public new virtual void Reset()
        {
            base.Reset();

            LoadSettings();
        }

        public override void Save()
        {
            SaveSettings();
            base.Save();
        }

        private void CollectSettings()
        {
            if (m_settingCollectors == null)
            {
                return; // no colectors present
            }

            m_settingCollectors.Collect();
        }

        private void LoadSettings()
        {
            foreach (ISetting userSetting in m_settings)
            {
                if (SettingLoading != null)
                {
                    userSetting.ValueLoading += UserSettingLoading;
                }

                userSetting.Load();

                if (SettingLoading != null)
                {
                    userSetting.ValueLoading -= UserSettingLoading;
                }
            }
        }

        private void SaveSettings()
        {
            foreach (ISetting userSetting in m_settings)
            {
                if (SettingSaving != null)
                {
                    userSetting.ValueSaving += UserSettingSaving;
                }

                userSetting.Save();

                if (SettingSaving != null)
                {
                    userSetting.ValueSaving -= UserSettingSaving;
                }
            }
        }

        protected virtual void OnCollectingSetting(SettingCollectorCancelEventArgs e)
        {
            if (CollectingSetting != null)
            {
                CollectingSetting(this, e);
            }
        }

        private void UserSettingSaving(object sender, SettingValueCancelEventArgs e)
        {
            if (SettingSaving != null)
            {
                SettingSaving(sender, e);
            }
        }

        private void UserSettingLoading(object sender, SettingValueCancelEventArgs e)
        {
            if (SettingLoading != null)
            {
                SettingLoading(sender, e);
            }
        }

        private void SettingCollectorsCollectingSetting(object sender, SettingCollectorCancelEventArgs e)
        {
            OnCollectingSetting(e);
        }
    }
}