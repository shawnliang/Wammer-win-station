using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Wammer.Model;
using Wammer.Cloud;
using Microsoft.Win32;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Wammer.Station
{
	public class ImportTask:ITask
	{
		#region Const
		const String PATH_MATCH_GROUP = @"path";
		const String PATHS_MATCH_PATTERN = @"(?<" + PATH_MATCH_GROUP + @">[^\[\],]*)";
		#endregion

		#region Events
		public event EventHandler<MetadataUploadEventArgs> MetadataUploaded;
		public event EventHandler<FileImportedEventArgs> FileImported;
		public event EventHandler<ImportDoneEventArgs> ImportDone;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ paths.
		/// </summary>
		/// <value>The m_ paths.</value>
		private IEnumerable<String> m_Paths { get; set; }

		/// <summary>
		/// Gets or sets the m_ group ID.
		/// </summary>
		/// <value>The m_ group ID.</value>
		private String m_GroupID { get; set; }

		/// <summary>
		/// Gets or sets the m_ session token.
		/// </summary>
		/// <value>The m_ session token.</value>
		private String m_SessionToken { get; set; }

		/// <summary>
		/// Gets or sets the m_ API key.
		/// </summary>
		/// <value>The m_ API key.</value>
		private String m_APIKey { get; set; }

		private List<string> m_IgnorePath { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ImportTask"/> class.
		/// </summary>
		/// <param name="apiKey">The API key.</param>
		/// <param name="sessionToken">The session token.</param>
		/// <param name="groupID">The group ID.</param>
		/// <param name="paths">The paths.</param>
		public ImportTask(string apiKey, string sessionToken, string groupID, string paths)
			: this()
		{
			var ms = Regex.Matches(paths, PATHS_MATCH_PATTERN);
			if (ms.Count == 0)
				throw new ArgumentException("Invalid paths format!!", "paths");

			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
			m_Paths = from m in ms.OfType<Match>()
					  let path = m.Groups[PATH_MATCH_GROUP].Value
					  where path.Length > 0
					  select path;
		}

		public ImportTask(string apiKey, string sessionToken, string groupID, IEnumerable<string> paths)
			: this()
		{
			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
			m_Paths = paths.Where((file) => file.Length > 0);
		}

		private ImportTask()
		{
			string[] unInterestedFolders = new string[]
			{
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetEnvironmentVariable("ProgramData"),
				Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"c:\$Recycle.bin"
			};

			m_IgnorePath = new List<string>();
			m_IgnorePath.AddRange(unInterestedFolders.Where(x => !string.IsNullOrEmpty(x)));
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the type of the MIME.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private string GetMIMEType(string file)
		{
			var extension = Path.GetExtension(file);
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
			{
				if (registryKey == null)
					return null;
				var value = registryKey.GetValue("Content Type");
				return (value == null) ? "application/unknown" : value.ToString();
			}
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes this instance.
		/// </summary>
		public void Execute()
		{
			Exception error = null;

			try
			{
				var importTime = DateTime.Now;

				var allFiles = new List<ObjectIdAndPath>();
				findInterestedFiles((file) =>
				{
					var att = new ObjectIdAndPath { file_path = file, object_id = Guid.NewGuid().ToString() };
					allFiles.Add(att);
				});


				allFiles.Sort((x, y) => { return y.file_path.CompareTo(x.file_path); });

				System.Threading.ThreadPool.QueueUserWorkItem((nothing) =>
				{
					int nProcessed = 0;
					do
					{
						var batch = allFiles.Skip(nProcessed).Take(50);
						var batchMeta = extractMetadata(batch);
						enqueueUploadMetadataTask(batchMeta);

						nProcessed += batch.Count();

					} while (nProcessed < allFiles.Count);
				});


				int nProc = 0;
				do{
					var batch = allFiles.Skip(nProc).Take(50);
					submitBatch(importTime, batch);
					nProc += batch.Count();

				} while (nProc < allFiles.Count);
				
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Import not completed", e);
				error = e;
			}
			finally
			{
				raiseImportDoneEvent(error);
			}
		}

		private void submitBatch(DateTime importTime, IEnumerable<ObjectIdAndPath> batch)
		{
			var post_id = Guid.NewGuid().ToString();
			foreach (var file in batch)
				saveToStream(importTime, post_id, file);

			var objectIDs = batch.Select(x => x.object_id);
			createPostContainer(importTime, post_id, objectIDs);
		}
		#endregion

		#region Private Method
		private void createPostContainer(DateTime importTime, string postID, IEnumerable<string> objectIDs)
		{
			var parameters = new NameValueCollection()
				{
					{CloudServer.PARAM_SESSION_TOKEN, m_SessionToken},
					{CloudServer.PARAM_API_KEY, m_APIKey},
					{CloudServer.PARAM_POST_ID, postID},
					{CloudServer.PARAM_TIMESTAMP, importTime.ToCloudTimeString()},
					{CloudServer.PARAM_GROUP_ID, m_GroupID},
					{CloudServer.PARAM_ATTACHMENT_ID_ARRAY, string.Format("[{0}]",string.Join(",", objectIDs.Select((x)=> "\""+x+"\"").ToArray()))},
					{CloudServer.PARAM_CONTENT, string.Format("Import {0} files", objectIDs.Count())},
					{CloudServer.PARAM_COVER_ATTACH, objectIDs.FirstOrDefault()},
					{CloudServer.PARAM_IMPORT, "true"},
				};
			PostUploadTaskController.Instance.AddPostUploadAction(postID, PostUploadActionType.NewPost, parameters, importTime, importTime);
		}

		private void findInterestedFiles(Action<string> fileAction)
		{
			var processedDir = new HashSet<string>();

			foreach (var path in m_Paths)
			{
				if (Directory.Exists(path))
				{
					var dir = new DirectoryInfo(path);
					dir.SearchFiles(new string[] { "*.jpg", "*.jpeg" },
						// file callback
						(file) => { fileAction(file); return true; },

						// dir callback
						(folder) =>
						{
							if (processedDir.Contains(folder))
								return false;

							foreach (var skipdir in m_IgnorePath)
								if (folder.StartsWith(skipdir, StringComparison.InvariantCultureIgnoreCase))
									return false;

							processedDir.Add(folder);

							return true;
						});
				}
				else if (File.Exists(path))
				{
					var ext = Path.GetExtension(path);
					if (ext.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
						ext.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
					{
						fileAction(path);
					}
				}
			}
		}

		private void saveToStream(DateTime importTime, string postID, ObjectIdAndPath file)
		{
			try
			{
				long begin = Stopwatch.GetTimestamp();

				var imp = new AttachmentUploadHandlerImp(
					new AttachmentUploadHandlerDB(),
					new AttachmentUploadStorage(new AttachmentUploadStorageDB()));

				var postProcess = new AttachmentProcessedHandler(new AttachmentUtility());

				imp.AttachmentProcessed += postProcess.OnProcessed;


				var uploadData = new UploadData()
				{
					object_id = file.object_id,
					raw_data = new ArraySegment<byte>(File.ReadAllBytes(file.file_path)),
					file_name = Path.GetFileName(file.file_path),
					mime_type = GetMIMEType(file.file_path),
					group_id = m_GroupID,
					api_key = m_APIKey,
					session_token = m_SessionToken,
					post_id = postID,
					file_path = file.file_path,
					import_time = importTime,
					file_create_time = file.CreateTime,
					type = AttachmentType.image,
					imageMeta = ImageMeta.Origin
				};
				imp.Process(uploadData);
				
				long end = Stopwatch.GetTimestamp();
				long duration = end - begin;
				if (duration < 0)
					duration += long.MaxValue;

				Wammer.PerfMonitor.UploadDownloadMonitor.Instance.OnAttachmentProcessed(this, new HttpHandlerEventArgs(duration));
				raiseFileImportedEvent(file.file_path);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to import file: " + file.file_path, e);
			}
		} 

		private IEnumerable<FileMetadata> extractMetadata(IEnumerable<ObjectIdAndPath> files)
		{
			var timezoneDiff = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;
			var exifExtractor = new ExifExtractor();

			foreach (var file in files)
			{
				var meta = new FileMetadata
				{
					object_id = file.object_id,
					type = "image",
					file_path = file.file_path,
					file_name = Path.GetFileName(file.file_path),
					file_create_time = file.CreateTime,
					timezone = timezoneDiff,
					exif = exifExtractor.extract(file.file_path)
				};

				yield return meta;
			}
		}
		
		private void enqueueUploadMetadataTask(IEnumerable<FileMetadata> batch)
		{
			try
			{
				var serializeSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
				var batchMetadata = JsonConvert.SerializeObject(batch, Formatting.None, serializeSetting);

				var task = new UploadMetadataTask(m_GroupID, batchMetadata, batchMetadata.Count());
				task.Uploaded += new EventHandler<MetadataUploadEventArgs>(metadataUploaded);
				AttachmentUploadQueueHelper.Instance.Enqueue(task, TaskPriority.High);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("metadata upload failed", e);
			}
		}

		void metadataUploaded(object sender, MetadataUploadEventArgs e)
		{
			var handler = MetadataUploaded;
			if (handler != null)
				handler(sender, e);
		}


		private void raiseFileImportedEvent(string file)
		{
			var handler = FileImported;
			if (handler != null)
				handler(this, new FileImportedEventArgs(file));
		}

		private void raiseImportDoneEvent(Exception e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this, new ImportDoneEventArgs { Error = e });
		}
		#endregion
	}


	public class FileImportedEventArgs : EventArgs
	{
		public string FilePath { get; private set; }

		public FileImportedEventArgs(string file)
		{
			FilePath = file;
		}
	}

	public class ImportDoneEventArgs : EventArgs
	{
		public Exception Error { get; set; }
	}

	class FileMetadata
	{
		public string object_id { get; set; }
		public string type { get; set; }
		public string file_path { get; set; }
		public string file_name { get; set; }
		public DateTime file_create_time { get; set; }
		public int timezone { get; set; }
		public exif exif { get; set; }

		[JsonIgnore]
		public DateTime EventTime
		{
			get 
			{
				return (exif != null && exif.DateTimeOriginal != null) ? 
					TimeHelper.ParseGeneralDateTime(exif.DateTimeOriginal) : file_create_time;
			}
		}
	}

	internal class ObjectIdAndPath
	{
		public string object_id { get; set; }
		public string file_path { get; set; }

		private DateTime? createTime;

		public DateTime CreateTime
		{
			get
			{
				if (createTime.HasValue)
					return createTime.Value;
				else
				{
					var t = File.GetCreationTime(file_path);
					createTime = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Kind);
					return createTime.Value;
				}
			}
		}
	}
}
