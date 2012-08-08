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
		private HashSet<UploadItem> items = new HashSet<UploadItem>();
		private readonly string filePath;
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public UploadQueuePersistency(string runtimeDataPath, string user_id)
		{
			this.filePath = Path.Combine(runtimeDataPath, user_id + "_NP.txt");
		}

		public void Add(UploadItem item)
		{
			items.Add(item);
			Save();
		}

		public void Remove(UploadItem item)
		{
			items.Remove(item);
			Save();
		}

		public void Load(UploadQueue queue)
		{
			try
			{
				using (StreamReader _sr = File.OpenText(filePath))
				{
					var _json = _sr.ReadToEnd();
				
					if (!GCONST.DEBUG)
						_json = StringUtility.Decompress(_json);

					items = JsonConvert.DeserializeObject<HashSet<UploadItem>>(_json);
					
					foreach (var item in items)
					{
						queue.AddLast(item);
					}
				}
			}
			catch (Exception _e)
			{
				logger.WarnException("Unable to load UploadQueue from " + filePath, _e);
			}
		}

		private void Save()
		{
			try
			{
				string _json = JsonConvert.SerializeObject(items);

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
