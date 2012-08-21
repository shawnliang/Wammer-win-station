using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using NLog;

namespace Waveface.Upload
{
	class UploadQueuePersistency
	{
		#region Var
		private HashSet<UploadItem> _items;
		private readonly string filePath;
		private static Logger logger = LogManager.GetCurrentClassLogger(); 
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>The items.</value>
		private HashSet<UploadItem> Items
		{
			get
			{
				return _items ?? (_items = new HashSet<UploadItem>());
			}
		}
		#endregion

		public UploadQueuePersistency(string runtimeDataPath, string user_id)
		{
			DebugInfo.ShowMethod();
			this.filePath = Path.Combine(runtimeDataPath, user_id + "_NP.txt");
		}

		public void Add(params UploadItem[] items)
		{
			DebugInfo.ShowMethod();

			foreach (var item in items)
				Items.Add(item);
			Save();
		}

		public void Remove(UploadItem item)
		{
			DebugInfo.ShowMethod();

			Items.Remove(item);
			Save();
		}

		public IEnumerable<UploadItem> Load()
		{
			DebugInfo.ShowMethod();

			try
			{
				using (StreamReader _sr = File.OpenText(filePath))
				{
					var _json = _sr.ReadToEnd();
				
					if (!GCONST.DEBUG)
						_json = StringUtility.Decompress(_json);

					var items = JsonConvert.DeserializeObject<HashSet<UploadItem>>(_json);

					this.Items.Clear();
					if (items != null)
					{
						this.Items.UnionWith(items);
					}
				}
			}
			catch (Exception _e)
			{
				logger.WarnException("Unable to load UploadQueue from " + filePath, _e);
			}

			return this.Items;
		}

		private void Save()
		{
			DebugInfo.ShowMethod();

			try
			{
				string _json = JsonConvert.SerializeObject(Items);

				if (!GCONST.DEBUG)
					_json = StringUtility.Compress(_json);

				using (StreamWriter _outfile = new StreamWriter(filePath))
				{
					_outfile.Write(_json);
				}
			}
			catch (Exception _e)
			{
				logger.WarnException("Unable to save UploadQueue", _e);
			}
		}

		
	}
}
