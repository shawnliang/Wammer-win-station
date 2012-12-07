using System;

namespace Wammer.Station
{
	public class MetadataUploadEventArgs : EventArgs
	{
		public int Count { get; set; }

		public MetadataUploadEventArgs(int count)
		{
			this.Count = count;
		}
	}
}
