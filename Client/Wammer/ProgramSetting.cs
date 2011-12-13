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
    }

}
