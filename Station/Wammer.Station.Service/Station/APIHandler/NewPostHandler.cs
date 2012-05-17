using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
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

			string type = Parameters[CloudServer.PARAM_TYPE];
			if (type == "link")
			{
				TunnelToCloud();
				return;
			}

			string groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			Driver driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
					"Driver not found!", (int) StationLocalApiError.InvalidDriver);

			var userGroup = driver.groups.FirstOrDefault(group => @group.group_id == groupID);

			if (userGroup == null)
				throw new WammerStationException(
					"Group not found!", (int) StationLocalApiError.NotFound);

			string sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			LoginedSession loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				throw new WammerStationException(
					"Logined session not found!", (int) StationLocalApiError.NotFound);

			List<string> attachmentIDs = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY] == null
			                             	? new List<string>()
			                             	: Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',').ToList();
			string content = Parameters[CloudServer.PARAM_CONTENT];
			string postID = Guid.NewGuid().ToString();
			DateTime timeStamp = DateTime.Now;
			int attachmentCount = attachmentIDs.Count;
			string creatorID = userGroup.creator_id;
			string codeName = loginedSession.apikey.name;

			List<AttachmentInfo> attachmentInfos = (from attachmentID in attachmentIDs
			                                        let attachment =
			                                        	AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID.Trim('"')))
			                                        where attachment != null
			                                        select AttachmentHelper.GetAttachmentnfo(attachment, codeName)).ToList();


			if (attachmentInfos.Count() != attachmentCount)
				throw new WammerStationException(
					"Attachement not found!", (int) StationLocalApiError.NotFound);

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
			           		event_time = timeStamp.ToString("u"),
			           		type = type
			           	};


			PostCollection.Instance.Save(post);

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.NewPost, Parameters);

			var response = new NewPostResponse();
			response.posts.Add(post);
			RespondSuccess(response);
		}

		#endregion
	}
}