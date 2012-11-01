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

namespace Wammer.Station
{
	public class ImportTask:ITask
	{
		#region Const
		const String PATH_MATCH_GROUP = @"path";
		const String PATHS_MATCH_PATTERN = @"(?<" + PATH_MATCH_GROUP + @">[^\[\],]*)";
		#endregion

		#region Events
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
		{
			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
			m_Paths = paths.Where((file) => file.Length > 0);
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
				var postID = Guid.NewGuid().ToString();
				var objectIDs = new List<string>();
				var import_time = DateTime.UtcNow;
				var interestedExtensions = new HashSet<String>() { ".jpg", ".jpeg", ".png", ".bmp" };

				var files = new List<string>();

				foreach (var path in m_Paths)
				{
					if (Directory.Exists(path))
					{
						var filesInDir = (new DirectoryInfo(path)).EnumerateFiles("*.*");
						var interestedFiles = filesInDir.Where((file) => interestedExtensions.Contains(Path.GetExtension(file).ToLower()));
						files.AddRange(interestedFiles.ToArray());
					}
					else if (File.Exists(path) && interestedExtensions.Contains(Path.GetExtension(path).ToLower()))
					{
						files.Add(path);
					}
				}
				
				foreach (var file in files)
				{
					if ((new FileInfo(file)).Length > 20 * 1024 * 1024)
						continue;

					var objectID = Guid.NewGuid().ToString();

					objectIDs.Add(objectID);

					var imp = new AttachmentUploadHandlerImp(
						new AttachmentUploadHandlerDB(),
						new AttachmentUploadStorage(new AttachmentUploadStorageDB()));

					var postProcess = new AttachmentProcessedHandler(new AttachmentUtility());

					imp.AttachmentProcessed += postProcess.OnProcessed;


					var uploadData = new UploadData()
					{
						object_id = objectID,
						raw_data = new ArraySegment<byte>(File.ReadAllBytes(file)),
						file_name = Path.GetFileName(file),
						mime_type = GetMIMEType(file),
						group_id = m_GroupID,
						api_key = m_APIKey,
						session_token = m_SessionToken,
						post_id = postID,
						file_path = file,
						import_time = importTime,
						file_create_time = File.GetCreationTime(file),
						type = AttachmentType.image,
						imageMeta = ImageMeta.Origin
					};
					imp.Process(uploadData);
					raiseFileImportedEvent(file);
				}

				var parameters = new NameValueCollection()
				{
					{CloudServer.PARAM_SESSION_TOKEN, m_SessionToken},
					{CloudServer.PARAM_API_KEY, m_APIKey},
					{CloudServer.PARAM_POST_ID, postID},
					{CloudServer.PARAM_TIMESTAMP, importTime.ToCloudTimeString()},
					{CloudServer.PARAM_GROUP_ID, m_GroupID},
					{CloudServer.PARAM_ATTACHMENT_ID_ARRAY, string.Format("[{0}]",string.Join(",",objectIDs.ToArray()))},
					{CloudServer.PARAM_CONTENT, string.Format("Import {0} files", objectIDs.Count)},
					{CloudServer.PARAM_COVER_ATTACH, objectIDs.FirstOrDefault()},
					{CloudServer.PARAM_IMPORT, "true"},
				};
				PostUploadTaskController.Instance.AddPostUploadAction(postID, PostUploadActionType.NewPost, parameters, importTime, importTime);
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
		#endregion

		#region Private Method
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
}
