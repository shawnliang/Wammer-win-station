using AutoMapper;
using fastJSON;
using log4net;
using log4net.Config;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station.APIHandler;
using Wammer.Station.AttachmentUpload;
using Wammer.Station.Timeline;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		#region Const
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface";
		#endregion

		private static readonly ILog logger = LogManager.GetLogger("StationService");
		private HttpServer functionServer;
		private HttpServer managementServer;
		private AttachmentViewHandler viewHandler;
		private PingHandler funcPingHandler = new PingHandler();
		private PostUpload.MobileDevicePostActivity mobileDevicePostActivity = new PostUpload.MobileDevicePostActivity();
		private MongoDBMonitor mongoMonitor;

		public StationService()
		{
			XmlConfigurator.Configure();
			InitializeComponent();
			ServiceName = SERVICE_NAME;
			HttpWebRequest.DefaultMaximumErrorResponseLength = 10 * 1024; // in KB => 10 * 1024 * K => 10 MB
			ServicePointManager.DefaultConnectionLimit = 200;

			AutoMapperSetting.IniteMap();
		}

		~StationService()
		{
			if (managementServer != null)
				managementServer.Dispose();

			if (functionServer != null)
				functionServer.Dispose();
		}

		public void Run()
		{
			OnStart(null);

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<NullTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<MakeThumbnailAndUpstreamTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<MakeThumbnailTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UploadMetadataTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UpstreamTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<CreatePhotoFolderCollectionTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<ResourceDownloadTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UpdateDriverDBTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<Wammer.Station.Doc.UpdateDocAccessTimeTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<NotifyCloudOfBodySyncedTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<NotifyCloudOfMultiBodySyncedTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<QueryIfDownstreamNeededTask>();

			mongoMonitor = new MongoDBMonitor(RunStation);
		}

		private void RunStation()
		{
			try
			{
				logger.Warn("============== Starting Stream Station =================");

				while (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319))
				{
					System.Threading.Thread.Sleep(1000);
					logger.Info("Waiting mongo db...");
				}

				InitResourceFolder();
				TaskQueue.Init();

				// retry queue
				Retry.RetryQueueHelper.Instance.LoadSavedTasks(item =>
				{
					UploadDownloadMonitor.Instance.OnTaskEnqueued(Retry.RetryQueueHelper.Instance, new TaskEventArgs(item.Task));
				});
				Retry.RetryQueueHelper.Instance.TaskEnqueued += UploadDownloadMonitor.Instance.OnTaskEnqueued;
				Retry.RetryQueueHelper.Instance.TaskDequeued += UploadDownloadMonitor.Instance.OnTaskDequeued;

				// upload queue
				AttachmentUploadQueueHelper.Instance.TaskEnqueued += UploadDownloadMonitor.Instance.OnTaskEnqueued;
				AttachmentUploadQueueHelper.Instance.TaskDequeued += UploadDownloadMonitor.Instance.OnTaskDequeued;
				AttachmentUploadQueueHelper.Instance.Init(); // TaskEnqueued and TaskDequeued event handler should be set in prior to Init() is called
				// so that performance counter value will be correct.

				// download queue
				BodySyncQueue.Instance.TaskEnqueued += UploadDownloadMonitor.Instance.OnTaskEnqueued;
				BodySyncQueue.Instance.TaskDequeued += UploadDownloadMonitor.Instance.OnTaskDequeued;

				ConfigThreadPool();


				AppDomain.CurrentDomain.UnhandledException +=
					CurrentDomain_UnhandledException;

				Environment.CurrentDirectory = Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location);


				logger.Info("Initialize Stream Service");

				Station.Instance.Start();

				JSON.Instance.UseUTCDateTime = true;


				Station.Instance.UserLogined += loginHandler_UserLogined;

				functionServer = new HttpServer(9981); // TODO: remove hard code

				functionServer.TaskEnqueue += HttpRequestMonitor.Instance.OnTaskEnqueue;


				var attachmentHandler = new AttachmentUploadHandler();
				var localUserTrackMgr = new Wammer.Station.LocalUserTrack.LocalUserTrackManager();

				attachmentHandler.AttachmentProcessed += new AttachmentProcessedHandler(new AttachmentUtility()).OnProcessed;
				attachmentHandler.AttachmentProcessed += localUserTrackMgr.OnAttachmentReceived;
				attachmentHandler.ProcessSucceeded += UploadDownloadMonitor.Instance.OnAttachmentProcessed;

				MakeThumbnailTask.ThumbnailGenerated += localUserTrackMgr.OnThumbnailGenerated;
				UpstreamTask.AttachmentUpstreamed += localUserTrackMgr.OnAttachmentUpstreamed;

				var cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl, Station.Instance.StationID);
				InitCloudForwarder(cloudForwarder);
				InitFunctionServerHandlers(attachmentHandler, cloudForwarder, localUserTrackMgr);


				logger.Info("Start function server");
				functionServer.Start();

				logger.Info("Add handlers to management server");
				managementServer = new HttpServer(9989);
				managementServer.TaskEnqueue += HttpRequestMonitor.Instance.OnTaskEnqueue;

				InitManagementServerHandler();

				logger.Info("Start management server");
				managementServer.Start();


				(new Task(() =>
				{
					while (true)
					{
						foreach (var user in DriverCollection.Instance.FindAll())
						{
							try
							{
								if (string.IsNullOrEmpty(user.session_token))
									continue;
								var collectionsResponse = CollectionApi.GetAllCollections(user.session_token, CloudServer.APIKey);


								foreach (var responseCollection in collectionsResponse.collections)
								{
									var collection = CollectionCollection.Instance.FindOne(Query.EQ("_id", responseCollection.collection_id));

									var needSaveToDB = ((collection == null) || (!responseCollection.modify_time.Equals(collection.modify_time, StringComparison.CurrentCultureIgnoreCase)));

									if (needSaveToDB)
									{
										collection = Mapper.Map<Cloud.Collection, Model.Collection>(responseCollection);
										CollectionCollection.Instance.Save(collection);
									}
								}
							}
							catch
							{
							}
						}

						Thread.Sleep(10000);
						Application.DoEvents();
					}
				})).Start();

				logger.Warn("Stream station is started");
			}
			catch (Exception ex)
			{
				logger.Error("Unknown exception", ex);
				throw;
			}
		}

		private static void InitCloudForwarder(BypassHttpHandler cloudForwarder)
		{
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/auth/"));
			//cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/users/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/groups/"));

			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/stations/"));
		}

		private void InitManagementServerHandler()
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var type in assembly.GetTypes())
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(HttpHandler)))
				{
					var defaultConstructor = type.GetConstructor(Type.EmptyTypes);

					if (defaultConstructor == null)
						continue;

					var handler = Activator.CreateInstance(type) as IHttpHandler;
					var info = handler.GetCustomAttribute<APIHandlerInfoAttribute>();

					if (info == null || (info.Type & APIHandlerType.ManagementAPI) != APIHandlerType.ManagementAPI)
						continue;

					managementServer.AddHandler(GetDefaultBathPath(info.Path), handler);
				}
			}

			var addDriverHandler = managementServer.m_Handlers[GetDefaultBathPath("/station/drivers/add/")] as AddDriverHandler;
			addDriverHandler.DriverAdded += addDriverHandler_DriverAdded;
			addDriverHandler.BeforeDriverSaved += addDriverHandler_BeforeDriverSaved;

			(managementServer.m_Handlers[GetDefaultBathPath("/station/drivers/remove/")] as RemoveOwnerHandler).DriverRemoved += removeOwnerHandler_DriverRemoved;
			(managementServer.m_Handlers[GetDefaultBathPath("/station/suspendSync/")] as SuspendSyncHandler).SyncSuspended += viewHandler.OnSyncSuspended;
			(managementServer.m_Handlers[GetDefaultBathPath("/station/suspendSync/")] as SuspendSyncHandler).SyncSuspended += funcPingHandler.OnSyncSuspended;
			(managementServer.m_Handlers[GetDefaultBathPath("/station/resumeSync/")] as ResumeSyncHandler).SyncResumed += viewHandler.OnSyncResumed;
			(managementServer.m_Handlers[GetDefaultBathPath("/station/resumeSync/")] as ResumeSyncHandler).SyncResumed += funcPingHandler.OnSyncResumed;
		}

		void removeOwnerHandler_DriverRemoved(object sender, DriverRemovedEventArgs e)
		{
			try
			{
				BodySyncQueue.Instance.RemoveAllByUserId(e.UserId);
			}
			catch (Exception ex)
			{
				logger.Warn("Unable to remove body sync tasks of removed user " + e.UserId, ex);
			}
		}

		private void InitFunctionServerHandlers(AttachmentUploadHandler attachmentHandler, BypassHttpHandler cloudForwarder, LocalUserTrack.LocalUserTrackManager localUserTrackMgr)
		{
			logger.Info("Add cloud forwarders to function server");
			functionServer.AddDefaultHandler(cloudForwarder);

			logger.Info("Add handlers to function server");

			functionServer.AddHandler("/", new DummyHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/upload/"),
									  attachmentHandler);

			// TO BE REMOVED
			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/get/"),
									  new ResouceDirGetHandler(""));

			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/set/"),
									  new ResouceDirSetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/get/"),
									  new AttachmentGetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/availability/ping/"),
									  funcPingHandler);

			functionServer.AddHandler(GetDefaultBathPath("/reachability/ping/"),
									  funcPingHandler);

			functionServer.AddHandler(GetDefaultBathPath("/posts/getLatest/"),
									  new HybridCloudHttpRouter(new PostGetLatestHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/get/"),
									  new HybridCloudHttpRouter(new PostGetHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/getSingle/"),
									  new HybridCloudHttpRouter(new PostGetSingleHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/fetchByFilter/"),
									  new HybridCloudHttpRouter(new PostFetchByFilterHandler()));

			var newPostHandler = new HybridCloudHttpRouter(new NewPostHandler(PostUploadTaskController.Instance));
			newPostHandler.RequestBypassed += mobileDevicePostActivity.OnPostCreated;
			newPostHandler.RequestBypassed += Station.Instance.PostUpsertNotifier.OnPostRequestBypassed;
			functionServer.AddHandler(GetDefaultBathPath("/posts/new/"), newPostHandler);


			var updatePostHandler = new HybridCloudHttpRouter((new UpdatePostHandler(PostUploadTaskController.Instance)));
			updatePostHandler.RequestBypassed += mobileDevicePostActivity.OnPostUpdated;
			updatePostHandler.RequestBypassed += Station.Instance.PostUpsertNotifier.OnPostRequestBypassed;
			functionServer.AddHandler(GetDefaultBathPath("/posts/update/"), updatePostHandler);

			var hidePostHandler = new HybridCloudHttpRouter((new HidePostHandler(PostUploadTaskController.Instance)));
			hidePostHandler.RequestBypassed += mobileDevicePostActivity.OnPostHidden;
			hidePostHandler.RequestBypassed += Station.Instance.PostUpsertNotifier.OnPostRequestBypassed;
			functionServer.AddHandler(GetDefaultBathPath("/posts/hide/"), hidePostHandler);


			var commentHandler = new HybridCloudHttpRouter((new NewPostCommentHandler(PostUploadTaskController.Instance)));
			commentHandler.RequestBypassed += mobileDevicePostActivity.OnCommentCreated;
			commentHandler.RequestBypassed += Station.Instance.PostUpsertNotifier.OnPostRequestBypassed;
			functionServer.AddHandler(GetDefaultBathPath("/posts/newComment/"), commentHandler);


			functionServer.AddHandler(GetDefaultBathPath("/footprints/setLastScan/"),
									  new HybridCloudHttpRouter(new FootprintSetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/footprints/getLastScan/"),
									  new HybridCloudHttpRouter(new FootprintGetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/changelogs/get/"), new UserTrackHandler(localUserTrackMgr));

			var loginHandler = new UserLoginHandler();
			functionServer.AddHandler(GetDefaultBathPath("/auth/login/"),
									  loginHandler);

			functionServer.AddHandler(GetDefaultBathPath("/auth/logout/"),
									  new UserLogoutHandler());

			viewHandler = new AttachmentViewHandler(Station.Instance.StationID);
			functionServer.AddHandler(GetDefaultBathPath("/attachments/view/"), viewHandler);
		}

		private void loginHandler_UserLogined(object sender, UserLoginEventArgs e)
		{
			TaskQueue.Enqueue(new UpdateDriverDBTask(e, Station.Instance.StationID), TaskPriority.High);
		}

		private static string GetDefaultBathPath(string relativedPath)
		{
			return "/" + CloudServer.DEF_BASE_PATH + relativedPath;
		}

		private void addDriverHandler_DriverAdded(object sender, DriverAddedEvtArgs e)
		{
			var posts = e.UserData as List<PostInfo>;

			var downloader = new ResourceDownloader(BodySyncQueue.Instance);
			downloader.PostRetrieved(this, new TimelineSyncEventArgs(e.Driver, posts));
		}

		private void addDriverHandler_BeforeDriverSaved(object sender, BeforeDriverSavedEvtArgs e)
		{
			var syncer = new TimelineSyncer(
				new PostProvider(),
				new TimelineSyncerDBWithDriverCached(e.Driver),
				new ChangeLogsApi()
				);

			List<PostInfo> posts = new List<PostInfo>();
			syncer.PostsRetrieved += (evtSender, evtArg) => posts.AddRange(evtArg.Posts);
			syncer.PullBackward(e.Driver, 20);

			e.UserData = posts;
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.IsTerminating)
			{
				logger.Fatal("Unhandled exception. Program terminates");
				logger.Fatal(e.ExceptionObject);
			}
			else
			{
				logger.Fatal("Unhandled exception: " + e.ExceptionObject);
			}
		}

		protected override void OnStop()
		{
			Station.Instance.Stop();

			functionServer.Stop();
			functionServer.Close();

			managementServer.Stop();
			managementServer.Close();
		}

		private void InitResourceFolder()
		{
			if (!Directory.Exists(FileStorage.ResourceFolder))
				Directory.CreateDirectory(FileStorage.ResourceFolder);
		}

		private void ConfigThreadPool()
		{
			int minWorker, minIO;

			ThreadPool.GetMinThreads(out minWorker, out minIO);

			minWorker = int.Parse(StationRegistry.GetValue("MinWorkerThreads", minWorker.ToString()).ToString());
			minIO = int.Parse(StationRegistry.GetValue("MinIOThreads", minIO.ToString()).ToString());

			if (minWorker > 0 && minIO > 0)
			{
				ThreadPool.SetMinThreads(minWorker, minIO);
				logger.WarnFormat("Min worker threads {0}, min IO completion threads {1}",
								  minWorker.ToString(), minIO.ToString());
			}

			int maxConcurrentTaskCount = int.Parse(StationRegistry.GetValue("MaxConcurrentTaskCount", "20").ToString());
			if (maxConcurrentTaskCount > 0)
				TaskQueue.MaxConcurrentTaskCount = maxConcurrentTaskCount;
		}
	}

	internal class DummyHandler : IHttpHandler
	{
		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void SetBeginTimestamp(long beginTime)
		{
		}

		public void HandleRequest()
		{
		}
	}

	internal class MongoDBMonitor
	{
		private object cs = new object();
		private System.Threading.Timer timer;
		private bool isReady;
		private Action onReady;

		public MongoDBMonitor(Action onReady)
		{
			timer = new System.Threading.Timer(Do, this, 1000, 2000);
			this.onReady = onReady;
		}

		private void Do(object state)
		{
			lock (cs)
			{
				if (isReady)
					return;

				if (Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319))
				{
					timer.Change(Timeout.Infinite, Timeout.Infinite);
					isReady = true;
					onReady();
				}
				else
					this.LogInfoMsg("MongoDB is not ready...");
			}
		}
	}
}
