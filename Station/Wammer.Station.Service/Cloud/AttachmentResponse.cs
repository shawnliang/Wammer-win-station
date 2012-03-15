using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			public string url { get; set; }
			public string file_name { get; set; }
			public int height { get; set; }
			public int width { get; set; }
			public long modify_time { get; set; }
			public long file_size { get; set; }
			public string mime_type { get; set; }
			public string md5 { get; set; }
		}

		public class ImageMeta
		{
			public ImageMetaDetail large;
			public ImageMetaDetail small;
			public ImageMetaDetail medium;
			public ImageMetaDetail square;
		}

		public int loc { get; set; }
		public string group_id { get; set; }
		public long meta_time { get; set; }
		public string description { get; set; }
		public string title { get; set; }
		public string file_name { get; set; }
		public string meta_status { get; set; }
		public string object_id { get; set; }
		public string creator_id { get; set; }
		public ImageMeta image_meta { get; set; }
		public bool default_post { get; set; }
		public long modify_time { get; set; }
		public string code_name { get; set; }
		public string hidden { get; set; }
		public string type { get; set; }
		public string device_id { get; set; }
	}
}
