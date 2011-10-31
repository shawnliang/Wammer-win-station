using System;
using System.Collections.Generic;
using System.Text;
using Wammer.Cloud;

namespace Wammer.Station
{
	
	public class Attachment
	{
		public bool IdCreatedByStation { get; set; }
		public string Filename { get; set; }
		public string ObjectId { get; set; }
		public byte[] RawData { get; set; }
		public string ContentType { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public AttachmentType Kind { get; set; }


		public Attachment()
		{
			IdCreatedByStation = false;
		}
	}
}

