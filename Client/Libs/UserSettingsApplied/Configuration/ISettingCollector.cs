
namespace Waveface.Configuration
{
    public interface ISettingCollector
    {
        event SettingCollectorCancelEventHandler CollectingSetting;
        ApplicationSettings ApplicationSettings { get; set; }
        void Collect();
    }
}