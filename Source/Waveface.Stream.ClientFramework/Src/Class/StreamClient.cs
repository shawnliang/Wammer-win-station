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
using MongoDB.Bson;

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
        public event EventHandler<PostsEventArgs> PostAdded;

        /// <summary>
        /// Occurs when [post updated].
        /// </summary>
        public event EventHandler<PostsEventArgs> PostUpdated;

        /// <summary>
        /// Occurs when [attachment downloaded].
        /// </summary>
        public event EventHandler<AttachmentsEventArgs> AttachmentDownloaded;
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
            this.PostUpdated += StreamClient_PostUpdated;
            this.AttachmentDownloaded += StreamClient_AttachmentDownloaded;

            m_Server.Start();

            (new Task(() =>
            {
                long count = 0;
                HashSet<string> postIDs = null ;
                while (true)
                {
                    try
                    {
                        if (LoginedUser != null)
                        {
                            var posts = PostCollection.Instance.Find(Query.And(Query.EQ("creator_id", LoginedUser.UserID), Query.EQ("code_name", "StreamEvent")));
                            var postCount = posts.Count();

                            if (postCount != count)
                            {
                                var firstInit = (postIDs == null || postIDs.Count == 0);

                                var currentPostIDs = new HashSet<string>(posts.Select(post => post.post_id));

                                var addedPostIDS = firstInit ? currentPostIDs : currentPostIDs.Except(postIDs);

                                if (!firstInit && addedPostIDS.Count() > 0)
                                {
                                    Trace.WriteLine("Post added detected...");
                                    OnPostAdded(new PostsEventArgs(addedPostIDS));
                                }

                                count = postCount;
                                postIDs = currentPostIDs;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(2000);
                    Application.DoEvents();
                }
            })).Start();


            (new Task(() =>
            {
                HashSet<string> postIDs = null;
                while (true)
                {
                    try
                    {
                        if (LoginedUser != null)
                        {
                            var posts = PostCollection.Instance.Find(Query.And(Query.EQ("creator_id", LoginedUser.UserID), Query.EQ("code_name", "StreamEvent")));

                            var firstInit = (postIDs == null || postIDs.Count == 0);

                            var currentPostIDs = new HashSet<string>(posts.Select(post => string.Format("{0}§{1}", post.post_id, post.attachment_id_array.Count)));

                            var updatedPostIDS = firstInit ? currentPostIDs : postIDs.Except(currentPostIDs);

                            if (!firstInit && updatedPostIDS.Count() > 0)
                            {
                                Trace.WriteLine("Post updated detected...");
                                OnPostUpdated(new PostsEventArgs(updatedPostIDS));
                            }

                            postIDs = currentPostIDs;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(2000);
                    Application.DoEvents();
                }
            })).Start();


            (new Task(() =>
            {
                long count = 0;
                HashSet<string> attachmentIDs = null;
                while (true)
                {
                    try
                    {
                        if (LoginedUser != null)
                        {
                            var attachments = AttachmentCollection.Instance.Find(Query.EQ("group_id", LoginedUser.GroupID));
                            var attachmentCount = attachments.Count();

                            if (attachmentCount != count)
                            {
                                var firstInit = (attachmentIDs == null || attachmentIDs.Count == 0);

                                var currentAttachmentIDs = new HashSet<string>(attachments.Select(attachment => attachment.object_id));

                                var addedAttachmentIDS = firstInit ? currentAttachmentIDs : currentAttachmentIDs.Except(attachmentIDs);

                                if (!firstInit && addedAttachmentIDS.Count() > 0)
                                {
                                    Trace.WriteLine("Attachment added detected...");
                                    OnAttachmentDownloaded(new AttachmentsEventArgs(addedAttachmentIDS));
                                }

                                count = attachmentCount;
                                attachmentIDs = currentAttachmentIDs;
                            }
                        }
                    }
                    catch (Exception)
                    {
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
        /// <param name="e">The <see cref="PostsEventArgs" /> instance containing the event data.</param>
        protected void OnPostAdded(PostsEventArgs e)
        {
            this.RaiseEvent(PostAdded, e);
        }

        /// <summary>
        /// Raises the <see cref="E:PostUpdated" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PostsEventArgs" /> instance containing the event data.</param>
        protected void OnPostUpdated(PostsEventArgs e)
        {
            this.RaiseEvent(PostUpdated, e);
        }

        /// <summary>
        /// Raises the <see cref="E:AttachmentDownloaded" /> event.
        /// </summary>
        /// <param name="e">The <see cref="AttachmentsEventArgs" /> instance containing the event data.</param>
        protected void OnAttachmentDownloaded(AttachmentsEventArgs e)
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
        /// <param name="e">The <see cref="AttachmentsEventArgs" /> instance containing the event data.</param>
        void StreamClient_AttachmentDownloaded(object sender, AttachmentsEventArgs e)
        {
            if (LoginedUser == null)
                return;

            if (!LoginedUser.SubscribedEvents.ContainsKey(SystemEventType.AttachmentDownloaded))
                return;

            var commandData = LoginedUser.SubscribedEvents[SystemEventType.AttachmentDownloaded];


            var eventParams = commandData.Parameters;
            var ids = e.IDs;

            if (eventParams == null)
                eventParams = new Dictionary<string, object>();

            if (eventParams.ContainsKey("attachment_id_array"))
                eventParams.Remove("attachment_id_array");

            eventParams.Add("attachment_id_array", new JArray(ids.ToArray()));

            if (eventParams.ContainsKey("page_size"))
                eventParams.Remove("page_size");

            eventParams.Add("page_size", ids.Count());

            var response = WebSocketCommandExecuter.Instance.Execute("getAttachments", eventParams);

            var responseParams = new Dictionary<String, Object>(response)
            {
                {"event_id", (int)SystemEventType.AttachmentDownloaded}
            };

            responseParams.Remove("page_no");
            responseParams.Remove("page_size");
            responseParams.Remove("page_count");
            responseParams.Remove("total_count");


            var executedValue = new JObject(
                    new JProperty("command", "subscribeEvent"),
                    new JProperty("response", JObject.FromObject(responseParams))
                    );

            var memo = commandData.Memo;
            if (memo != null)
                executedValue.Add(new JProperty("memo", memo));

            var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

            m_Server.Send(LoginedUser.WebSocketChannelID, responseMessage);
        }

        /// <summary>
        /// Handles the PostAdded event of the StreamClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PostsEventArgs" /> instance containing the event data.</param>
        void StreamClient_PostAdded(object sender, PostsEventArgs e)
        {
            if (LoginedUser == null)
                return;

            if (!LoginedUser.SubscribedEvents.ContainsKey(SystemEventType.PostAdded))
                return;

            var commandData = LoginedUser.SubscribedEvents[SystemEventType.PostAdded];


            var eventParams = commandData.Parameters;
            var ids = e.IDs;

            if (eventParams == null)
                eventParams = new Dictionary<string, object>();

            if (eventParams.ContainsKey("post_id_array"))
                eventParams.Remove("post_id_array");

            eventParams.Add("post_id_array", new JArray(ids.ToArray()));

            if (eventParams.ContainsKey("page_size"))
                eventParams.Remove("page_size");

            eventParams.Add("page_size", ids.Count());

            var response = WebSocketCommandExecuter.Instance.Execute("getPosts", eventParams);

            var responseParams = new Dictionary<String, Object>(response)
            {
                {"event_id", (int)SystemEventType.PostAdded}
            };

            responseParams.Remove("page_no");
            responseParams.Remove("page_size");
            responseParams.Remove("page_count");
            responseParams.Remove("total_count");


            var executedValue = new JObject(
                    new JProperty("command", "subscribeEvent"),
                    new JProperty("response", JObject.FromObject(responseParams))
                    );

            var memo = commandData.Memo;
            if (memo != null)
                executedValue.Add(new JProperty("memo", memo));

            var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

            m_Server.Send(LoginedUser.WebSocketChannelID, responseMessage);
        }

        void StreamClient_PostUpdated(object sender, PostsEventArgs e)
        {
            if (LoginedUser == null)
                return;

            if (!LoginedUser.SubscribedEvents.ContainsKey(SystemEventType.PostUpdated))
                return;

            var commandData = LoginedUser.SubscribedEvents[SystemEventType.PostUpdated];


            var eventParams = commandData.Parameters;

            var ids = e.IDs.Select(id => id.Split('§')[0]);

            if (eventParams == null)
                eventParams = new Dictionary<string, object>();

            if (eventParams.ContainsKey("post_id_array"))
                eventParams.Remove("post_id_array");

            eventParams.Add("post_id_array", new JArray(ids.ToArray()));

            if (eventParams.ContainsKey("page_size"))
                eventParams.Remove("page_size");

            eventParams.Add("page_size", ids.Count());

            var response = WebSocketCommandExecuter.Instance.Execute("getPosts", eventParams);

            var responseParams = new Dictionary<String, Object>(response)
            {
                {"event_id", (int)SystemEventType.PostUpdated}
            };

            responseParams.Remove("page_no");
            responseParams.Remove("page_size");
            responseParams.Remove("page_count");
            responseParams.Remove("total_count");


            var executedValue = new JObject(
                    new JProperty("command", "subscribeEvent"),
                    new JProperty("response", JObject.FromObject(responseParams))
                    );

            var memo = commandData.Memo;
            if (memo != null)
                executedValue.Add(new JProperty("memo", memo));

            var responseMessage = JsonConvert.SerializeObject(executedValue, Formatting.Indented);

            m_Server.Send(LoginedUser.WebSocketChannelID, responseMessage);
        }
        #endregion
    }
}
