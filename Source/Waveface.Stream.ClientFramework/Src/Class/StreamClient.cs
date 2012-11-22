using System;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using Waveface.Stream.Model;
using System.Collections.Generic;
using AutoMapper;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

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
        private WebClientControlServer _server;
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
        /// Gets the m_ server.
        /// </summary>
        /// <value>The m_ server.</value>
        private WebClientControlServer m_Server
        {
            get
            {
                return _server ?? (_server = new WebClientControlServer(1337));
            }
        }
        #endregion


        #region Public Property
        /// <summary>
        /// Gets or sets the logined user.
        /// </summary>
        /// <value>
        /// The logined user.
        /// </value>
        public LoginedUser LoginedUser { get; private set; }
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

        /// <summary>
        /// Occurs when [post added].
        /// </summary>
        public event EventHandler PostAdded;

        /// <summary>
        /// Occurs when [attachment downloaded].
        /// </summary>
        public event EventHandler AttachmentDownloaded;
        #endregion


        #region Constructor
        /// <summary>
        /// Prevents a default instance of the <see cref="StreamClient" /> class from being created.
        /// </summary>
		private StreamClient()
		{
            Init();
		}
		#endregion



        #region Private Method
        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init()
        {
            SynchronizationContextHelper.SetMainSyncContext();

            AutoMapperSetting.IniteMap();

            this.Logined += StreamClient_Logined;
            this.PostAdded += StreamClient_PostAdded;
            this.AttachmentDownloaded += StreamClient_AttachmentDownloaded;

            m_Server.Start();

            (new Task(() =>
            {
                long count = 0;
                while (true)
                {
                    if (LoginedUser != null)
                    {
                        var postCount = PostCollection.Instance.Find(Query.EQ("creator_id", LoginedUser.UserID)).Count();

                        if (postCount != count)
                        {
                            OnPostAdded(EventArgs.Empty);

                            Trace.WriteLine("Post chang detected...");
                            count = postCount;
                        }
                    }

                    Thread.Sleep(2000);
                    Application.DoEvents();
                }
            })).Start();

            (new Task(() =>
            {
                long count = 0;
                while (true)
                {
                    if (LoginedUser != null)
                    {
                        var attachmentCount = AttachmentCollection.Instance.Find(Query.EQ("group_id", LoginedUser.GroupID)).Count();

                        if (attachmentCount != count)
                        {
                            OnAttachmentDownloaded(EventArgs.Empty);

                            Trace.WriteLine("Attachment added detected...");
                            count = attachmentCount;
                        }
                    }

                    Thread.Sleep(2000);
                    Application.DoEvents();
                }
            })).Start();
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

        /// <summary>
        /// Raises the <see cref="E:PostAdded" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void OnPostAdded(EventArgs e)
        {
            this.RaiseEvent(PostAdded, e);
        }

        /// <summary>
        /// Raises the <see cref="E:AttachmentDownloaded" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void OnAttachmentDownloaded(EventArgs e)
        {
            this.RaiseEvent(AttachmentDownloaded, e);
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
            if (LoginedUser != null && LoginedUser.EMail.Equals(email))
                return null;

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

            LoginedUser = new LoginedUser(response);
        }

        /// <summary>
        /// Handles the AttachmentDownloaded event of the StreamClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void StreamClient_AttachmentDownloaded(object sender, EventArgs e)
        {
            if (LoginedUser == null)
                return;

            if (LoginedUser.SubscribedSystemEvent == SystemEventType.None || (LoginedUser.SubscribedSystemEvent != SystemEventType.All && !LoginedUser.SubscribedSystemEvent.HasFlag(SystemEventType.AttachmentDownloaded)))
                return;

            var executedValue = new JObject(
                    new JProperty("command", "subscribeEvent"),
                    new JProperty("response", JObject.FromObject(new Dictionary<String, Object>()
                    {
                        {"event_id", (int)SystemEventType.AttachmentDownloaded}
                    }))
                    );

            var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

            m_Server.Send(LoginedUser.WebSocketChannelID, responseMessage);
        }

        /// <summary>
        /// Handles the PostAdded event of the StreamClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void StreamClient_PostAdded(object sender, EventArgs e)
        {
            if (LoginedUser == null)
                return;

            if (LoginedUser.SubscribedSystemEvent == SystemEventType.None || (LoginedUser.SubscribedSystemEvent != SystemEventType.All && !LoginedUser.SubscribedSystemEvent.HasFlag(SystemEventType.PostAdded)))
                return;

            var executedValue = new JObject(
                    new JProperty("command", "subscribeEvent"),
                    new JProperty("response", JObject.FromObject(new Dictionary<String, Object>()
                    {
                        {"event_id", (int)SystemEventType.PostAdded}
                    }))
                    );

            var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

            m_Server.Send(LoginedUser.WebSocketChannelID, responseMessage);
        }
        #endregion
    }
}
