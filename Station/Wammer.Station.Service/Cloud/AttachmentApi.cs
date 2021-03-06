﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Model;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

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
	}
}
