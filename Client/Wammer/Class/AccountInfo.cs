//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Waveface.API.V2;
//using System.Diagnostics;

//namespace Waveface
//{
//    public class AccountInfo
//    {
//        #region Var
//        private WService _service;
//        #endregion

//        #region Private Property
//        private WService m_Service
//        {
//            get
//            {
//                return _service ?? (_service = new WService());
//            }
//        }

//        private string m_SessionToken { get; set; }

//        private string m_UserID { get; set; }
//        #endregion


//        #region Constructor
//        /// <summary>
//        /// Initializes a new instance of the <see cref="AccountInfo"/> class.
//        /// </summary>
//        /// <param name="sessionToken">The session token.</param>
//        public AccountInfo(string sessionToken, string userID)
//        {
//            m_SessionToken = sessionToken;
//            m_UserID = userID;
//            Update();
//        }
//        #endregion


//        #region Public Method
//        public void Update()
//        {
//            var response = m_Service.users_get(m_SessionToken , m_UserID);

//            var user = response.user;
//            Since = user.since;

//            var facebook = (from item in response.sns
//                           where item.type.Equals("facebook", StringComparer.CurrentCultureIgnoreCase)
//                           select item).FirstOrDefault();

//            if (facebook != null)
//            {
//                IsFacebookImportEnabled = facebook.enabled;
//                FacebookID = facebook.snsid;
//            }
//            else
//            {
//                IsFacebookImportEnabled = false;
//                FacebookID = string.Empty;
//            }

//            UploadedPhotoCount = response.storages.waveface.usage.image_objects;
//        }
//        #endregion
//    }
//}
