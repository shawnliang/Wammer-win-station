using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using Wammer.Cloud;
using Wammer.PerfMonitor;
using Wammer.Station.APIHandler;
using Wammer.PostUpload;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface";

		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer managementServer;
		private HttpServer functionServer;
		private StationTimer stationTimer;
		private string stationId;
		private string resourceBasePath;
		private BodySyncTaskQueue bodySyncTaskQueue = new BodySyncTaskQueue();
		private BodySyncTaskRunner[] bodySyncRunners;
		private PostUploadTaskRunner postUploadRunner = new PostUploadTaskRunner(PostUploadTaskQueue.Instance);


		public StationService()
		{			
			log4net.Config.XmlConfigurator.Configure();
			InitializeComponent();
			this.ServiceName = SERVICE_NAME;
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
			try
			{
				logger.Info("============== Starting Waveface Station =================");
				ConfigThreadPool();

				ResetPerformanceCounter();

				AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				Environment.CurrentDirectory = Path.GetDirectoryName(
										Assembly.GetExecutingAssembly().Location);

				logger.Debug("Initialize Waveface Service");
				InitStationId();
				InitResourceBasePath();

				fastJSON.JSON.Instance.UseUTCDateTime = true;

				functionServer = new HttpServer(9981); // TODO: remove hard code


				stationTimer = new StationTimer(functionServer, bodySyncTaskQueue, stationId);

				functionServer.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(functionServer_TaskEnqueue);


				APIHandler.AttachmentUploadHandler attachmentHandler = new APIHandler.AttachmentUploadHandler();
				AttachmentUploadMonitor attachmentMonitor = new AttachmentUploadMonitor();

				attachmentHandler.AttachmentProcessed += 
					new AttachmentUpload.AttachmentProcessedHandler(
						new AttachmentUpload.AttachmentUtility()).OnProcessed;
				attachmentHandler.ProcessSucceeded += attachmentMonitor.OnProcessSucceeded;

				
				BypassHttpHandler cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
				InitCloudForwarder(cloudForwarder);

				AttachmentDownloadMonitor downstreamMonitor = new AttachmentDownloadMonitor();
				bodySyncTaskQueue.Enqueued += downstreamMonitor.OnDownstreamTaskEnqueued;

				InitFunctionServerHandlers(attachmentHandler,cloudForwarder, downstreamMonitor);
				

				logger.Debug("Start function server");
				functionServer.Start();
				stationTimer.Start();

				int bodySyncThreadNum = 1;
				bodySyncRunners = new BodySyncTaskRunner[bodySyncThreadNum];
				for (int i = 0; i < bodySyncThreadNum; i++)
				{
					var bodySyncRunner = new BodySyncTaskRunner(bodySyncTaskQueue);
					bodySyncRunners[i] = bodySyncRunner;

					bodySyncRunner.TaskExecuted += downstreamMonitor.OnDownstreamTaskDone;
					bodySyncRunner.Start();
				}

				postUploadRunner.Start();

				logger.Debug("Add handlers to management server");
				managementServer = new HttpServer(9989);
				managementServer.TaskEnqueue += managementServer_TaskEnqueue;

				AddDriverHandler addDriverHandler = new AddDriverHandler(stationId, resourceBasePath);
				InitManagementServerHandler(addDriverHandler);

				addDriverHandler.DriverAdded += new EventHandler<DriverAddedEvtArgs>(addDriverHandler_DriverAdded);
				addDriverHandler.BeforeDriverSaved += new EventHandler<BeforeDriverSavedEvtArgs>(addDriverHandler_BeforeDriverSaved);
				logger.Debug("Start management server");
				managementServer.Start();




				logger.Info("Waveface station is started");
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
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/users/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/groups/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/stations/"));
		}

		private void InitManagementServerHandler(AddDriverHandler addDriverHandler)
		{
			managementServer.AddHandler(GetDefaultBathPath("/station/resumeSync/"), new ResumeSyncHandler(postUploadRunner, stationTimer, bodySyncRunners));
			managementServer.AddHandler(GetDefaultBathPath("/station/suspendSync/"), new SuspendSyncHandler(postUploadRunner, stationTimer, bodySyncRunners));
			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/add/"), addDriverHandler);
			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/list/"), new ListDriverHandler());
			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/remove/"), new RemoveOwnerHandler(stationId));
			managementServer.AddHandler(GetDefaultBathPath("/station/status/get/"), new StatusGetHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/list"), new ListCloudStorageHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/oauth/"), new DropBoxOAuthHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/connect/"), new DropBoxConnectHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/update/"), new DropBoxUpdateHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/disconnect/"), new DropboxDisconnectHandler());
			managementServer.AddHandler(GetDefaultBathPath("/availability/ping/"), new PingHandler());
		}

		private void InitFunctionServerHandlers(APIHandler.AttachmentUploadHandler attachmentHandler, BypassHttpHandler cloudForwarder, AttachmentDownloadMonitor downstreamMonitor)
		{
			logger.Debug("Add cloud forwarders to function server");
			functionServer.AddDefaultHandler(cloudForwarder);

			logger.Debug("Add handlers to function server");

			functionServer.AddHandler("/", new DummyHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/upload/"), 
				attachmentHandler);

			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/get/"),
				new ResouceDirGetHandler(resourceBasePath));

			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/set/"),
				new ResouceDirSetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/get/"),
				new AttachmentGetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/availability/ping/"),
				new PingHandler());

			functionServer.AddHandler(GetDefaultBathPath("/reachability/ping/"),
				new PingHandler());

			functionServer.AddHandler(GetDefaultBathPath("/posts/getLatest/"),
				new HybridCloudHttpRouter(new PostGetLatestHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/get/"),
				new HybridCloudHttpRouter(new PostGetHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/getSingle/"),
				new HybridCloudHttpRouter(new PostGetSingleHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/new/"),
				new NewPostHandler(PostUploadTaskController.Instance));

			functionServer.AddHandler(GetDefaultBathPath("/posts/update/"),
				new UpdatePostHandler(PostUploadTaskController.Instance));

			functionServer.AddHandler(GetDefaultBathPath("/posts/hide/"),
				new HidePostHandler(PostUploadTaskController.Instance));

			functionServer.AddHandler(GetDefaultBathPath("/posts/newComment/"),
			new NewPostCommentHandler(PostUploadTaskController.Instance));

			functionServer.AddHandler(GetDefaultBathPath("/footprints/setLastScan/"),
				new HybridCloudHttpRouter(new FootprintSetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/footprints/getLastScan/"),
				new HybridCloudHttpRouter(new FootprintGetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/usertracks/get/"),
				new HybridCloudHttpRouter(new APIHandler.UserTrackHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/auth/login/"),
				new UserLoginHandler());

			functionServer.AddHandler(GetDefaultBathPath("/auth/logout/"), 
				new UserLogoutHandler());			

			AttachmentViewHandler viewHandler = new AttachmentViewHandler(stationId);
			functionServer.AddHandler(GetDefaultBathPath("/attachments/view/"),
							viewHandler);
			
			viewHandler.FileDownloadStarted += downstreamMonitor.OnDownstreamTaskEnqueued;
			viewHandler.FileDownloadInProgress += downstreamMonitor.OnDownstreamTaskInProgress;
			viewHandler.FileDownloadFinished += downstreamMonitor.OnDownstreamTaskDone;
		}

		private static string GetDefaultBathPath(string relativedPath)
		{
			return "/" + CloudServer.DEF_BASE_PATH + relativedPath;
		}

		void functionServer_TaskEnqueue(object sender, TaskQueueEventArgs e)
		{
			HttpRequestMonitor.Instance.Enqueue();
		}

		void managementServer_TaskEnqueue(object sender, TaskQueueEventArgs e)
		{
			HttpRequestMonitor.Instance.Enqueue();
		}

		#region Private Method
		private void ResetPerformanceCounter()
		{
			PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, true);
			PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, true);
			PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, true);
			PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, true);
		} 
		#endregion

		void addDriverHandler_DriverAdded(object sender, DriverAddedEvtArgs e)
		{
			PublicPortMapping.Instance.DriverAdded(sender, e);

		}

		void addDriverHandler_BeforeDriverSaved(object sender, BeforeDriverSavedEvtArgs e)
		{
			Timeline.TimelineSyncer syncer = new Timeline.TimelineSyncer(
				new Timeline.PostProvider(),
				new Timeline.TimelineSyncerDBWithDriverCached(e.Driver),
				new UserTracksApi()
			);

			ResourceDownloader downloader = new ResourceDownloader(bodySyncTaskQueue, stationId);
			syncer.PostsRetrieved += new EventHandler<Timeline.TimelineSyncEventArgs>(downloader.PostRetrieved);
			syncer.PullTimeline(e.Driver);
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
			int bodySyncThreadNum = 1;

			for (int i = 0; i < bodySyncThreadNum; i++)
			{
				bodySyncRunners[i].Stop();
			}

			postUploadRunner.Stop();

			functionServer.Stop();
			functionServer.Close();

			stationTimer.Stop();
			stationTimer.Close();

			managementServer.Stop();
			managementServer.Close();

			PublicPortMapping.Instance.Close();
		}

		private void InitResourceBasePath()
		{
			resourceBasePath = (string)StationRegistry.GetValue("resourceBasePath", null);
			if (resourceBasePath == null)
			{
				resourceBasePath = "resource";
				if (!Directory.Exists(resourceBasePath))
					Directory.CreateDirectory(resourceBasePath);
			}
		}

		private void InitStationId()
		{
			stationId = (string)StationRegistry.GetValue("stationId", null);

			if (stationId == null)
			{
				stationId = Guid.NewGuid().ToString();
				StationRegistry.SetValue("stationId", stationId);
			}
		}

		private void ConfigThreadPool()
		{
			int minWorker,minIO;

			System.Threading.ThreadPool.GetMinThreads(out minWorker, out minIO);

			minWorker = int.Parse(StationRegistry.GetValue("MinWorkerThreads", minWorker.ToString()).ToString());
			minIO = int.Parse(StationRegistry.GetValue("MinIOThreads", minIO.ToString()).ToString());

			if (minWorker > 0 && minIO > 0)
			{
				System.Threading.ThreadPool.SetMinThreads(minWorker, minIO);
				logger.InfoFormat("Min worker threads {0}, min IO completion threads {1}",
					minWorker.ToString(), minIO.ToString());
			}

			int maxConcurrentTaskCount = int.Parse(StationRegistry.GetValue("MaxConcurrentTaskCount", "6").ToString());
			if (maxConcurrentTaskCount > 0)
				TaskQueue.MaxConcurrentTaskCount = maxConcurrentTaskCount;
		}

	}


	class DummyHandler : IHttpHandler
	{
		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			//Debug.Fail("should not reach this code");
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public void SetBeginTimestamp(long beginTime)
		{
		}
	}
}
