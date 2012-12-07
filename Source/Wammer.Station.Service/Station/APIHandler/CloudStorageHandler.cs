using log4net;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/cloudstorage/list/")]
	public class ListCloudStorageHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			var cloudstorages = new List<CloudStorageStatus>();

			// currently only support one driver
			Driver driver = DriverCollection.Instance.FindOne();
			CloudStorage cloudstorage = CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));
			if (cloudstorage != null)
			{
				cloudstorages.Add(new CloudStorageStatus
									{
										type = "dropbox",
										connected = true,
										quota = cloudstorage.Quota,
										used = new DropboxFileStorage(driver, cloudstorage).GetUsedSize(),
										account = cloudstorage.UserAccount
									}
					);
			}
			RespondSuccess(new ListCloudStorageResponse { cloudstorages = cloudstorages });
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class DropBoxOAuthHandler : HttpHandler
	{
		private static readonly ILog logger = LogManager.GetLogger("cloudStorage");

		public override void HandleRequest()
		{
			// currently only support one driver
			var driver = DriverCollection.Instance.FindOne();
			var api = new StorageApi(driver.user_id);

			var res = api.StorageAuthorize(CloudStorageType.DROPBOX);

			logger.InfoFormat("Dropbox OAuth URL = {0}", res.storages.authorization_url);

			RespondSuccess(
				new GetDropboxOAuthResponse
					{
						api_ret_code = 0,
						api_ret_message = "success",
						status = 200,
						timestamp = DateTime.UtcNow,
						oauth_url = res.storages.authorization_url
					});
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}


	public class DropBoxConnectHandler : HttpHandler
	{
		private static readonly ILog logger = LogManager.GetLogger("cloudStorage");

		public override void HandleRequest()
		{
			long quota = Convert.ToInt64(Parameters["quota"]);
			string folder = Parameters["folder"];
			bool linked = false;

			// currently only support one driver
			Driver driver = DriverCollection.Instance.FindOne();
			var api = new StorageApi(driver.user_id);

			try
			{
				var storageDoc = CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));

				// try connecting Dropbox if cloudstorage has no Dropbox info
				if (storageDoc == null)
				{
					var linkRes = api.StorageLink(CloudStorageType.DROPBOX);
					linked = true;

					VerifyAccountLink(folder, linkRes.storages.token);

					var res = api.StorageCheck(CloudStorageType.DROPBOX);
					if (res.storages.status != 0)
					{
						logger.ErrorFormat("Waveface Cloud report Dropbox connection failure, response = {0}", res.ToFastJSON());
						throw new WammerStationException("Dropbox has not linked yet", (int)DropboxApiError.ConnectDropboxFailed);
					}


					CloudStorageCollection.Instance.Save(new CloudStorage
															{
																Id = Guid.NewGuid().ToString(),
																Type = "dropbox",
																Folder = folder,
																Quota = quota,
																UserAccount = res.storages.account
															}
						);
				}

				RespondSuccess();
			}
			catch (Exception)
			{
				if (linked)
					api.StorageUnlink(CloudStorageType.DROPBOX);

				throw;
			}
		}

		private void VerifyAccountLink(string folder, string token)
		{
			// cloud will put a token file on Waveface folder for account verification, verify it in at most 60 secs
			string tokenFilePath = Path.Combine(folder, "waveface_" + token);
			int retry = 120;
			logger.InfoFormat("Check token file existence, path = {0}", tokenFilePath);
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
			return MemberwiseClone();
		}
	}


	public class DropBoxUpdateHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			long quota = Convert.ToInt64(Parameters["quota"]);

			CloudStorage storageDoc = CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));
			if (storageDoc == null)
				throw new WammerStationException("Dropbox is not connected", (int)DropboxApiError.DropboxNotConnected);

			storageDoc.Quota = quota;
			CloudStorageCollection.Instance.Save(storageDoc);
			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class DropboxDisconnectHandler : HttpHandler
	{
		private static readonly ILog logger = LogManager.GetLogger("cloudStorage");

		public override void HandleRequest()
		{
			// currently only support one driver
			Driver driver = DriverCollection.Instance.FindOne();
			var api = new StorageApi(driver.user_id);

			CloudStorage storageDoc = CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));
			if (storageDoc != null)
			{
				logger.Info("Unlink Dropbox account");

				api.StorageUnlink(CloudStorageType.DROPBOX);
				CloudStorageCollection.Instance.Remove(Query.EQ("Type", "dropbox"));
			}

			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class GetDropboxOAuthResponse : CloudResponse
	{
		public string oauth_url { get; set; }
	}

	public class ListCloudStorageResponse : CloudResponse
	{
		public List<CloudStorageStatus> cloudstorages { get; set; }
	}

	public class CloudStorageStatus
	{
		public string type { get; set; }
		public bool connected { get; set; }
		public long quota { get; set; }
		public long used { get; set; }
		public string account { get; set; }
	}
}