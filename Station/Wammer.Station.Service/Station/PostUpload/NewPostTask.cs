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
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", this.UserId));
			if (driver != null)
			{
				using (WebClient agent = new WebClient())
				{
					try
					{
						if (Parameters.ContainsKey(CloudServer.PARAM_ATTACHMENT_ID_ARRAY))
						{
							var attachmentIDs =
								from attachmentString in Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',').ToList()
								select attachmentString.Trim('"', '"');

							foreach (String id in attachmentIDs)
							{
								if (!IsAttachmentExist(id))
								{
									throw new WammerStationException("Attachment " + id + " does not exist", (int)StationLocalApiError.NotReady);
								}
							}
						}

						PostApi postApi = new PostApi(driver);
						postApi.NewPost(agent, this.PostId, this.Timestamp, this.Parameters);
					}
					catch (WammerCloudException e)
					{
						this.LogDebugMsg("Error while executing new post task.", e);

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
