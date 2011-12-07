using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
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
	[ServiceContract]
	public interface ICloudStorageService
	{
		[OperationContract]
		[WebGet(UriTemplate = "dropbox/oauth")]
		Stream GetDropboxOAuth();

		[OperationContract]
		[WebGet(UriTemplate = "dropbox/connect?quota={quota}&folder={folder}")]
		Stream ConnectDropbox(long quota, string folder);

		[OperationContract]
		[WebGet(UriTemplate = "dropbox/update?quota={quota}")]
		Stream UpdateDropbox(long quota);

		[OperationContract]
		[WebGet(UriTemplate = "dropbox/disconnect")]
		Stream DisconnectDropbox();
	}

	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class CloudStorageService : ICloudStorageService
	{
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CloudStorageService));

		public Stream GetDropboxOAuth()
		{
			try
			{
				logger.Debug("Get Dropbox OAuth info");

				// currently only support one driver
				Drivers driver = Drivers.collection.FindOne();
				Storage storage = new Storage(driver.user_id);
				StorageAuthResponse res = storage.StorageAuthorize(new WebClient(), CloudStorageType.DROPBOX);
				logger.DebugFormat("Dropbox OAuth URL = {0}", res.storages.authorization_url);
				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current, new GetDropboxOAuthResponse
					{
						api_ret_code = 0,
						api_ret_msg = "success",
						status = 200,
						timestamp = DateTime.UtcNow,
						oauth_url = res.storages.authorization_url
					}
				);
			}
			catch (WammerCloudException ex)
			{
				logger.Error("Unable to connect to Dropbox due to cloud error", ex);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, ex.WammerError, ex.Message);
			}
			catch (Exception ex)
			{
				logger.Error("Unable to connect to Dropbox due to unknown exception", ex);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)StationApiError.Error, ex.Message);
			}
		}

		public Stream ConnectDropbox(long quota, string folder)
		{
			bool linked = false;
	
			// currently only support one driver
			Drivers driver = Drivers.collection.FindOne();
			Storage storage = new Storage(driver.user_id);

			try
			{
				logger.Debug("Dropbox is installed, connect to Dropbox");
				CloudStorage storageDoc = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));

				// try connecting Dropbox if cloudstorage has no Dropbox info
				if (storageDoc == null)
				{

					using (WebClient agent = new WebClient())
					{
						StorageLinkResponse linkRes;
						linkRes = storage.StorageLink(agent, CloudStorageType.DROPBOX);
						linked = true;

						VerifyAccountLink(folder, linkRes.storages.token);

						StorageCheckResponse res = storage.StorageCheck(agent, CloudStorageType.DROPBOX);
						if (res.storages.status != 0)
						{
							logger.ErrorFormat("Waveface Cloud report Dropbox connection failure, response = {0}", fastJSON.JSON.Instance.ToJSON(res, false, false, false, false));
							throw new WammerStationException("Dropbox has not linked yet", (int)DropboxApiError.ConnectDropboxFailed);
						}
					}

					// TODO: use generic storage for dropbox
					long used = 0;
					if (Directory.Exists(Path.Combine(folder, "resource")))
					{
						// calculate the sync folder size since it might contain old files
						DirectoryInfo di = new DirectoryInfo(Path.Combine(folder, "resource"));
						FileInfo[] fis = di.GetFiles();
						foreach (FileInfo fi in fis)
						{
							used = used + fi.Length;
						}
					}

					CloudStorage.collection.Save(new CloudStorage
						{
							Id = Guid.NewGuid().ToString(),
							Type = "dropbox",
							Folder = folder,
							Quota = quota,
							Used = used
						}
					);
				}

				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
			}
			catch (WammerStationException ex)
			{
				if (linked)
					storage.StorageUnlink(new WebClient(), CloudStorageType.DROPBOX);

				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, ex.WammerError, ex.Message);
			}
			catch (WammerCloudException ex)
			{
				if (linked)
					storage.StorageUnlink(new WebClient(), CloudStorageType.DROPBOX);

				logger.Error("Unable to connect to Dropbox due to cloud error", ex);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, ex.WammerError, ex.Message);
			}
			catch (Exception ex)
			{
				if (linked)
					storage.StorageUnlink(new WebClient(), CloudStorageType.DROPBOX);

				logger.Error("Unable to connect to Dropbox due to unknown exception", ex);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)StationApiError.Error, ex.Message);
			}
		}

		public Stream UpdateDropbox(long quota)
		{
			CloudStorage storageDoc = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
			if (storageDoc != null)
			{
				logger.InfoFormat("Update Dropbox quota to {0}", quota);
				storageDoc.Quota = quota;
				CloudStorage.collection.Save(storageDoc);
				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
			}
			else
			{
				logger.Error("Dropbox is not connected, unable to update");
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)DropboxApiError.DropboxNotConnected, "Dropbox is not connected");
			}
		}

		public Stream DisconnectDropbox()
		{
			logger.Debug("Disconnect from Dropbox");
			CloudStorage.collection.Remove(Query.EQ("Type", "dropbox"));
			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
		}

		private void VerifyAccountLink(string folder, string token)
		{
			// cloud will put a token file on Waveface folder for account verification, verify it in at most 3 secs
			string tokenFilePath = Path.Combine(folder, "waveface_" + token);
			int retry = 10;
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
