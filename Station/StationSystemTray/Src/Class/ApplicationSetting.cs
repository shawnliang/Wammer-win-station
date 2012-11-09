using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Linq;


namespace StationSystemTray
{
	class ApplicationSetting : ApplicationSettingsBase
	{
		[UserScopedSetting]
		[DefaultSettingValue("")]
		public string CurrentSession
		{
			get { return (string) this["currentSession"]; }
			set { this["currentSession"] = value; }
		}

		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool isUpgraded
		{
			get { return (bool)this["isUpgraded"]; }
			set { this["isUpgraded"] = value; }
		}
	}
}
