using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Waveface.Stream.Model;

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

		private const string STREAM_RELATIVED_FOLDER = @"Waveface\Stream\";
		private const string DATA_RELATIVED_FOLDER = STREAM_RELATIVED_FOLDER + @"Data\";
		private const string STREAM_DATX_FILE_NAME = @"Stream.datx";

		private const string RELATIVED_LOGINED_SESSION_XML_FILE = @"LoginedSession.xml";
		#endregion


		#region Static Var
		private static StreamClient _instance;
		#endregion


		#region Var
		private WebClientControlServer _server;
		private String _dataPath;
		private String _streamDatxFile;
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

		private string m_DataPath
		{
			get
			{
				return _dataPath ?? (_dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATA_RELATIVED_FOLDER));
			}
		}

		private string m_StreamDatxFile
		{
			get
			{
				return _streamDatxFile ?? (_streamDatxFile = Path.Combine(m_DataPath, STREAM_DATX_FILE_NAME));
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

		/// <summary>
		/// Gets or sets the is logined.
		/// </summary>
		/// <value>
		/// The is logined.
		/// </value>
		public Boolean IsLogined
		{
			get
			{
				return LoginedUser != null;
			}
		}
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
		/// Occurs when [logouting].
		/// </summary>
		public event EventHandler Logouting;

		/// <summary>
		/// Occurs when [logouted].
		/// </summary>
		public event EventHandler Logouted;

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

		/// <summary>
		/// Occurs when [collection added].
		/// </summary>
		public event EventHandler<CollectionsEventArgs> CollectionAdded;

		/// <summary>
		/// Occurs when [collection updated].
		/// </summary>
		public event EventHandler<CollectionsEventArgs> CollectionUpdated;
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
			this.Logouted += StreamClient_Logouted;
			this.PostAdded += StreamClient_PostAdded;
			this.PostUpdated += StreamClient_PostUpdated;
			this.AttachmentDownloaded += StreamClient_AttachmentDownloaded;
			this.CollectionAdded += StreamClient_CollectionAdded;
			this.CollectionUpdated += StreamClient_CollectionUpdated;

			if (File.Exists(m_StreamDatxFile) && Datx.IsFileExist(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE))
			{
				var sessionToken = Datx.Read<String>(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE, GetStreamDatxPassword());
				Login(sessionToken);
			}

			m_Server.Start();

			(new Task(() =>
			{
				long count = 0;
				HashSet<string> postIDs = null;
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

			(new Task(() =>
			{
				long count = 0;
				HashSet<string> collectionIDs = null;
				while (true)
				{
					try
					{
						if (LoginedUser != null)
						{
							var collections = CollectionCollection.Instance.Find(Query.And(Query.EQ("creator_id", LoginedUser.UserID), Query.EQ("hidden", false)));
							var collectionCount = collections.Count();

							if (collectionCount != count)
							{
								var firstInit = (collectionIDs == null || collectionIDs.Count == 0);

								var currentCollectionIDs = new HashSet<string>(collections.Select(collection => collection.collection_id));

								var addedCollectionIDS = firstInit ? currentCollectionIDs : currentCollectionIDs.Except(collectionIDs);

								if (!firstInit && addedCollectionIDS.Count() > 0)
								{
									Trace.WriteLine("Collection added detected...");
									OnCollectionAdded(new CollectionsEventArgs(addedCollectionIDS));
								}

								count = collectionCount;
								collectionIDs = currentCollectionIDs;
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
				HashSet<string> collectionIDs = null;
				while (true)
				{
					try
					{
						if (LoginedUser != null)
						{
							var collections = CollectionCollection.Instance.Find(Query.And(Query.EQ("creator_id", LoginedUser.UserID), Query.EQ("hidden", false)));

							var firstInit = (collectionIDs == null || collectionIDs.Count == 0);

							var currentCollectionIDs = new HashSet<string>(collections.Select(collection => string.Format("{0}§{1}", collection.collection_id, collection.attachment_id_array.Count)));

							var updatedCollectionIDS = firstInit ? currentCollectionIDs : collectionIDs.Except(currentCollectionIDs);

							if (!firstInit && updatedCollectionIDS.Count() > 0)
							{
								Trace.WriteLine("Collection updated detected...");
								OnCollectionUpdated(new CollectionsEventArgs(updatedCollectionIDS));
							}

							collectionIDs = currentCollectionIDs;
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

		private SecureString GetStreamDatxPassword()
		{
			//Waveface Stream 98D3B9C7-A57B-4A01-ADB8-329CD0F7E669

			byte[] buffer = new byte[52];
			buffer[0] = 0x57;
			buffer[1] = 0x61;
			buffer[2] = 0x76;
			buffer[3] = 0x65;
			buffer[4] = 0x66;
			buffer[5] = 0x61;
			buffer[6] = 0x63;
			buffer[7] = 0x65;
			buffer[8] = 0x20;
			buffer[9] = 0x53;
			buffer[10] = 0x74;
			buffer[11] = 0x72;
			buffer[12] = 0x65;
			buffer[13] = 0x61;
			buffer[14] = 0x6d;
			buffer[15] = 0x20;
			buffer[16] = 0x39;
			buffer[17] = 0x38;
			buffer[18] = 0x44;
			buffer[19] = 0x33;
			buffer[20] = 0x42;
			buffer[21] = 0x39;
			buffer[22] = 0x43;
			buffer[23] = 0x37;
			buffer[24] = 0x2d;
			buffer[25] = 0x41;
			buffer[26] = 0x35;
			buffer[27] = 0x37;
			buffer[28] = 0x42;
			buffer[29] = 0x2d;
			buffer[30] = 0x34;
			buffer[31] = 0x41;
			buffer[32] = 0x30;
			buffer[33] = 0x31;
			buffer[34] = 0x2d;
			buffer[35] = 0x41;
			buffer[36] = 0x44;
			buffer[37] = 0x42;
			buffer[38] = 0x38;
			buffer[39] = 0x2d;
			buffer[40] = 0x33;
			buffer[41] = 0x32;
			buffer[42] = 0x39;
			buffer[43] = 0x43;
			buffer[44] = 0x44;
			buffer[45] = 0x30;
			buffer[46] = 0x46;
			buffer[47] = 0x37;
			buffer[48] = 0x45;
			buffer[49] = 0x36;
			buffer[50] = 0x36;
			buffer[51] = 0x39;

			SecureString ret = new SecureString();
			foreach (byte b in buffer)
				ret.AppendChar(Convert.ToChar(b));
			ret.MakeReadOnly();

			return ret;
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
		/// Raises the <see cref="E:Logouting" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnLogouting(EventArgs e)
		{
			this.RaiseEvent(Logouting, e);
		}

		/// <summary>
		/// Raises the <see cref="E:Logouted" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnLogouted(EventArgs e)
		{
			this.RaiseEvent(Logouted, e);
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

		/// <summary>
		/// Raises the <see cref="E:CollectionAdded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CollectionsEventArgs" /> instance containing the event data.</param>
		protected void OnCollectionAdded(CollectionsEventArgs e)
		{
			this.RaiseEvent(CollectionAdded, e);
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionUpdated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CollectionsEventArgs" /> instance containing the event data.</param>
		protected void OnCollectionUpdated(CollectionsEventArgs e)
		{
			this.RaiseEvent(CollectionUpdated, e);
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

		/// <summary>
		/// Logins the specified session token.
		/// </summary>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public string Login(string sessionToken)
		{
			if (LoginedUser != null)
				return null;

			var response = string.Empty;

			try
			{
				OnLogining(EventArgs.Empty);

				var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

				if (loginedUser == null)
					return null;

				var userID = loginedUser.user.user_id;

				response = StationAPI.Login(
					userID,
					sessionToken);

				return response;
			}
			finally
			{
				OnLogined(new LoginedEventArgs(response));
			}
		}

		public void Logout()
		{
			if (LoginedUser == null)
				return;

			try
			{
				OnLogouting(EventArgs.Empty);
				StationAPI.Logout(LoginedUser.SessionToken);
			}
			finally
			{
				OnLogouted(EventArgs.Empty);
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

			Datx.Insert<String>(LoginedUser.SessionToken, m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE, GetStreamDatxPassword());
		}

		void StreamClient_Logouted(object sender, EventArgs e)
		{
			LoginedUser = null;
			Datx.RemoveFile(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE);
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

		void StreamClient_CollectionAdded(object sender, CollectionsEventArgs e)
		{
			if (LoginedUser == null)
				return;

			if (!LoginedUser.SubscribedEvents.ContainsKey(SystemEventType.CollectionAdded))
				return;

			var commandData = LoginedUser.SubscribedEvents[SystemEventType.CollectionAdded];


			var eventParams = commandData.Parameters;
			var ids = e.IDs;

			if (eventParams == null)
				eventParams = new Dictionary<string, object>();

			if (eventParams.ContainsKey("collection_id_array"))
				eventParams.Remove("collection_id_array");

			eventParams.Add("collection_id_array", new JArray(ids.ToArray()));

			if (eventParams.ContainsKey("page_size"))
				eventParams.Remove("page_size");

			eventParams.Add("page_size", ids.Count());

			var response = WebSocketCommandExecuter.Instance.Execute("getCollections", eventParams);

			var responseParams = new Dictionary<String, Object>(response)
            {
                {"event_id", (int)SystemEventType.CollectionAdded}
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


		void StreamClient_CollectionUpdated(object sender, CollectionsEventArgs e)
		{
			if (LoginedUser == null)
				return;

			if (!LoginedUser.SubscribedEvents.ContainsKey(SystemEventType.CollectionAdded))
				return;

			var commandData = LoginedUser.SubscribedEvents[SystemEventType.CollectionAdded];


			var eventParams = commandData.Parameters;
			var ids = e.IDs.Select(id => id.Split('§')[0]);

			if (eventParams == null)
				eventParams = new Dictionary<string, object>();

			if (eventParams.ContainsKey("collection_id_array"))
				eventParams.Remove("collection_id_array");

			eventParams.Add("collection_id_array", new JArray(ids.ToArray()));

			if (eventParams.ContainsKey("page_size"))
				eventParams.Remove("page_size");

			eventParams.Add("page_size", ids.Count());

			var response = WebSocketCommandExecuter.Instance.Execute("getCollections", eventParams);

			var responseParams = new Dictionary<String, Object>(response)
            {
                {"event_id", (int)SystemEventType.CollectionAdded}
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
