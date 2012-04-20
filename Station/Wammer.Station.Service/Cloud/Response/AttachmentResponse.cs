using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Wammer.Model;

namespace Wammer.Cloud
{
	public class AttachmentResponse : CloudResponse
	{
		public Attachment attachment { get; set; }

		public AttachmentResponse()
			: base(200, 0, "success")
		{
		}

		public AttachmentResponse(Attachment att)
			: base(200, 0, "success")
		{
			attachment = att;
		}

		public AttachmentResponse(int status, DateTime timestamp)
			: base()
		{
		}
	}

	public class AttachmentGetResponse : AttachmentResponse
	{
		private AttachmentInfo attachmentInfo;

		public AttachmentGetResponse()
			: base()
		{
		}

		public AttachmentGetResponse(int status, DateTime timestamp,
			int loc, string group_id, long meta_time, string description,
			string title, string file_name, string meta_status, string object_id,
			string creator_id, AttachmentInfo.ImageMeta image_meta, bool default_post,
			long modify_time, string code_name, string hidden, string type,
			string device_id)
			: base(status, timestamp)
		{
			this.attachmentInfo = new AttachmentInfo
			{
				loc = loc,
				group_id = group_id,
				meta_time = meta_time,
				description = description,
				title = title,
				file_name = file_name,
				meta_status = meta_status,
				object_id = object_id,
				creator_id = creator_id,
				image_meta = image_meta,
				default_post = default_post,
				modify_time = modify_time,
				code_name = code_name,
				hidden = hidden,
				type = type,
				device_id = device_id
			};
		}
	}

	public class AttachmentInfo
	{
		public class ImageMetaDetail
		{
			[BsonIgnoreIfNull]
			public string url { get; set; }
			[BsonIgnoreIfNull]
			public string file_name { get; set; }
			[BsonIgnoreIfNull]
			public int height { get; set; }
			[BsonIgnoreIfNull]
			public int width { get; set; }
			[BsonIgnoreIfNull]
			public long modify_time { get; set; }
			[BsonIgnoreIfNull]
			public long file_size { get; set; }
			[BsonIgnoreIfNull]
			public string mime_type { get; set; }
			[BsonIgnoreIfNull]
			public string md5 { get; set; }
		}

		public class ImageMeta
		{
			[BsonIgnoreIfNull]
			public ImageMetaDetail large;
			[BsonIgnoreIfNull]
			public ImageMetaDetail small;
			[BsonIgnoreIfNull]
			public ImageMetaDetail medium;
			[BsonIgnoreIfNull]
			public ImageMetaDetail square;
		}

		[BsonIgnoreIfNull]
		public int loc { get; set; }
		[BsonIgnoreIfNull]
		public string group_id { get; set; }
		[BsonIgnoreIfNull]
		public long meta_time { get; set; }
		[BsonIgnoreIfNull]
		public string description { get; set; }
		[BsonIgnoreIfNull]
		public string title { get; set; }
		[BsonIgnoreIfNull]
		public string file_name { get; set; }
		[BsonIgnoreIfNull]
		public string meta_status { get; set; }
		[BsonIgnoreIfNull]
		public string object_id { get; set; }
		[BsonIgnoreIfNull]
		public string creator_id { get; set; }
		[BsonIgnoreIfNull]
		public string url { get; set; }
		[BsonIgnoreIfNull]
		public ImageMeta image_meta { get; set; }
		[BsonIgnoreIfNull]
		public bool default_post { get; set; }
		[BsonIgnoreIfNull]
		public long modify_time { get; set; }
		[BsonIgnoreIfNull]
		public string code_name { get; set; }
		[BsonIgnoreIfNull]
		public string hidden { get; set; }
		[BsonIgnoreIfNull]
		public string type { get; set; }
		[BsonIgnoreIfNull]
		public string device_id { get; set; }
	}
}
