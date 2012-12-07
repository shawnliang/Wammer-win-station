using Microsoft.Win32;

namespace Wammer.Station
{
	public class StationRegistry
	{
		private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";

		public static string StationId
		{
			get { return (string)GetValue("stationId", string.Empty); }
		}

		public static object GetValue(string valueName, object defaultValue)
		{
			object value = Registry.GetValue(KEY_PATH,
											 valueName, defaultValue);
			return value ?? defaultValue;
		}

		public static void SetValue(string valueName, object value)
		{
			Registry.SetValue(KEY_PATH, valueName, value);
		}

		public static void DeleteValue(string valueName)
		{
			RegistryKey openSubKey = Registry.LocalMachine.OpenSubKey("Software");
			if (openSubKey != null)
			{
				RegistryKey wammerRegKey =
					openSubKey.OpenSubKey("Wammer");

				if (wammerRegKey == null)
					return;

				RegistryKey stationRegKey =
					wammerRegKey.OpenSubKey("WinStation", true);

				if (stationRegKey == null)
					return;

				stationRegKey.DeleteValue(valueName, false);
			}
		}
	}
}