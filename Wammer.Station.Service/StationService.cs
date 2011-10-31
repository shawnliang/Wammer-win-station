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

using Microsoft.Win32;
using Wammer.Station;
using Wammer.Cloud;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;


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

			fastJSON.JSON.Instance.UseUTCDateTime = true;

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
							new ViewObjectHandler(storage));


			MongoDB.Driver.MongoServer mongodb = MongoDB.Driver.MongoServer.Create(
												string.Format("mongodb://localhost:{0}/?safe=true", 
												StationRegistry.GetValue("dbPort", 10319)));

			ObjectUploadHandler attachmentHandler = new ObjectUploadHandler(storage, mongodb);
			ImagePostProcessing imgProc = new ImagePostProcessing(storage);
			attachmentHandler.ImageAttachmentSaved += imgProc.HandleAttachmentSaved;
			attachmentHandler.ImageAttachmentCompleted += imgProc.HandleRequestCompleted;
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
							attachmentHandler);

			server.Start();

			if (!LogOnStation(9981))
			{
				logger.Info("Not connected with Wammer Cloud yet");
				//TODO: start a timer to retry
			}
			else
			{
				logger.Info("Station log on Wammer Cloud successfully");
			}
		}

		private bool LogOnStation(int port)
		{
			try
			{
				string stationId = (string)StationRegistry.GetValue("stationId", null);
				string stationToken = (string)StationRegistry.GetValue("stationToken", null);

				if (stationId == null || stationToken == null)
					return false;

				Wammer.Cloud.Station station = new Cloud.Station(stationId, stationToken);
				Dictionary<object, object> parameters = new Dictionary<object, object>();
				parameters.Add("host_name", Dns.GetHostName());
				parameters.Add("ip_address", StationInfo.IPv4Address.ToString());
				parameters.Add("port", port.ToString());
				station.LogOn(new System.Net.WebClient(), parameters);

				CloudServer.SessionToken = station.Token;
				StationRegistry.SetValue("stationToken", station.Token);
				return true;
			}
			catch (Exception e)
			{
				logger.Warn("Unable to logon station with Wammer Cloud", e);
				return false;
			}
		}

		protected override void OnStop()
		{
			server.Stop();
			server.Close();
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
			return new DummyHandler();
		}
	}
}
