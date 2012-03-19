using System.Collections.Generic;
using System.Net;
using System.ComponentModel;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Station;


namespace Wammer.Cloud
{
	public class AttachmentApi
	{
		private string userToken { get; set; }

		public enum Location
		{
			Cloud = 0,
			Station = 1,
			Dropbox = 2
		}

		public AttachmentApi(string user_id)
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));
			this.userToken = driver.session_token;
		}

		public void AttachmentSetLoc(WebClient agent, int loc, string object_id, string file_path)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "loc", loc },
				{ "object_id", object_id },
				{ "file_path", file_path },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken},
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/setloc", parameters);
		}

		public void AttachmentUnsetLoc(WebClient agent, int loc, string object_id)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "loc", loc },
				{ "object_id", object_id },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.PARAM_API_KEY }
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/unsetloc", parameters);
		}

		public AttachmentGetResponse AttachmentGet(WebClient agent, string object_id)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_OBJECT_ID, object_id},
				{CloudServer.PARAM_SESSION_TOKEN, this.userToken},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			AttachmentGetResponse res = 
				CloudServer.requestPath<AttachmentGetResponse>(agent, "attachments/get", parameters);
			return res;
		}

		public void AttachmentView(WebClient agent, AsyncCompletedEventHandler handler, ResourceDownloadEventArgs evtargs)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object> 
			{ 
				{CloudServer.PARAM_OBJECT_ID, evtargs.attachment.object_id},
				{CloudServer.PARAM_SESSION_TOKEN, this.userToken},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			if (evtargs.imagemeta != ImageMeta.Origin)
			{
				parameters.Add(CloudServer.PARAM_IMAGE_META, evtargs.imagemeta.ToString().ToLower());
			}

			CloudServer.requestAsyncDownload(agent, "attachments/view", parameters, evtargs.filepath, handler, evtargs);
		}
	}
}
