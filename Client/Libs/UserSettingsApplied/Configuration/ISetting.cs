namespace Waveface.Configuration
{
    public interface ISetting
    {
        event SettingValueCancelEventHandler ValueSaving;
        event SettingValueCancelEventHandler ValueLoading;

        ApplicationSettings ApplicationSettings { get; set; }
        SettingScope Scope { get; }
        string Description { get; }

        bool ThrowOnErrorDeserializing { get; }
        bool ThrowOnErrorSerializing { get; }
        bool ThrowOnErrorSaving { get; }
        bool ThrowOnErrorLoading { get; }
        bool HasChanged { get; }
        void Load();
        void Save();
    }
}