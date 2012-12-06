using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
	public class LoginedUser : IXmlSerializable
    {
        #region Var
        private String _response;
        private JObject _jObject;
        private JToken _user;
        private String _email;
        private String _userID;
        private String _groupID;
        private String _sessionToken;
        private Dictionary<SystemEventType, WebSocketCommandData> _subscribedEvents;
        #endregion 


        #region Private Property
        /// <summary>
        /// Gets or sets the m_ response.
        /// </summary>
        /// <value>
        /// The m_ response.
        /// </value>
        private String m_Response 
        {
            get
            {
                return _response ?? string.Empty;
            }
            set
            {
                _response = value;
            }
        }

        /// <summary>
        /// Gets the m_ J object.
        /// </summary>
        /// <value>
        /// The m_ J object.
        /// </value>
        private JObject m_JObject
        {
            get
            {
                return _jObject ?? (_jObject = JObject.Parse(m_Response));
            }
        }

        /// <summary>
        /// Gets the m_ user.
        /// </summary>
        /// <value>
        /// The m_ user.
        /// </value>
        private JToken m_User
        {
            get 
            {
                return _user ?? (_user = m_JObject["user"]);
            }
        }
        #endregion


        #region Public Property
        /// <summary>
        /// Gets the E mail.
        /// </summary>
        /// <value>
        /// The E mail.
        /// </value>
        public String EMail
        {
            get
            {
                return _email ?? (_email = m_User["email"].ToString());
            }
        }


        /// <summary>
        /// Gets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        public String UserID
        {
            get
            {
                return _userID ?? (_userID = m_User["user_id"].ToString());
            }
        }

        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        /// <value>
        /// The session token.
        /// </value>
        public String SessionToken
        {
            get
            {
                return _sessionToken ?? (_sessionToken = m_JObject["session_token"].ToString());
            }
        }

        /// <summary>
        /// Gets the group ID.
        /// </summary>
        /// <value>
        /// The group ID.
        /// </value>
        public String GroupID
        {
            get
            {
                return _groupID ?? (_groupID = m_JObject["groups"][0]["group_id"].ToString());
            }
        }

        /// <summary>
        /// Gets or sets the web socket channel ID.
        /// </summary>
        /// <value>
        /// The web socket channel ID.
        /// </value>
        public String WebSocketChannelID { get; internal set; }

        /// <summary>
        /// Gets the subscribed system event.
        /// </summary>
        /// <value>
        /// The subscribed system event.
        /// </value>
        public Dictionary<SystemEventType, WebSocketCommandData> SubscribedEvents
        {
            get 
            {
                return _subscribedEvents ?? (_subscribedEvents = new Dictionary<SystemEventType, WebSocketCommandData>());
            }
         }
        #endregion


        #region Constructor
		public LoginedUser()
		{

		}

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginedUser" /> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public LoginedUser(string response)
        {
            m_Response = response;
        }
        #endregion



		#region Implement IXmlSerializable

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			string startElementName = reader.Name;
			string currentElementName;

			this.SubscribedEvents.Clear();
			do
			{
				currentElementName = reader.Name;
				if (currentElementName == startElementName && (reader.MoveToContent() == System.Xml.XmlNodeType.EndElement || reader.IsEmptyElement))
				{
					reader.Read();
					break;
				}

				switch (currentElementName)
				{
					case "Response":
						m_Response = reader.ReadElementString();
						break;

					default:
						reader.Read();
						break;

				}
			} while (true);
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString("Response", m_Response);
			//foreach (var subscribedEvent in SubscribedEvents)
			//{
			//	writer.WriteStartElement("SubscribedEvent");
			//	writer.WriteAttributeString("Key", subscribedEvent.Key);
			//	writer.WriteEndElement();
			//}
		}

		#endregion
	}
}
