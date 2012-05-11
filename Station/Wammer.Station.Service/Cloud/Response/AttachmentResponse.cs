using System;
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
		public AttachmentGetResponse()
			: base()
		{
		}
	}

	public class AttachmentInfo
	{
		public AttachmentInfo() {}

		public AttachmentInfo(Attachment attachment)
		{
			group_id = attachment.group_id;
			file_name = attachment.file_name;
			object_id = attachment.object_id;
			type = attachment.type.ToString();
			image_meta = new ImageMeta { 
				large = new ImageMetaDetail(attachment.image_meta.large),
				medium = new ImageMetaDetail(attachment.image_meta.medium),
				small = new ImageMetaDetail(attachment.image_meta.small),
				square = new ImageMetaDetail(attachment.image_meta.square)
			};
		}

		public class ImageMetaDetail
		{
			public ImageMetaDetail() {}

			public ImageMetaDetail(ThumbnailInfo thumbnail)
			{
				url = thumbnail.url;
				width = thumbnail.width;
				height = thumbnail.height;
				mime_type = thumbnail.mime_type;
				file_size = thumbnail.file_size;
				md5 = thumbnail.md5;
			}

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
