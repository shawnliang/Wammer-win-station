using System;

namespace Wammer.Utility
{
	static class TimeHelper
	{
		private static DateTime JAN_1_1970 = new DateTime(1970, 1, 1);

		public static long GetSecondsSince1970()
		{
			TimeSpan span = DateTime.Now - JAN_1_1970;
			return (long)span.TotalSeconds;
		}

		public static DateTime ConvertToDateTime(long unixTimeStamp)
		{
			return JAN_1_1970.AddSeconds(unixTimeStamp);
		}

		public static long ConvertToUnixTimeStamp(DateTime datetime)
		{
			return (long)(datetime - JAN_1_1970).TotalSeconds;
		}
	}
}
