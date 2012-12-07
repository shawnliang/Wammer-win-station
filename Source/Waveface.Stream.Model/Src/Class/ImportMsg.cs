using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	public class ImportMsg
	{
		public List<string> files { get; set; }
		public string session_token { get; set; }
		public string group_id { get; set; }
		public string apikey { get; set; }
	}
}
