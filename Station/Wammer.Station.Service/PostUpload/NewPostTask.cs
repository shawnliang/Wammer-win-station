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
	public class NewPostTask : PostUploadTask
	{
		public override void Execute()
		{
			try
			{
				Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", this.UserId));
				if (driver != null)
				{
					PostApi api = new PostApi(driver);
					api.NewPost(new WebClient(), this.PostId, this.Timestamp, this.Parameters);
				}
			}
			catch (WammerCloudException e)
			{
				this.LogDebugMsg("Error while calling new post api.", e);

				if (CloudServer.IsNetworkError(e))
				{
					throw e;
				}

				if (CloudServer.IsSessionError(e))
				{
					throw e;
				}

				if (!Enum.IsDefined(typeof(PostApiError), e.WammerError))
				{
					throw e;
				}

				// cloud will always reject the request, so ignore the task.
				return;
			}
		}
	}
}
