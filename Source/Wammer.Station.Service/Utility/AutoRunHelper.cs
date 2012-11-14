using Microsoft.Win32;

namespace Wammer.Utility
{
	public static class AutoRun
	{
		private const string AUTO_RUN_SUB_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
		private const string AUTO_RUN_REG_KEY = @"HKEY_CURRENT_USER\" + AUTO_RUN_SUB_KEY;

		public static void Add(string name, string path)
		{
			if (path.StartsWith("\"") && path.EndsWith("\""))
				Registry.SetValue(AUTO_RUN_REG_KEY, name, path);
			else
				Registry.SetValue(AUTO_RUN_REG_KEY, name, "\"" + path + "\"");
		}

		public static void Remove(string name)
		{
			RegistryKey _curUserRegKey = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY, true);

			if (_curUserRegKey == null)
				return;

			_curUserRegKey.DeleteValue(name, false);
		}

		public static bool Exists(string name)
		{
			RegistryKey _key = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY);

			if (_key == null)
				return false;

			return (_key.GetValue(name) != null);
		}
	}
}