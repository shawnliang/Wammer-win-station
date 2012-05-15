using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.PostUpload
{
	public class CommentTask : PostUploadTask
	{
		public override void Execute()
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", UserId));
			if (driver != null)
			{
				using (var agent = new WebClient())
				{
					try
					{
						var postApi = new PostApi(driver);
						postApi.NewComment(agent, Timestamp, Parameters);
					}
					catch (WammerCloudException e)
					{
						this.LogDebugMsg("Error while executing new comment task.", e);

						if (CloudServer.IsNetworkError(e) || CloudServer.IsSessionError(e))
						{
							throw;
						}
					}
				}
			}
		}
	}
}