using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Model;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Wammer.Cloud
{
	public class Attachment
	{
		private string userToken { get; set; }

		public enum Location
		{
			Cloud = 0,
			Station = 1,
			Dropbox = 2
		}

		public Attachment(string user_id)
		{
			Drivers driver = Drivers.collection.FindOne(Query.EQ("_id", user_id));
			this.userToken = driver.session_token;
		}

		public void AttachmentSetLoc(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			CloudServer.requestPath<CloudResponse>(agent, "attachments/setloc", parameters);
		}

		public void AttachmentUnsetLoc(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			CloudServer.requestPath<CloudResponse>(agent, "attachments/unsetloc", parameters);
		}
	}
}
