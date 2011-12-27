using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Waveface
{
	static class StationRegHelper
	{
		private static string STATION_REG_KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";
		private static string STATION_REG_KEY_PATH_WOW6432NODE = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Wammer\WinStation";
		private static bool is32Bit;

		static StationRegHelper()
		{
			if (IntPtr.Size == 4)
				is32Bit = true;
		}

		public static object GetValue(string valueName, object defaultValue)
		{
			if (is32Bit)
				return Registry.GetValue(STATION_REG_KEY_PATH, valueName, defaultValue);
			else
				// I am a 64 bit process but station is 32 bit process.
				// So I read WoW6432Node to get station registry.
				return Registry.GetValue(STATION_REG_KEY_PATH_WOW6432NODE, valueName, defaultValue);
		}
	
	}
}
