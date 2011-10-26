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

		public ObjectUploadHandler()
			: base()
		{
			storage = new FileStorage("resource");
		}

		public override object Clone()
		{
			return new ObjectUploadHandler();
		}

		protected override void HandleRequest()
		{
			FileUpload file = GetFileFromMultiPartData();
			string savedName = GetSavedFilename(file);
			storage.Save("space1", file.type, savedName, file.fileContent);

			ObjectUploadResponse json =
							ObjectUploadResponse.CreateSuccess(file.objectId);
			respondSuccess(Response, json);
		}

		private static string GetSavedFilename(FileUpload file)
		{
			string originalSuffix = Path.GetExtension(file.filename);
			if (originalSuffix != null)
			{
				return file.objectId + originalSuffix;
			}
			else
			{
				return file.objectId;
			}
		}

		private static void respondSuccess(HttpListenerResponse response,
												ObjectUploadResponse jsonObj)
		{
			response.StatusCode = 200;
			response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(response.OutputStream))
			{
				string json = fastJSON.JSON.Instance.ToJSON(jsonObj, false, false, false, false);
				response.ContentLength64 = json.Length;
				w.Write(json);
			}
		}

		private FileUpload GetFileFromMultiPartData()
		{
			FileUpload file = new FileUpload();

			file.objectId = Parameters["object_id"];
			file.type = (FileType)Enum.Parse(typeof(FileType), Parameters["file_type"]);
			file.fileContent = Files[0].Data;
			file.filename = Files[0].Name;
			file.contentType = Files[0].ContentType;

			if (file.objectId == null)
				throw new FormatException("object_id is missing in file upload multipart data");

			if (file.filename == null)
				throw new FormatException("filename is missing in file upload multipart data");

			if (file.fileContent == null)
				throw new FormatException("file is missing in file upload multipart data");

			if (file.type == FileType.None)
				throw new FormatException("filetype is missing in file upload multipart data");

			return file;
		}
	}


	class FileUpload
	{
		public string filename;
		public FileType type;
		public string objectId;
		public byte[] fileContent;
		public string contentType;

		public FileUpload()
		{
			filename = null;
			type = FileType.None;
			objectId = null;
			fileContent = null;
			contentType = null;
		}
	}
}
