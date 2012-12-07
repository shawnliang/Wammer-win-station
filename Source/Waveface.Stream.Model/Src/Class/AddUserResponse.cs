using System;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	public class AddUserResponse : CloudResponse
	{
		public AddUserResponse()
			: base(200, DateTime.UtcNow, 0, "success")
		{
		}

		public string UserId { get; set; }
		public bool IsPrimaryStation { get; set; }
		public List<UserStation> Stations { get; set; }
	}
}
