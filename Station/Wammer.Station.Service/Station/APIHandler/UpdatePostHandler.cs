using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Collections.Generic;
using Wammer.Utility;

namespace Wammer.Station
{
	public class UpdatePostHandler : HttpHandler
	{
		#region Private Property
		private IPostUploadSupportable m_PostUploader { get; set; }
		#endregion

		#region Constructor
		public UpdatePostHandler(IPostUploadSupportable postUploader)
		{
			m_PostUploader = postUploader;
		}
		#endregion

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY,
				CloudServer.PARAM_SESSION_TOKEN,
				CloudServer.PARAM_GROUP_ID,
				CloudServer.PARAM_POST_ID,
				CloudServer.PARAM_LAST_UPDATE_TIME);

			var postID = Parameters[CloudServer.PARAM_POST_ID];

			var post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));
			if (post == null)
				throw new WammerStationException(
							"Post not found!", (int)StationApiError.NotFound);

			var content = Parameters[CloudServer.PARAM_CONTENT];			
			if (content != null)
			{				
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("content", content));

				post.content = content;
			}

			var preview = Parameters[CloudServer.PARAM_PREVIEW];
			if (preview != null)
			{
				var previewObj = fastJSON.JSON.Instance.ToObject<Preview>(preview);

				if (previewObj == null)
					throw new WammerStationException(
						"preview format incorrect!", (int)StationApiError.Error);

				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("preview", previewObj.ToBsonDocument()));

				post.preview = previewObj;
			}

			var attachmentIDArray = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY];
			
			if(attachmentIDArray != null)
				attachmentIDArray = attachmentIDArray.Trim('[', ']');

			var attachmentIDs = attachmentIDArray == null ? new List<string>() : attachmentIDArray.Split(',')
				.Where((item) => !string.IsNullOrEmpty(item))
				.Select((item)=> item.Trim('"')).ToList();

			if (attachmentIDs.Count > 0)
			{
				var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
				var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

				if (loginedSession == null)
					throw new WammerStationException(
								"Logined session not found!", (int)StationApiError.NotFound);

				var codeName = loginedSession.apikey.name;

				var attachmentInfos = (from attachmentID in attachmentIDs
									   let attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID))
									   where attachment != null
									   select AttachmentHelper.GetAttachmentnfo(attachment, codeName)).ToList();
								
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update
					.Set("attachment_count", attachmentIDs.Count)
					.Set("attachment_id_array", new BsonArray(attachmentIDs))
					.Set("attachments", new BsonArray(attachmentInfos.ConvertAll((item)=> item.ToBsonDocument()))));


				post.attachment_id_array = attachmentIDs;
				post.attachment_count = attachmentIDs.Count;
				post.attachments = attachmentInfos;	
			}

			var coverAttach = Parameters[CloudServer.PARAM_COVER_ATTACH];
			if (coverAttach != null)
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("cover_attach", coverAttach));

				post.cover_attach = coverAttach;
			}

			var favorite = Parameters[CloudServer.PARAM_FAVORITE];
			if (favorite != null)
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("favorite", favorite));

				post.favorite = int.Parse(favorite);
			}

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.NewPost, Parameters);

			var response = new UpdatePostResponse();
			response.post = post;
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