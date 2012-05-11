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
using System.Text.RegularExpressions;

namespace Wammer.Station
{
	public class UpdatePostHandler : HttpHandler
	{
		#region Const
		private const string URL_MATCH_PATTERN = @"(http://[^\s]*|https://[^\s]*)";
		#endregion

		#region Private Property
		private IPostUploadSupportable m_PostUploader { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdatePostHandler"/> class.
		/// </summary>
		/// <param name="postUploader">The post uploader.</param>
		public UpdatePostHandler(IPostUploadSupportable postUploader)
		{
			m_PostUploader = postUploader;
		}
		#endregion

		#region Private Method
		private void UpdateType(PostInfo post)
		{
			var type = Parameters[CloudServer.PARAM_TYPE];
			if (type != null)
			{
				var postID = Parameters[CloudServer.PARAM_POST_ID];
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("type", type));

				post.type = type;
			}
		}

		/// <summary>
		/// Updates the content.
		/// </summary>
		/// <param name="post">The post.</param>
		private void UpdateContent(PostInfo post)
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			UpdateContent(post, postID);
		}

		/// <summary>
		/// Updates the content.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="postID">The post ID.</param>
		private void UpdateContent(PostInfo post, string postID)
		{
			var content = Parameters[CloudServer.PARAM_CONTENT];
			if (content != null)
			{
				var type = Parameters[CloudServer.PARAM_TYPE];
				if (type == "link")
				{
					var preview = Parameters[CloudServer.PARAM_PREVIEW];
					if (preview == null)
					{
						if (!Regex.IsMatch(content, URL_MATCH_PATTERN, RegexOptions.IgnoreCase))
						{
							throw new WammerStationException(
								"content incorrect!", (int)StationLocalApiError.Error);
						}
					}
				}

				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("content", content));

				post.content = content;
			}
		}

		/// <summary>
		/// Updates the preview.
		/// </summary>
		/// <param name="post">The post.</param>
		private void UpdatePreview(PostInfo post)
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			UpdatePreview(post, postID);
		}

		/// <summary>
		/// Updates the preview.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="postID">The post ID.</param>
		private void UpdatePreview(PostInfo post, string postID)
		{
			var type = Parameters[CloudServer.PARAM_TYPE];
			if (type != null && type != "link")
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("preview", null));
				post.preview = null;
				return;
			}

			var preview = Parameters[CloudServer.PARAM_PREVIEW];
			if (preview != null)
			{
				var previewObj = fastJSON.JSON.Instance.ToObject<Preview>(preview);

				if (previewObj == null)
					throw new WammerStationException(
						"preview format incorrect!", (int)StationLocalApiError.Error);

				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("preview", previewObj.ToBsonDocument()));

				post.preview = previewObj;
			}
		}

		/// <summary>
		/// Updates the favorite.
		/// </summary>
		/// <param name="post">The post.</param>
		private void UpdateFavorite(PostInfo post)
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			UpdateFavorite(post, postID);
		}

		/// <summary>
		/// Updates the favorite.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="postID">The post ID.</param>
		private void UpdateFavorite(PostInfo post, string postID)
		{
			var favorite = Parameters[CloudServer.PARAM_FAVORITE];
			if (favorite != null)
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("favorite", favorite));

				post.favorite = int.Parse(favorite);
			}
		}

		/// <summary>
		/// Updates the cover attach.
		/// </summary>
		/// <param name="post">The post.</param>
		private void UpdateCoverAttach(PostInfo post)
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			UpdateCoverAttach(post, postID);
		}

		/// <summary>
		/// Updates the cover attach.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="postID">The post ID.</param>
		private void UpdateCoverAttach(PostInfo post, string postID)
		{
			var coverAttach = Parameters[CloudServer.PARAM_COVER_ATTACH];
			if (coverAttach != null)
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("cover_attach", coverAttach));

				post.cover_attach = coverAttach;
			}
		}
		/// <summary>
		/// Updates the local post data from cloud.
		/// </summary>
		private void UpdateLocalPostDataFromCloud()
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var driver = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (driver == null)
				throw new WammerStationException(
							"Driver not found!", (int)StationLocalApiError.InvalidDriver);

			var api = new PostApi(driver);
			var singlePostResponse = api.PostGetSingle(new DefaultWebClient(), groupID, postID);

			var responsePost = singlePostResponse.post;
			if (responsePost == null)
				throw new WammerStationException(
						"Post not found!", (int)StationLocalApiError.NotFound);

			PostCollection.Instance.Save(responsePost);
		}

		/// <summary>
		/// Updates the attachement ID array.
		/// </summary>
		/// <param name="post">The post.</param>
		private void UpdateAttachementIDArray(PostInfo post)
		{
			var postID = Parameters[CloudServer.PARAM_POST_ID];
			UpdateAttachementIDArray(post, postID);
		}

		/// <summary>
		/// Updates the attachement ID array.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="postID">The post ID.</param>
		private void UpdateAttachementIDArray(PostInfo post, string postID)
		{
			var type = Parameters[CloudServer.PARAM_TYPE];
			if (type == "link" || type == "text")
			{
				PostCollection.Instance.Update(Query.EQ("_id", postID), Update
					.Set("attachment_count", 0)
					.Set("attachment_id_array", null)
					.Set("attachments", null));

				post.attachment_id_array = null;
				post.attachment_count = 0;
				post.attachments = null;
				return;
			}

			var attachmentIDArray = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY];

			if (attachmentIDArray != null)
				attachmentIDArray = attachmentIDArray.Trim('[', ']');

			var attachmentIDs = attachmentIDArray == null ? new List<string>() : attachmentIDArray.Split(',')
				.Where((item) => !string.IsNullOrEmpty(item))
				.Select((item) => item.Trim('"')).ToList();

			if (attachmentIDs.Count > 0)
			{
				var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
				var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

				if (loginedSession == null)
					throw new WammerStationException(
								"Logined session not found!", (int)StationLocalApiError.NotFound);

				var codeName = loginedSession.apikey.name;

				var attachmentInfos = (from attachmentID in attachmentIDs
									   let attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID))
									   where attachment != null
									   select AttachmentHelper.GetAttachmentnfo(attachment, codeName)).ToList();

				PostCollection.Instance.Update(Query.EQ("_id", postID), Update
					.Set("attachment_count", attachmentIDs.Count)
					.Set("attachment_id_array", new BsonArray(attachmentIDs))
					.Set("attachments", new BsonArray(attachmentInfos.ConvertAll((item) => item.ToBsonDocument()))));


				post.attachment_id_array = attachmentIDs;
				post.attachment_count = attachmentIDs.Count;
				post.attachments = attachmentInfos;
			}
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY,
				CloudServer.PARAM_SESSION_TOKEN,
				CloudServer.PARAM_GROUP_ID,
				CloudServer.PARAM_POST_ID,
				CloudServer.PARAM_LAST_UPDATE_TIME);

			if (Parameters.Count <= 5)
				throw new WammerStationException(
						"Without any optional parameter!", (int)StationLocalApiError.Error);

			var type = Parameters[CloudServer.PARAM_TYPE];
			if (type == "link")
			{
				TunnelToCloud();

				UpdateLocalPostDataFromCloud();
				return;
			}

			var postID = Parameters[CloudServer.PARAM_POST_ID];
			var post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));
			if (post == null)
				throw new WammerStationException(
							"Post not found!", (int)StationLocalApiError.NotFound);

			UpdateType(post);
			UpdateContent(post);
			UpdatePreview(post);
			UpdateAttachementIDArray(post);
			UpdateCoverAttach(post);
			UpdateFavorite(post);

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.UpdatePost, Parameters);

			var response = new UpdatePostResponse {post = post};
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