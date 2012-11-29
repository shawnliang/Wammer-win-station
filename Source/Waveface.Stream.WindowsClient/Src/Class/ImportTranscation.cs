using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;
using WebSocketSharp;

namespace Waveface.Stream.WindowsClient
{
	public class ImportTranscation
	{
		private WebSocket socket;
		private string user_id;
		private string session_token;
		private string apikey;
		private IEnumerable<string> paths;


		private bool doneNotified;

		public event EventHandler<ImportDoneEventArgs> ImportDone;
		public event EventHandler<FileImportedEventArgs> FileImported;
		public event EventHandler<MetadataUploadEventArgs> MetadataUploaded;

		public ImportTranscation(string user_id, string session_token, string apikey, IEnumerable<string> paths)
		{
			this.socket = new WebSocket("ws://127.0.0.1:9983");

            socket.OnMessage += new EventHandler<WebSocketSharp.MessageEventArgs>(socket_OnMessage);
			socket.OnError += new EventHandler<ErrorEventArgs>(socket_OnError);
			socket.OnClose += new EventHandler<CloseEventArgs>(socket_OnClose);

			this.session_token = session_token;
			this.apikey = apikey;
			this.paths = paths;
			this.user_id = user_id;
		}

		void socket_OnError(object sender, ErrorEventArgs e)
		{
			raiseTransactionFinished(e.Message);
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
				raiseTransactionFinished(notify.import_done.Error);
			}

			if (notify.metadata_uploaded != null)
			{
				raiseMetadataUploadedEvent(notify.metadata_uploaded.count);
			}
		}

		void socket_OnClose(object sender, CloseEventArgs e)
		{
			raiseTransactionFinished(null);
		}


		private void raiseMetadataUploadedEvent(int count)
		{
			try
			{
				var handler = MetadataUploaded;
				if (handler != null)
					handler(this, new MetadataUploadEventArgs(count));
			}
			catch
			{
			}
		}

		private void raiseFileImportedEvent(string file)
		{
			try
			{
				var handler2 = FileImported;
				if (handler2 != null)
					handler2(this, new FileImportedEventArgs(file));
			}
			catch
			{
			}
		}

		private void raiseTransactionFinished(string error)
		{
			try
			{
				lock (socket)
				{
					if (doneNotified)
						return;

					var handler2 = ImportDone;
					if (handler2 != null)
						handler2(this, new ImportDoneEventArgs { Error = (error == null) ? null : new Exception(error) });

					doneNotified = true;
				}
			}
			catch
			{
			}
		}

		public void ImportFileAsync()
		{
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

			var user = DriverCollection.Instance.FindOneById(user_id);
			var group_id = user.groups[0].group_id;

			var import = new GenericCommand
			{
				import = new ImportMsg
				{
					files = paths.ToList(),
					apikey = apikey,
					session_token = session_token,
					group_id = group_id,
				}
			};

			socket.Send(import.ToFastJSON());
		}
	}
}
