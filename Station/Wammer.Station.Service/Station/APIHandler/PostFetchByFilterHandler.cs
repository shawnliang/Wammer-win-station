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
			else
			{
				// currently fetchByFilter only supports fetching posts by post id list,
				// otherwise we bypass the request to cloud
				TunnelToCloud();
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