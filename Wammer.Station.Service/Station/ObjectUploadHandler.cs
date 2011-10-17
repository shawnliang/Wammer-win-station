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

		public void Handle(object state)
		{
			HttpListenerContext context = (HttpListenerContext)state;
			FileUpload file = new FileUpload();

			try
			{
				string boundary = GetMultipartBoundary(context);

				MultiPart.Parser parser = new Parser(boundary);

				Part[] parts = parser.Parse(context.Request.InputStream);

				file = GetFileFromMultiPartData(parts);
				storage.Save("space1", file.type, file.filename, file.fileContent);

				ObjectUploadResponse response =
								ObjectUploadResponse.CreateSuccess(file.objectId);
				respondSuccess(context, response);
			}
			catch (Exception e)
			{
				respondFailure(context, file, e);
			}
		}

		private static FileUpload respondFailure(HttpListenerContext ctx,
												FileUpload file, Exception e)
		{
			ObjectUploadResponse response =
					ObjectUploadResponse.CreateFailure(file.objectId, -1, e);

			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
			{
				string resText = fastJSON.JSON.Instance.ToJSON(
										response, false, false, false, false);
				ctx.Response.ContentLength64 = resText.Length;
				w.Write(resText);
			}

			return file;
		}

		private static void respondSuccess(HttpListenerContext ctx,
												ObjectUploadResponse response)
		{
			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
			{
				string resText = fastJSON.JSON.Instance.ToJSON(
										response, false, false, false, false);
				ctx.Response.ContentLength64 = resText.Length;
				w.Write(resText);
			}
		}

		private static string GetMultipartBoundary(HttpListenerContext context)
		{
			try
			{
				int idx = context.Request.ContentType.IndexOf(BOUNDARY);
				string boundary = context.Request.ContentType.Substring(
															idx + BOUNDARY.Length);
				return boundary;
			}
			catch (Exception e)
			{
				string contentType = context.Request.ContentType;
				if (contentType==null)
					contentType = "(null)";

				throw new FormatException("Error finding multipart boundary. " +
											"Content-Type: " + contentType, e);
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
						break;
					case "filename":
						file.filename = parts[i].Text;
						break;
					case "filetype":
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
				throw new FormatException("object_id is missing in file"
													+ " upload multipart data");

			if (file.filename == null)
				throw new FormatException("filename is missing in file"
													+ " upload multipart data");

			if (file.fileContent == null)
				throw new FormatException("file is missing in file"
													+ " upload multipart data");

			if (file.type == null)
				throw new FormatException("filetype is missing in file"
													+ " upload multipart data");

			return file;
		}
	}


	struct FileUpload
	{
		public string filename;
		public FileType type;
		public string objectId;
		public byte[] fileContent;
		
		public bool IsValid
		{
			get
			{
				return filename != null &&
					objectId != null &&
					fileContent != null &&
					type != FileType.None;
			}
		}
	}
}
