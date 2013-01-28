using System.Collections.Generic;

namespace Wammer.Station.Notify
{

	// See https://github.com/waveface/Wammer-station/wiki/Station-Websocket-APIs


	public class ConnectMsg
	{
		public string session_token { get; set; }
		public string apikey { get; set; }
		public string user_id { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(session_token) &&
				!string.IsNullOrEmpty(apikey) &&
				!string.IsNullOrEmpty(user_id);
		}
	}

	public class SubscribeMSg
	{
	}

	public class ErrorMsg
	{
		public int code { get; set; }
		public string reason { get; set; }
	}

	public class NotifyMsg
	{
		public bool updated { get; set; }
		public string message { get; set; }
	}

	#region station only web socket msgs
	public class ImportMsg
	{
		public List<string> files { get; set; }
		public string session_token { get; set; }
		public string group_id { get; set; }
		public string apikey { get; set; }
	}

	public class MetadataUploadedMsg
	{
		public int count { get; set; }
	}

	public class FileImportedMsg
	{
		public string file { get; set; }
	}

	public class ImportDoneMsg
	{
		public string Error { get; set; }
	}
	#endregion

	public class DeviceSyncStatus
	{
		public int files_to_backup { get; set; }
	}


	public class GenericCommand
	{
		public ErrorMsg error { get; set; }
		public ConnectMsg connect { get; set; }
		public SubscribeMSg subscribe { get; set; }
		public NotifyMsg notify { get; set; }
		public DeviceSyncStatus sync_status { get; set; }

		#region Station only web socket msgs
		public ImportMsg import { get; set; }
		public FileImportedMsg file_imported { get; set; }
		public ImportDoneMsg import_done { get; set; }
		public MetadataUploadedMsg metadata_uploaded { get; set; }
		#endregion

	}

	public enum ErrorCode
	{
		AccessDenied = 3001,
	}
}
