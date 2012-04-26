using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.PostUpload
{
	public class HidePostTask : PostUploadTask
	{
		public override void Execute()
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", this.UserId));
			if (driver != null)
			{
				using (WebClient agent = new WebClient())
				{
					try
					{
						PostApi postApi = new PostApi(driver);
						if (!IsPostExist(postApi, agent))
						{
							// give up the task if the post does not exist
							return;
						}
						postApi.HidePost(agent, this.Timestamp, this.Parameters);
					}
					catch (WammerCloudException e)
					{
						this.LogDebugMsg("Error while executing hide post task.", e);

						if (CloudServer.IsNetworkError(e) || CloudServer.IsSessionError(e))
						{
							throw e;
						}

						// cloud will always reject the request, so ignore the task.
						return;
					}
				}
			}
		}
	}
}
