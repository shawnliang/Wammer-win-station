using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using System.Net;
using Wammer.Model;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace Wammer.Station
{
	public class NewPostHandler : HttpHandler
	{
		#region Private Method
		private AttachmentInfo GetAttachmentnfo(Attachment attachment, string codeName)
		{ 
			var attachmentInfo = new AttachmentInfo()
			{
				group_id = attachment.group_id,
				file_name = attachment.file_name,
				meta_status = "OK",
				object_id = attachment.object_id,
				creator_id = attachment.creator_id,
				modify_time = DateTime.Now.Ticks,
				code_name = codeName,
				type = attachment.type.ToString(),
				url = attachment.url,
				title = attachment.title,
				description = attachment.description,
				hidden = attachment.group_id
			};

			if (attachment.image_meta != null)
			{
				attachmentInfo.image_meta = new AttachmentInfo.ImageMeta();

				SetAttachmentInfoImageMeta(attachment.image_meta.small, attachmentInfo.image_meta.small);
				SetAttachmentInfoImageMeta(attachment.image_meta.medium, attachmentInfo.image_meta.medium);
				SetAttachmentInfoImageMeta(attachment.image_meta.large, attachmentInfo.image_meta.large);
			}

			return attachmentInfo;
		}

		private static void SetAttachmentInfoImageMeta(ThumbnailInfo attachmentThumbnailInfo, AttachmentInfo.ImageMetaDetail attachmentInfoThumbnailInfo)
		{
			if (attachmentThumbnailInfo != null)
			{
				attachmentInfoThumbnailInfo.url = attachmentThumbnailInfo.url;
				attachmentInfoThumbnailInfo.height = attachmentThumbnailInfo.height;
				attachmentInfoThumbnailInfo.width = attachmentThumbnailInfo.width;
				attachmentInfoThumbnailInfo.modify_time = attachmentThumbnailInfo.modify_time.Ticks;
				attachmentInfoThumbnailInfo.file_size = attachmentThumbnailInfo.file_size;
				attachmentInfoThumbnailInfo.mime_type = attachmentThumbnailInfo.mime_type;
				attachmentInfoThumbnailInfo.md5 = attachmentThumbnailInfo.md5;
			}
		}
		#endregion

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			CheckParameter(
				CloudServer.PARAM_API_KEY, 
				CloudServer.PARAM_SESSION_TOKEN, 
				CloudServer.PARAM_GROUP_ID);

			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
							"Driver not found!", (int)StationApiError.InvalidDriver);

			var userGroup = driver.groups.Where((group) => group.group_id == groupID).FirstOrDefault();

			if (userGroup == null)
				throw new WammerStationException(
							"Group not found!", (int)StationApiError.NotFound);

			var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];		
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				throw new WammerStationException(
							"Logined session not found!", (int)StationApiError.NotFound);

			var attachmentIDs = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY] == null ? new List<string>() : Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',').ToList();
			var content = Parameters[CloudServer.PARAM_CONTENT];
			var postID = Guid.NewGuid().ToString();
			var timeStamp = DateTime.Now;
			var attachmentCount = attachmentIDs.Count;
			var creatorID = userGroup.creator_id;
			var codeName = loginedSession.apikey.name;

			var attachments = from attachmentID in attachmentIDs
							  let attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID))
							  where attachment != null
							  select GetAttachmentnfo(attachment, codeName);
						

			if(attachments.Count() != attachmentCount)
				throw new WammerStationException(
						"Attachement not found!", (int)StationApiError.NotFound);
					
			var post = new PostInfo()
			{
				attachments = attachments.ToList(),
				post_id = postID,
				timestamp = timeStamp,
				update_time = timeStamp,
				attachment_id_array = attachmentIDs,
				attachment_count = attachmentCount,
				group_id = groupID,
				creator_id = creatorID,
				code_name = codeName,
				content = content				
			};

			PostCollection.Instance.Save(post);

			var response = new NewPostResponse();
			response.posts.Add(post);

			IPostUploadSupportable postUploader = PostUploadTaskController.Instance;
			postUploader.AddPostUploadAction(PostUploadActionType.NewPost, Parameters);

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