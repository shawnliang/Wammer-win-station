#region

using System;
using System.Globalization;

#endregion

namespace Waveface
{
    internal class DateTimeHelp
    {
        public static string ToISO8601(DateTime dt)
        {
            return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", DateTimeFormatInfo.InvariantInfo);
        }

        public static string ToUniversalTime_ToISO8601(DateTime dt)
        {
            return ToISO8601(dt.ToUniversalTime());
        }

        public static string ISO8601ToDotNet(string dt)
        {
            DateTime _dt;

            DateTime.TryParseExact(
                dt,
                @"yyyy-MM-dd\THH:mm:ss\Z",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out _dt);

            return _dt.ToString();
        }

        public static DateTime ISO8601ToDateTime(string dt)
        {
            DateTime _dt;

            DateTime.TryParseExact(
                dt,
                @"yyyy-MM-dd\THH:mm:ss\Z",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out _dt);

            return _dt;
        }

        public static string PrettyDate(String TimeSubmitted)
        {
            // accepts standard DateTime: 5/12/2011 2:36:00 PM 
            // returns: "# month(s)/week(s)/day(s)/hour(s)/minute(s)/second(s)) ago"
            string _ret = TimeSubmitted;

            DateTime _submittedDate = DateTime.Parse(TimeSubmitted);
            DateTime _now = DateTime.Now;
            TimeSpan _diff = _now - _submittedDate;

            if (_diff.Seconds <= 0)
            {
                _ret = TimeSubmitted;
            }
            else if (_diff.Days > 30)
            {
                _ret = _diff.Days / 30 + " month" + (_diff.Days / 30 >= 2 ? "s " : " ") + "ago";
            }
            else if (_diff.Days > 7)
            {
                _ret = _diff.Days / 7 + " week" + (_diff.Days / 7 >= 2 ? "s " : " ") + "ago";
            }
            else if (_diff.Days >= 1)
            {
                _ret = _diff.Days + " day" + (_diff.Days >= 2 ? "s " : " ") + "ago";
            }
            else if (_diff.Hours >= 1)
            {
                _ret = _diff.Hours + " hour" + (_diff.Hours >= 2 ? "s " : " ") + "ago";
            }
            else if (_diff.Minutes >= 1)
            {
                _ret = _diff.Minutes + " minute" + (_diff.Minutes >= 2 ? "s " : " ") + "ago";
            }
            else
            {
                _ret = _diff.Seconds + " second" + (_diff.Seconds >= 2 ? "s " : " ") + "ago";
            }

            return _ret;
        }
    }
}