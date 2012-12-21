using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
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
		public UserLogInResponse()
		{
		}

		public UserLogInResponse(int status, DateTime timestamp, string token)
			: base(status, timestamp)
		{
			session_token = token;
		}

		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }
	}

	public class GetUserResponse : CloudResponse
	{
		public GetUserResponse()
		{
		}

		public GetUserResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}

		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
		public UserInfo user { get; set; }
		public UserStorages storages { get; set; }
	}

	public class FindMyStationResponse : CloudResponse
	{
		public string session_token { get; set; }
		public List<UserGroup> groups { get; set; }
		public List<UserStation> stations { get; set; }
	}
}