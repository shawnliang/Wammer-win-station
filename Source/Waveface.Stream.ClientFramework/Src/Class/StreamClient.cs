using System;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using Waveface.Stream.Model;
using System.Collections.Generic;
using AutoMapper;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
	public class StreamClient
	{
		#region Private Const
		private const string APP_NAME = "Stream";
		private const string STATION_ID_KEY = "stationId";
		#endregion


		#region Static Var
		private static StreamClient _instance;
		#endregion


        #region Var
        private List<LoginedUser> _loginedUsers;
        #endregion


        #region Static Public Property
        /// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StreamClient Instance
		{
			get
			{
				return _instance ?? (_instance = new StreamClient());
			}
		}
		#endregion


        #region Private Property
        /// <summary>
        /// Gets the m_ logined users.
        /// </summary>
        /// <value>
        /// The m_ logined users.
        /// </value>
        private List<LoginedUser> m_LoginedUsers
        {
            get { return _loginedUsers ?? (_loginedUsers = new List<LoginedUser>()); }
        }
        #endregion


        #region Public Property
        /// <summary>
        /// Gets the logined users.
        /// </summary>
        /// <value>
        /// The logined users.
        /// </value>
        public IEnumerable<LoginedUser> LoginedUsers { get { return m_LoginedUsers; } }
		#endregion


        #region Event
        /// <summary>
        /// Occurs when [logining].
        /// </summary>
        public event EventHandler Logining;

        /// <summary>
        /// Occurs when [logined].
        /// </summary>
        public event EventHandler<LoginedEventArgs> Logined;
        #endregion


        #region Constructor
        static StreamClient()
        {
            Mapper.CreateMap<PostInfo, PostData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.post_id))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.event_time));

            Mapper.CreateMap<Attachment, AttachmentData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.object_id))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.event_time.HasValue ? src.event_time.Value.ToUTCISO8601ShortString() : null))
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));

            Mapper.CreateMap<ImageProperty, ImageMetaData>();

            Mapper.CreateMap<ThumbnailInfo, ThumbnailData>()
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));
        } 

        /// <summary>
        /// Prevents a default instance of the <see cref="StreamClient" /> class from being created.
        /// </summary>
		private StreamClient()
		{
            Init();
		}
		#endregion


        #region Private Static Method
        /// <summary>
        /// Gets the attachment file path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="savedFileName">Name of the saved file.</param>
        /// <returns></returns>
        private static string GetAttachmentFilePath(string url, string savedFileName)
        {
            if (string.IsNullOrEmpty(savedFileName))
                return null;

            var loginedSession = LoginedSessionCollection.Instance.FindOne();

            if (loginedSession == null)
                return null;

            var groupID = loginedSession.groups.FirstOrDefault().group_id;

            Driver user = DriverCollection.Instance.FindDriverByGroupId(groupID);
            if (user == null)
                return null;

            var imageMetaType = ImageMeta.None;

            if (url.Contains("small"))
                imageMetaType = ImageMeta.Small;
            else if (url.Contains("medium"))
                imageMetaType = ImageMeta.Medium;
            else if (url.Contains("large"))
                imageMetaType = ImageMeta.Large;
            else
                imageMetaType = ImageMeta.Origin;

            var fileStorage = new FileStorage(user);
            return (new Uri(fileStorage.GetFullFilePath(savedFileName, imageMetaType))).ToString();
        }
        #endregion



        #region Private Method
        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init()
        {
            this.Logined += StreamClient_Logined;
        }
        #endregion


        #region Protected Method
        /// <summary>
        /// Raises the <see cref="E:Logining" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void OnLogining(EventArgs e)
        {
            this.RaiseEvent(Logining, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Logined" /> event.
        /// </summary>
        /// <param name="e">The <see cref="LoginedEventArgs" /> instance containing the event data.</param>
        protected void OnLogined(LoginedEventArgs e)
        {
            this.RaiseEvent<LoginedEventArgs>(Logined, e);
        }
        #endregion


        #region Public Method
        /// <summary>
        /// Logins the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
		public string Login(string email, string password)
		{
            if (m_LoginedUsers.Where(item => item.EMail.Equals(email, StringComparison.InvariantCulture)).Any())
                return string.Empty;

            var response = string.Empty;

            try
            {
                OnLogining(EventArgs.Empty);

                response = StationAPI.Login(
                    email,
                    password,
                    (string)StationRegistry.GetValue(STATION_ID_KEY, string.Empty),
                    Environment.MachineName);

                return response;
            }
            finally
            {
                OnLogined(new LoginedEventArgs(response));
            }
		} 
		#endregion


        #region Event Process
        /// <summary>
        /// Handles the Logined event of the StreamClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void StreamClient_Logined(object sender, LoginedEventArgs e)
        {
            var response = e.Response;

            if (response.Length == 0)
                return;

            m_LoginedUsers.Add(new LoginedUser(response));
        }
        #endregion
    }
}
