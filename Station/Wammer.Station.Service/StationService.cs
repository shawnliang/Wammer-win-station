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
				AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				Environment.CurrentDirectory = Path.GetDirectoryName(
										Assembly.GetExecutingAssembly().Location);

				logger.Debug("Initialize Waveface Service");
				InitStationId();
				InitResourceBasePath();

				fastJSON.JSON.Instance.UseUTCDateTime = true;
				stationTimer = new StationTimer();

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
								new AttachmentViewHandler());

				AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler();
				ImagePostProcessing imgProc = new ImagePostProcessing();
				attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
				attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;

				CloudStorageSync cloudSync = new CloudStorageSync();
				attachmentHandler.AttachmentSaved += cloudSync.HandleAttachmentSaved;

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
								attachmentHandler);

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/status/get/",
								new StatusGetHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/get/",
								new ResouceDirGetHandler(resourceBasePath));

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/set/",
								new ResouceDirSetHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/get/",
								new AttachmentGetHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/oauth/",
								new DropBoxOAuthHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/connect/",
								new DropBoxConnectHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/update/",
								new DropBoxUpdateHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/disconnect/",
								new DropboxDisconnectHandler());

				logger.Debug("Check if need to start function server");
				Model.Service svc = ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
				if (svc != null)
				{
					if (svc.State == ServiceState.Online)
					{
						logger.Debug("Start function server");
						functionServer.Start();
					}
				}

				logger.Debug("Add handlers to management server");
				managementServer = new HttpServer(9989);
				AddDriverHandler addDriverHandler = new AddDriverHandler(stationId, resourceBasePath, functionServer);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/online/", new StationOnlineHandler(functionServer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/offline/", new StationOfflineHandler(functionServer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/add/", addDriverHandler);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/remove/", new RemoveOwnerHandler(stationId, functionServer));
				addDriverHandler.DriverAdded += PublicPortMapping.Instance.DriverAdded;

				logger.Debug("Start management server");
				managementServer.Start();
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
	}
}
