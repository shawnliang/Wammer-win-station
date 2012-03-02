using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Waveface.API.V2;

namespace Waveface.FilterUI
{
    public class FilterHelper
    {
        public static List<Fetch_Filter> GetList()
        {
            MR_fetchfilters_list _list = Main.Current.RT.REST.SearchFilters_List();

            return (_list == null) ? null : _list.fetch_filters;
        }

        #region TimeRangeFilter

        public static TimeRangeFilter GetTimeRangeFilter(DateTime from_, DateTime to, int limit, string type, string offset)
        {
            TimeRangeFilter _ft = new TimeRangeFilter();
            _ft.time = new string[2];
            _ft.time[0] = DateTimeHelp.ToUniversalTime_ToISO8601(from_);
            _ft.time[1] = DateTimeHelp.ToUniversalTime_ToISO8601(to);
            _ft.limit = limit;
            _ft.type = type;
            _ft.offset = offset;

            return _ft;
        }

        public static string GetTimeRangeFilterJson(TimeRangeFilter timeRangeFilter)
        {
            return JsonConvert.SerializeObject(timeRangeFilter);
        }

        public static string GetTimeRangeFilterJson(DateTime from_, DateTime to, int limit, string type, string offset)
        {
            TimeRangeFilter _ft = GetTimeRangeFilter(from_, to, limit, type, offset);

            return JsonConvert.SerializeObject(_ft);
        }

        public static bool IsTimeRangeFilterEqual(TimeRangeFilter t1, TimeRangeFilter t2)
        {
            return
                (t1.limit == t2.limit) &&
                (t1.offset == t2.offset) &&
                (t1.time[0] == t2.time[0]) &&
                (t1.time[1] == t2.time[1]) &&
                (t1.type == t2.type);
        }

        #endregion

        #region TimeStampFilter

        public static TimeStampFilter GetTimeStampFilter(DateTime dt, int limit, string type, string offset)
        {
            TimeStampFilter _fts = new TimeStampFilter();
            _fts.timestamp = DateTimeHelp.ToUniversalTime_ToISO8601(dt);
            _fts.limit = limit;
            _fts.type = type;
            _fts.offset = offset;

            return _fts;
        }

        public static string GetTimeStampFilterJson(TimeStampFilter timeStampFilter)
        {
            return JsonConvert.SerializeObject(timeStampFilter);
        }

        public static string GetTimeStampFilterJson(DateTime dt, int limit, string type, string offset)
        {
            TimeStampFilter _fts = GetTimeStampFilter(dt, limit, type, offset);

            return JsonConvert.SerializeObject(_fts);
        }

        public static bool IsTimeStampFilterEqual(TimeStampFilter t1, TimeStampFilter t2)
        {
            return
                (t1.limit == t2.limit) &&
                (t1.offset == t2.offset) &&
                (t1.timestamp == t2.timestamp) &&
                (t1.type == t2.type);
        }

        #endregion
    }
}
