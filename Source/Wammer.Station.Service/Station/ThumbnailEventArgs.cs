using System;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	public class ThumbnailEventArgs : EventArgs
	{
		public string object_id { get; private set; }
		public string group_id { get; private set; }
		public ImageMeta meta { get; private set; }


		public ThumbnailEventArgs(string object_id, string group_id, ImageMeta meta)
		{
			this.object_id = object_id;
			this.group_id = group_id;
			this.meta = meta;
		}
	}
}
