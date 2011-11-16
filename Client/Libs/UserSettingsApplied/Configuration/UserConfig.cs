#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

#endregion

namespace Waveface.Configuration
{
    public class UserConfig
    {
        private readonly System.Configuration.Configuration m_configuration;

        public UserConfig(System.Configuration.Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            m_configuration = configuration;
        }

        
        public System.Configuration.Configuration Configuration
        {
            get { return m_configuration; }
        }

        public string FilePath
        {
            get { return m_configuration.FilePath; }
        }
        
        public string FileName
        {
            get { return new FileInfo(m_configuration.FilePath).Name; }
        }
        
        public List<ClientSettingsSection> Sections
        {
            get
            {
                List<ClientSettingsSection> sections = new List<ClientSettingsSection>();

                foreach (ConfigurationSectionGroup _sectionGroup in m_configuration.SectionGroups)
                {
                    UserSettingsGroup _userSettingsGroup = _sectionGroup as UserSettingsGroup;
                   
                    if (_userSettingsGroup == null)
                    {
                        continue;
                    }

                    foreach (ConfigurationSection _section in _userSettingsGroup.Sections)
                    {
                        ClientSettingsSection _clientSettingsSection = _section as ClientSettingsSection;
                        
                        if (_clientSettingsSection == null)
                        {
                            continue;
                        }

                        sections.Add(_clientSettingsSection);
                    }
                }

                return sections;
            }
        }
        
        private UserSettingsGroup UserSettingsGroup
        {
            get
            {
                foreach (ConfigurationSectionGroup sectionGroup in m_configuration.SectionGroups)
                {
                    UserSettingsGroup _userSettingsGroup = sectionGroup as UserSettingsGroup;
                    
                    if (_userSettingsGroup != null)
                    {
                        return _userSettingsGroup;
                    }
                }

                return null;
            }
        }

        public void RemoveAllSections()
        {
            UserSettingsGroup _userSettingsGroup = UserSettingsGroup;
            
            if (_userSettingsGroup == null)
            {
                return;
            }

            m_configuration.SectionGroups.Remove(_userSettingsGroup.SectionGroupName);
        }
        
        public void RemoveSection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            UserSettingsGroup _userSettingsGroup = UserSettingsGroup;
           
            if (_userSettingsGroup == null)
            {
                return;
            }

            ConfigurationSection _section = _userSettingsGroup.Sections[name];
            
            if (_section == null)
            {
                throw new InvalidOperationException("invalid section " + name);
            }

            _userSettingsGroup.Sections.Remove(name);
        }
        
        public bool HasSameSettings(UserConfig compareUserConfig)
        {
            if (compareUserConfig == null)
            {
                throw new ArgumentNullException("compareUserConfig");
            }

            if (m_configuration.SectionGroups.Count != compareUserConfig.m_configuration.SectionGroups.Count)
            {
                return false;
            }

            foreach (ConfigurationSectionGroup _compareSectionGroup in compareUserConfig.m_configuration.SectionGroups)
            {
                UserSettingsGroup _compareUserSettingsGroup = _compareSectionGroup as UserSettingsGroup;
                
                if (_compareUserSettingsGroup == null)
                {
                    continue;
                }

                UserSettingsGroup _userSettingsGroup =
                    m_configuration.SectionGroups[_compareSectionGroup.Name] as UserSettingsGroup;
               
                if (_userSettingsGroup == null || _userSettingsGroup.Sections.Count != _compareSectionGroup.Sections.Count)
                {
                    return false;
                }

                foreach (ConfigurationSection _compareSection in _compareSectionGroup.Sections)
                {
                    ClientSettingsSection _compareClientSettingsSection = _compareSection as ClientSettingsSection;
                   
                    if (_compareClientSettingsSection == null)
                    {
                        continue;
                    }

                    ClientSettingsSection _clientSettingsSection =
                        _userSettingsGroup.Sections[_compareSection.SectionInformation.Name] as ClientSettingsSection;
                    
                    if (_clientSettingsSection == null ||
                        _clientSettingsSection.Settings.Count != _compareClientSettingsSection.Settings.Count)
                    {
                        return false;
                    }

                    foreach (SettingElement _compateSettingElement in _compareClientSettingsSection.Settings)
                    {
                        SettingElement _settingElement = _clientSettingsSection.Settings.Get(_compateSettingElement.Name);
                       
                        if (_settingElement == null ||
                            !_settingElement.Value.ValueXml.InnerXml.Equals(_compateSettingElement.Value.ValueXml.InnerXml))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void Save()
        {
            m_configuration.Save();
        }
        
        public void SaveAs(string fileName)
        {
            m_configuration.SaveAs(fileName);
        }
        
        public void Import(UserConfig importUserConfig, bool overwriteSettings)
        {
            if (importUserConfig == null)
            {
                throw new ArgumentNullException("importUserConfig");
            }

            foreach (ConfigurationSectionGroup _importSectionGroup in importUserConfig.m_configuration.SectionGroups)
            {
                UserSettingsGroup _importUserSettingsGroup = _importSectionGroup as UserSettingsGroup;
               
                if (_importUserSettingsGroup == null)
                {
                    continue;
                }

                UserSettingsGroup _userSettingsGroup = m_configuration.SectionGroups[_importSectionGroup.Name] as UserSettingsGroup;
               
                if (_userSettingsGroup == null)
                {
                    _userSettingsGroup = new UserSettingsGroup();
                    m_configuration.SectionGroups.Add(_importSectionGroup.Name, _userSettingsGroup);
                }

                foreach (ConfigurationSection importSection in _importSectionGroup.Sections)
                {
                    ClientSettingsSection _importClientSettingsSection = importSection as ClientSettingsSection;
                    
                    if (_importClientSettingsSection == null)
                    {
                        continue;
                    }

                    ClientSettingsSection _clientSettingsSection =
                        _userSettingsGroup.Sections[importSection.SectionInformation.Name] as ClientSettingsSection;
                    
                    if (_clientSettingsSection == null)
                    {
                        _clientSettingsSection = new ClientSettingsSection();
                        _userSettingsGroup.Sections.Add(importSection.SectionInformation.Name, _clientSettingsSection);
                    }

                    foreach (SettingElement _importSettingElement in _importClientSettingsSection.Settings)
                    {
                        bool _newSetting = false;

                        SettingElement _settingElement = _clientSettingsSection.Settings.Get(_importSettingElement.Name);
                       
                        if (_settingElement == null)
                        {
                            _newSetting = true;
                            _settingElement = new SettingElement();
                            _settingElement.Name = _importSettingElement.Name;
                            _settingElement.SerializeAs = _importSettingElement.SerializeAs;
                            _clientSettingsSection.Settings.Add(_settingElement);
                        }

                        if (!_newSetting && !overwriteSettings)
                        {
                            continue;
                        }

                        SettingValueElement _settingValueElement = new SettingValueElement();
                        _settingValueElement.ValueXml = _importSettingElement.Value.ValueXml;
                        _settingElement.SerializeAs = _importSettingElement.SerializeAs;
                        _settingElement.Value = _settingValueElement;
                        _clientSettingsSection.Settings.Add(_settingElement);
                    }
                }
            }
        }

        public static UserConfig FromOpenExe()
        {
            System.Configuration.Configuration _configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            return new UserConfig(_configuration);
        }

        public static UserConfig FromFile(string path)
        {
            ExeConfigurationFileMap _exeConfigurationFileMap = new ExeConfigurationFileMap();
            _exeConfigurationFileMap.ExeConfigFilename = path;
            System.Configuration.Configuration _configuration =
                ConfigurationManager.OpenMappedExeConfiguration(_exeConfigurationFileMap, ConfigurationUserLevel.None);
            return new UserConfig(_configuration);
        }
    }
}