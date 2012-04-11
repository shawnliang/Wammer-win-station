#region

using System;
using System.Globalization;
using Waveface.Localization;

#endregion

namespace Waveface
{
    internal class DateTimeHelp
    {
        public static DateTime ConvertUnixTimestampToDateTime(long timestamp)
        {
            DateTime begin1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return begin1970.AddSeconds(timestamp);
        }

        public static string ToISO8601(DateTime dt)
        {
            return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", DateTimeFormatInfo.InvariantInfo);
        }

        public static string ToUniversalTime_ToISO8601(DateTime dt)
        {
            return ToISO8601(dt.ToUniversalTime());
        }

        public static string ISO8601ToDotNet(string dt, bool @short)
        {
            DateTime _dt;

            DateTime.TryParseExact(
                dt,
                @"yyyy-MM-dd\THH:mm:ss\Z",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out _dt);

            if (@short)
            {
                return _dt.ToString("MM/dd HH:mm:ss");
            }
            else
            {
                return _dt.ToString();
            }
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

        public static int CompareISO8601(string dtNew, string dtOld)
        {
            DateTime _dt1 = ISO8601ToDateTime(dtNew);
            DateTime _dt2 = ISO8601ToDateTime(dtOld);

            return _dt1.CompareTo(_dt2);
        }

        public static bool CompareISO8601_New(string dtNew, string dtOld) //New, Old
        {
            DateTime _dt1 = ISO8601ToDateTime(dtNew);
            DateTime _dt2 = ISO8601ToDateTime(dtOld);

            return _dt1.CompareTo(_dt2) > 0;
        }

        public static string PrettyDate(String timeSubmitted, bool shortFormat)
        {
            // accepts standard DateTime: 5/12/2011 2:36:00 PM 
            // returns: "# month(s)/week(s)/day(s)/hour(s)/minute(s)/second(s)) ago"
            string _ret;

            DateTime _submittedDate = DateTime.Parse(timeSubmitted);
            DateTime _now = DateTime.Now;
            TimeSpan _diff = _now - _submittedDate;

            if(shortFormat)
            {
                timeSubmitted = _submittedDate.ToString("yyyy-MM-dd");
            }

            if (CultureManager.ApplicationUICulture.Name == "zh-TW")
            {
                if (_diff.Seconds <= 0)
                {
                    _ret = timeSubmitted;
                }
                /*
                else if (_diff.Days > 30)
                {
                    _ret = _diff.Days/30 + " 個月前";
                }
                else if (_diff.Days > 7)
                {
                    _ret = _diff.Days/7 + " 星期前";
                }
                */
                else if (_diff.Days >= 1)
                {
                    if (_diff.Days < 7)
                        _ret = _diff.Days + "天前";
                    else
                        _ret = timeSubmitted;
                }
                else if (_diff.Hours >= 1)
                {
                    _ret = _diff.Hours + "小時前";
                }
                else if (_diff.Minutes >= 1)
                {
                    _ret = _diff.Minutes + "分鐘前";
                }
                else
                {
                    _ret = _diff.Seconds + "秒前";
                }
            }
            else
            {
                if (_diff.Seconds <= 0)
                {
                    _ret = timeSubmitted;
                }
                /*
                else if (_diff.Days > 30)
                {
                    _ret = _diff.Days/30 + " month" + (_diff.Days/30 >= 2 ? "s " : " ") + "ago";
                }
                else if (_diff.Days > 7)
                {
                    _ret = _diff.Days/7 + " week" + (_diff.Days/7 >= 2 ? "s " : " ") + "ago";
                }
                */
                else if (_diff.Days >= 1)
                {
                    if (_diff.Days < 7)
                        _ret = _diff.Days + "day" + (_diff.Days >= 2 ? "s " : " ") + "ago";
                    else
                        _ret = timeSubmitted;
                }
                else if (_diff.Hours >= 1)
                {
                    _ret = _diff.Hours + "hour" + (_diff.Hours >= 2 ? "s " : " ") + "ago";
                }
                else if (_diff.Minutes >= 1)
                {
                    _ret = _diff.Minutes + "minute" + (_diff.Minutes >= 2 ? "s " : " ") + "ago";
                }
                else
                {
                    _ret = _diff.Seconds + "second" + (_diff.Seconds >= 2 ? "s " : " ") + "ago";
                }
            }

            return _ret;
        }
    }
}