using MongoDB.Bson.Serialization.Attributes;
using System;
using Wammer.Model;

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
			this.md5 = att.md5;
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

	[Serializable]
	[BsonIgnoreExtraElements]
	public class AttachmentInfo
	{
		public AttachmentInfo()
		{
		}

		public AttachmentInfo(Attachment attachment)
		{
			group_id = attachment.group_id;
			file_name = attachment.file_name;
			object_id = attachment.object_id;
			type = attachment.type.ToString();
			image_meta = new ImageMeta();

			if (attachment.image_meta != null)
			{
				if (attachment.image_meta.large != null)
					image_meta.large = new ImageMetaDetail(attachment.image_meta.large);
				if (attachment.image_meta.medium != null)
					image_meta.medium = new ImageMetaDetail(attachment.image_meta.medium);
				if (attachment.image_meta.small != null)
					image_meta.small = new ImageMetaDetail(attachment.image_meta.small);
				if (attachment.image_meta.square != null)
					image_meta.square = new ImageMetaDetail(attachment.image_meta.square);
			}
		}

		public ImageMetaDetail GetThumbnail(Wammer.Model.ImageMeta meta)
		{
			switch (meta)
			{
				case Wammer.Model.ImageMeta.Small:
					return image_meta.small;
				case Model.ImageMeta.Medium:
					return image_meta.medium;
				case Model.ImageMeta.Large:
					return image_meta.large;
				case Model.ImageMeta.Square:
					return image_meta.square;
				default:
					throw new ArgumentOutOfRangeException("meta is not allowed: " + meta);
			}
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

		[BsonIgnoreIfNull]
		public string file_create_time { get; set; }

		[BsonIgnoreIfNull]
		public string md5 { get; set; }

		[BsonIgnoreIfNull]
		public string file_path { get; set; }

		[BsonIgnoreIfNull]
		public string import_time { get; set; }

		[BsonIgnoreIfNull]
		public DateTime event_time { get; set; }

		#region Nested type: ImageMeta
		[Serializable]
		[BsonIgnoreExtraElements]
		public class ImageMeta
		{
			[BsonIgnoreIfNull]
			public exif exif { get; set; }

			public int width { get; set; }

			public int height { get; set; }

			[BsonIgnoreIfNull]
			public ImageMetaDetail large { get; set; }

			[BsonIgnoreIfNull]
			public ImageMetaDetail medium { get; set; }

			[BsonIgnoreIfNull]
			public ImageMetaDetail small { get; set; }

			[BsonIgnoreIfNull]
			public ImageMetaDetail square { get; set; }

			/// <summary>
			/// Cloud extract gps from exif info and put here.
			/// </summary>
			[BsonIgnoreIfNull]
			public Model.Gps gps { get; set; }
		}

		#endregion

		#region Nested type: ImageMetaDetail
		[Serializable]
		[BsonIgnoreExtraElements]
		public class ImageMetaDetail
		{
			public ImageMetaDetail()
			{
			}

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

			/// <summary>
			/// Gets or sets mime_type
			/// </summary>
			[BsonIgnoreIfNull]
			public string mime_type { get; set; }

			/// <summary>
			/// Gets or sets md5
			/// </summary>
			[BsonIgnoreIfNull]
			public string md5 { get; set; }


		}

		#endregion
	}
}
