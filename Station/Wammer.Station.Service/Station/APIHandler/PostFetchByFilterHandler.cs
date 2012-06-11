using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/posts/fetchByFilter/")]
	public class PostFetchByFilterHandler : HttpHandler
	{

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_GROUP_ID);
			var groupId = Parameters[CloudServer.PARAM_GROUP_ID];
			var userList = new List<UserInfo>
			                          	{
			                          		new UserInfo
			                          			{
			                          				user_id = Session.user.user_id,
			                          				nickname = Session.user.nickname,
			                          				avatar_url = Session.user.avatar_url
			                          			}
			                          	};


			if (Parameters[CloudServer.PARAM_POST_ID_LIST] != null)
			{
				var postIds = from pidstring in Parameters[CloudServer.PARAM_POST_ID_LIST].Trim('[', ']').Split(',').ToList()
							  select pidstring.Trim('"', '"');

				var postList = PostCollection.Instance.Find(Query.And(
					Query.EQ("group_id", groupId),
					Query.In("_id", BsonArray.Create(postIds)))).ToList();

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
	}
}