

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

		public Thumbnail GetThumbnail(Wammer.Model.ImageMeta meta)
		{
			switch (meta)
			{
				case Wammer.Model.ImageMeta.Small:
					return small;
				case Model.ImageMeta.Medium:
					return medium;
				case Model.ImageMeta.Large:
					return large;
				case Model.ImageMeta.Square:
					return square;
				default:
					throw new System.ArgumentException("Unknown meta: " + meta);
			}
		}
	}

	public class Thumbnail
	{
		public int width { get; set; }
		public int height { get; set; }

	}

}
