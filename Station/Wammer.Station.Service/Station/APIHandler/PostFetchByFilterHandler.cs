using System;
using System.Linq;
using System.Collections.Generic;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Wammer.Station.APIHandler
{
	public class PostFetchByFilterHandler : HttpHandler
	{
		private const int DEFAULT_LIMIT = 20;
		private const int MAX_LIMIT = 200;

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_GROUP_ID);
			string groupId = Parameters[CloudServer.PARAM_GROUP_ID];
			List<UserInfo> userList = new List<UserInfo>();
			userList.Add(new UserInfo
			{
				user_id = Session.user.user_id,
				nickname = Session.user.nickname,
				avatar_url = Session.user.avatar_url
			});


			if (Parameters[CloudServer.PARAM_POST_ID_LIST] != null)
			{
				var postIds = from pidstring in Parameters[CloudServer.PARAM_POST_ID_LIST].Trim('[', ']').Split(',').ToList()
							  select pidstring.Trim('"', '"');

				List<PostInfo> postList = PostCollection.Instance.Find(Query.And(
					Query.EQ("group_id", groupId),
					Query.In("_id", BsonArray.Create(postIds)))).ToList<PostInfo>();

				RespondSuccess(
					new PostFetchByFilterResponse
					{
						remaining_count = 0,
						get_count = postList.Count,
						posts = postList,
						users = userList,
						group_id = groupId
					}
				);
			}
			else if (Parameters[CloudServer.PARAM_FILTER_ENTITY] != null)
			{
				FilterEntity filter = fastJSON.JSON.Instance.ToObject<FilterEntity>(Parameters[CloudServer.PARAM_FILTER_ENTITY]);
				IMongoQuery typeQuery = (filter.type == null ? Query.Null : Query.EQ("type", filter.type));
				int offset = filter.offset;
				int limit = filter.limit;
				int[] limitRange = filter.limit_range;

				if (limit == 0)
				{
					throw new WammerStationException(
						PostApiError.InvalidParameterLimit.ToString(),
						(int)PostApiError.InvalidParameterLimit
					);
				}

				NormalizeLimit(limit);

				if (limitRange != null)
				{
					NormalizeLimit(limitRange[0]);
					if (limitRange[0] >= 0)
					{
						throw new WammerStationException(
							PostApiError.InvalidParameterLimit.ToString(),
							(int)PostApiError.InvalidParameterLimit
						);
					}

					NormalizeLimit(limitRange[1]);
					if (limitRange[1] <= 0)
					{
						throw new WammerStationException(
							PostApiError.InvalidParameterLimit.ToString(),
							(int)PostApiError.InvalidParameterLimit
						);
					}
				}

				if (filter.timestamp != null)
				{
					List<PostInfo> postList;
					long totalCount = 0;

					if (limit != null)
					{
						if (limit < 0)
						{
							postList = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
								typeQuery
							)).SetSortOrder(SortBy.Descending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limit)).ToList<PostInfo>();
							postList.Reverse();
							totalCount = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
								typeQuery
							)).Count();
						}
						else
						{
							postList = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
								typeQuery
							)).SetSortOrder(SortBy.Ascending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limit)).ToList<PostInfo>();
							totalCount = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
								typeQuery
							)).Count();
						}

						RespondSuccess(new PostFetchByFilterResponse
						{
							remaining_count = totalCount - postList.Count - offset,
							get_count = postList.Count,
							posts = postList,
							users = userList,
							group_id = groupId
						});
					}
					else if (limitRange != null)
					{
						postList = PostCollection.Instance.Find(Query.And(
							Query.EQ("group_id", groupId),
							Query.EQ("hidden", "false"),
							Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
							typeQuery
						)).SetSortOrder(SortBy.Descending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limitRange[0])).ToList<PostInfo>();
						postList.Reverse();
						postList.AddRange(PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GT("timestamp", TimeHelper.ParseCloudTimeString(filter.timestamp)),
								typeQuery
							)).SetSortOrder(SortBy.Ascending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limit)).ToList<PostInfo>());

						RespondSuccess(new PostFetchByFilterResponse
						{
							remaining_count = totalCount - postList.Count - offset,
							get_count = postList.Count,
							posts = postList,
							users = userList,
							group_id = groupId
						});
					}
					else
					{
						throw new WammerStationException("Certain usage is not supported.", (int)StationApiError.NotSupported);
					}
				}
				else if (filter.time != null)
				{
					List<PostInfo> postList;
					long totalCount = 0;

					if (limit != null)
					{
						if (limit < 0)
						{
							postList = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[0])),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[1])),
								typeQuery
							)).SetSortOrder(SortBy.Descending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limit)).ToList<PostInfo>();
							totalCount = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[0])),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[1])),
								typeQuery
							)).Count();
						}
						else
						{
							postList = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[0])),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[1])),
								typeQuery
							)).SetSortOrder(SortBy.Ascending("timestamp")).SetSkip(offset).SetLimit(Math.Abs(limit)).ToList<PostInfo>();
							totalCount = PostCollection.Instance.Find(Query.And(
								Query.EQ("group_id", groupId),
								Query.EQ("hidden", "false"),
								Query.GTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[0])),
								Query.LTE("timestamp", TimeHelper.ParseCloudTimeString(filter.time[1])),
								typeQuery
							)).Count();
						}

						RespondSuccess(new PostFetchByFilterResponse
						{
							remaining_count = totalCount - postList.Count - offset,
							get_count = postList.Count,
							posts = postList,
							users = userList,
							group_id = groupId
						});
					}
					else
					{
						throw new WammerStationException("Certain usage is not supported.", (int)StationApiError.NotSupported);
					}
				}
			}
			else
			{
				throw new WammerStationException("Certain usage is not supported.", (int)StationApiError.NotSupported);
			}
		}
		#endregion

		#region Private Method
		private void NormalizeLimit(int limit)
		{
			if (limit > MAX_LIMIT)
			{
				limit = MAX_LIMIT;
			}
			else if (limit < -MAX_LIMIT)
			{
				limit = -MAX_LIMIT;
			}
		}
		#endregion


		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}