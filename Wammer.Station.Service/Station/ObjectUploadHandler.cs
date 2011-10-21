using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class ObjectUploadHandler : IHttpHandler
	{
		private FileStorage storage;
		private const string BOUNDARY = "boundary=";

		public ObjectUploadHandler()
		{
			storage = new FileStorage("resource");
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			FileUpload file = new FileUpload();

			try
			{
				string boundary = GetMultipartBoundary(request);

				MultiPart.Parser parser = new Parser(boundary);

				Part[] parts = parser.Parse(request.InputStream);

				file = GetFileFromMultiPartData(parts);
				string savedName = GetSavedFilename(file);
				storage.Save("space1", file.type, savedName, file.fileContent);

				ObjectUploadResponse json =
								ObjectUploadResponse.CreateSuccess(file.objectId);
				respondSuccess(response, json);
			}
			catch (FormatException e)
			{
				ObjectUploadResponse json = ObjectUploadResponse.CreateFailure(file.objectId, 400, e);
				HttpHelper.RespondFailure(response, json);
			}
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

		private static string GetMultipartBoundary(HttpListenerRequest request)
		{
			try
			{
				int idx = request.ContentType.ToLower().IndexOf(BOUNDARY);
				string boundary = request.ContentType.Substring(
															idx + BOUNDARY.Length);
				return boundary;
			}
			catch (Exception e)
			{
				string contentType = request.ContentType;
				if (contentType==null)
					contentType = "(null)";

				throw new FormatException("Error finding multipart boundary. Content-Type: " + 
																					contentType, e);
			}
		}

		private static FileUpload GetFileFromMultiPartData(Part[] parts)
		{
			FileUpload file = new FileUpload();

			for (int i = 0; i < parts.Length; i++)
			{
				if (parts[i].ContentDisposition == null)
					continue;

				string field = parts[i].ContentDisposition.Parameters["name"];

				switch (field)
				{
					case "file":
						file.fileContent = parts[i].Bytes;
						file.filename = parts[i].ContentDisposition.Parameters["filename"];
						file.contentType = parts[i].Headers["Content-Type"];
						break;
					case "file_type":
						file.type = (FileType)Enum.Parse(typeof(FileType),
																parts[i].Text);
						break;
					case "object_id":
						file.objectId = parts[i].Text;
						break;
					default:
						continue;
				}
			}

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

		public bool IsValid
		{
			get
			{
				return filename != null && objectId != null && 
					fileContent != null && type != FileType.None;
			}
		}
	}
}
