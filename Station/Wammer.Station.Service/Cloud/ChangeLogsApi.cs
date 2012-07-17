using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using Wammer.Station;

namespace Wammer.Cloud
{
	public interface IChangeLogsApi
	{
		ChangeLogResponse GetChangeHistory(Driver user, int since_seq_num);
	}

	public class ChangeLogsApi: IChangeLogsApi
	{
		public ChangeLogResponse GetChangeHistory(Driver user, int since_seq_num)
		{
			return GetChangeHistory(user.session_token, CloudServer.APIKey, user.groups[0].group_id, since_seq_num);
		}

		public static ChangeLogResponse GetChangeHistory(string session_token,
		                                                 string apikey, string group_id, int since_seq_num)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"apikey", apikey},
			                 		{"session_token", session_token},
			                 		{"group_id", group_id},
			                 		{"include_entities", "true"},
			                 		{"since_seq_num", since_seq_num}
			                 	};

			try
			{
				return CloudServer.request<ChangeLogResponse>(CloudServer.BaseUrl + "changelogs/get", parameters, false);
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == (int)UserTrackApiError.NoData)
					return new ChangeLogResponse() { next_seq_num = since_seq_num, api_ret_code = 200 };
				else
					throw;
			}
		}
	}
}
