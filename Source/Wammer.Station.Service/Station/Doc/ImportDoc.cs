using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wammer.Cloud;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Station.Doc
{
	public static class ImportDoc
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ImportDoc));

		public static void Process(Driver user, string object_id, string file_path, DateTime accessTime)
		{
			AttachmentSaveResult saveResult = null;
			MakePreviewResult previewResult = null;

			try
			{
				// copy to res folder
				var storage = new AttachmentUploadStorage(new AttachmentUploadStorageDB());
				var fileInfo = new UploadData
								{
									type = AttachmentType.doc,
									imageMeta = ImageMeta.None,
									object_id = object_id,
									group_id = user.groups[0].group_id,
									file_name = Path.GetFileName(file_path),
									file_create_time = File.GetCreationTime(file_path),
									raw_data = new ArraySegment<byte>(File.ReadAllBytes(file_path))
								};
				saveResult = storage.Save(fileInfo, null);

				// generate previews
				previewResult = MakeDocPreview(object_id, saveResult.FullPath, user.user_id);


				// write to db
				Attachment db = new Attachment
				{
					creator_id = user.user_id,
					device_id = StationRegistry.StationId,
					file_create_time = File.GetCreationTime(file_path),
					file_name = Path.GetFileName(file_path),
					file_path = file_path,
					file_size = new FileInfo(file_path).Length,
					fromLocal = true,
					group_id = user.groups[0].group_id,
					MD5 = MD5Helper.ComputeMD5(File.ReadAllBytes(file_path)),
					mime_type = MimeTypeHelper.GetMIMEType(file_path),
					modify_time = DateTime.Now,
					object_id = object_id,
					saved_file_name = saveResult.RelativePath,
					type = AttachmentType.doc,
					doc_meta = new DocProperty
					{
						file_name = Path.GetFileName(file_path),
						preview_files = previewResult.IsSuccess() ? previewResult.files : null,
						access_time = new List<DateTime> { accessTime },
						modify_time = File.GetLastWriteTime(file_path),
						preview_pages = previewResult.IsSuccess() ? previewResult.files.Count : 0
					}
				};
				AttachmentCollection.Instance.Save(db);

				// upload metadata to cloud
				var metas = new List<object>{
					new {
						object_id = object_id,
						file_create_time = db.file_create_time.Value.TrimToSec(),
						file_path = db.file_path,
						file_name = db.file_name,
						group_id = user.groups[0].group_id,
						type = "doc",
						doc_meta = new {
							file_name = db.doc_meta.file_name,
							access_time = db.doc_meta.access_time.Select(x => x.TrimToSec()).ToList(),
							modify_time = db.doc_meta.modify_time.TrimToSec()
						}
					}
				};

				var serializeSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc };
				var batchMetadata = JsonConvert.SerializeObject(metas, Formatting.None, serializeSetting);

				AttachmentUploadQueueHelper.Instance.Enqueue(
					new UploadMetadataTask
					{
						group_id = user.groups[0].group_id,
						metaCount = 1,
						metadata = batchMetadata
					}, TaskPriority.High);


				// upload orig doc to cloud
				AttachmentUploadQueueHelper.Instance.Enqueue(
					new AttachmentUpload.UpstreamTask(object_id, ImageMeta.None, TaskPriority.VeryLow),
					TaskPriority.VeryLow);


				// create post to cloud
				var postApi = new PostApi(user);
				postApi.NewPost(object_id, DateTime.Now, new Dictionary<string, string>
				{
					{ CloudServer.PARAM_ATTACHMENT_ID_ARRAY, "[\"" + object_id + "\"]" },
					{ CloudServer.PARAM_TYPE, "import" },
					{ CloudServer.PARAM_API_KEY, CloudServer.APIKey },
					{ CloudServer.PARAM_SESSION_TOKEN, user.session_token},
					{ CloudServer.PARAM_GROUP_ID, user.groups[0].group_id},
				});
			}
			catch (Exception ex)
			{
				if (previewResult != null)
				{
					if (Directory.Exists(previewResult.previewFolder))
						Directory.Delete(previewResult.previewFolder, true);
				}

				if (saveResult != null && !string.IsNullOrEmpty(saveResult.FullPath))
				{
					if (File.Exists(saveResult.FullPath))
						File.Delete(saveResult.FullPath);
				}

				AttachmentCollection.Instance.Remove(Query.EQ("_id", object_id));

				throw;
			}
		}

		private static bool IsPowerPoint(string file_path)
		{
			bool IsPowerPoint = false;

			var ext = Path.GetExtension(file_path).ToLower();
			if (ext.Equals(".ppt") || ext.Equals(".pptx"))
			{
				IsPowerPoint = true;
			}
			return IsPowerPoint;
		}



		public static MakePreviewResult MakeDocPreview(string object_id, string full_saved_file_name, string user_id)
		{
			try
			{
				var userCache = FileStorage.GetCachePath(user_id);
				var previewFolder = Path.Combine(userCache, object_id);

				// create preview folder
				if (!Directory.Exists(previewFolder))
					Directory.CreateDirectory(previewFolder);

				var fullPreviewFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), previewFolder);

				// generate previews
				List<string> previewFiles;
				if (IsPowerPoint(full_saved_file_name))
					previewFiles = PptConvert.ConvertPptToJpg(full_saved_file_name, fullPreviewFolder);
				else
					previewFiles = GeneratePdfPreviews(full_saved_file_name, fullPreviewFolder, 1, 1);


				previewFiles = previewFiles.Select(x => x.Substring(x.IndexOf(previewFolder))).ToList();

				return new MakePreviewResult { previewFolder = previewFolder, files = previewFiles };
			}
			catch (Exception e)
			{
				logger.Warn("Unable to create preview for " + full_saved_file_name, e);
				return new MakePreviewResult { files = new List<string>(), previewFolder = "" };
			}
		}

		private static object pdfLock = new object();

		public static List<string> GeneratePdfPreviews(string pdfFile, string previewFolder, int firstPageToConvert = -1, int lastPageToConvert = -1)
		{
			lock (pdfLock)
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

		public bool IsSuccess()
		{
			return files != null && files.Count > 0;
		}
	}
}
