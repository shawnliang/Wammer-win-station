
using System;
using System.Configuration;

namespace Waveface.Configuration
{
    public abstract class Setting : ISetting
    {
        public event SettingValueCancelEventHandler ValueSaving;
        public event SettingValueCancelEventHandler ValueLoading;

        public ApplicationSettings ApplicationSettings { get; set; }
        public SettingScope Scope { get; set; }
        public string Description { get; set; }
        public bool ThrowOnErrorDeserializing { get; set; }
        public bool ThrowOnErrorSerializing { get; set; }
        public bool ThrowOnErrorSaving { get; set; }
        public bool ThrowOnErrorLoading { get; set; }
        public abstract bool HasChanged { get; }

        protected Setting()
        {
            Scope = SettingScope.User;
        }

        public abstract void Load();
        public abstract void Save();

        protected virtual object OnValueSaving(string name, object value)
        {
            if (ValueSaving == null)
            {
                return value;
            }

            SettingValueCancelEventArgs _e = new SettingValueCancelEventArgs(this, name, value);
            ValueSaving(this, _e);
            return _e.Cancel ? null : _e.TargetValue;
        }

        protected virtual object OnValueLoading(string name, object value)
        {
            if (ValueLoading == null)
            {
                return value;
            }

            SettingValueCancelEventArgs _e = new SettingValueCancelEventArgs(this, name, value);
            ValueLoading(this, _e);
            return _e.Cancel ? null : _e.TargetValue;
        }

        protected void CreateSettingProperty(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            ApplicationSettings _applicationSettings = ApplicationSettings;

            if (_applicationSettings == null || _applicationSettings.DefaultProvider == null)
            {
                return;
            }

            SettingsProperty _settingsProperty = _applicationSettings.Properties[name];

            if (_settingsProperty != null)
            {
                return; // already present
            }

            SettingsAttributeDictionary _attributes = new SettingsAttributeDictionary();
            SettingAttribute _attribute;

            switch (Scope)
            {
                case SettingScope.Application:
                    // attribute = new ApplicationScopedSettingAttribute();
                    throw new NotImplementedException(); // currently not supported
                case SettingScope.User:
                    _attribute = new UserScopedSettingAttribute();
                    break;
                default:
                    return;
            }

            _attributes.Add(_attribute.TypeId, _attribute);

            _settingsProperty = new SettingsProperty(
                name, // name
                type, // type
                _applicationSettings.DefaultProvider, // settings provider
                false, // is readonly
                defaultValue, // default
                serializeAs, // serialize as
                _attributes, // attribute
                ThrowOnErrorDeserializing, // throw on deserialization
                ThrowOnErrorSerializing); // throw on serialization

            _applicationSettings.Properties.Add(_settingsProperty);
        }

        protected void CreateSettingPropertyValue(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            CreateSettingProperty(name, type, serializeAs, defaultValue);

            ApplicationSettings _applicationSettings = ApplicationSettings;
            SettingsProperty _settingsProperty = _applicationSettings.Properties[name];

            if (_settingsProperty != null)
            {
                return; // already present
            }

            SettingsPropertyValue settingsPropertyValue = new SettingsPropertyValue(_settingsProperty);
            _applicationSettings.PropertyValues.Add(settingsPropertyValue);
        }

        protected object LoadValue(string name, Type type, SettingsSerializeAs serializeAs)
        {
            return LoadValue(name, type, serializeAs, null);
        }

        protected object LoadValue(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            CreateSettingProperty(name, type, serializeAs, defaultValue);
            object _value = ApplicationSettings[name];
            return OnValueLoading(name, _value);
        }

        protected void SaveValue(string name, Type type, SettingsSerializeAs serializeAs, object value)
        {
            SaveValue(name, type, serializeAs, value, null);
        }

        protected void SaveValue(string name, Type type, SettingsSerializeAs serializeAs, object value, object defaultValue)
        {
            if (OnValueSaving(name, value) == null)
            {
                return;
            }

            CreateSettingPropertyValue(name, type, serializeAs, defaultValue);
            ApplicationSettings[name] = value;
        }
    }
}
