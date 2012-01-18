
using System.Configuration;


namespace Waveface
{
    public class ProgramSetting: ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string StationToken
        {
            get { return (string)this["StationToken"]; }
            set { this["StationToken"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool RememberPassword
        {
            get { return (bool)this["RememberPassword"]; }
            set { this["RememberPassword"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string Email
        {
            get { return (string)this["Email"]; }
            set { this["Email"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }
    }

    public enum QuitOption
    {
        Logout,
        QuitProgram
    }
}
