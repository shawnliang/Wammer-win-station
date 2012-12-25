using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	public class PostCheckIn
	{
		public string name { get; set; }

		public float? latitude { get; set; }

		public float? longitude { get; set; }
	}
}
