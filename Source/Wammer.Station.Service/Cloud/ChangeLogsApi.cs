using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public interface IChangeLogsApi
	{
		ChangeLogResponse GetChangeHistory(Driver user, int since_seq_num);
	}

	public class ChangeLogsApi : IChangeLogsApi
	{
		public ChangeLogResponse GetChangeHistory(Driver user, int since_seq_num)
		{
			return GetChangeHistory(user.session_token, CloudServer.APIKey, user.groups[0].group_id, since_seq_num);
		}

		public static ChangeLogResponse GetChangeHistory(string session_token,
														 string apikey, string group_id, int since_seq_num, bool include_entities = true)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"apikey", apikey},
			                 		{"session_token", session_token},
			                 		{"group_id", group_id},
									{"include_entities", include_entities ? "true" : "false"},
			                 	};

			if (since_seq_num >= 0)
				parameters.Add("since_seq_num", since_seq_num);

			return CloudServer.request<ChangeLogResponse>(CloudServer.BaseUrl + "changelogs/get", parameters, false);
		}
	}
}
