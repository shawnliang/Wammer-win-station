using System.Collections.Generic;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/posts/getSingle/")]
	public class PostGetSingleHandler : HttpHandler
	{
		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("group_id", "post_id");

			string groupId = Parameters["group_id"];
			string postId = Parameters["post_id"];

			if (!PermissionHelper.IsGroupPermissionOK(groupId, Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int) PostApiError.PermissionDenied
					);
			}

			PostInfo singlePost = PostCollection.Instance.FindOne(
				Query.And(Query.EQ("group_id", groupId), Query.EQ("_id", postId)));

			if (singlePost == null)
			{
				throw new WammerStationException(
					PostApiError.PostNotExist.ToString(),
					(int) PostApiError.PostNotExist
					);
			}

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
				new PostGetSingleResponse
					{
						post = singlePost,
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