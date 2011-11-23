using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace Wammer.Station
{
	public class InstallHelper
	{
		[CustomAction]
		public static ActionResult SignoffStation(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			string logFile = Path.Combine(wavefaceDir, @"log\uninstall.log");

			try
			{
				StationInfo station = StationInfo.collection.FindOne();
				if (station == null || station.Id == null || station.SessionToken == null)
				    return ActionResult.Success;

				Wammer.Cloud.Station.SignOff(new WebClient(), station.Id, station.SessionToken);

				using (StreamWriter w = new StreamWriter(File.OpenWrite(logFile)))
				{
					w.WriteLine("success");
				}

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				using (StreamWriter w = new StreamWriter(File.OpenWrite(logFile)))
				{
					w.WriteLine(e.ToString());
				}

				// Signoff failure is not critical, still return success.
				return ActionResult.Success;
			}
		}
	}
}
