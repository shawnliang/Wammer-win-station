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
using System.ServiceModel;
using System.ServiceModel.Web;

using Microsoft.Win32;
using Wammer.Station;
using Wammer.Cloud;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;
		private List<WebServiceHost> serviceHosts = new List<WebServiceHost>();
		private AtomicDictionary<string, FileStorage> groupFolderMap = 
			new AtomicDictionary<string, FileStorage>();

		public StationService()
		{
			log4net.Config.XmlConfigurator.Configure();
			InitializeComponent();
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
			Environment.CurrentDirectory = Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Location);
			MongoDB.Driver.MongoServer mongodb = MongoDB.Driver.MongoServer.Create(
									string.Format("mongodb://localhost:{0}/?safe=true",
									StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code

			fastJSON.JSON.Instance.UseUTCDateTime = true;
			StationInfo.Init(mongodb);
			LoadGroupFolderMapping(mongodb);


			server = new HttpServer(9981); // TODO: remove hard code
			BypassHttpHandler cloudForwarder = new BypassHttpHandler(
															CloudServer.HostName, CloudServer.Port);
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
			server.AddDefaultHandler(cloudForwarder);

			FileStorage storage = new FileStorage("resource");

			server.AddHandler("/", new DummyHandler());
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
							new AttachmentViewHandler(mongodb, groupFolderMap));

			AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler(mongodb, groupFolderMap);
			ImagePostProcessing imgProc = new ImagePostProcessing(storage);
			attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
			attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
							attachmentHandler);

			server.Start();


			// Start WCF REST services
			AddWebServiceHost(new AttachmentService(mongodb), 9981, "attachments/");
			
			StationManagementService statMgmtSvc = new StationManagementService(mongodb, 
																	StationInfo.Id, groupFolderMap);
			statMgmtSvc.DriverAdded += new EventHandler<DriverEventArgs>(statMgmtSvc_DriverAdded);
			AddWebServiceHost(statMgmtSvc, 9981, "station/");
		}

		void statMgmtSvc_DriverAdded(object sender, DriverEventArgs e)
		{
			try
			{
				if (e.Driver.groups.Count < 1)
					throw new Exception("Driver " + e.Driver.email + " has no associated group");

				groupFolderMap.Add(
					e.Driver.groups[0].group_id, new FileStorage(e.Driver.folder));

				StationInfo.SessionToken = e.StationToken;
				StationInfo.Save();
			}
			catch (Exception ex)
			{
				logger.Warn("Unable to add group folder mapping entry", ex);
			}
		}

		protected override void OnStop()
		{
			foreach (WebServiceHost svc in serviceHosts)
			{
				svc.Close();
			}

			server.Stop();
			server.Close();
		}

		private void AddWebServiceHost(object service, int port, string basePath)
		{
			string url = string.Format("http://{0}:{1}/{2}/{3}", 
				Dns.GetHostName(), port, CloudServer.DEF_BASE_PATH, basePath);

			WebServiceHost svcHost = new WebServiceHost(service, new Uri(url));
			svcHost.Open();
			serviceHosts.Add(svcHost);
		}

		private void LoadGroupFolderMapping(MongoDB.Driver.MongoServer mongodb)
		{
			MongoDB.Driver.MongoCursor<StationDriver> drivers = 
				mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers").FindAll();

			foreach (StationDriver driver in drivers)
			{
				groupFolderMap.Add(driver.groups[0].group_id, new FileStorage(driver.folder));
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
