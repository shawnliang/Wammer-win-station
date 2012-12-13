using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public class UserInfo
	{
		#region Static Var
		private static UserInfo _instance;
		#endregion


		#region Var
		private MR_users_get _response;
		#endregion


		#region Public Static Property
		public static UserInfo Instance
		{
			get
			{
				return _instance ?? (_instance = new UserInfo());
			}
		}
		#endregion


		#region Private Property
		private DateTime m_UpdateTime { get; set; }

		/// <summary>
		/// Gets or sets the m_ session token.
		/// </summary>
		/// <value>The m_ session token.</value>
		private string m_SessionToken { get; set; }

		/// <summary>
		/// Gets or sets the m_ user ID.
		/// </summary>
		/// <value>The m_ user ID.</value>
		private string m_UserID { get; set; }


		/// <summary>
		/// Gets or sets the m_ response.
		/// </summary>
		/// <value>The m_ response.</value>
		private MR_users_get m_Response
		{
			get
			{
				var currentTime = DateTime.Now;
				if (_response == null || (currentTime - m_UpdateTime).TotalMinutes >= 10)
				{
					Update();
				}
				return _response;
			}
			set
			{
				if (_response == value)
					return;

				_response = value;

				var currentTime = DateTime.Now;
				m_UpdateTime = currentTime;
			}
		}
		#endregion


		#region Public Property
		public String NickName
		{
			get
			{
				return m_Response.user.nickname;
			}
		}

		public String Email
		{
			get
			{
				return m_Response.user.email;
			}
		}

		public DateTime Since
		{
			get
			{
				return TimeHelper.ConvertToDateTime(m_Response.user.since);
			}
		}

		//public long UploadedPhotoCount
		//{
		//	get
		//	{
		//		return m_Response.storages.waveface.usage.image_objects;
		//	}
		//}

		public Boolean Subscribed
		{
			get
			{
				return m_Response.user.subscribed;
			}
		}

		public IEnumerable<Device> Devices
		{
			get
			{
				return m_Response.user.devices;
			}
		}

		public List<SNS1> SNS1
		{
			get
			{
				return m_Response.sns;
			}
		}

		public List<SNS2> SNS2
		{
			get
			{
				return m_Response.user.sns;
			}
		}
		#endregion


		#region Constructor
		private UserInfo()
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne();

			if (loginedSession == null)
				return;

			m_SessionToken = loginedSession.session_token;
			m_UserID = loginedSession.user.user_id;
		}
		#endregion


		#region Private Method
		private MR_users_get GetUserData()
		{
			try
			{
				return JsonConvert.DeserializeObject<MR_users_get>(StationAPI.GetUser(m_SessionToken, m_UserID));
			}
			catch (Exception)
			{
			}
			return null;
		}
		#endregion


		#region Public Method
		public void Update()
		{
			var response = GetUserData();

			if (response != null)
			{
				m_Response = response;
			}
		}
		#endregion
	}

	public class General_R
	{
		public string status { get; set; }
		public string session_token { get; set; }
		public string session_expires { get; set; }
		public string timestamp { get; set; }
		public string api_ret_code { get; set; }
		public string api_ret_message { get; set; }
	}

	public class DiskUsage
	{
		public long avail { get; set; }
		public string group_id { get; set; }
		public long used { get; set; }
	}

	public class Group
	{
		public string group_id { get; set; }
		public string creator_id { get; set; }
		public string station_id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
	}

	public class Device
	{
		public string device_id { get; set; }
		public string last_visit { get; set; }
		public string device_type { get; set; }
		public string device_name { get; set; }
	}

	public class User
	{
		public string user_id { get; set; }
		public bool subscribed { get; set; }
		public int since { get; set; }
		public List<Device> devices { get; set; }
		public string state { get; set; }
		public string avatar_url { get; set; }
		public List<SNS2> sns { get; set; }
		public bool verified { get; set; }
		public string nickname { get; set; }
		public string email { get; set; }
	}

	public class Station
	{
		public string station_id { get; set; }
		public string creator_id { get; set; }
		public string timestamp { get; set; }
		public string last_seen { get; set; }
		public string location { get; set; }
		public List<DiskUsage> diskusage { get; set; }
		public string status { get; set; }
		public string accessible { get; set; }
		public int last_ping { get; set; }
		public string public_location { get; set; }
		public string version { get; set; }
		public string computer_name { get; set; }
		public bool upnp { get; set; }
		public string type { get; set; }
	}
	public class WFStorage
	{
		public WFStorageUsage usage;
		public WFStorageAvailable available;
		public WFStorageQuota quota;
		public QuotaInterval interval;
		public bool over_quota;
	}


	public class QuotaInterval
	{
		public long quota_interval_end { get; set; }
		public long quota_interval_begin { get; set; }
		public int quota_interval_left_days { get; set; }
	}

	public class WFStorageQuota
	{
		public long dropbox_objects { get; set; }
		public long origin_sizes { get; set; }
		public long total_objects { get; set; }
		public long month_total_objects { get; set; }
		public long origin_files { get; set; }
		public long month_doc_objects { get; set; }
		public long meta_files { get; set; }
		public long meta_sizes { get; set; }
		public long total_files { get; set; }
		public long month_image_objects { get; set; }
		public long image_objects { get; set; }
		public long total_sizes { get; set; }
	}

	public class WFStorageAvailable
	{
		public long avail_month_image_objects { get; set; }
		public long avail_month_total_objects { get; set; }
		public long avail_month_doc_objects { get; set; }
	}


	public class WFStorageUsage
	{
		public long dropbox_objects { get; set; }
		public long origin_sizes { get; set; }
		public long total_objects { get; set; }
		public long month_total_objects { get; set; }
		public long origin_files { get; set; }
		public long month_doc_objects { get; set; }
		public long doc_objects { get; set; }
		public long meta_files { get; set; }
		public long meta_sizes { get; set; }
		public long total_files { get; set; }
		public long month_image_objects { get; set; }
		public long image_objects { get; set; }
		public long total_sizes { get; set; }
	}

	public class Storages
	{
		public WFStorage waveface;
	}

	public class MR_users_get : General_R
	{
		public User user { get; set; }
		public List<Group> groups { get; set; }
		public List<Station> stations { get; set; }
		public List<SNS1> sns { get; set; }
		public Device device { get; set; }
		public Storages storages { get; set; }
	}

	public class SNS1
	{
		public bool enabled { get; set; }
		public string snsid { get; set; }
		public string status { get; set; }
		public string type { get; set; }
		public string lastSync { get; set; }
		public string toDate { get; set; }
	}

	public class SNS2
	{
		public bool enabled { get; set; }
		public string snsid { get; set; }
		public List<string> status { get; set; }
		public string type { get; set; }
		public string lastSync { get; set; }
		public string toDate { get; set; }
	}
}
