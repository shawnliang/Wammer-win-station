using ICSharpCode.SharpZipLib.Zip;
using NetOffice.OfficeApi.Enums;
using NetOffice.PowerPointApi.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using PowerPoint = NetOffice.PowerPointApi;

namespace Wammer.Station.Doc
{
	public static class ImportDoc
	{
		public static void Process(Driver user, string object_id, string file_path, DateTime accessTime)
		{
			// copy to res folder
			var storage = new FileStorage(user);
			var saved_file_name = storage.CopyToStorage(file_path);
			var full_saved_file_name = Path.Combine(storage.basePath, saved_file_name);

			// create preview folder
			if (!Directory.Exists("cache"))
				Directory.CreateDirectory("cache");

			var previewFolder = Path.Combine("cache", object_id);
			if (!Directory.Exists(previewFolder))
				Directory.CreateDirectory(previewFolder);
			var fullPreviewFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), previewFolder);

			// generate previews
			var ext = Path.GetExtension(file_path).ToLower();
			IEnumerable<string> previewPaths;
			if (ext.Equals(".ppt") || ext.Equals(".pptx"))
				previewPaths = GeneratePowerPointPreviews(full_saved_file_name, fullPreviewFolder);
			else if (ext.Equals(".pdf"))
				previewPaths = GeneratePdfPreviews(full_saved_file_name, fullPreviewFolder);
			else
				throw new InvalidDataException("Unknow file type: " + file_path);


			previewPaths = previewPaths.Select(x => x.Substring(x.IndexOf(previewFolder)));

			// pack previews to "Stream_Doc_Priviews.zip"
			var zip = new FastZip();
			var previewZipFile = Path.Combine("cache", object_id + ".zip");
			zip.UseZip64 = UseZip64.Off;
			zip.CreateZip(previewZipFile, previewFolder, false, null);


			// write to db
			string mimeType = "application/octet-stream";
			if (Path.GetExtension(file_path).Equals(".ppt", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.ms-powerpoint";
			else if (Path.GetExtension(file_path).Equals(".pptx", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
			else if (Path.GetExtension(file_path).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/pdf";


			Attachment db = new Attachment
			{
				creator_id = user.user_id,
				device_id = StationRegistry.StationId,
				file_create_time = File.GetCreationTime(file_path),
				file_modify_time = File.GetLastWriteTime(file_path),
				file_name = Path.GetFileName(file_path),
				file_path = file_path,
				file_size = new FileInfo(file_path).Length,
				fromLocal = true,
				group_id = user.groups[0].group_id,
				md5 = MD5Helper.ComputeMD5(File.ReadAllBytes(file_path)),
				mime_type = mimeType,
				modify_time = DateTime.Now,
				object_id = object_id,
				saved_file_name = saved_file_name,
				type = AttachmentType.doc,
				doc_meta = new DocProperty
				{
					file_name = Path.GetFileName(file_path),
					preview_files = previewPaths.ToList(),
					access_time = accessTime,
					modify_time = File.GetLastWriteTime(file_path),
				}
			};
			Model.AttachmentCollection.Instance.Save(db);

			// upload previews to cloud
			using (var zipStream = File.OpenRead(previewZipFile))
			{
				AttachmentApi.Upload(zipStream, db.group_id, db.object_id, "Stream_Doc_Previews.zip", db.mime_type, ImageMeta.None,
					 AttachmentType.doc, CloudServer.APIKey, user.session_token, 1024, null,
					 null, db.file_path, null, null, null, db.file_create_time, db.doc_meta);
			}


			// create post to cloud
			var postApi = new PostApi(user);
			postApi.NewPost(object_id, DateTime.Now, new Dictionary<string, string>
			{
				{ CloudServer.PARAM_ATTACHMENT_ID_ARRAY, "[\"" + object_id + "\"]" },
				{ CloudServer.PARAM_TYPE, "doc" },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey },
				{ CloudServer.PARAM_SESSION_TOKEN, user.session_token},
				{ CloudServer.PARAM_GROUP_ID, user.groups[0].group_id},
				{ CloudServer.PARAM_IMPORT, "true"}
			});
		}

		public static IEnumerable<string> GeneratePdfPreviews(string pdfFile, string previewFolder)
		{
			var converter = new PDFConvert();

			converter.OutputToMultipleFile = true;
			converter.FirstPageToConvert = converter.LastPageToConvert = -1;
			converter.FitPage = false;
			converter.JPEGQuality = 0;
			converter.OutputFormat = "jpeg";
			converter.Convert(pdfFile, Path.Combine(previewFolder, "pdf.jpg"));

			//return Directory.GetFiles(previewFolder, "*.*").OrderBy(x => x);

			return renameToDefinedPreviewName(Directory.GetFiles(previewFolder, "*.*"));
		}

		public static IEnumerable<string> GeneratePowerPointPreviews(string pptFile, string previewFolder)
		{
			var file = pptFile;
			var folder = previewFolder;

			if (!Path.IsPathRooted(pptFile))
				file = Path.Combine(Environment.CurrentDirectory, pptFile);

			if (!Path.IsPathRooted(previewFolder))
				folder = Path.Combine(Environment.CurrentDirectory, previewFolder);

			using (PowerPoint.Application powerApplication = new PowerPoint.Application())
			{
				NetOffice.PowerPointApi.Presentation prep = powerApplication.Presentations.Open(file, 0, 0, MsoTriState.msoFalse);
				prep.SaveAs(folder, PpSaveAsFileType.ppSaveAsJPG);
			}

			return renameToDefinedPreviewName(Directory.GetFiles(previewFolder, "*.*"));
		}

		private static IEnumerable<string> renameToDefinedPreviewName(IEnumerable<string> previews)
		{
			List<string> rets = new List<string>();

			int index = 1;
			foreach (var file in previews.OrderBy(x => File.GetCreationTime(x)))
			{
				var newName = index.ToString("d8") + ".jpg";
				var dir = Path.GetDirectoryName(file);
				var newPath = Path.Combine(dir, newName);

				File.Move(file, newPath);
				rets.Add(newPath);
				index++;
			}

			return rets;
		}
	}
}
