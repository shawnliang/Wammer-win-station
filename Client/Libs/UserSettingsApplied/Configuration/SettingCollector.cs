namespace Waveface.Configuration
{
    public abstract class SettingCollector : ISettingCollector
    {
        public event SettingCollectorCancelEventHandler CollectingSetting;

        public ApplicationSettings ApplicationSettings { get; set; }

        public abstract void Collect();

        protected virtual bool OnCollectingSetting(object element)
        {
            if (CollectingSetting != null)
            {
                SettingCollectorCancelEventArgs _e = new SettingCollectorCancelEventArgs(element);
                CollectingSetting(this, _e);
                return _e.Cancel == false;
            }

            return true;
        }
    }
}