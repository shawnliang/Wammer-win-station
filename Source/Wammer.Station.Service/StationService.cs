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
using Waveface.Stream.Core;
using Waveface.Stream.Model;

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
		private MongoDBMonitor mongoMonitor;

		public StationService()
		{
			XmlConfigurator.Configure();
			InitializeComponent();
			ServiceName = SERVICE_NAME;
			HttpWebRequest.DefaultMaximumErrorResponseLength = 10 * 1024; // in KB => 10 * 1024 * K => 10 MB
			ServicePointManager.DefaultConnectionLimit = 200;

			Waveface.Stream.Model.AutoMapperSetting.IniteMap();
			AutoMapperSetting.IniteMap();
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
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<MakeThumbnailAndUpstreamTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<NullTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<MakeThumbnailTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UploadMetadataTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UpstreamTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<CreateFolderCollectionTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<ResourceDownloadTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<UpdateDriverDBTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<Wammer.Station.Doc.UpdateDocAccessTimeTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<QueryIfDownstreamNeededTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<DownloadDocPreviewsTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<MakeDocPreviewsTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<DummyResourceDownloadTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<FirstTimelineSyncTask>();
			MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<NullNamedTask>();

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
				BodySyncQueue.Instance.Init();
				ConfigThreadPool();


				AppDomain.CurrentDomain.UnhandledException +=
					CurrentDomain_UnhandledException;

				Environment.CurrentDirectory = Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location);


				logger.Info("Initialize Stream Service");

				Station.Instance.Start();

				JSON.Instance.UseUTCDateTime = true;

				initializeDatabase();


				Station.Instance.UserLogined += loginHandler_UserLogined;

				functionServer = new HttpServer(9981); // TODO: remove hard code

				functionServer.TaskEnqueue += HttpRequestMonitor.Instance.OnTaskEnqueue;


				var attachmentHandler = new AttachmentUploadHandler();

				attachmentHandler.AttachmentProcessed += new AttachmentProcessedHandler(new AttachmentUtility()).OnProcessed;
				attachmentHandler.AttachmentProcessed += (s, e) =>
				{
					SystemEventSubscriber.Instance.TriggerAttachmentArrivedEvent(e.AttachmentId);
				};

				attachmentHandler.ProcessSucceeded += UploadDownloadMonitor.Instance.OnAttachmentProcessed;


				var cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl, Station.Instance.StationID);
				InitCloudForwarder(cloudForwarder);
				InitFunctionServerHandlers(attachmentHandler, cloudForwarder);


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
										collection = Mapper.Map<Cloud.Collection, Waveface.Stream.Model.Collection>(responseCollection);
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

				Waveface.Stream.Core.AutoMapperSetting.IniteMap();

				var eventSubscriber = SystemEventSubscriber.Instance;
				PostDBDataCollection.Instance.Saved += (s, e) => 
				{
					eventSubscriber.TriggerPostAddedEvent(e.ID);
				};

				PostDBDataCollection.Instance.Updated += (s, e) =>
				{
					eventSubscriber.TriggerPostUpdatedEvent(e.ID);
				};

				AttachmentCollection.Instance.Saved += (s, e) =>
				{
					eventSubscriber.TriggerAttachmentAddedEvent(e.ID);
				};

				AttachmentCollection.Instance.Updated += (s, e) =>
				{
					eventSubscriber.TriggerAttachmentUpdatedEvent(e.ID);
				};

				CollectionCollection.Instance.Saved += (s, e) =>
				{
					eventSubscriber.TriggerCollectionAddedEvent(e.ID);
				};

				CollectionCollection.Instance.Updated += (s, e) =>
				{
					eventSubscriber.TriggerCollectionUpdatedEvent(e.ID);
				};

				WebClientControlServer.Instance.Start();
			}
			catch (Exception ex)
			{
				logger.Error("Unknown exception", ex);
				throw;
			}
		}

		private void initializeDatabase()
		{
			TaskStatusCollection.Instance.HideAll();
			TaskStatusCollection.Instance.AbortAllIncompleteTasks();

			Waveface.Stream.Model.ConnectionCollection.Instance.RemoveAll();
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

		private void InitFunctionServerHandlers(AttachmentUploadHandler attachmentHandler, BypassHttpHandler cloudForwarder)
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
			TaskQueue.Enqueue(new FirstTimelineSyncTask { user = e.Driver }, TaskPriority.High, true);
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

	internal class MongoDBMonitor:IDisposable
	{
		#region Var
		private object cs = new object();
		private System.Threading.Timer timer;
		private bool isReady;
		private Action onReady; 
		#endregion

		#region Private Property
		private bool m_Disposed { get; set; }
		#endregion

		public MongoDBMonitor(Action onReady)
		{
			timer = new System.Threading.Timer(Do, this, 1000, 2000);
			this.onReady = onReady;
		}

		~MongoDBMonitor()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (m_Disposed)
				return;

			if (disposing)
			{
				timer.Dispose();
			}

			m_Disposed = true;
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
