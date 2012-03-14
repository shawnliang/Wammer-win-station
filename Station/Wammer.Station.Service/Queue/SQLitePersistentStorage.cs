using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Wammer.Queue
{
	public class SQLitePersistentStorage : IPersistentStore
	{
		private SQLiteConnection dbConn;

		public SQLitePersistentStorage(string dbFile)
		{
			SQLiteConnectionStringBuilder connString = new SQLiteConnectionStringBuilder();
			connString.DataSource = dbFile;

			dbConn = new SQLiteConnection(connString.ToString());
			dbConn.Open();
		}

		public WMSQueue TryLoadQueue(string qname)
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandText = @"SELECT 1 FROM sqlite_master WHERE [name] = $name and [type] ='table'";
			cmd.Parameters.AddWithValue("$name", "queue_items_" + qname);
			cmd.Connection = dbConn;

			if (cmd.ExecuteScalar() == null)
			{
				SQLiteCommand cmd2 = new SQLiteCommand();
				cmd2.CommandText = "CREATE TABLE queue_items_" + qname + " (id GUID, data blob);";
				cmd2.CommandText += "CREATE INDEX index_id_queue_items_" + qname + " ON queue_items_" + qname + " (id)";
				cmd2.Connection = dbConn;
				cmd2.ExecuteNonQuery();

				return new WMSQueue(qname, this);
			}
			else
			{
				SQLiteCommand cmd2 = new SQLiteCommand();
				cmd2.CommandText = string.Format("SELECT id, data FROM queue_items_{0}", qname);
				cmd2.Connection = dbConn;

				List<WMSMessage> messages = new List<WMSMessage>();
				using (SQLiteDataReader r = cmd2.ExecuteReader())
				{
					WMSMessage msg = new WMSMessage((Guid)r["id"], r["data"]);
					msg.IsPersistent = true;
					messages.Add(msg);
				}

				return new WMSQueue(qname, this, messages);
			}			
		}

		public void Save(WMSMessage msg)
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandText = "INSERT INTO queue_items_" + msg.Queue.Name + " (id, data) values ($id, $data)";
			cmd.Connection = this.dbConn;
			cmd.Parameters.AddWithValue("$id", msg.Id);
			cmd.Parameters.AddWithValue("$data", msg.Data);
			cmd.ExecuteNonQuery();
		}

		public void Remove(WMSMessage msg)
		{
			SQLiteCommand cmd = new SQLiteCommand();
			cmd.CommandText = "DELETE FROM queue_items_" + msg.Queue.Name + " WHERE [id] = $id";
			cmd.Connection = this.dbConn;
			cmd.Parameters.AddWithValue("$id", msg.Id);
			cmd.ExecuteNonQuery();
		}

		public void Close()
		{
			dbConn.Close();
		}
	}
}
