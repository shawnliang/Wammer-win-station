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

	public class UserLoginSettingContainer
	{
		private ApplicationSettings settings;
		private object cs;

		public UserLoginSettingContainer(ApplicationSettings settings)
		{
			this.settings = settings;
			this.cs = new object();
		}

		public void ResetUserLoginSetting(List<UserLoginSetting> userlogins, string lastlogin)
		{
			lock (cs)
			{
				settings.Users.Clear();
				settings.LastLogin = string.Empty;
				foreach (UserLoginSetting userlogin in userlogins)
				{
					settings.Users.Add(userlogin);
					if (userlogin.Email == lastlogin)
					{
						settings.LastLogin = lastlogin;
					}
				}
				settings.Save();
			}
		}

		public void UpsertUserLoginSetting(UserLoginSetting userlogin)
		{
			lock (cs)
			{
				bool duplicated = false;

				foreach (UserLoginSetting oldUserlogin in settings.Users)
				{
					if (oldUserlogin.Email == userlogin.Email)
					{
						oldUserlogin.Password = userlogin.Password;
						oldUserlogin.RememberPassword = userlogin.RememberPassword;
						duplicated = true;
					}
				}

				if (!duplicated)
				{
					settings.Users.Add(userlogin);
				}

				settings.LastLogin = userlogin.Email;
				settings.Save();
			}
		}

		public UserLoginSetting GetUserLogin(string email)
		{
			lock (cs)
			{
				foreach (UserLoginSetting userlogin in settings.Users)
				{
					if (userlogin.Email.ToLower() == email.ToLower())
					{
						return userlogin;
					}
				}
				return null;
			}
		}

		public UserLoginSetting GetLastUserLogin()
		{
			lock (cs)
			{
				foreach (UserLoginSetting userlogin in settings.Users)
				{
					if (userlogin.Email == settings.LastLogin)
					{
						return userlogin;
					}
				}
				return null;
			}
		}
	}
}
