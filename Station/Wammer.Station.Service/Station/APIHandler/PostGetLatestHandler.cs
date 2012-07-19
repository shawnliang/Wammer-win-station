using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/posts/getLatest/")]
	public class PostGetLatestHandler : HttpHandler
	{
		private const int DEFAULT_LIMIT = 50;
		private const int MAX_LIMIT = 200;

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			if (CloudServer.VersionNotCompatible)
				throw new WammerStationException("Version not supported", (int)GeneralApiError.NotSupportClient);

			CheckParameter("group_id");

			string groupId = Parameters["group_id"];

			if (!PermissionHelper.IsGroupPermissionOK(groupId, Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int) PostApiError.PermissionDenied
					);
			}

			int limit = (Parameters["limit"] == null ? DEFAULT_LIMIT : int.Parse(Parameters["limit"]));
			if (limit > MAX_LIMIT)
			{
				limit = MAX_LIMIT;
			}
			else if (limit < 1)
			{
				throw new WammerStationException(
					PostApiError.InvalidParameterLimit.ToString(),
					(int) PostApiError.InvalidParameterLimit
					);
			}

			MongoCursor<PostInfo> posts = PostCollection.Instance
				.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false")))
				.SetLimit(limit)
				.SetSortOrder(SortBy.Descending("timestamp"));

			List<PostInfo> postList = posts.ToList();

			var userList = new List<UserInfo>
			               	{
			               		new UserInfo
			               			{
			               				user_id = Session.user.user_id,
			               				nickname = Session.user.nickname,
			               				avatar_url = Session.user.avatar_url
			               			}
			               	};

			long totalCount = PostCollection.Instance
				.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"))).Count();

			RespondSuccess(
				new PostGetLatestResponse
					{
						total_count = totalCount,
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