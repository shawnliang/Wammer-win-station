using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Station
{
	public class StationRegistry
	{
		private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";

		public static object GetValue(string valueName, object defaultValue)
		{
			object value = Microsoft.Win32.Registry.GetValue(KEY_PATH,
													valueName, defaultValue);
			return value == null ? defaultValue : value;
		}

		public static void SetValue(string valueName, object value)
		{
			Microsoft.Win32.Registry.SetValue(KEY_PATH, valueName, value);
		}
	}
}
