using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Cloud
{
	class UserTracksApi
	{
		public static UserTrackResponse GetChangeHistory(WebClient agent, string session_token,
			string apikey, string group_id, string since)
		{
			Dictionary<object, object> parameters = new Dictionary<object,object>{
						 {"apikey", apikey},
						 {"session_token", session_token},
						 {"group_id", group_id}
			};

			if (since == null)
				parameters.Add("since", "");
			else
				parameters.Add("since", since);

			return CloudServer.request<UserTrackResponse>(agent, CloudServer.BaseUrl + "usertracks/get", parameters);
		}
	}
}
