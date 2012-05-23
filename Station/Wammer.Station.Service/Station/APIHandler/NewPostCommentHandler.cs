using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class NewPostCommentHandler : HttpHandler
	{
		#region Private Property

		private IPostUploadSupportable m_PostUploader { get; set; }

		#endregion

		#region Constructor

		public NewPostCommentHandler(IPostUploadSupportable postUploader)
		{
			m_PostUploader = postUploader;
		}

		#endregion

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(
				CloudServer.PARAM_API_KEY,
				CloudServer.PARAM_SESSION_TOKEN,
				CloudServer.PARAM_GROUP_ID,
				CloudServer.PARAM_POST_ID,
				CloudServer.PARAM_CONTENT);

			string sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			LoginedSession loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				throw new WammerStationException(
					"Logined session not found!", (int) StationLocalApiError.NotFound);

			string postID = Parameters[CloudServer.PARAM_POST_ID];

			PostInfo post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));
			if (post == null)
				throw new WammerStationException(
					"Post not found!", (int) StationLocalApiError.NotFound);

			string groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			Driver driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
					"Driver not found!", (int) StationLocalApiError.InvalidDriver);

			var userGroup = driver.groups.FirstOrDefault(group => @group.group_id == groupID);

			if (userGroup == null)
				throw new WammerStationException(
					"Group not found!", (int) StationLocalApiError.NotFound);

			if (post.comments == null)
				post.comments = new List<Comment>();

			DateTime currentTimeStamp = DateTime.Now;
			string newPostContent = Parameters[CloudServer.PARAM_CONTENT];
			string creatorID = userGroup.creator_id;
			string codeName = loginedSession.apikey.name;
			var newPostComment = new Comment
			                     	{
			                     		content = newPostContent,
			                     		timestamp = TimeHelper.ToCloudTimeString(currentTimeStamp),
			                     		code_name = codeName,
			                     		creator_id = creatorID
			                     	};

			if (loginedSession.device != null)
				newPostComment.device_id = loginedSession.device.device_id;

			post.comment_count += 1;
			post.comments.Add(newPostComment);

			PostCollection.Instance.Update(Query.EQ("_id", postID), Update
			                                                        	.Set("comment_count", post.comment_count)
			                                                        	.Set("comments",
			                                                        	     new BsonArray(
			                                                        	     	post.comments.ConvertAll(
			                                                        	     		item => item.ToBsonDocument()))));

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.Comment, Parameters);

			var response = new NewPostCommentResponse
			               	{
			               		post = post
			               	};

			RespondSuccess(response);
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