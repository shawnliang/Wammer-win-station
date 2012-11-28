using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Wammer.Utility
{
	public static class TimeHelper
	{
		private const string CLOUD_TIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
		private static DateTime JAN_1_1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ConvertToDateTime(long unixTimeStamp)
		{
			return JAN_1_1970.AddSeconds(unixTimeStamp);
		}

		public static long ConvertToUnixTimeStamp(DateTime datetime)
		{
			return (long) (datetime - JAN_1_1970).TotalSeconds;
		}

		/// <summary>
		/// Parses yyyy MM dd hh mm ss (Z)
		/// </summary>
		/// <param name="timestr"></param>
		/// <returns></returns>
		public static DateTime ParseGeneralDateTime(string timestr)
		{
			timestr = timestr.Trim();

			var m = Regex.Match(timestr, @"(\d+)\D+(\d+)\D+(\d+)\D+(\d+)\D+(\d+)\D+(\d+)");
			if (m.Groups.Count != 7)
				throw new FormatException("timestr is not recognized as a parsable time");

			return new DateTime(
				int.Parse(m.Groups[1].Value),
				int.Parse(m.Groups[2].Value),
				int.Parse(m.Groups[3].Value),
				int.Parse(m.Groups[4].Value),
				int.Parse(m.Groups[5].Value),
				int.Parse(m.Groups[6].Value),
				(timestr.EndsWith("Z") || timestr.EndsWith("Z")) ? DateTimeKind.Utc : DateTimeKind.Local);
		}

		public static DateTime ParseCloudTimeString(string cloudTimeString)
		{
			DateTime dt = DateTime.ParseExact(cloudTimeString, CLOUD_TIME_FORMAT, CultureInfo.InvariantCulture,
			                                  DateTimeStyles.AssumeUniversal);

			return dt.ToUniversalTime();
		}

		public static string ToCloudTimeString(this DateTime datetime)
		{
			return datetime.ToUniversalTime().ToString(CLOUD_TIME_FORMAT);
		}
	}
}