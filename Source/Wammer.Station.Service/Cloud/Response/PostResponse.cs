using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

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
	public class Preview
	{
		[BsonIgnoreIfNull]
		public string description { get; set; }

		[BsonIgnoreIfNull]
		public string title { get; set; }

		[BsonIgnoreIfNull]
		public string url { get; set; }

		[BsonIgnoreIfNull]
		public string provider_display { get; set; }

		[BsonIgnoreIfNull]
		public string favicon_url { get; set; }

		[BsonIgnoreIfNull]
		public string thumbnail_url { get; set; }

		[BsonIgnoreIfNull]
		public string type { get; set; }

		[BsonIgnoreIfNull]
		public List<Image> images { get; set; }

		#region inner classes
		[BsonIgnoreExtraElements]
		public class Image
		{
			[BsonIgnoreIfNull]
			public string url;
			[BsonIgnoreIfNull]
			public string type;
		}
		#endregion
		
	}

	[BsonIgnoreExtraElements]
	public class Comment
	{
		[BsonIgnoreIfNull]
		public string content { get; set; }

		[BsonIgnoreIfNull]
		public string timestamp { get; set; }

		[BsonIgnoreIfNull]
		public string creator_id { get; set; }

		[BsonIgnoreIfNull]
		public string code_name { get; set; }

		[BsonIgnoreIfNull]
		public string device_id { get; set; }
	}

    [BsonIgnoreExtraElements]
    public class Gps
    {
        [BsonIgnoreIfNull]
        public float latitude { get; set; }

        [BsonIgnoreIfNull]
        public int zoom_level { get; set; }

        [BsonIgnoreIfNull]
        public string name { get; set; }

        [BsonIgnoreIfNull]
        public float longitude { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Person
    {
        [BsonIgnoreIfNull]
        public string name { get; set; }

        [BsonIgnoreIfNull]
        public string avatar { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ExtraParameter
    {
        [BsonIgnoreIfNull]
        public string name { get; set; }

        [BsonIgnoreIfNull]
        public List<string> values { get; set; }
    }

	[BsonIgnoreExtraElements]
	public class PostInfo
	{
		[BsonIgnoreIfNull]
		public List<AttachmentInfo> attachments { get; set; }

		[BsonIgnoreIfNull]
		public int attachment_count { get; set; }

		[BsonIgnoreIfNull]
		public string event_time { get; set; }

		[BsonIgnoreIfNull]
		public List<Comment> comments { get; set; }

		[BsonIgnoreIfNull]
		public string content { get; set; }

		[BsonIgnoreIfNull]
		public int comment_count { get; set; }

		[BsonIgnoreIfNull]
		public string hidden { get; set; }

		[BsonIgnoreIfNull]
		public Preview preview { get; set; }

		[BsonIgnoreIfNull]
		public string type { get; set; }

		[BsonIgnoreIfNull]
		public List<string> style { get; set; }

		[BsonIgnoreIfNull]
		public DateTime update_time { get; set; }

		[BsonIgnoreIfNull]
		public Boolean import { get; set; }

        [BsonIgnoreIfNull]
        public DateTime timestamp { get; set; }

        [BsonId]
        public string post_id { get; set; }

        [BsonIgnoreIfNull]
        public List<Person> people { get; set; }

        [BsonIgnoreIfNull]
        public List<ExtraParameter> extra_parameters { get; set; }

        [BsonIgnoreIfNull]
        public Gps gps { get; set; }

        [BsonIgnoreIfNull]
        public List<string> tags { get; set; }

        [BsonIgnoreIfNull]
        public string code_name { get; set; }

        [BsonIgnoreIfNull]
		public List<string> attachment_id_array { get; set; }

		[BsonIgnoreIfNull]
		public string device_id { get; set; }

		[BsonIgnoreIfNull]
		public string group_id { get; set; }

		[BsonIgnoreIfNull]
		public int favorite { get; set; }

		[BsonIgnoreIfNull]
		public string soul { get; set; }

		[BsonIgnoreIfNull]
		public string creator_id { get; set; }

		[BsonIgnoreIfNull]
		public string cover_attach { get; set; }

		[BsonIgnoreIfNull]
		public int seq_num { get; set; }
	}
}