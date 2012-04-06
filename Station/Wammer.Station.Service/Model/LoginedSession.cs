using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Wammer.Model
{
	/// <summary>
	/// 
	/// </summary>
	[BsonIgnoreExtraElements]
	public class LoginedSession
	{
		#region Var
		private string _sessionToken;
		private string _deviceID;
		private string _apiKey;
		private string _apiKeyName;
		private JObject _jObject;
		#endregion

		#region Private Property
		private JObject m_JObject
		{
			get
			{
				if (_jObject == null)
					_jObject = JObject.Parse(Json);
				return _jObject;
			}
		}
		#endregion

		#region Public Property
		/// <summary>
		/// Gets or sets the session token.
		/// </summary>
		/// <value>The session token.</value>
		[BsonId]
		public string SessionToken
		{
			get
			{
				if (_sessionToken == null)
					_sessionToken = m_JObject["session_token"].ToString();
				return _sessionToken;
			}
			set
			{
				//Ignore value set...
			}
		}

		/// <summary>
		/// Gets or sets the json.
		/// </summary>
		/// <value>The json.</value>
		[BsonIgnoreIfNull]
		public string Json { get; set; }

		/// <summary>
		/// Gets or sets the device ID.
		/// </summary>
		/// <value>The device ID.</value>
		[BsonIgnore]
		public string DeviceID
		{
			get
			{
				if (_deviceID == null)
					_deviceID = m_JObject["device"]["device_id"].ToString();
				return _deviceID;
			}
		}

		/// <summary>
		/// Gets or sets the API key.
		/// </summary>
		/// <value>The API key.</value>
		[BsonIgnore]
		public string APIKey
		{
			get
			{
				if (_apiKey == null)
					_apiKey = m_JObject["apikey"]["apikey"].ToString();
				return _apiKey;
			}
		}

		/// <summary>
		/// Gets or sets the name of the API key.
		/// </summary>
		/// <value>The name of the API key.</value>
		[BsonIgnore]
		public string APIKeyName
		{
			get
			{
				if (_apiKeyName == null)
					_apiKeyName = m_JObject["apikey"]["name"].ToString();
				return _apiKeyName;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="LoginedSession"/> class.
		/// </summary>
		public LoginedSession()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginedSession"/> class.
		/// </summary>
		/// <param name="json">The json.</param>
		public LoginedSession(string json)
		{
			this.Json = json;
		}
		#endregion
	}
}
