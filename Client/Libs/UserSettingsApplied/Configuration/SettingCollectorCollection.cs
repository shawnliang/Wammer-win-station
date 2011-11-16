
#region

using System;
using System.Collections;

#endregion

namespace Waveface.Configuration
{
    public sealed class SettingCollectorCollection : IEnumerable
    {    
        public event SettingCollectorCancelEventHandler CollectingSetting;

        private readonly ArrayList m_settingCollectors = new ArrayList();
        private readonly ApplicationSettings m_applicationSettings;
        
        public ApplicationSettings ApplicationSettings
        {
            get { return m_applicationSettings; }
        }
    
        public int Count
        {
            get { return m_settingCollectors.Count; }
        }

        public SettingCollectorCollection(ApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException("applicationSettings");
            }

            m_applicationSettings = applicationSettings;
        }     

        public IEnumerator GetEnumerator()
        {
            return m_settingCollectors.GetEnumerator();
        }

        public void Add(ISettingCollector setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            setting.ApplicationSettings = m_applicationSettings;
            setting.CollectingSetting += SettingCollectingSetting;
            m_settingCollectors.Add(setting);
        }
        
        public void Remove(ISettingCollector setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException("setting");
            }

            setting.CollectingSetting -= SettingCollectingSetting;
            m_settingCollectors.Remove(setting);
        }
        
        public void Clear()
        {
            foreach (ISettingCollector _settingCollector in m_settingCollectors)
            {
                Remove(_settingCollector);
            }
        }
        
        public void Collect()
        {
            foreach (ISettingCollector _settingCollector in m_settingCollectors)
            {
                _settingCollector.Collect();
            }
        }
        
        private void SettingCollectingSetting(object sender, SettingCollectorCancelEventArgs e)
        {
            if (CollectingSetting != null)
            {
                CollectingSetting(this, e);
            }
        }
    }
}