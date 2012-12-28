using Microsoft.Win32;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
namespace Wammer.Station
{
	public class ImportTask : ITask
	{
		#region Const
		const String PATH_MATCH_GROUP = @"path";
		const String PATHS_MATCH_PATTERN = @"(?<" + PATH_MATCH_GROUP + @">[^\[\],]*)";
		#endregion

		#region Events
		public event EventHandler<MetadataUploadEventArgs> MetadataUploaded;
		public event EventHandler<FilesEnumeratedArgs> FilesEnumerated;
		public event EventHandler<FileImportEventArgs> FileImportFailed;
		public event EventHandler<FileImportEventArgs> FileImported;
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

		private int timezoneDiff;
		#endregion

		#region Public Property
		public Guid TaskId { get; private set; }
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

			timezoneDiff = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

			TaskId = Guid.NewGuid();
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes this instance.
		/// </summary>
		public void Execute()
		{
			this.LogInfoMsg("Importing from: " + string.Join(", ", m_Paths.ToArray()));
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
				allFiles = filterDuplicateFiles(allFiles);
				raiseFilesEnumeratedEvent(allFiles.Count);
			
				var allSavedFiles = new List<ObjectIdAndPath>();
				var allFailedFiles = new List<ObjectIdAndPath>();

				int nProc = 0;
				do
				{
					var batch = allFiles.Skip(nProc).Take(50);
					var saved = submitBatch(importTime, batch, ref allFailedFiles);

					var meta = extractMetadata(saved);
					enqueueUploadMetadataTask(meta);

					allSavedFiles.AddRange(saved);

					nProc += batch.Count();

				} while (nProc < allFiles.Count);


				var folderCollections = buildFolderCollections(allSavedFiles);
				TaskQueue.Enqueue(new CreateFolderCollectionTask(folderCollections, m_SessionToken, m_APIKey), TaskPriority.Medium);
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

		private static Dictionary<string, FolderCollection> buildFolderCollections(List<ObjectIdAndPath> allSavedFiles)
		{
			var folderCollections = new Dictionary<string, FolderCollection>();
			foreach (var file in allSavedFiles)
			{
				var folderPath = Path.GetDirectoryName(file.file_path);

				if (folderCollections.ContainsKey(folderPath))
					folderCollections[folderPath].Objects.Add(file.object_id);
				else
					folderCollections.Add(folderPath, new FolderCollection(folderPath, file.object_id));
			}
			return folderCollections;
		}

		private List<ObjectIdAndPath> filterDuplicateFiles(List<ObjectIdAndPath> allFiles)
		{
			var notDupFiles = new List<ObjectIdAndPath>();
			foreach (var item in allFiles)
			{
				var sameSizeFiles = AttachmentCollection.Instance.Find(
					Query.And(
						Query.EQ("group_id", m_GroupID),
						Query.EQ("file_size", new FileInfo(item.file_path).Length) ) );

				bool hasDup = false;
				foreach (var sameSizeFile in sameSizeFiles)
				{
					if (sameSizeFile.file_path.Equals(item.file_path, StringComparison.CurrentCultureIgnoreCase) ||
						sameSizeFile.file_name.Equals(Path.GetFileName(item.file_path), StringComparison.InvariantCultureIgnoreCase))
					{
						hasDup = true;
						break;
					}
				}

				if (!hasDup)
					notDupFiles.Add(item);
			}

			return notDupFiles;
		}

		private List<ObjectIdAndPath> submitBatch(DateTime importTime, IEnumerable<ObjectIdAndPath> batch, ref List<ObjectIdAndPath> failed)
		{
			List<ObjectIdAndPath> saved = new List<ObjectIdAndPath>();

			var post_id = Guid.NewGuid().ToString();
			foreach (var file in batch)
			{
				try
				{
					saveToStream(importTime, post_id, file);
					saved.Add(file);
				}
				catch (Exception e)
				{
					file.Error = e.Message;
					failed.Add(file);
					raiseFileImportFailedEvent(file);
				}
			}

			var objectIDs = saved.Select(x => x.object_id);
			createPostContainer(importTime, post_id, objectIDs);

			return saved;
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
					{CloudServer.PARAM_TYPE, "image"},
					{CloudServer.PARAM_TIMESTAMP, importTime.ToUTCISO8601ShortString()},
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
				mime_type = MimeTypeHelper.GetMIMEType(file.file_path),
				group_id = m_GroupID,
				api_key = m_APIKey,
				session_token = m_SessionToken,
				post_id = postID,
				file_path = file.file_path,
				import_time = importTime,
				file_create_time = file.CreateTime,
				type = AttachmentType.image,
				imageMeta = ImageMeta.Origin,
				fromLocal = true,
				timezone = timezoneDiff
			};
			imp.Process(uploadData);

			SystemEventSubscriber.Instance.TriggerAttachmentArrivedEvent(file.object_id);

			long end = Stopwatch.GetTimestamp();
			long duration = end - begin;
			if (duration < 0)
				duration += long.MaxValue;

			Wammer.PerfMonitor.UploadDownloadMonitor.Instance.OnAttachmentProcessed(this, new HttpHandlerEventArgs(duration));
			raiseFileImportedEvent(file);
		}

		private IEnumerable<FileMetadata> extractMetadata(IEnumerable<ObjectIdAndPath> files)
		{
			var exifExtractor = new ExifExtractor();

			foreach (var file in files)
			{
				var att = AttachmentCollection.Instance.FindOneById(file.object_id);

				var meta = new FileMetadata
				{
					object_id = file.object_id,
					type = "image",
					file_path = file.file_path,
					file_name = Path.GetFileName(file.file_path),
					file_create_time = file.CreateTime,
					timezone = timezoneDiff,
					exif = att.image_meta.exif
				};

				yield return meta;
			}
		}

		private void enqueueUploadMetadataTask(IEnumerable<FileMetadata> batch)
		{
			try
			{
				var serializeSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc };
				var batchMetadata = JsonConvert.SerializeObject(batch, Formatting.None, serializeSetting);

				var task = new UploadMetadataTask(m_GroupID, batchMetadata, batch.Count());
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


		private void raiseFileImportedEvent(ObjectIdAndPath file)
		{
			var handler = FileImported;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseImportDoneEvent(Exception e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this, new ImportDoneEventArgs { Error = e, TaskId = TaskId });
		}

		private void raiseFilesEnumeratedEvent(int count)
		{
			EventHandler<FilesEnumeratedArgs> handler = FilesEnumerated;
			if (handler != null)
				handler(this, new FilesEnumeratedArgs { TotalCount = count, TaskId = TaskId });
		}

		private void raiseFileImportFailedEvent(ObjectIdAndPath file)
		{
			EventHandler<FileImportEventArgs> handler = FileImportFailed;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}
		#endregion
	}


	public class FileImportEventArgs : EventArgs
	{
		public ObjectIdAndPath file { get; set; }
		public Guid TaskId { get; set; }
	}

	public class ImportDoneEventArgs : EventArgs
	{
		public Guid TaskId { get; set; }
		public Exception Error { get; set; }
	}

	public class FilesEnumeratedArgs : EventArgs
	{
		public Guid TaskId { get; set; }
		public int TotalCount { get; set; }
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

		private uint getRationalValue(object[] rational)
		{
			var value = Convert.ToUInt32(rational[0]) / Convert.ToUInt32(rational[1]);
			return value;
		}

		[JsonIgnore]
		public DateTime EventTime
		{
			get
			{
				if (exif != null)
				{
					if (exif.gps != null && !string.IsNullOrEmpty(exif.gps.GPSDateStamp) && exif.gps.GPSTimeStamp != null)
					{
						var eventTime = DateTime.ParseExact(exif.gps.GPSDateStamp, "yyyy:MM:dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

						var hour = getRationalValue(exif.gps.GPSTimeStamp[0]);
						var min = getRationalValue(exif.gps.GPSTimeStamp[1]);
						var sec = getRationalValue(exif.gps.GPSTimeStamp[2]);

						return eventTime.AddHours((double)hour).AddMinutes((double)min).AddSeconds((double)sec);
					}

					if (exif.DateTimeOriginal != null)
						return TimeHelper.ParseGeneralDateTime(exif.DateTimeOriginal);

					if (exif.DateTimeDigitized != null)
						return TimeHelper.ParseGeneralDateTime(exif.DateTimeDigitized);

					if (exif.DateTime != null)
						return TimeHelper.ParseGeneralDateTime(exif.DateTime);
				}

				return file_create_time;
			}
		}
	}

	[Serializable]
	public class FolderCollection
	{
		public string FolderPath { get; private set; }
		public string FolderName { get; private set; }
		public List<string> Objects { get; private set; }

		public FolderCollection(string folderPath)
		{
			FolderPath = folderPath;
			FolderName = Path.GetFileName(folderPath);
			Objects = new List<string>();
		}

		public FolderCollection(string folderPath, string object_id)
			: this(folderPath)
		{
			Objects.Add(object_id);
		}

		public override int GetHashCode()
		{
			return FolderPath.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return FolderPath.Equals(obj);
		}
	}
}
