using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class GetCalendarEntries : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "getCalendarEntries"; }
		}
		#endregion


		#region Private Method
		private string GetGroupByKey(DateTime dt, int groupBy)
		{
			var groupByKey = string.Empty;
			switch (groupBy)
			{
				case 0:
					groupByKey = dt.ToString(@"yyyy-MM-dd");
					break;
				case 1:
					groupByKey = dt.ToString(@"yyyy-MM");
					break;
				case 2:
					groupByKey = dt.ToString(@"yyyy");
					break;
			}

			return groupByKey;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			var parameters = data.Parameters;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var userID = loginedUser.user.user_id;

			var sinceDate = parameters.ContainsKey("since_date") ? DateTime.Parse(parameters["since_date"].ToString()) : default(DateTime?);
			var untilDate = parameters.ContainsKey("until_date") ? DateTime.Parse(parameters["until_date"].ToString()) : default(DateTime?);
			var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
			var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;
			var queryParam = Query.And(Query.EQ("creator_id", userID), Query.NE("visibility", false));



			if (sinceDate != null)
				queryParam = Query.And(queryParam, Query.GTE("event_since_time", sinceDate.Value.ToUTCISO8601ShortString()));

			if (untilDate != null)
				queryParam = Query.And(queryParam, Query.LTE("event_since_time", untilDate.Value.ToUTCISO8601ShortString()));


			var groupBy = parameters.ContainsKey("group_by") ? int.Parse(parameters["group_by"].ToString()) : 0;

			var posts = PostDBDataCollection.Instance.Find(queryParam).SetSortOrder(SortBy.Descending("event_since_time")).ToArray();

			var canendarEntries = (from post in posts
								   let eventTime = post.EventSinceTime.Value
								   let groupByKey = GetGroupByKey(eventTime, groupBy)
								   group post by groupByKey into g
								   select new CalendarEntry()
								   {
									   SinceDate = g.Min(p => p.EventSinceTime.Value.ToUTCISO8601ShortString()),
									   UntilDate = g.Max(p => p.EventSinceTime.Value.ToUTCISO8601ShortString()),
									   PostCount = g.Count(),
									   AttachmentCount = g.Sum(p => (p.AttachmentIDs == null)? 0: p.AttachmentIDs.Count())
								   });


			var totalCount = canendarEntries.Count();
			var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);

			canendarEntries = canendarEntries.Skip(skipCount).Take(pageSize);

			return new Dictionary<string, Object>() 
			{
				{"calendar_entries", canendarEntries},
				{"page_no", pageNo},
				{"page_size", pageSize},
				{"page_count", pageCount},
			    {"total_count", totalCount}
			};
		}
		#endregion
	}
}
