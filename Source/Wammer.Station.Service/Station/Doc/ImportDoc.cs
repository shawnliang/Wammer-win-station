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
using MongoDB.Driver.Builders;

namespace Wammer.Station.Doc
{
	public static class ImportDoc
	{
		public static void Process(Driver user, string object_id, string file_path, DateTime accessTime)
		{
			string full_saved_file_name = "";
			MakePreviewResult previewResult = null;

			try
			{
				// copy to res folder
				var storage = new FileStorage(user);
				var saved_file_name = storage.CopyToStorage(file_path);
				full_saved_file_name = Path.Combine(storage.basePath, saved_file_name);

				previewResult = MakeDocPreview(object_id, full_saved_file_name, user.user_id);


				// write to db
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
					mime_type = MimeTypeHelper.GetMIMEType(file_path),
					modify_time = DateTime.Now,
					object_id = object_id,
					saved_file_name = saved_file_name,
					type = AttachmentType.doc,
					doc_meta = new DocProperty
					{
						file_name = Path.GetFileName(file_path),
						preview_files = previewResult.files,
						access_time = new List<DateTime> { accessTime },
						modify_time = File.GetLastWriteTime(file_path),
					}
				};
				Model.AttachmentCollection.Instance.Save(db);

				// upload previews to cloud
				var ext = Path.GetExtension(file_path).ToLower();
				if (ext.Equals(".ppt") || ext.Equals(".pptx"))
				{
					// pack previews to "Stream_Doc_Priviews.zip"
					var zip = new FastZip();
					var previewZipFile = Path.Combine("cache", object_id + ".zip");
					zip.UseZip64 = UseZip64.Off;
					zip.CreateZip(previewZipFile, previewResult.previewFolder, false, null);

					using (var zipStream = File.OpenRead(previewZipFile))
					{
						AttachmentApi.Upload(zipStream, db.group_id, db.object_id, "Stream_Doc_Previews.zip", db.mime_type, ImageMeta.None,
							 AttachmentType.doc, CloudServer.APIKey, user.session_token, 32768, null,
							 null, db.file_path, null, null, null, db.file_create_time, db.doc_meta);
					}
				}

				// upload orig doc to cloud
				AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(
					new AttachmentUpload.UpstreamTask(object_id, ImageMeta.None, TaskPriority.VeryLow),
					TaskPriority.VeryLow);


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
			catch (Exception ex)
			{
				if (Directory.Exists(previewResult.previewFolder))
					Directory.Delete(previewResult.previewFolder, true);

				if (File.Exists(full_saved_file_name))
					File.Delete(full_saved_file_name);

				AttachmentCollection.Instance.Remove(Query.EQ("_id", object_id));

				throw;
			}
			finally
			{
				var previewZip = Path.Combine("cache", object_id + ".zip");
				if (File.Exists(previewZip))
					File.Delete(previewZip);
			}
		}

		public static MakePreviewResult MakeDocPreview(string object_id, string full_saved_file_name, string user_id)
		{
			
			var userCache = Path.Combine("cache", user_id);
			var previewFolder = Path.Combine(userCache, object_id);

			// create preview folder
			if (!Directory.Exists("cache"))
				Directory.CreateDirectory("cache");
			
			if (!Directory.Exists(userCache))
				Directory.CreateDirectory(userCache);

			if (!Directory.Exists(previewFolder))
				Directory.CreateDirectory(previewFolder);

			var fullPreviewFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), previewFolder);

			// generate previews
			var ext = Path.GetExtension(full_saved_file_name).ToLower();
				
			List<string> previewFiles;
			if (ext.Equals(".ppt") || ext.Equals(".pptx"))
				previewFiles = GeneratePowerPointPreviews(full_saved_file_name, fullPreviewFolder);
			else if (ext.Equals(".pdf"))
				previewFiles = GeneratePdfPreviews(full_saved_file_name, fullPreviewFolder, 1, 1);
			else
				throw new InvalidDataException("Unknow file type: " + full_saved_file_name);
			
			previewFiles = previewFiles.Select(x => x.Substring(x.IndexOf(previewFolder))).ToList();

			return new MakePreviewResult { previewFolder = previewFolder, files = previewFiles };
		}

		public static List<string> GeneratePdfPreviews(string pdfFile, string previewFolder, int firstPageToConvert = -1, int lastPageToConvert = -1)
		{
			var converter = new PDFConvert();

			converter.OutputToMultipleFile = true;
			converter.FirstPageToConvert = firstPageToConvert;
			converter.LastPageToConvert = lastPageToConvert;
			converter.FitPage = false;
			converter.JPEGQuality = 0;
			converter.OutputFormat = "jpeg";
			converter.Convert(pdfFile, Path.Combine(previewFolder, "pdf.jpg"));

			return renameToDefinedPreviewName(Directory.GetFiles(previewFolder, "*.*"));
		}

		public static List<string> GeneratePowerPointPreviews(string pptFile, string previewFolder)
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

		private static List<string> renameToDefinedPreviewName(IEnumerable<string> previews)
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

	public class MakePreviewResult
	{
		public string previewFolder { get; set; }
		public List<string> files { get; set; }
	}
}
