using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;

using MongoDB.Driver.Builders;
using Microsoft.Win32;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using TCMPortMapper;

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

				AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				Environment.CurrentDirectory = Path.GetDirectoryName(
										Assembly.GetExecutingAssembly().Location);

				logger.Debug("Initialize Waveface Service");
				InitStationId();
				InitResourceBasePath();

				fastJSON.JSON.Instance.UseUTCDateTime = true;

				functionServer = new HttpServer(9981); // TODO: remove hard code

				logger.Debug("Add cloud forwarders to function server");
				BypassHttpHandler cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
				functionServer.AddDefaultHandler(cloudForwarder);

				logger.Debug("Add handlers to function server");
				functionServer.AddHandler("/", new DummyHandler());
				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
								new AttachmentViewHandler(stationId));

				AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler();
				AttachmentUploadMonitor attachmentMonitor = new AttachmentUploadMonitor();
				ImagePostProcessing imgProc = new ImagePostProcessing();
				imgProc.ThumbnailUpstreamed += attachmentMonitor.OnThumbnailUpstreamed;
				
				attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
				attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
				attachmentHandler.ThumbnailUpstreamed += attachmentMonitor.OnThumbnailUpstreamed;
				

				CloudStorageSync cloudSync = new CloudStorageSync();
				attachmentHandler.AttachmentSaved += cloudSync.HandleAttachmentSaved;
				attachmentHandler.ProcessSucceeded += attachmentMonitor.OnProcessSucceeded;

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
								attachmentHandler);

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/get/",
								new ResouceDirGetHandler(resourceBasePath));

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/set/",
								new ResouceDirSetHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/get/",
								new AttachmentGetHandler());



				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/availability/ping/",
								new PingHandler());

				if (Wammer.Utility.AutoRun.Exists("WavefaceStation"))
				{
					logger.Debug("Start function server");
					functionServer.Start();
				}

				logger.Debug("Add handlers to management server");
				managementServer = new HttpServer(9989);
				AddDriverHandler addDriverHandler = new AddDriverHandler(stationId, resourceBasePath, functionServer);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/online/", new StationOnlineHandler(functionServer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/offline/", new StationOfflineHandler(functionServer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/add/", addDriverHandler);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/remove/", new RemoveOwnerHandler(stationId, functionServer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/status/get/", new StatusGetHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/list", new ListCloudStorageHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/oauth/", new DropBoxOAuthHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/connect/", new DropBoxConnectHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/update/", new DropBoxUpdateHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/disconnect/", new DropboxDisconnectHandler());

				addDriverHandler.DriverAdded += PublicPortMapping.Instance.DriverAdded;
				logger.Debug("Start management server");
				managementServer.Start();

				stationTimer = new StationTimer(functionServer);

				logger.Info("Waveface station is started");
			}
			catch (Exception ex)
			{
				logger.Error("Unknown exception", ex);
				throw;
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.IsTerminating)
			{
				logger.Fatal("Unhandled exception. Program terminates");
				logger.Fatal(e.ToString());
			}
			else
			{
				logger.Fatal("Unhandled exception" + e.ToString());
			}
		}

		protected override void OnStop()
		{
			stationTimer.Stop();

			functionServer.Stop();
			functionServer.Close();

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
			int minWorker;
			int minIO;

			System.Threading.ThreadPool.GetMinThreads(out minWorker, out minIO);

			minWorker = (int)StationRegistry.GetValue("MinWorkerThreads", minWorker);
			minIO = (int)StationRegistry.GetValue("MinIOThreads", minIO);

			if (minWorker > 0 && minIO > 0)
			{
				System.Threading.ThreadPool.SetMinThreads(minWorker, minIO);
				logger.InfoFormat("Min worker threads {0}, min completion port threads {1}",
					minWorker, minIO);
			}



			int maxWorker;
			int maxIO;

			System.Threading.ThreadPool.GetMaxThreads(out maxWorker, out maxIO);
			maxWorker = (int)StationRegistry.GetValue("MaxWorkerThreads", maxWorker);
			maxIO = (int)StationRegistry.GetValue("MaxIOThreads", maxIO);

			if (maxWorker > 0 && maxIO > 0)
			{
				System.Threading.ThreadPool.SetMaxThreads(maxWorker, maxIO);
				logger.InfoFormat("Max worker threads {0}, min completion port threads {1}",
					maxWorker, maxIO);
			}
		}

	}


	class DummyHandler : IHttpHandler
	{
		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			Debug.Fail("should not reach this code");
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
