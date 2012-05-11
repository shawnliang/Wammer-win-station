using System;
using System.Collections.Generic;
using System.Net;
using Wammer.Utility;

namespace Wammer.Cloud
{
	public interface IUserTrackApi
	{
		UserTrackResponse GetChangeHistory(WebClient agent, Wammer.Model.Driver user, DateTime since);
	}

	public class UserTracksApi : IUserTrackApi
	{
		public static UserTrackResponse GetChangeHistory(WebClient agent, string session_token,
			string apikey, string group_id, string since)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"apikey", apikey},
			                 		{"session_token", session_token},
			                 		{"group_id", group_id},
			                 		{"include_entities", "true"},
			                 		{"since", since ?? ""}
			                 	};

			return CloudServer.request<UserTrackResponse>(agent, CloudServer.BaseUrl + "usertracks/get", parameters, false);
		}

		public UserTrackResponse GetChangeHistory(WebClient agent, Wammer.Model.Driver user, DateTime since)
		{
			if (user == null || user.session_token == null || user.groups == null ||
				user.groups.Count == 0 || user.groups[0].group_id == null)
				throw new ArgumentException("user, session token or group_id is null");

			return GetChangeHistory(agent, user.session_token, CloudServer.APIKey, user.groups[0].group_id, since.ToCloudTimeString());
		}
	}
}
