using System;
using System.Collections.Generic;
using Wammer.Utility;
using Wammer.Model;

namespace Wammer.Cloud
{
	public class UserStation
	{
		public string status { get; set; }
		public string station_id { get; set; }
		public string creator_id { get; set; }
		public string location { get; set; }
		public long last_seen { get; set; }
		public string computer_name { get; set; }
		public string accessible { get; set; }
		public string public_location { get; set; }
		public bool upnp { get; set; }
		public string type { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public DateTime LastSeen 
		{
			get
			{
				return TimeHelper.ConvertToDateTime(last_seen);
			}

			set
			{
				last_seen = TimeHelper.ConvertToUnixTimeStamp(value);
			}
		}
	}

	public class UserGroup
	{
		public string description { get; set; }
		public string creator_id { get; set; }
		public string group_id { get; set; }
		public string name { get; set; }
	}

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

	public class UserStorages
	{
		public UserStorage waveface { get; set; }
	}

	public class UserStorage
	{
		public UserStorageUsage usage { get; set; }
		public UserStorageAvail available { get; set; }
		public UserStorageInterval interval { get; set; }
		public bool over_quota { get; set; }
	}

	public class UserStorageAvail
	{
		public long avail_month_total_objects { get; set; }
	}

	public class UserStorageUsage
	{
		public long origin_sizes { get; set; }
		public long total_objects { get; set; }
		public long month_total_objects { get; set; }
		public long meta_sizes { get; set; }
		public long origin_files { get; set; }
		public long meta_files { get; set; }
		public long total_files { get; set; }
		public long month_image_objects { get; set; }
		public long image_objects { get; set; }
		public long total_sizes { get; set; }
	}

	public class UserStorageInterval
	{
		public long quota_interval_end { get; set; }
		public int quota_interval_left_days { get; set; }
		public long quota_interval_begin { get; set; }

		public DateTime GetIntervalEndTime()
		{
			return TimeHelper.ConvertToDateTime(quota_interval_end);
		}
	}

	public class UserLogInResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }

		public UserLogInResponse()
			: base()
		{
		}

		public UserLogInResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			this.session_token = token;
		}
	}

	public class GetUserResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get;set; }

		public GetUserResponse()
			: base()
		{
		}

		public GetUserResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}
	}

	public class FindMyStationResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
	}
}
