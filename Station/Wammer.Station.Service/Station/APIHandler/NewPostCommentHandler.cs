using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using System.Collections.Generic;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

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

			var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				throw new WammerStationException(
							"Logined session not found!", (int)StationLocalApiError.NotFound);

			var postID = Parameters[CloudServer.PARAM_POST_ID];

			var post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));
			if (post == null)
				throw new WammerStationException(
							"Post not found!", (int)StationLocalApiError.NotFound);

			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
							"Driver not found!", (int)StationLocalApiError.InvalidDriver);

			var userGroup = driver.groups.Where((group) => group.group_id == groupID).FirstOrDefault();

			if (userGroup == null)
				throw new WammerStationException(
							"Group not found!", (int)StationLocalApiError.NotFound);

			if (post.comments == null)
				post.comments = new List<Comment>();

			var currentTimeStamp = DateTime.Now;
			var newPostContent = Parameters[CloudServer.PARAM_CONTENT];
			var creatorID = userGroup.creator_id;
			var codeName = loginedSession.apikey.name;
			var newPostComment = new Comment()
			{
				content = newPostContent,
				timestamp = currentTimeStamp.ToString("u"),
				code_name = codeName,
				creator_id = creatorID
			};

			if (loginedSession.device != null)
				newPostComment.device_id = loginedSession.device.device_id;

			post.comment_count += 1;
			post.comments.Add(newPostComment);

			PostCollection.Instance.Update(Query.EQ("_id", postID), Update
				.Set("comment_count", post.comment_count)
				.Set("comments", new BsonArray(post.comments.ConvertAll((item) => item.ToBsonDocument()))));

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.Comment, Parameters);

			var response = new NewPostCommentResponse() 
			{
				post = post
			};

			RespondSuccess(response);
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