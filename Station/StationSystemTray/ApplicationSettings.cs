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
		public string CurLoginedSession
		{
			get { return (string)this["CurLoginedSession"]; }
			set { this["CurLoginedSession"] = value; }
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
		public string LastLoginedEmail
		{
			get { return (string)this["LastLoginedEmail"]; }
			set { this["LastLoginedEmail"] = value; }
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

		private void nolock_ResetUserLoginSetting(List<UserLoginSetting> userlogins)
		{
			settings.Users.Clear();
			settings.Users.AddRange(userlogins);

			settings.Save();
		}

		public string GetCurLoginedSession()
		{
			lock (cs)
			{ 
				return settings.CurLoginedSession; 
			}
		}

		public void CleartCurLoginedSession()
		{
			SaveCurLoginedSession(string.Empty);
		}

		public void SaveCurLoginedSession(string sessionToken)
		{
			lock (cs)
			{
				settings.CurLoginedSession = sessionToken;
				settings.Save();
			}
		}

		public void SaveCurLoginedUser(UserLoginSetting userlogin)
		{
			lock (cs)
			{
				nolock_updateUserLogin(userlogin);

				settings.LastLoginedEmail = userlogin.Email;
				settings.Save();
			}
		}

		private void nolock_updateUserLogin(UserLoginSetting userlogin)
		{
			bool isExisted = false;

			foreach (UserLoginSetting oldUserlogin in settings.Users)
			{
				if (oldUserlogin.Email == userlogin.Email)
				{
					oldUserlogin.Password = userlogin.Password;
					oldUserlogin.RememberPassword = userlogin.RememberPassword;
					isExisted = true;
					break;
				}
			}

			if (!isExisted)
			{
				settings.Users.Add(userlogin);
			}
		}

		public void RemoveUserLogin(string email)
		{
			lock (cs)
			{
				var newusers = settings.Users.Where(userlogin => userlogin.Email != email.ToLower()).ToList();

				nolock_ResetUserLoginSetting(newusers);
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
				return settings.Users.FirstOrDefault(user => !string.IsNullOrEmpty(settings.LastLoginedEmail) && user.Email == settings.LastLoginedEmail);
			}
		}
	}
}