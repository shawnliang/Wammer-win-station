using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public abstract class PostResponse : CloudResponse
	{
		protected PostResponse()
		{
		}

		protected PostResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}

		public List<PostInfo> posts { get; set; }
		public List<UserInfo> users { get; set; }

		public abstract bool HasMoreData { get; }
	}

	public class NewPostResponse : CloudResponse
	{
		#region Var

		private List<UserInfo> _users;

		#endregion

		#region Public Property

		public PostInfo post { get; set; }

		public List<UserInfo> users
		{
			get { return _users ?? (_users = new List<UserInfo>()); }
			set { _users = value; }
		}

		public string post_id { get; set; }
		public string group_id { get; set; }
		#endregion
	}

	public class UpdatePostResponse : CloudResponse
	{
		#region Public Property

		public PostInfo post { get; set; }

		#endregion
	}

	public class HidePostResponse : CloudResponse
	{
		#region Property

		public String post_id { get; set; }

		#endregion
	}

	public class NewPostCommentResponse : CloudResponse
	{
		#region Var

		private List<UserInfo> _users;

		#endregion

		#region Public Property

		public PostInfo post { get; set; }

		public List<UserInfo> users
		{
			get { return _users ?? (_users = new List<UserInfo>()); }
			set { _users = value; }
		}

		#endregion
	}

	public class PostFetchByFilterResponse : PostResponse
	{
		public PostFetchByFilterResponse()
		{
		}

		public PostFetchByFilterResponse(string group_id, int get_count, int remaining_count,
										 List<PostInfo> posts, List<UserInfo> users)
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.remaining_count = remaining_count;
			this.posts = posts;
			this.users = users;
		}

		public string group_id { get; set; }
		public int get_count { get; set; }
		public int remaining_count { get; set; }

		public override bool HasMoreData
		{
			get { return remaining_count > 0; }
		}
	}

	public class PostGetResponse : PostResponse
	{
		public PostGetResponse()
		{
		}

		public PostGetResponse(string group_id, int get_count, int remaining_count,
							   List<PostInfo> posts, List<UserInfo> users)
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.remaining_count = remaining_count;
			this.posts = posts;
			this.users = users;
		}

		public string group_id { get; set; }
		public int get_count { get; set; }
		public long remaining_count { get; set; }

		public override bool HasMoreData
		{
			get { return remaining_count > 0; }
		}
	}

	public class PostGetSingleResponse : PostResponse
	{
		public PostGetSingleResponse()
		{
		}

		public PostGetSingleResponse(PostInfo post, List<UserInfo> users)
		{
			this.post = post;
			this.users = users;
		}

		public PostInfo post { get; set; }

		public override bool HasMoreData
		{
			get { return false; }
		}
	}

	public class PostGetLatestResponse : PostResponse
	{
		public PostGetLatestResponse()
		{
		}

		public PostGetLatestResponse(string group_id, int get_count, int total_count,
									 List<PostInfo> posts, List<UserInfo> users)
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.total_count = total_count;
			this.posts = posts;
			this.users = users;
		}

		public string group_id { get; set; }
		public int get_count { get; set; }
		public long total_count { get; set; }

		public override bool HasMoreData
		{
			get { return total_count > get_count; }
		}
	}


	[BsonIgnoreExtraElements]
	public class PostCheckIn
	{
		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public float? latitude { get; set; }

		[BsonIgnoreIfNull]
		public float? longitude { get; set; }
	}


	[BsonIgnoreExtraElements]
	public class PostGps
	{
		[BsonIgnoreIfNull]
		public float? latitude { get; set; }

		[BsonIgnoreIfNull]
		public int? zoom_level { get; set; }

		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public float? longitude { get; set; }

		[BsonIgnoreIfNull]
		public List<String> region_tags { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class ExtraParameter
	{
		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public List<string> values { get; set; }
	}

}
