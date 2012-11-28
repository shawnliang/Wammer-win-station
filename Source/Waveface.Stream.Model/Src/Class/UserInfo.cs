using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	public class UserInfo
	{
		public string user_id { get; set; }
		public List<Device> devices { get; set; }
		public string state { get; set; }
		public string avatar_url { get; set; }
		public bool verified { get; set; }
		public string nickname { get; set; }
		public string email { get; set; }
	}
}
