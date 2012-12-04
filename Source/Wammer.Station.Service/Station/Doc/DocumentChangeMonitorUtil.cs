using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Wammer.Model;
using Wammer.Utility;

using NetOffice;
using PowerPoint = NetOffice.PowerPointApi;
using NetOffice.PowerPointApi.Enums;
using NetOffice.OfficeApi.Enums;
using ICSharpCode.SharpZipLib.Zip;
using Wammer.Cloud;


namespace Wammer.Station.Doc
{
	public class DocumentChangeMonitorUtil : IDocumentChangeMonitorUtil
	{
		public DateTime GetFileWriteTime(string path)
		{
			return System.IO.File.GetLastAccessTime(path);
		}

		public void ProcessChangedDoc(MonitorItem target, DateTime fileModifyTime)
		{
			var user = Model.DriverCollection.Instance.FindOneById(target.user_id);
			if (user == null)
				return;

			var object_id = Guid.NewGuid().ToString();

			// copy to res folder
			var storage = new FileStorage(user);
			var saved_file_name = storage.CopyToStorage(target.path);

			// create preview folder
			if (!Directory.Exists("cache"))
				Directory.CreateDirectory("cache");

			var previewFolder = Path.Combine("cache", object_id);
			if (!Directory.Exists(previewFolder))
				Directory.CreateDirectory(previewFolder);

			// generate previews
			var ext = Path.GetExtension(target.path).ToLower();
			IEnumerable<string> previewPaths;
			if (ext.Equals(".ppt") || ext.Equals(".pptx"))
				previewPaths = GeneratePowerPointPreviews(saved_file_name, previewFolder);
			else if (ext.Equals(".pdf"))
				previewPaths = GeneratePdfPreviews(saved_file_name, previewFolder);
			else
				throw new InvalidDataException("Unknow file type: " + target.path);

			// pack previews to "Stream_Doc_Priviews.zip"
			var zip = new FastZip();
			var previewZipFile = Path.Combine("cache", object_id + ".zip");
			zip.CreateZip(previewZipFile, previewFolder, false, "*.jpg");

			// write to db
			string mimeType = "application/octet-stream";
			if (Path.GetExtension(target.path).Equals(".ppt", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.ms-powerpoint";
			else if (Path.GetExtension(target.path).Equals(".pptx", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
			else if (Path.GetExtension(target.path).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
				mimeType = "application/pdf";


			Attachment db = new Attachment
			{
				creator_id = user.user_id,
				device_id = StationRegistry.StationId,
				file_create_time = File.GetCreationTime(target.path),
				file_modify_time = File.GetLastWriteTime(target.path),
				file_name = Path.GetFileName(target.path),
				file_path = target.path,
				file_size = new FileInfo(target.path).Length,
				fromLocal = true,
				group_id = user.groups[0].group_id,
				md5 = MD5Helper.ComputeMD5(File.ReadAllBytes(target.path)),
				mime_type = mimeType,
				modify_time = DateTime.Now,
				object_id = object_id,
				saved_file_name = saved_file_name,
				type = AttachmentType.doc,
				doc_meta = new DocProperty
				{
					preview_files = previewPaths.ToList() 
				}
			};
			Model.AttachmentCollection.Instance.Save(db);

			// upload previews to cloud
			using (var zipStream = File.OpenRead(previewZipFile))
			{
				AttachmentApi.Upload(zipStream, db.group_id, db.object_id, db.file_name, db.mime_type, ImageMeta.None,
					 AttachmentType.doc, CloudServer.APIKey, user.session_token, 1024, null,
					 null, db.file_path, null, null, null, db.file_create_time);
			}


			// create post to cloud
			var postApi = new PostApi(user);
			postApi.NewPost(object_id, DateTime.Now, new Dictionary<string, string>
			{
				{ CloudServer.PARAM_ATTACHMENT_ID_ARRAY, "[\"" + object_id + "\"]" },
				{ CloudServer.PARAM_TYPE, "doc" },
				{ CloudServer.APIKey, CloudServer.APIKey },
				{ CloudServer.SessionToken, user.session_token},
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
			using (PowerPoint.Application powerApplication = new PowerPoint.Application())
			{
				NetOffice.PowerPointApi.Presentation prep = powerApplication.Presentations.Open(pptFile, 0, 0, MsoTriState.msoFalse);
				prep.SaveAs(previewFolder, PpSaveAsFileType.ppSaveAsJPG);
			}

			//return Directory.GetFiles(previewFolder, "*.*").OrderBy(x => x);
			return renameToDefinedPreviewName(Directory.GetFiles(previewFolder, "*.*"));
		}

		private static IEnumerable<string> renameToDefinedPreviewName(IEnumerable<string> previews)
		{
			int index = 1;
			foreach (var file in previews.OrderBy(x => x))
			{
				var newName = "preview-" + index.ToString("d10") + ".jpg";
				var dir = Path.GetDirectoryName(file);
				var newPath = Path.Combine(dir, newName);

				File.Move(file, newPath);
				yield return newPath;

				index++;
			}
		}
	}
}
