using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Cloud;
using System.Net;

namespace Wammer.Station.TimelineChange
{
	public class PostInfoProvider : IPostInfoProvider
	{
		public ChangeHistory RetrieveChangedPosts(WebClient agent, string session_token, string start_time, string apikey, string group_id)
		{
			UserTrackResponse resp = UserTracksApi.GetChangeHistory(agent, session_token, apikey, group_id, start_time);

			return new ChangeHistory 
			{ 
				ChangedPostIds = resp.post_id_list,
				LastSyncTime = resp.lastest_timestamp
			};
		}

		public List<PostInfo> RetrievePosts(WebClient agent, List<string> posts)
		{
			throw new NotImplementedException();
		}
	}
}
