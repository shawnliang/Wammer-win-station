using MongoDB.Driver.Builders;
using System;
using Wammer.Cloud;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/posts/hide/")]
	public class HidePostHandler : HttpHandler
	{
		#region Private Property

		private IPostUploadSupportable m_PostUploader { get; set; }

		#endregion

		#region Constructor

		public HidePostHandler(IPostUploadSupportable postUploader)
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
				CloudServer.PARAM_POST_ID);

			string postID = Parameters[CloudServer.PARAM_POST_ID];
			PostInfo post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));

			var lastUpdateTime = post.update_time;

			post.hidden = "true";
			post.update_time = DateTime.Now;

			PostCollection.Instance.Update(Query.EQ("_id", postID), Update
				.Set("hidden", "true")
				.Set("update_time", post.update_time));

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.Hide, Parameters, post.update_time, lastUpdateTime);

			RespondSuccess(new HidePostResponse
							{
								post_id = postID
							});
		}

		#endregion
	}
}