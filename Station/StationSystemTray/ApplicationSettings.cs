using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class ApplicationSettings : ApplicationSettingsBase
	{
		[UserScopedSetting()]
		[DefaultSettingValue("")]
		public List<UserLoginSetting> Users
		{
			get
			{
				return (List<UserLoginSetting>)this["Users"];
			}
			set
			{
				this["Users"] = value;
			}
		}

		[UserScopedSetting()]
		[DefaultSettingValue("")]
		public string LastLogin
		{
			get
			{
				return (string)this["LastLogin"];
			}
			set
			{
				this["LastLogin"] = value;
			}
		}
	}

	public class UserLoginSetting
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public bool RememberPassword { get; set; }
	}
}
