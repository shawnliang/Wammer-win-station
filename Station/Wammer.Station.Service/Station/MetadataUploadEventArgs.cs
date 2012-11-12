using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
