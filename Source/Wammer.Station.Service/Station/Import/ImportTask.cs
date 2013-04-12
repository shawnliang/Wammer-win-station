using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Wammer.Station.Import;
namespace Wammer.Station
{
	public class ImportTask : ITask
	{
		#region Events
		public event EventHandler<TaskStartedEventArgs> TaskStarted;
		public event EventHandler<FilesEnumeratedArgs> FilesEnumerated;
		public event EventHandler<FileImportEventArgs> FileIndexed;
		public event EventHandler<FileImportEventArgs> FileSkipped;
		public event EventHandler<FileImportEventArgs> FileImportFailed;
		public event EventHandler<FileImportEventArgs> FileImported;
		public event EventHandler<ImportDoneEventArgs> ImportDone;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ paths.
		/// </summary>
		/// <value>The m_ paths.</value>
		public IEnumerable<String> Paths { get; set; }

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

		private bool m_CopyToStation { get; set; }

		/// <summary>
		/// Gets or sets the m_ API key.
		/// </summary>
		/// <value>The m_ API key.</value>
		private String m_APIKey { get; set; }

		private SearchOption m_searchOption { get; set; }

		private int timezoneDiff;
		private List<ObjectIdAndPath> allFailedFiles = new List<ObjectIdAndPath>();
		#endregion

		#region Public Property
		public Guid TaskId { get; private set; }
		#endregion

		#region Constructor
		public ImportTask(string apiKey, string sessionToken, string groupID, IEnumerable<string> paths, bool copyToStation, SearchOption searchOption = SearchOption.TopDirectoryOnly)
			: this()
		{
			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
			m_CopyToStation = copyToStation;
			m_searchOption = searchOption;
			Paths = paths.Where((file) => file.Length > 0);
		}

		private ImportTask()
		{
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
			if (string.IsNullOrEmpty(m_APIKey) || string.IsNullOrEmpty(m_SessionToken) || string.IsNullOrEmpty(m_GroupID) || !Paths.Any())
				return;

			this.LogInfoMsg("Importing from: \n" + string.Join("\n", Paths.ToArray()));
			Exception error = null;

			try
			{
				raiseTaskStartedEvent();

				var user = DriverCollection.Instance.FindDriverByGroupId(m_GroupID);

				var importTime = DateTime.Now;


				// Scan all photo
				var photoCrawler = new PhotoCrawler();
				var inputFiles = Paths.Where(file => Path.GetExtension(file).Length > 0);
				var allFiles = photoCrawler.FindPhotos(Paths, photoCrawlerError, m_searchOption).Select(file => new ObjectIdAndPath { file_path = file, object_id = Guid.NewGuid().ToString() }).ToList();
				raiseFilesEnumeratedEvent(allFiles.Count);


				// de-dup
				allFiles = dedup(allFiles);
				allFiles.Sort((x, y) => { return File.GetLastWriteTime(y.file_path).CompareTo(File.GetLastWriteTime(x.file_path)); });


				// extract metadata, send to cloud by batch
				var unsentMeta = new List<FileMetadata>();
				var allMeta = new List<FileMetadata>();
				foreach (var file in allFiles)
				{
					var fileMeta = extractMetadata(file);

					if (m_CopyToStation)
						enqueueCopyPhotoTask(fileMeta, user);
					else
						enqueueIndexPhotoOnlyTask(fileMeta, user);


					unsentMeta.Add(fileMeta);
					allMeta.Add(fileMeta);
					if (unsentMeta.Count == 50)
					{
						enqueueUploadMetadataTask(unsentMeta);
						createPostContainer(importTime, Guid.NewGuid().ToString(), unsentMeta.Select(x => x.object_id));

						unsentMeta.Clear();
					}
				}


				if (unsentMeta.Count > 0)
				{
					enqueueUploadMetadataTask(unsentMeta);
					createPostContainer(importTime, Guid.NewGuid().ToString(), unsentMeta.Select(x => x.object_id));
				}


				// build collections
				var folderCollections = FolderCollection.Build(allMeta.Where(meta => !inputFiles.Contains(meta.file_path)).Cast<ObjectIdAndPath>());
				TaskQueue.Enqueue(new CreateFolderCollectionTask(folderCollections, m_GroupID), TaskPriority.Medium, true);


				// escalate cover photos to high priority upload queue
				var coverPhotos = folderCollections.Values.Select(col => col.Objects.FirstOrDefault());
				// TODO: escalate...
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

		private void enqueueIndexPhotoOnlyTask(FileMetadata fileMeta, Driver user)
		{
			var task = new IndexPhotoOnlyTask(fileMeta, user.user_id, TaskId);
			TaskQueue.Enqueue(task, TaskPriority.Medium, true);
		}

		private void enqueueCopyPhotoTask(FileMetadata fileMeta, Driver user)
		{
			var task = new CopyPhotoTask(fileMeta, user.user_id, TaskId);
			TaskQueue.Enqueue(task, TaskPriority.Medium, true);
		}

		private List<ObjectIdAndPath> dedup(List<ObjectIdAndPath> allFiles)
		{
			var dedupResult = new HashSet<FileDedupItem>();

			foreach (var file in allFiles)
			{
				var item = new FileDedupItem(file);

				if (hasDupItemInDB(file) || dedupResult.Contains(item))
					raiseFileSkippedEvent(file);
				else
					dedupResult.Add(item);
			}

			return dedupResult.Select(x => x.file).ToList();
		}

		private bool hasDupItemInDB(ObjectIdAndPath item)
		{
			var sameSizeFiles = AttachmentCollection.Instance.Find(
							Query.And(
								Query.EQ("group_id", m_GroupID),
								Query.EQ("file_size", new FileInfo(item.file_path).Length)));

			bool hasDup = false;
			foreach (var sameSizeFile in sameSizeFiles)
			{
				if (!string.IsNullOrEmpty(sameSizeFile.file_name) &&
					sameSizeFile.file_name.Equals(Path.GetFileName(item.file_path), StringComparison.InvariantCultureIgnoreCase))
				{
					hasDup = true;
					break;
				}
			}
			return hasDup;
		}

		#endregion

		#region Private Method

		private void photoCrawlerError(string path, Exception e)
		{
			this.LogWarnMsg("Unable to enumerate files under " + path, e);
		}

		private void createPostContainer(DateTime importTime, string postID, IEnumerable<string> objectIDs)
		{
			var parameters = new NameValueCollection()
				{
					{CloudServer.PARAM_SESSION_TOKEN, m_SessionToken},
					{CloudServer.PARAM_API_KEY, m_APIKey},
					{CloudServer.PARAM_POST_ID, postID},
					{CloudServer.PARAM_TYPE, "import"},
					{CloudServer.PARAM_TIMESTAMP, importTime.ToUTCISO8601ShortString()},
					{CloudServer.PARAM_GROUP_ID, m_GroupID},
					{CloudServer.PARAM_ATTACHMENT_ID_ARRAY, string.Format("[{0}]",string.Join(",", objectIDs.Select((x)=> "\""+x+"\"").ToArray()))},
					{CloudServer.PARAM_CONTENT, string.Format("Import {0} files", objectIDs.Count().ToString())},
					{CloudServer.PARAM_COVER_ATTACH, objectIDs.FirstOrDefault()},
				};
			PostUploadTaskController.Instance.AddPostUploadAction(postID, PostUploadActionType.NewPost, parameters, importTime, importTime);
		}

		private FileMetadata extractMetadata(ObjectIdAndPath file)
		{
			var exifExtractor = new ExifExtractor();
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

			raiseFileIndexedEvent(file);
			return meta;
		}

		private void enqueueUploadMetadataTask(IEnumerable<FileMetadata> batch)
		{
			try
			{
				var serializeSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc };
				var batchMetadata = JsonConvert.SerializeObject(batch, Formatting.None, serializeSetting);

				var task = new UploadMetadataTask(m_GroupID, batchMetadata, batch.Count());
				AttachmentUploadQueueHelper.Instance.Enqueue(task, TaskPriority.High);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("metadata upload failed", e);
			}
		}

		private void raiseImportDoneEvent(Exception e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this,
					new ImportDoneEventArgs
					{
						Error = e,
						TaskId = TaskId,
						FailedFiles = allFailedFiles.Select(x => x.file_path).ToList()
					});
		}

		private void raiseFilesEnumeratedEvent(int count)
		{
			EventHandler<FilesEnumeratedArgs> handler = FilesEnumerated;
			if (handler != null)
				handler(this, new FilesEnumeratedArgs { TotalCount = count, TaskId = TaskId });
		}

		private void raiseFileIndexedEvent(ObjectIdAndPath file)
		{
			var handler = FileIndexed;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseFileSkippedEvent(ObjectIdAndPath file)
		{
			var handler = FileSkipped;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseTaskStartedEvent()
		{
			var handler = TaskStarted;
			if (handler != null)
				handler(this, new TaskStartedEventArgs { TaskId = TaskId });
		}
		#endregion
	}

	public class TaskStartedEventArgs : EventArgs
	{
		public Guid TaskId { get; set; }
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
		public List<string> FailedFiles { get; set; }
	}

	public class FilesEnumeratedArgs : EventArgs
	{
		public Guid TaskId { get; set; }
		public int TotalCount { get; set; }
	}


	public class FileMetadata : ObjectIdAndPath
	{
		public string type { get; set; }
		public string file_name { get; set; }
		public DateTime file_create_time { get; set; }
		public int timezone { get; set; }
		public exif exif { get; set; }

		private DateTime? _event_time;

		[JsonIgnore]
		public DateTime EventTime
		{
			get
			{
				if (!_event_time.HasValue)
					_event_time = computeEventTime();

				return _event_time.Value;
			}
		}

		private DateTime computeEventTime()
		{
			return AttachmentUpload.EventTime.GuessFromExif(exif, file_create_time, timezone, file_path);
		}
	}

	[Serializable]
	public class FolderCollection
	{
		public string FolderPath { get; private set; }
		public string FolderName { get; set; }
		public List<string> Objects { get; private set; }

		public FolderCollection(string folderPath)
		{
			FolderPath = folderPath;
			FolderName = string.IsNullOrEmpty(Path.GetFileName(folderPath)) ? folderPath : Path.GetFileName(folderPath);
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

		public static Dictionary<string, FolderCollection> Build(IEnumerable<ObjectIdAndPath> files)
		{
			var folderCollections = new Dictionary<string, FolderCollection>();

			foreach (var file in files)
			{
				var folderPath = Path.GetDirectoryName(file.file_path);

				if (folderCollections.ContainsKey(folderPath))
					folderCollections[folderPath].Objects.Add(file.object_id);
				else
					folderCollections.Add(folderPath, new FolderCollection(folderPath, file.object_id));
			}

			return folderCollections;
		}
	}
}
