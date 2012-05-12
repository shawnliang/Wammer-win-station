using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StationSystemTray
{
	public class ApplicationSettings : ApplicationSettingsBase
	{
		[UserScopedSetting]
		[DefaultSettingValue("")]
		public List<UserLoginSetting> Users
		{
			get { return (List<UserLoginSetting>)this["Users"]; }
			set { this["Users"] = value; }
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
		public string LastLogin
		{
			get { return (string)this["LastLogin"]; }
			set { this["LastLogin"] = value; }
		}

		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool isUpgraded
		{
			get { return (bool)this["isUpgraded"]; }
			set { this["isUpgraded"] = value; }
		}
	}

	public class UserLoginSetting
	{
		public string Email { get; set; }

		public string Password { get; set; }

		public string SessionToken { get; set; }

		public bool RememberPassword { get; set; }
	}

	public class UserLoginSettingContainer
	{
		private readonly object cs;
		private readonly ApplicationSettings settings;

		public UserLoginSettingContainer(ApplicationSettings settings)
		{
			this.settings = settings;
			cs = new object();
		}

		public void ResetUserLoginSetting(List<UserLoginSetting> userlogins, string lastlogin)
		{
			lock (cs)
			{
				settings.LastLogin = lastlogin;

				settings.Users.Clear();
				settings.Users.AddRange(userlogins);

				settings.Save();
			}
		}

		public string GetLastLogin()
		{
			return settings.LastLogin;
		}

		public void ClearLastLoginSession()
		{
			UserLoginSetting userlogin = GetLastUserLogin();
			userlogin.SessionToken = string.Empty;
			settings.LastLogin = string.Empty;

			UpsertUserLoginSetting(userlogin);
		}

		public void UpdateLastLogin(string sessionToken)
		{
			settings.LastLogin = sessionToken;
			settings.Save();
		}

		public void UpsertUserLoginSetting(UserLoginSetting userlogin)
		{
			lock (cs)
			{
				bool isExisted = false;

				foreach (UserLoginSetting oldUserlogin in settings.Users)
				{
					if (oldUserlogin.Email == userlogin.Email)
					{
						oldUserlogin.Password = userlogin.Password;
						oldUserlogin.SessionToken = userlogin.SessionToken;
						oldUserlogin.RememberPassword = userlogin.RememberPassword;
						isExisted = true;
						break;
					}
				}

				if (!isExisted)
				{
					settings.Users.Add(userlogin);
				}

				settings.LastLogin = userlogin.SessionToken;
				settings.Save();
			}
		}

		public void RemoveUserLogin(string email)
		{
			lock (cs)
			{
				var newusers = settings.Users.Where(userlogin => userlogin.Email != email.ToLower()).ToList();

				ResetUserLoginSetting(newusers, settings.LastLogin);
			}
		}

		public UserLoginSetting GetUserLogin(string email)
		{
			lock (cs)
			{
				return settings.Users.FirstOrDefault(userlogin => userlogin.Email.ToLower() == email.ToLower());
			}
		}

		public UserLoginSetting GetLastUserLogin()
		{
			lock (cs)
			{
				return settings.Users.FirstOrDefault(userlogin => !string.IsNullOrEmpty(settings.LastLogin) && userlogin.SessionToken == settings.LastLogin);
			}
		}
	}
}