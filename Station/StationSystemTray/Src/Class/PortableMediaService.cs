using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebSocketSharp;
using Wammer.Station.Notify;
using Wammer.Utility;

namespace StationSystemTray
{
	class PortableMediaService : IPortableMediaService
	{
		public event EventHandler<FileImportEventArgs> FileImported;
		public event EventHandler<ImportDoneEventArgs> ImportDone;

		private volatile bool doneNotified;
		private WebSocket socket;

		public PortableMediaService()
		{
			socket = new WebSocket("ws://127.0.0.1:9983/");
			socket.OnClose += new EventHandler<CloseEventArgs>(socket_OnClose);
			socket.OnMessage += new EventHandler<WebSocketSharp.MessageEventArgs>(socket_OnMessage);
			socket.OnError += new EventHandler<WebSocketSharp.ErrorEventArgs>(socket_OnError);
		}

		void socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			raiseImportDoneEvent(e.Message);
		}

		void socket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			var notify = fastJSON.JSON.Instance.ToObject<GenericCommand>(e.Data);

			if (notify.file_imported != null)
			{
				raiseFileImportedEvent(notify.file_imported.file);
			}

			if (notify.import_done != null)
			{
				raiseImportDoneEvent(notify.import_done.Error);
			}
		}

		void socket_OnClose(object sender, CloseEventArgs e)
		{
			raiseImportDoneEvent(null);
		}

		private void raiseFileImportedEvent(string file)
		{
			var handler = FileImported;
			if (handler != null)
				handler(this, new FileImportEventArgs { FilePath = file });
		}

		private void raiseImportDoneEvent(string error)
		{
			lock (socket)
			{
				if (doneNotified)
					return;

				var handler = ImportDone;
				if (handler != null)
					handler(this, new ImportDoneEventArgs { Error = error });

				doneNotified = true;
			}
		}

		public IEnumerable<PortableDevice> GetPortableDevices()
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				if ((drive.DriveType == DriveType.CDRom || drive.DriveType == DriveType.Removable) &&
					drive.IsReady)
				{
					yield return new PortableDevice
					{
						DrivePath = drive.Name,
						Name = string.Format("({0}) {1}", drive.Name, drive.VolumeLabel)
					};
				}
			}
			
		}

		public IEnumerable<string> GetFileList(string path)
		{
			var jpgs = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
			foreach (var file in jpgs)
				yield return file;

			var jpegs = Directory.GetFiles(path, "*.jpeg", SearchOption.AllDirectories);
			foreach (var file in jpegs)
				yield return file;

			var pngs = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
			foreach (var file in pngs)
				yield return file;
		}

		public void ImportAsync(IEnumerable<string> files, string user_id, string session_token, string apikey)
		{
			if (socket.IsConnected)
				socket.Close();

			socket.Connect();

			doneNotified = false;

			var connect = new GenericCommand
			{
				connect = new ConnectMsg
				{
					apikey = apikey,
					session_token = session_token,
					user_id = user_id
				}
			};

			socket.Send(connect.ToFastJSON());


			var session = Wammer.Model.LoginedSessionCollection.Instance.FindOneById(session_token);
			var group_id = session.groups[0].group_id;

			var import = new GenericCommand
			{
				import = new ImportMsg
				{
					files = files.ToList(),
					apikey = StationAPI.API_KEY,
					session_token = session_token,
					group_id = group_id,
				}
			};

			socket.Send(import.ToFastJSON());
		}
	}
}
