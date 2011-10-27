using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class ObjectUploadHandler : HttpHandler
	{
		private FileStorage storage;

		/// <summary>
		/// Fired on the uploaded attachment is saved
		/// </summary>
		public event EventHandler<AttachmentUploadEventArgs> AttachmentSaved;
		/// <summary>
		/// Fired on the whole request processing is completed
		/// </summary>
		public event EventHandler<AttachmentUploadEventArgs> RequestCompleted;

		public ObjectUploadHandler(FileStorage fileStore)
			: base()
		{
			storage = fileStore;
		}

		public ObjectUploadHandler()
			: base()
		{
			storage = new FileStorage("resource");
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			Attachment file = GetFileFromMultiPartData();

			if (file.ObjectId == null)
				file.ObjectId = Guid.NewGuid().ToString();

			string savedName = GetSavedFilename(file);
			storage.Save(savedName, file.RawData);

			AttachmentUploadEventArgs evtArgs = new AttachmentUploadEventArgs(file);
			OnAttachmentSaved(evtArgs);

			ObjectUploadResponse json = ObjectUploadResponse.CreateSuccess(file.ObjectId);
			HttpHelper.RespondSuccess(Response, json);

			OnRequestCompleted(evtArgs);
		}

		protected void OnAttachmentSaved(AttachmentUploadEventArgs evt)
		{
			EventHandler<AttachmentUploadEventArgs> handler = AttachmentSaved;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		protected void OnRequestCompleted(AttachmentUploadEventArgs evt)
		{
			EventHandler<AttachmentUploadEventArgs> handler = RequestCompleted;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static string GetSavedFilename(Attachment file)
		{
			string originalSuffix = Path.GetExtension(file.Filename);
			if (originalSuffix != null)
			{
				return file.ObjectId + originalSuffix;
			}
			else
			{
				return file.ObjectId;
			}
		}

		private Attachment GetFileFromMultiPartData()
		{
			Attachment file = new Attachment();

			file.ObjectId = Parameters["object_id"];
			file.RawData = Files[0].Data;
			file.Filename = Files[0].Name;
			file.ContentType = Files[0].ContentType;
			file.Title = Parameters["title"];
			file.Description = Parameters["description"];

			if (Parameters["type"]==null)
				throw new FormatException("type is missing in file upload multipart data");

			try
			{
				file.Kind = (AttachmentType)Enum.Parse(typeof(AttachmentType),
																			Parameters["type"], true);
			}
			catch (ArgumentException e)
			{
				throw new FormatException("Unknown attachment type: " + Parameters["type"], e);
			}

			if (file.Filename == null)
				throw new FormatException("filename is missing in file upload multipart data");

			if (file.RawData == null)
				throw new FormatException("file is missing in file upload multipart data");

			return file;
		}
	}

	public class AttachmentUploadEventArgs : EventArgs
	{
		public Attachment Attachment { get; private set; }

		public AttachmentUploadEventArgs(Attachment attachment)
			:base()
		{
			this.Attachment = attachment;
		}
	}
}
