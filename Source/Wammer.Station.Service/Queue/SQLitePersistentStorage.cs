using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Wammer.Queue
{
	public class SQLitePersistentStorage : IPersistentStore
	{
		private readonly SQLiteConnection dbConn;

		public SQLitePersistentStorage(string dbFile)
		{
			var connString = new SQLiteConnectionStringBuilder {DataSource = dbFile};

			dbConn = new SQLiteConnection(connString.ToString());
			dbConn.Open();
		}

		#region IPersistentStore Members

		public WMSQueue TryLoadQueue(string qname)
		{
			var cmd = new SQLiteCommand {CommandText = @"SELECT 1 FROM sqlite_master WHERE [name] = $name and [type] ='table'"};
			cmd.Parameters.AddWithValue("$name", "queue_items_" + qname);
			cmd.Connection = dbConn;

			if (cmd.ExecuteScalar() == null)
			{
				var cmd2 = new SQLiteCommand {CommandText = "CREATE TABLE queue_items_" + qname + " (id GUID, data blob);"};
				cmd2.CommandText += "CREATE INDEX index_id_queue_items_" + qname + " ON queue_items_" + qname + " (id)";
				cmd2.Connection = dbConn;
				cmd2.ExecuteNonQuery();

				return new WMSQueue(qname, this);
			}
			else
			{
				var cmd2 = new SQLiteCommand
				           	{CommandText = string.Format("SELECT id, data FROM queue_items_{0}", qname), Connection = dbConn};

				var messages = new List<WMSMessage>();
				using (SQLiteDataReader r = cmd2.ExecuteReader())
				{
					if (r.HasRows)
					{
						var msg = new WMSMessage((Guid) r["id"], r["data"]) {IsPersistent = true};
						messages.Add(msg);
					}
				}

				return new WMSQueue(qname, this, messages);
			}
		}

		public void Save(WMSMessage msg)
		{
			var cmd = new SQLiteCommand
			          	{
			          		CommandText = "INSERT INTO queue_items_" + msg.Queue.Name + " (id, data) values ($id, $data)",
			          		Connection = dbConn
			          	};
			cmd.Parameters.AddWithValue("$id", msg.Id);
			cmd.Parameters.AddWithValue("$data", msg.Data);
			cmd.ExecuteNonQuery();
		}

		public void Remove(WMSMessage msg)
		{
			var cmd = new SQLiteCommand
			          	{
			          		CommandText = "DELETE FROM queue_items_" + msg.Queue.Name + " WHERE [id] = $id",
			          		Connection = dbConn
			          	};
			cmd.Parameters.AddWithValue("$id", msg.Id);
			cmd.ExecuteNonQuery();
		}

		#endregion

		public void Close()
		{
			dbConn.Close();
		}
	}
}