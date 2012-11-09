using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using Wammer.Station.Notify;
using Wammer.Utility;

namespace Wammer.Station.Management
{
	public class ImportTranscation
	{
		private WebSocket socket;
		private string user_id;
		private string session_token;
		private string apikey;
		private IEnumerable<string> paths;


		private bool doneNotified;

		public event EventHandler<FileImportEventArgs> FileImported;
		public event EventHandler<TransactionFinishedEventArgs> TransactionFinished;
		

		public ImportTranscation(string user_id, string session_token, string apikey, IEnumerable<string> paths)
		{
			this.socket = new WebSocket("ws://127.0.0.1:9983");

			socket.OnMessage += new EventHandler<MessageEventArgs>(socket_OnMessage);
			socket.OnError += new EventHandler<ErrorEventArgs>(socket_OnError);
			socket.OnClose += new EventHandler<CloseEventArgs>(socket_OnClose);

			this.session_token = session_token;
			this.apikey = apikey;
			this.paths = paths;
		}

		void socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
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
		}

		void socket_OnClose(object sender, CloseEventArgs e)
		{
			raiseTransactionFinished(null);
		}

		private void raiseFileImportedEvent(string file)
		{
			try
			{
				var handler = FileImported;
				if (handler != null)
					handler(this, new FileImportEventArgs { FilePath = file });
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

					var handler = TransactionFinished;
					if (handler != null)
						handler(this, new TransactionFinishedEventArgs { Error = error });

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

			var user = Model.DriverCollection.Instance.FindOneById(user_id);
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


	public class FileImportEventArgs : EventArgs
	{
		public string FilePath { get; set; }
	}

	public class TransactionFinishedEventArgs : EventArgs
	{
		public string Error { get; set; }
	}
}
