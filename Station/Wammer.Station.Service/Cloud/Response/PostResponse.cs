using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Cloud
{
	abstract public class PostResponse : CloudResponse
	{
		public List<PostInfo> posts { get; set; }
		public List<UserInfo> users { get; set; }

		public PostResponse()
			: base()
		{
		}

		public PostResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}

		abstract public bool HasMoreData { get; }
	}

	public class NewPostResponse : CloudResponse
	{
		#region Var
		private List<PostInfo> _posts;
		private List<UserInfo> _users;
		#endregion

		#region Public Property
		public List<PostInfo> posts 
		{
			get
			{
				if (_posts == null)
					_posts = new List<PostInfo>();
				return _posts;
			}
			set 
			{
				_posts = value;
			}
		}

		public List<UserInfo> users
		{
			get
			{
				if (_users == null)
					_users = new List<UserInfo>();
				return _users;
			}
			set
			{
				_users = value;
			}
		} 
		#endregion
	}

	public class UpdatePostResponse : CloudResponse
	{
		#region Public Property
		public PostInfo post{get;set;}
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
		public PostInfo post{get;set;}

		public List<UserInfo> users
		{
			get
			{
				if (_users == null)
					_users = new List<UserInfo>();
				return _users;
			}
			set
			{
				_users = value;
			}
		} 
		#endregion
	}

	public class PostFetchByFilterResponse : PostResponse
	{
		public string group_id { get; set; }
		public int get_count { get; set; }
		public int remaining_count { get; set; }

		public PostFetchByFilterResponse()
			: base()
		{
		}

		public PostFetchByFilterResponse(string group_id, int get_count, int remaining_count, 
			List<PostInfo> posts, List<UserInfo> users)
			: base()
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.remaining_count = remaining_count;
			this.posts = posts;
			this.users = users;
		}

		public override bool HasMoreData
		{
			get { return remaining_count > 0; }
		}
	}

	public class PostGetResponse : PostResponse
	{
		public string group_id { get; set; }
		public int get_count { get; set; }
		public long remaining_count { get; set; }

		public PostGetResponse()
			: base()
		{
		}

		public PostGetResponse(string group_id, int get_count, int remaining_count,
			List<PostInfo> posts, List<UserInfo> users)
			: base()
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.remaining_count = remaining_count;
			this.posts = posts;
			this.users = users;
		}

		public override bool HasMoreData
		{
			get { return remaining_count > 0; }
		}
	}

	public class PostGetSingleResponse : PostResponse
	{
		public PostInfo post { get; set; }

		public PostGetSingleResponse()
			: base()
		{
		}

		public PostGetSingleResponse(PostInfo post, List<UserInfo> users)
			: base()
		{
			this.post = post;
			this.users = users;
		}

		public override bool HasMoreData
		{
			get { return false; }
		}
	}

	public class PostGetLatestResponse : PostResponse
	{
		public string group_id { get; set; }
		public int get_count { get; set; }
		public long total_count { get; set; }

		public PostGetLatestResponse()
			: base()
		{
		}

		public PostGetLatestResponse(string group_id, int get_count, int total_count, 
			List<PostInfo> posts, List<UserInfo> users)
			: base()
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.total_count = total_count;
			this.posts = posts;
			this.users = users;
		}

		public override bool HasMoreData
		{
			get { return total_count > get_count; }
		}
	}

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
	}

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
		public DateTime update_time { get; set; }
		[BsonIgnoreIfNull]
		public DateTime timestamp { get; set; }

		[BsonId]
		public string post_id { get; set; }
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
	}
}
