using System.Collections.Generic;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class StationSignUpResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }
	}

	public class StationLogOnResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }
	}

	public class StationHeartbeatResponse : CloudResponse
	{
		public string session_token { get; set; }
		public UserStation station { get; set; }
	}
}