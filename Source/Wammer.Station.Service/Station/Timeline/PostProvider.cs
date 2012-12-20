using System;
using System.Collections.Generic;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Station.Timeline
{
	internal class PostProvider : IPostProvider
	{
		#region IPostProvider Members

		public PostResponse GetLastestPosts(Driver user, int limit)
		{
			var api = new PostApi(user);
			return api.PostFetchByFilter(new FilterEntity { limit = -limit, timestamp = DateTime.Now.ToUTCISO8601ShortString() });
		}

		public PostResponse GetPostsBefore(Driver user, DateTime before, int limit)
		{
			if (limit > 0)
				limit = -limit;

			var api = new PostApi(user);
			return api.PostFetchByFilter(new FilterEntity { limit = limit, timestamp = before.ToUTCISO8601ShortString() });
		}

		public List<PostInfo> RetrievePosts(Driver user, List<string> posts)
		{
			if (posts == null || posts.Count == 0)
				return new List<PostInfo>();

			var api = new PostApi(user);
			return api.PostFetchByPostId(posts).posts;
		}

		public PostResponse GetPostsBySeq(Driver user, int seq, int limit)
		{
			var api = new PostApi(user);
			return api.PostFetchBySeq(seq, limit);
		}

		#endregion

	}
}