using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
{
	public class UserInfo
	{
		#region Static Var
		private static UserInfo _instance;
		#endregion


		#region Var
		private string _response;
		private MR_users_get _responseObj;
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
		private DateTime? m_UpdateTime { get; set; }

		private string m_SessionToken
		{
			get
			{
				return StreamClient.Instance.LoginedUser.SessionToken;
			}
		}

		/// <summary>
		/// Gets the m_ user ID.
		/// </summary>
		/// <value>
		/// The m_ user ID.
		/// </value>
		private string m_UserID
		{
			get
			{
				return StreamClient.Instance.LoginedUser.UserID;
			}
		}

		private string m_PreviousResponse { get; set; }

		/// <summary>
		/// Gets or sets the m_ response.
		/// </summary>
		/// <value>
		/// The m_ response.
		/// </value>
		private string m_Response
		{
			get
			{
				var currentTime = DateTime.Now;
				if (String.IsNullOrEmpty(_response) || (m_UpdateTime.HasValue && (currentTime - m_UpdateTime.Value).TotalMinutes >= 5))
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
				_responseObj = null;

				var currentTime = DateTime.Now;
				m_UpdateTime = currentTime;

				if (string.IsNullOrEmpty(value))
					return;

				if (!value.Equals(m_PreviousResponse, StringComparison.CurrentCultureIgnoreCase))
					OnUserInfoUpdated(EventArgs.Empty);

				m_PreviousResponse = value;
			}
		}

		/// <summary>
		/// Gets or sets the m_ response obj.
		/// </summary>
		/// <value>
		/// The m_ response obj.
		/// </value>
		private MR_users_get m_ResponseObj
		{
			get
			{
				try
				{
					return _responseObj ?? (_responseObj = JsonConvert.DeserializeObject<MR_users_get>(m_Response));
				}
				catch (Exception)
				{
					return null;
				}
			}
			set
			{
				_responseObj = value;
			}
		}
		#endregion


		#region Public Property
		public String NickName
		{
			get
			{
				return m_ResponseObj.user.nickname;
			}
		}

		public String Email
		{
			get
			{
				return m_ResponseObj.user.email;
			}
		}

		public DateTime Since
		{
			get
			{
				return TimeHelper.ConvertToDateTime(m_ResponseObj.user.since);
			}
		}

		public Boolean Subscribed
		{
			get
			{
				return m_ResponseObj.user.subscribed;
			}
		}

		public IEnumerable<Device> Devices
		{
			get
			{
				return m_ResponseObj.user.devices;
			}
		}

		public List<SNS1> SNS1
		{
			get
			{
				return m_ResponseObj.sns;
			}
		}

		public List<SNS2> SNS2
		{
			get
			{
				return m_ResponseObj.user.sns;
			}
		}

		public long TotalQuota
		{
			get
			{
				return (m_ResponseObj.quota.total != null) ? m_ResponseObj.quota.total.origin_size : m_ResponseObj.quota.doc.origin_size + m_ResponseObj.quota.image.origin_size;
			}
		}

		public long TotalUsage
		{
			get
			{
				return m_ResponseObj.usage.doc.origin_size + m_ResponseObj.usage.image.origin_size;
			}
		}

		public long PhotoMetaCount
		{
			get
			{
				return m_ResponseObj.usage.image.objects;
			}
		}

		public long WebMetaCount
		{
			get
			{
				return m_ResponseObj.usage.webthumb.objects;
			}
		}

		public long DocumentMetaCount
		{
			get
			{
				return m_ResponseObj.usage.doc.objects;
			}
		}


		public Boolean Paid
		{
			get
			{
				return m_ResponseObj.billing.type.Equals("paid", StringComparison.CurrentCultureIgnoreCase);
			}
		}

		public string Plan
		{
			get
			{
				return m_ResponseObj.billing.plan;
			}
		}
		#endregion


		#region Event
		public event EventHandler UserInfoUpdated;
		public event EventHandler<ExceptionEventArgs> UserInfoUpdateFail;
		#endregion


		#region Private Method
		private string GetUserData()
		{
			var response = StationAPI.GetUser(m_SessionToken, m_UserID);
			return response;
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:UserInfoUpdated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnUserInfoUpdated(EventArgs e)
		{
			this.RaiseEvent(UserInfoUpdated, e);
		}

		/// <summary>
		/// Raises the <see cref="E:UserInfoUpdateFail" /> event.
		/// </summary>
		/// <param name="e">The <see cref="ExceptionEventArgs" /> instance containing the event data.</param>
		protected void OnUserInfoUpdateFail(ExceptionEventArgs e)
		{
			this.RaiseEvent<ExceptionEventArgs>(UserInfoUpdateFail, e);
		}
		#endregion


		#region Public Method
		public void Clear()
		{
			m_UpdateTime = null;
			m_Response = null;
			m_ResponseObj = null;
		}

		public void Update()
		{
			try
			{
				m_Response = GetUserData();
			}
			catch (Exception e)
			{
				OnUserInfoUpdateFail(new ExceptionEventArgs(e));
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

	public class QuotaItem
	{
		public long origin_size { get; set; }
		public long origin_files { get; set; }
	}

	public class Quota
	{
		public QuotaItem doc { get; set; }
		public QuotaItem image { get; set; }
		public QuotaItem total { get; set; }
	}

	public class UsageItem
	{
		public long meta_files { get; set; }
		public long objects { get; set; }
		public long meta_size { get; set; }
		public long origin_files { get; set; }
		public long origin_size { get; set; }
	}

	public class Usage
	{
		public UsageItem doc { get; set; }
		public UsageItem image { get; set; }
		public UsageItem webthumb { get; set; }
	}

	public class Billing
	{
		public string type { get; set; }
		public string plan { get; set; }
		public int cycle { get; set; }
	}

	public class MR_users_get : General_R
	{
		public User user { get; set; }
		public List<Group> groups { get; set; }
		public Billing billing { get; set; }
		public List<Station> stations { get; set; }
		public List<SNS1> sns { get; set; }
		public Quota quota { get; set; }
		public Usage usage { get; set; }
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
