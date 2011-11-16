
#region

using System.Windows.Forms;
using Waveface.Configuration;

#endregion

namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{
    public class SettingListBox : ListBox
    {
        public SettingListBox()
        {
            if (DesignMode)
            {
                return;
            }

            ControlSettings _controlSettings = new ControlSettings(this);
            _controlSettings.Settings.Add(new PropertySetting(this, "SelectedIndex"));
        }
    }
}