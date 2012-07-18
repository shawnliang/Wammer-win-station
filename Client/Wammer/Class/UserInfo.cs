using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.API.V2;

namespace Waveface
{
	public class UserInfo
	{
		#region Static Var
		private static UserInfo _instance;
		#endregion


		#region Var
		private WService _service;
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
		/// Gets the m_ service.
		/// </summary>
		/// <value>The m_ service.</value>
		private WService m_Service
		{
			get
			{
				return _service ?? (_service = new WService());
			}
		}

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
				return DateTimeHelp.ConvertUnixTimestampToDateTime(m_Response.user.since);
			}
		}

		public long UploadedPhotoCount 
		{
			get
			{
				return m_Response.storages.waveface.usage.image_objects;
			}
		}

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
			m_SessionToken = Main.Current.RT.Login.session_token;
			m_UserID = Main.Current.RT.Login.user.user_id;
		}
		#endregion


		#region Private Method
		private MR_users_get GetUserData()
		{
			try
			{
				return m_Service.users_get(m_SessionToken, m_UserID);
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
}
