using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Cloud
{
	public class UserTrackResponse: CloudResponse
	{
		public int get_count { get; set; }
		public List<string> post_id_list { get; set; }
		public string group_id { get; set; }
		public string lastest_timestamp { get; set; }
		public int remaining_count { get; set; }
	}
}
