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

		public double? latitude { get; set; }

		public double? longitude { get; set; }
	}
}
