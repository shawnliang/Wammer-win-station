using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/posts/new/")]
	public class NewPostHandler : HttpHandler
	{
		#region Private Property

		private IPostUploadSupportable m_PostUploader { get; set; }

		#endregion

		#region Constructor

		public NewPostHandler(IPostUploadSupportable postUploader)
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
				CloudServer.PARAM_GROUP_ID);

			var type = Parameters[CloudServer.PARAM_TYPE];
			if (type == "link")
			{
				TunnelToCloud();
				return;
			}

			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
					"Driver not found!", (int) StationLocalApiError.InvalidDriver);

			var userGroup = driver.groups.FirstOrDefault(group => @group.group_id == groupID);

			if (userGroup == null)
				throw new WammerStationException(
					"Group not found!", (int) StationLocalApiError.NotFound);

			var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				throw new WammerStationException(
					"Logined session not found!", (int) StationLocalApiError.NotFound);

			var attachmentIDs = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY] == null
											? new List<string>()
											: Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',').Select(x => x.Trim('"')).ToList();

			var content = Parameters[CloudServer.PARAM_CONTENT];
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			if (string.IsNullOrEmpty(postID))
				postID = Guid.NewGuid().ToString();
			var timeStamp = DateTime.Now;
			var attachmentCount = attachmentIDs.Count;
			var creatorID = userGroup.creator_id;
			var codeName = loginedSession.apikey.name;
			var cover_attach = Parameters[CloudServer.PARAM_COVER_ATTACH];
			var favorite = Parameters[CloudServer.PARAM_FAVORITE];

			var attachmentInfos = AttachmentHelper.GetAttachmentInfoList(attachmentIDs, codeName);

			var post = new PostInfo
						{
							attachments = attachmentInfos,
							post_id = postID,
							timestamp = timeStamp,
							update_time = timeStamp,
							attachment_id_array = attachmentIDs,
							attachment_count = attachmentCount,
							group_id = groupID,
							creator_id = creatorID,
							code_name = codeName,
							content = content,
							hidden = "false",
							comment_count = 0,
							comments = new List<Comment>(),
							preview = new Preview(),
							event_time = timeStamp.ToCloudTimeString(),
							type = type,
							cover_attach = cover_attach,
							favorite = "1".Equals(favorite) ? 1 : 0
						};

			PostCollection.Instance.Save(post);

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.NewPost, Parameters, timeStamp, timeStamp);

			var response = new NewPostResponse { post = post, post_id = postID, group_id = groupID };
			RespondSuccess(response);
		}

		#endregion
	}
}
