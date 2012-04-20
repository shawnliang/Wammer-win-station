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
							List<string> attachmentIDs = Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',').ToList();
							AttachmentApi attachApi = new AttachmentApi(driver);
							foreach (String id in attachmentIDs)
							{
								if (!IsAttachmentExist(attachApi, agent, id))
								{
									throw new WammerStationException("Attachment " + id + " does not exist", (int)StationApiError.NotReady);
								}
							}
						}

						PostApi postApi = new PostApi(driver);
						if (IsPostExist(postApi, agent))
						{
							// give up the task if the post id is already used by another post
							return;
						}
						postApi.NewPost(new WebClient(), this.PostId, this.Timestamp, this.Parameters);
					}
					catch (WammerCloudException e)
					{
						this.LogDebugMsg("Error while calling new post api.", e);

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
