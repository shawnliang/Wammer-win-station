using Newtonsoft.Json;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	public class UserData
	{
		#region Public Property
		/// <summary>
		/// Gets or sets the session_token.
		/// </summary>
		/// <value>
		/// The session_token.
		/// </value>
		[JsonProperty("session_token", NullValueHandling = NullValueHandling.Ignore)]
		public string SessionToken { get; set; }

		/// <summary>
		/// Gets or sets the nickname.
		/// </summary>
		/// <value>
		/// The nickname.
		/// </value>
		[JsonProperty("nickname", NullValueHandling = NullValueHandling.Ignore)]
		public string NickName { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the devices.
		/// </summary>
		/// <value>
		/// The devices.
		/// </value>
		[JsonProperty("devices", NullValueHandling = NullValueHandling.Ignore)]
		public List<DeviceData> Devices { get; set; }
		#endregion

		#region Public Method
		/// <summary>
		/// Shoulds the serialize session token.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeSessionToken()
		{
			return SessionToken != null && SessionToken.Length > 0;
		}

		/// <summary>
		/// Shoulds the serialize email.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeEmail()
		{
			return Email != null && Email.Length > 0;
		}

		/// <summary>
		/// Shoulds the name of the serialize nick.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeNickName()
		{
			return NickName != null && NickName.Length > 0;
		}

		/// <summary>
		/// Shoulds the serialize devices.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeDevices()
		{
			return Devices != null && Devices.Count > 0;
		}
		#endregion
	}
}
