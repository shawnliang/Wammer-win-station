using System;
using Wammer.Model;

namespace Wammer.Station.JSONClass
{
	public class AttachmentView
	{
		public string creator_id { get; set; }
		public string file_name { get; set; }
		public string md5 { get; set; }
		public string type { get; set; }
		public string group_id { get; set; }
		public string redirect_to { get; set; }
		public ImageProperty image_meta { get; set; }
	}

	public class ImageProperty
	{
		public int width { get; set; }
		public int height { get; set; }

		public Thumbnail small { get; set; }
		public Thumbnail medium { get; set; }
		public Thumbnail large { get; set; }
		public Thumbnail square { get; set; }

		public Thumbnail GetThumbnail(ImageMeta meta)
		{
			switch (meta)
			{
				case ImageMeta.Small:
					return small;
				case ImageMeta.Medium:
					return medium;
				case ImageMeta.Large:
					return large;
				case ImageMeta.Square:
					return square;
				default:
					throw new ArgumentException("Unknown meta: " + meta);
			}
		}
	}

	public class Thumbnail
	{
		public int width { get; set; }
		public int height { get; set; }
	}
}