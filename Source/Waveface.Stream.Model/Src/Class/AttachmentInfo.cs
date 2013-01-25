using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Waveface.Stream.Model
{
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

		public ImageMetaDetail GetThumbnail(Waveface.Stream.Model.ImageMeta meta)
		{
			switch (meta)
			{
				case Waveface.Stream.Model.ImageMeta.Small:
					return image_meta.small;
				case Waveface.Stream.Model.ImageMeta.Medium:
					return image_meta.medium;
				case Waveface.Stream.Model.ImageMeta.Large:
					return image_meta.large;
				case Waveface.Stream.Model.ImageMeta.Square:
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
		public bool hidden { get; set; }

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

		[BsonIgnoreIfNull]
		public DocProperty doc_meta { get; set; }

		[BsonIgnoreIfNull]
		public WebProperty web_meta { get; set; }

		[BsonIgnoreIfNull]
		public string mime_type { get; set; }

		public long file_size { get; set; }

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
			public Gps gps { get; set; }
		}

		#endregion

		#region Nested type: ImageMetaDetail
		[Serializable]
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
