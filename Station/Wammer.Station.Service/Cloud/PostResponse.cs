using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Cloud
{
	public class PostResponse : CloudResponse
	{
		public string group_id { get; set; }
		public int get_count { get; set; }
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
	}

	public class PostFetchByFilterResponse : PostResponse
	{
		public int remaining_count { get; set; }

		public PostFetchByFilterResponse()
			: base()
		{
		}

		public PostFetchByFilterResponse(int status, DateTime timestamp, 
			string group_id, int get_count, int remaining_count, 
			List<PostInfo> posts, List<UserInfo> users)
			: base()
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.remaining_count = remaining_count;
			this.posts = posts;
			this.users = users;
		}
	}

	public class PostGetLatestResponse : PostResponse
	{
		public int total_count { get; set; }

		public PostGetLatestResponse()
			: base()
		{
		}

		public PostGetLatestResponse(int status, DateTime timestamp, 
			string group_id, int get_count, int total_count, 
			List<PostInfo> posts, List<UserInfo> users)
			: base()
		{
			this.group_id = group_id;
			this.get_count = get_count;
			this.total_count = total_count;
			this.posts = posts;
			this.users = users;
		}
	}

	public class Preview
	{
		public string description { get; set; }
		public string title { get; set; }
		public string url { get; set; }
		public string provider_display { get; set; }
		public string favicon_url { get; set; }
		public string thumbnail_url { get; set; }
		public string type { get; set; }
	}

	public class Comment
	{
		public string content { get; set; }
		public string timestamp { get; set; }
		public string creator_id { get; set; }
		public string code_name { get; set; }
		public string device_id { get; set; }
	}

	public class PostInfo
	{
		public List<AttachmentInfo> attachments { get; set; }
		public int attachment_count { get; set; }
		public string event_time { get; set; }
		public List<Comment> comments { get; set; }
		public string content { get; set; }
		public int comment_count { get; set; }
		public string hidden { get; set; }
		public Preview preview { get; set; }
		public string type { get; set; }
		public string update_time { get; set; }
		public string timestamp { get; set; }
		public string post_id { get; set; }
		public string code_name { get; set; }
		public List<string> attachment_id_array { get; set; }
		public string device_id { get; set; }
		public string group_id { get; set; }
		public int favorite { get; set; }
		public string soul { get; set; }
		public string creator_id { get; set; }
		public string cover_attach { get; set; }
	}
}
