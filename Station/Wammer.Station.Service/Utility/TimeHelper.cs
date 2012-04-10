using System;
using System.Globalization;

namespace Wammer.Utility
{
	public static class TimeHelper
	{
		private static DateTime JAN_1_1970 = new DateTime(1970, 1, 1);
		private const string CLOUD_TIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

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
			DateTime dt = DateTime.ParseExact(cloudTimeString, CLOUD_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
			
			return dt.ToUniversalTime();
		}

		public static string ToCloudTimeString(this DateTime datetime)
		{
			return datetime.ToUniversalTime().ToString(CLOUD_TIME_FORMAT);
		}
	}
}
