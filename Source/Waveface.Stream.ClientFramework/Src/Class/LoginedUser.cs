using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginedUser
    {
        #region Var
        private String _response;
        private JObject _jObject;
        private JToken _user;
        private String _email;
        private String _userID;
        private String _sessionToken;
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

        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginedUser" /> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public LoginedUser(string response)
        {
            m_Response = response;
        }
        #endregion
    }
}
