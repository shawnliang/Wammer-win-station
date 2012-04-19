using System;
using System.Linq;
using Wammer.Station;
using Wammer.Station.AttachmentUpload;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station.APIHandler
{
	public class AttachmentUploadHandler : HttpHandler
	{
		private AttachmentUploadHandlerImp imp;

		public event EventHandler<AttachmentUpload.AttachmentEventArgs> AttachmentProcessed
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

		public AttachmentUploadHandler()
		{
			imp = new AttachmentUploadHandlerImp(new AttachmentUploadHandlerDB());
		}

		protected override void HandleRequest()
		{
			CheckParameter("session_token", "apikey", "group_id", "type", "file_name");

			UploadData data = GetUploadData();

			RespondSuccess(imp.Process(data).ToFastJSON());
		}
		
		private UploadData GetUploadData()
		{
			UploadData data = new UploadData();

			if (Files.Count == 0)
				throw new FormatException("No file is uploaded");

			data.object_id = Parameters["object_id"];
			data.raw_data = Files[0].Data;
			data.file_name = Files[0].Name;
			data.mime_type = Files[0].ContentType;
			data.title = Parameters["title"];
			data.description = Parameters["description"];
			data.group_id = Parameters["group_id"];
			
			
			try
			{
				data.type = (AttachmentType)Enum.Parse(typeof(AttachmentType),
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
				data.imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta), Parameters["image_meta"], true);

			if (data.raw_data.Array == null)
				throw new FormatException("file is missing in file upload multipart data");

			return data;
		}

		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}