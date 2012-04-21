using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class HidePostHandler : HttpHandler
	{
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
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

			var response = new HidePostResponse() 
			{
				post_id = postID
			};

			RespondSuccess(response);
		}
		#endregion
	}
}