
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
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

			var postID = Parameters[CloudServer.PARAM_POST_ID];
			var post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));

			post.hidden = "true";

			PostCollection.Instance.Update(Query.EQ("_id", postID), Update.Set("hidden", "true"));

			if (m_PostUploader != null)
				m_PostUploader.AddPostUploadAction(postID, PostUploadActionType.Hide, Parameters);

			RespondSuccess(new HidePostResponse() 
			{
				post_id = postID
			});
		}
		#endregion
	}
}