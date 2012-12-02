using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerUtility : IMonitorAddHandlerUtility
	{
		public Model.Attachment GenerateDocAttachment(string docPath, string user_id)
		{
			var group_id = Model.DriverCollection.Instance.GetGroupIdByUser(user_id);
			if (group_id == null)
				throw new WammerStationException("user does not exist: " + user_id, -1);

			string mimeType = "application/octet-stream";
			if (Path.GetExtension(docPath).Equals(".ppt", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.ms-powerpoint";
			else if (Path.GetExtension(docPath).Equals(".pptx", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
			else if (Path.GetExtension(docPath).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/pdf";


			return new Attachment
			{
				creator_id = user_id,
				group_id = group_id,
				file_create_time = File.GetCreationTime(docPath),
				file_modify_time = File.GetLastWriteTime(docPath),
				file_name = Path.GetFileName(docPath),
				file_path = docPath,
				file_size = new FileInfo(docPath).Length,
				md5 = MD5Helper.ComputeMD5(File.ReadAllBytes(docPath)),
				mime_type = mimeType,
				modify_time = DateTime.Now,
				object_id = Guid.NewGuid().ToString(),
				saved_file_name = docPath,
				type = AttachmentType.doc
			};
		}

		public void UpdateDocOpenTimeAsync(string object_id, DateTime openTime)
		{
			throw new NotImplementedException();
		}
	}
}
