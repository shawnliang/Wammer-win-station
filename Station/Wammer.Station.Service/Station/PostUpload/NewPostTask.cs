using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;

namespace Wammer.PostUpload
{
	public class NewPostTask : PostUploadTask
	{
		protected override void Do(Driver driver)
		{
			if (Parameters.ContainsKey(CloudServer.PARAM_ATTACHMENT_ID_ARRAY))
			{
				IEnumerable<string> attachmentIDs =
					from attachmentString in Parameters[CloudServer.PARAM_ATTACHMENT_ID_ARRAY].Trim('[', ']').Split(',')
					select attachmentString.Trim('"', '"');

				foreach (String id in attachmentIDs)
				{
					if (!IsAttachmentUploaded(id, driver.session_token))
					{
						throw new WammerStationException("Attachment " + id + " is not uploaded to cloud yet",
														 (int)StationLocalApiError.NotReady);
					}
				}
			}

			var postApi = new PostApi(driver);
			postApi.NewPost(PostId, Timestamp, Parameters);
		}
	}
}
