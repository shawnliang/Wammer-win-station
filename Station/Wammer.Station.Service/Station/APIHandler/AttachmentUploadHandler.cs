using System;
using Wammer.Model;
using Wammer.Station.AttachmentUpload;
using System.IO;
using System.Drawing;
using Wammer.Utility;
using System.Diagnostics;
using Wammer.Cloud;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/attachments/upload/")]
	public class AttachmentUploadHandler : HttpHandler
	{
		private readonly AttachmentUploadHandlerImp imp;

		public AttachmentUploadHandler()
		{
			DebugInfo.ShowMethod();
			imp = new AttachmentUploadHandlerImp(new AttachmentUploadHandlerDB());
		}

		public event EventHandler<AttachmentEventArgs> AttachmentProcessed
		{
			add
			{
				lock (imp)
				{
					imp.AttachmentProcessed += value;
				}
			}

			remove
			{
				lock (imp)
				{
					imp.AttachmentProcessed -= value;
				}
			}
		}

		public override void HandleRequest()
		{
			DebugInfo.ShowMethod();

			CheckParameter("session_token", "apikey", "group_id", "type");

			UploadData data = GetUploadData();

			Size imageSize = ImageHelper.GetImageSize(data.raw_data);

			Debug.WriteLine(Parameters["object_id"]);
			Debug.WriteLine(Parameters["image_meta"]);

			if (data.object_id == null)
				data.object_id = Guid.NewGuid().ToString();

			RespondSuccess(ObjectUploadResponse.CreateSuccess(data.object_id));
			imp.Process(data, imageSize);
		}

		private UploadData GetUploadData()
		{
			DebugInfo.ShowMethod();

			var data = new UploadData();

			if (Files.Count == 0)
				throw new FormatException("No file is uploaded");

			var file = Files[0];
			data.object_id = Parameters["object_id"];
			data.raw_data = file.Data;
			data.file_name = file.Name;
			data.mime_type = file.ContentType;
			data.title = Parameters["title"];
			data.description = Parameters["description"];
			data.group_id = Parameters["group_id"];

			data.api_key = Parameters["apikey"];
			data.session_token = Parameters["session_token"];
			data.post_id = Parameters["post_id"];
			data.memo = Parameters["memo"];

			try
			{
				data.type = (AttachmentType) Enum.Parse(typeof (AttachmentType),
				                                        Parameters["type"], true);
			}
			catch (ArgumentException e)
			{
				throw new FormatException("Unknown attachment type: " + Parameters["type"], e);
			}

			if (data.type == AttachmentType.doc)
				data.imageMeta = ImageMeta.None;
			else if (string.IsNullOrEmpty(Parameters["image_meta"]))
				data.imageMeta = ImageMeta.Origin;
			else
				data.imageMeta = (ImageMeta) Enum.Parse(typeof (ImageMeta), Parameters["image_meta"], true);

			if (data.raw_data.Array == null)
				throw new FormatException("file is missing in file upload multipart data");

			if (data.raw_data.Count > 20 * 1024 *1024)
				throw new WammerStationException("file size is over 20MB", (int)StationLocalApiError.ImageTooLarge);

			if (string.IsNullOrEmpty(data.file_name))
				throw new FormatException("file_name is null or empty");

			if (string.IsNullOrEmpty(Path.GetExtension(data.file_name)))
				throw new WammerStationException("file_name must have an extension", (int)StationLocalApiError.InvalidImage);	

			return data;
		}

		#region Public Method

		public override object Clone()
		{
			DebugInfo.ShowMethod();

			return MemberwiseClone();
		}

		#endregion
	}
}