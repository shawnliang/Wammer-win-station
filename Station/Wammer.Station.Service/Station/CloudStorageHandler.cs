using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;

using Wammer.Station;
using Wammer.Utility;
using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{

	public class DropBoxOAuthHandler : HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("cloudStorage");

		protected override void HandleRequest()
		{
			// currently only support one driver
			Drivers driver = Drivers.collection.FindOne();
			StorageApi api = new StorageApi(driver.user_id);
			StorageAuthResponse res = api.StorageAuthorize(new WebClient(), CloudStorageType.DROPBOX);
			logger.DebugFormat("Dropbox OAuth URL = {0}", res.storages.authorization_url);

			RespondSuccess(
				new GetDropboxOAuthResponse
				{
					api_ret_code = 0,
					api_ret_msg = "success",
					status = 200,
					timestamp = DateTime.UtcNow,
					oauth_url = res.storages.authorization_url
				});	
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}


	public class DropBoxConnectHandler : HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("cloudStorage");

		protected override void HandleRequest()
		{
			long quota = Convert.ToInt64(Parameters["quota"]);
			string folder = Parameters["folder"];
			bool linked = false;

			// currently only support one driver
			Drivers driver = Drivers.collection.FindOne();
			StorageApi api = new StorageApi(driver.user_id);

			try
			{
				CloudStorage storageDoc = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));

				// try connecting Dropbox if cloudstorage has no Dropbox info
				if (storageDoc == null)
				{
					using (WebClient agent = new WebClient())
					{
						StorageLinkResponse linkRes;
						linkRes = api.StorageLink(agent, CloudStorageType.DROPBOX);
						linked = true;

						VerifyAccountLink(folder, linkRes.storages.token);

						StorageCheckResponse res = api.StorageCheck(agent, CloudStorageType.DROPBOX);
						if (res.storages.status != 0)
						{
							logger.ErrorFormat("Waveface Cloud report Dropbox connection failure, response = {0}", res.ToFastJSON());
							throw new WammerStationException("Dropbox has not linked yet", (int)DropboxApiError.ConnectDropboxFailed);
						}
					}

					CloudStorage.collection.Save(new CloudStorage
						{
							Id = Guid.NewGuid().ToString(),
							Type = "dropbox",
							Folder = folder,
							Quota = quota
						}
					);
				}

				RespondSuccess();
			}
			catch (Exception)
			{
				if (linked)
					api.StorageUnlink(new WebClient(), CloudStorageType.DROPBOX);

				throw;
			}
		}

		private void VerifyAccountLink(string folder, string token)
		{
			// cloud will put a token file on Waveface folder for account verification, verify it in at most 10 secs
			string tokenFilePath = Path.Combine(folder, "waveface_" + token);
			int retry = 20;
			logger.DebugFormat("Check token file existence, path = {0}", tokenFilePath);
			while (!File.Exists(tokenFilePath))
			{
				Thread.Sleep(500);
				retry--;
				if (retry == 0)
				{
					break;
				}
			}
			if (!File.Exists(tokenFilePath))
			{
				logger.ErrorFormat("Dropbox token file does not exist, path = {0}", tokenFilePath);
				throw new WammerStationException("Link to wrong Dropbox account", (int)DropboxApiError.LinkWrongAccount);
			}
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}


	public class DropBoxUpdateHandler : HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("cloudStorage");

		protected override void HandleRequest()
		{
			long quota = Convert.ToInt64(Parameters["quota"]);

			CloudStorage storageDoc = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
			if (storageDoc == null)
				throw new WammerStationException("Dropbox is not connected", (int)DropboxApiError.DropboxNotConnected);

			storageDoc.Quota = quota;
			CloudStorage.collection.Save(storageDoc);
			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class DropboxDisconnectHandler: HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("cloudStorage");

		protected override void HandleRequest()
		{
			CloudStorage.collection.Remove(Query.EQ("Type", "dropbox"));
			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class GetDropboxOAuthResponse : CloudResponse
	{
		public string oauth_url { get; set; }

		public GetDropboxOAuthResponse()
			: base()
		{
		}
	}
}
