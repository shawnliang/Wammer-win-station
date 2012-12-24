using MongoDB.Bson.Serialization.Attributes;
using System;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class AttachmentResponse : CloudResponse
	{
		// default constructor is for Json Serialization
		public AttachmentResponse()
			: base(200, 0, "success")
		{
		}

		public AttachmentResponse(Attachment att)
			: base(200, 0, "success")
		{
			this.object_id = att.object_id;
			this.file_name = att.file_name;
			this.file_size = att.file_size;
			this.type = att.type.ToString();
			this.image = att.image;
			this.mime_type = att.mime_type;
			this.md5 = att.MD5;
			this.url = att.url;
			this.creator_id = att.creator_id;
			this.image_meta = att.image_meta;
			this.modify_time = att.modify_time;
			this.group_id = att.group_id;
			this.title = att.title;
			this.description = att.description;
		}

		public string object_id { get; set; }
		public string file_name { get; set; }
		public long file_size { get; set; }
		public string type { get; set; }
		public string image { get; set; }
		public string mime_type { get; set; }
		public string md5 { get; set; }
		public string url { get; set; }
		public string creator_id { get; set; }
		public ImageProperty image_meta { get; set; }
		public DateTime modify_time { get; set; }
		public string group_id { get; set; }
		public string title { get; set; }
		public string description { get; set; }
	}
}
