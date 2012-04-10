using System;
using System.Globalization;

namespace Wammer.Utility
{
	public static class TimeHelper
	{
		private static DateTime JAN_1_1970 = new DateTime(1970, 1, 1);

		public static DateTime ConvertToDateTime(long unixTimeStamp)
		{
			return JAN_1_1970.AddSeconds(unixTimeStamp);
		}

		public static long ConvertToUnixTimeStamp(DateTime datetime)
		{
			return (long)(datetime - JAN_1_1970).TotalSeconds;
		}

		public static DateTime ParseCloudTimeString(string cloudTimeString)
		{
			return DateTime.ParseExact(cloudTimeString, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
		}
	}
}
