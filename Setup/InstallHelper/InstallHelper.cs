using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

using Microsoft.Deployment.WindowsInstaller;

using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using log4net;

namespace Wammer.Station
{
	public class InstallHelper
	{
		private static ILog Logger;

		static InstallHelper()
		{
			Stream stream =
				System.Reflection.Assembly.GetExecutingAssembly().
				GetManifestResourceStream("InstallHelper.log4net.config");

			if (stream != null)
				log4net.Config.XmlConfigurator.Configure(stream);

			Logger = log4net.LogManager.GetLogger("InstallHelper");
		}

		[CustomAction]
		public static ActionResult SignoffStation(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			try
			{
				StationInfo station = StationCollection.FindOne();
				if (station == null || station.Id == null || station.SessionToken == null)
				{
					Logger.Info("No station Id or token exist. Skip sign off station.");
					return ActionResult.Success;
				}

				Wammer.Cloud.StationApi.SignOff(new WebClient(), station.Id, station.SessionToken);
				Logger.Info("Sign off station success");
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Warn("Sign off station not success. Continue as if without error.", e);
				return ActionResult.Success;
			}
		}

		[CustomAction]
		public static ActionResult CleanStationInfo(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			try
			{
				StationRegistry.DeleteValue("stationId");
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station id in registry", e);
			}

			try
			{
				Model.Drivers.collection.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station driver from MongoDB", e);
			}

			try
			{
				Model.StationCollection.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station info from MongoDB", e);
			}

			try
			{
				Model.CloudStorage.collection.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete cloud storage from MongoDB", e);
			}

			try
			{
				Model.ServiceCollection.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete service collection from MongoDB", e);
			}

			string userDataFolder = "";
			try
			{
				string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				userDataFolder = Path.Combine(appPath, "waveface");

				Logger.Info("Deleting " + userDataFolder);
				Directory.Delete(userDataFolder, true);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete " + userDataFolder, e);
			}

			return ActionResult.Success;
		}
	}
}
