using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station.Timeline
{
	class PostProvider : IPostProvider
	{
		public PostResponse GetLastestPosts(System.Net.WebClient agent, Driver user, int limit)
		{
			PostApi api = new PostApi(user);
			return api.PostGetLatest(agent, limit);
		}

		public PostResponse GetPostsBefore(System.Net.WebClient agent, Driver user, DateTime before, int limit)
		{
			if (limit > 0)
				limit = -limit;

			PostApi api = new PostApi(user);
			return api.PostFetchByFilter(agent, new FilterEntity { limit = limit, timestamp = before.ToCloudTimeString() });
		}

		public List<PostInfo> RetrievePosts(System.Net.WebClient agent, Driver user, List<string> posts)
		{
			if (posts == null || posts.Count == 0)
				return new List<PostInfo>();

			PostApi api = new PostApi(user);
			return api.PostFetchByPostId(agent, posts).posts;
		}
	}
}
