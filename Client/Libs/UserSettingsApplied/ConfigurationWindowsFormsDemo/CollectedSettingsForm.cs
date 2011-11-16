#region

using System.Windows.Forms;
using Waveface.Configuration;

#endregion

namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{
    public partial class CollectedSettingsForm : Form
    {
        private readonly FormSettings m_formSettings;

        public CollectedSettingsForm()
        {
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.CollectingSetting += FormSettingsCollectingSetting;

            m_formSettings.SettingCollectors.Add(new PropertySettingCollector(this, typeof (Splitter), "SplitPosition"));
            m_formSettings.SettingCollectors.Add(new PropertySettingCollector(this, typeof (TextBox), "Text"));
            m_formSettings.SettingCollectors.Add(new PropertySettingCollector(this, typeof (DateTimePicker), "Value"));
            m_formSettings.SettingCollectors.Add(new PropertySettingCollector(this, typeof (CheckBox), "Checked"));
            m_formSettings.SettingCollectors.Add(new PropertySettingCollector(this, typeof (ComboBox), "SelectedIndex"));
        }

        private void FormSettingsCollectingSetting(object sender, SettingCollectorCancelEventArgs e)
        {
            if (e.Element == optionLevel3)
            {
                e.Cancel = true;
            }
        }
    }
}