using System;
using System.Collections.Generic;
using System.Net;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station.Timeline
{
	internal class PostProvider : IPostProvider
	{
		#region IPostProvider Members

		public PostResponse GetLastestPosts(WebClient agent, Driver user, int limit)
		{
			var api = new PostApi(user);
			return api.PostGetLatest(agent, limit);
		}

		public PostResponse GetPostsBefore(WebClient agent, Driver user, DateTime before, int limit)
		{
			if (limit > 0)
				limit = -limit;

			var api = new PostApi(user);
			return api.PostFetchByFilter(agent, new FilterEntity {limit = limit, timestamp = before.ToCloudTimeString()});
		}

		public List<PostInfo> RetrievePosts(WebClient agent, Driver user, List<string> posts)
		{
			if (posts == null || posts.Count == 0)
				return new List<PostInfo>();

			var api = new PostApi(user);
			return api.PostFetchByPostId(agent, posts).posts;
		}

		#endregion
	}
}