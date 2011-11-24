using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;

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
		[WebGet(UriTemplate = "list")]
		Stream ListCloudStorage();

		[OperationContract]
		[WebGet(UriTemplate = "dropbox/connect?quota={quota}")]
		Stream ConnectDropbox(long quota);

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

		public Stream ListCloudStorage()
		{
			List<StorageStatus> cloudstorages = new List<StorageStatus>();

			if (DropboxHelper.IsInstalled())
			{
				logger.Debug("Dropbox is installed");
				CloudStorage storage = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
				if (storage != null)
				{
					logger.Debug("Already connected to Dropbox");
					cloudstorages.Add(new StorageStatus
						{
							type = "dropbox",
							connected = true,
							quota = storage.Quota,
							used = storage.Used
						}
					);
				}
				else
				{
					logger.Debug("Not connected to Dropbox");
					cloudstorages.Add(new StorageStatus
						{
							type = "dropbox",
							connected = false
						}
					);
				}
			}

			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current, new ListCloudStorageResponse
				{
					api_ret_code = 0,
					api_ret_msg = "success",
					status = 200,
					timestamp = DateTime.UtcNow,
					cloudstorages = cloudstorages
				}
			);
		}

		public Stream ConnectDropbox(long quota)
		{
			if (DropboxHelper.IsInstalled())
			{
				logger.Debug("Dropbox is installed, connect to Dropbox");
				CloudStorage storage = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
				if (storage == null)
				{
					string syncFolder = Path.Combine(DropboxHelper.GetSyncFolder(), @"\Waveface\resource");
					if (string.IsNullOrEmpty(syncFolder))
					{
						logger.Error("Dropbox sync folder is empty");
						WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)DropboxApiError.NoSyncFolder); 
					}
					CloudStorage.collection.Save(new CloudStorage
						{
							Id = Guid.NewGuid().ToString(),
							Type = "dropbox",
							Folder = syncFolder,
							Quota = quota,
							Used = 0
						}
					);
				}
				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
			}
			else
			{
				logger.Error("Dropbox is not installed, unable to connect to it");
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)DropboxApiError.DropboxNotInstalled);
			}
		}

		public Stream UpdateDropbox(long quota)
		{
			if (DropboxHelper.IsInstalled())
			{
				CloudStorage storage = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
				if (storage != null)
				{
					logger.InfoFormat("Update Dropbox quota to {0}", quota);
					storage.Quota = quota;
					CloudStorage.collection.Save(storage);
					return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
				}
				else
				{
					logger.Error("Dropbox is not connected, unable to update");
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)DropboxApiError.DropboxNotConnected);
				}
			}
			else
			{
				logger.Error("Dropbox is not installed, unable to update");
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, (int)DropboxApiError.DropboxNotInstalled);
			}
		}

		public Stream DisconnectDropbox()
		{
			logger.Debug("Disconnect from Dropbox");
			CloudStorage.collection.Remove(Query.EQ("Type", "dropbox"));
			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
		}
	}

	public class ListCloudStorageResponse : CloudResponse
	{
		public List<StorageStatus> cloudstorages { get; set; }

		public ListCloudStorageResponse()
			:base()
		{
		}
	}

	public class StorageStatus
	{
		public string type { get; set; }
		public bool connected { get; set; }
		public long quota { get; set; }
		public long used { get; set; }
	}
}
