using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.PostUpload
{
	public class HidePostTask : PostUploadTask
	{
		public override void Execute()
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", this.UserId));
			if (driver != null)
			{
				using (var agent = new WebClient())
				{
					try
					{
						var postApi = new PostApi(driver);
						postApi.HidePost(agent, this.Timestamp, this.Parameters);
					}
					catch (WammerCloudException e)
					{
						this.LogDebugMsg("Error while executing hide post task.", e);

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
