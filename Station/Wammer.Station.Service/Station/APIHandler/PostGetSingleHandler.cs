using System;
using System.Linq;
using System.Collections.Generic;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class PostGetSingleHandler : HttpHandler
	{
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			CheckParameter("group_id", "post_id");

			string groupId = Parameters["group_id"];
			string postId = Parameters["post_id"];

			if (!PermissionHelper.IsGroupPermissionOK(groupId, this.Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int)PostApiError.PermissionDenied
				);
			}

			PostInfo singlePost = PostCollection.Instance.FindOne(
				Query.And(Query.EQ("group_id", groupId), Query.EQ("_id", postId)));

			if (singlePost == null)
			{
				throw new WammerStationException(
					PostApiError.PostNotExist.ToString(),
					(int)PostApiError.PostNotExist
				);
			}

			List<UserInfo> userList = new List<UserInfo>();
			userList.Add(new UserInfo
			{
				user_id = Session.user.user_id,
				nickname = Session.user.nickname,
				avatar_url = Session.user.avatar_url
			});

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
			return this.MemberwiseClone();
		}
		#endregion
	}
}