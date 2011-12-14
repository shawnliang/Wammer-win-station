using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace Waveface
{
    class ProgramSetting: ApplicationSettingsBase
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
        public bool IsLoggedIn
        {
            get { return (bool)this["IsLoggedIn"]; }
            set { this["IsLoggedIn"] = value; }
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
