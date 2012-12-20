using System;
using System.Collections.Generic;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public interface IUserTrackApi
	{
		UserTrackResponse GetChangeHistory(Driver user, DateTime since);
	}

	public class UserTracksApi : IUserTrackApi
	{
		#region IUserTrackApi Members

		public UserTrackResponse GetChangeHistory(Driver user, DateTime since)
		{
			if (user == null || user.session_token == null || user.groups == null ||
				user.groups.Count == 0 || user.groups[0].group_id == null)
				throw new ArgumentException("user, session token or group_id is null");

			return GetChangeHistory(user.session_token, CloudServer.APIKey, user.groups[0].group_id,
									since.ToUTCISO8601ShortString());
		}

		#endregion

		public static UserTrackResponse GetChangeHistory(string session_token,
														 string apikey, string group_id, string since)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"apikey", apikey},
			                 		{"session_token", session_token},
			                 		{"group_id", group_id},
			                 		{"include_entities", "true"},
			                 		{"since", since ?? string.Empty}
			                 	};

			try
			{
				return CloudServer.request<UserTrackResponse>(CloudServer.BaseUrl + "usertracks/get", parameters, false);
			}
			catch (WammerCloudException e)
			{
				if (e.InnerException is ArgumentOutOfRangeException)
					return new UserTrackResponse() { group_id = group_id };
				else
					throw;
			}
		}
	}
}