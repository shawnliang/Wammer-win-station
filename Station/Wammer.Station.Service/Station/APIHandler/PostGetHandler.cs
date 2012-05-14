using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class PostGetHandler : HttpHandler
	{
		private const int MAX_LIMIT = 200;

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("group_id", "datum", "limit");

			string groupId = Parameters["group_id"];
			DateTime datum = TimeHelper.ParseCloudTimeString(Parameters["datum"]);

			if (!PermissionHelper.IsGroupPermissionOK(groupId, Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int) PostApiError.PermissionDenied
					);
			}

			int limit = int.Parse(Parameters["limit"]);
			if (limit > MAX_LIMIT)
			{
				limit = MAX_LIMIT;
			}
			else if (limit < -MAX_LIMIT)
			{
				limit = -MAX_LIMIT;
			}
			else if (limit == 0)
			{
				throw new WammerStationException(
					PostApiError.InvalidParameterLimit.ToString(),
					(int) PostApiError.InvalidParameterLimit
					);
			}

			MongoCursor<PostInfo> posts = null;
			long totalCount = 0;
			if (limit < 0)
			{
				posts = PostCollection.Instance
					.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"), Query.LTE("timestamp", datum)))
					.SetLimit(Math.Abs(limit))
					.SetSortOrder(SortBy.Descending("timestamp"));

				totalCount = PostCollection.Instance
					.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"), Query.LTE("timestamp", datum)))
					.Count();
			}
			else
			{
				posts = PostCollection.Instance
					.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"), Query.GTE("timestamp", datum)))
					.SetLimit(Math.Abs(limit))
					.SetSortOrder(SortBy.Ascending("timestamp"));

				totalCount = PostCollection.Instance
					.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"), Query.GTE("timestamp", datum)))
					.Count();
			}

			Debug.Assert(posts != null, "posts != null");
			var postList = posts.ToList();

			var userList = new List<UserInfo>
			               	{
			               		new UserInfo
			               			{
			               				user_id = Session.user.user_id,
			               				nickname = Session.user.nickname,
			               				avatar_url = Session.user.avatar_url
			               			}
			               	};

			RespondSuccess(
				new PostGetResponse
					{
						remaining_count = totalCount - postList.Count,
						get_count = postList.Count,
						group_id = groupId,
						posts = postList,
						users = userList
					}
				);
		}

		#endregion

		#region Public Method

		public override object Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}
}