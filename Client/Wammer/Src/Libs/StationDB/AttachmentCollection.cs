using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;

namespace Waveface.Libs.StationDB
{
    static class AttachmentCollection
    {
        public static string QueryFileName(string object_id)
        {
            var doc = StationDB.GetCollection("attachments").FindOneById(object_id);

            if (doc == null)
                return null;

            return doc["file_name"].AsString;
        }
		
		public static string QuerySavedFileName(string object_id)
		{
			var doc = StationDB.GetCollection("attachments").FindOneById(object_id);
			if (doc == null)
				return null;

			if (doc.Contains("saved_file_name"))
				return doc["saved_file_name"].AsString;
			else
				return string.Empty;
		}
    }
}
