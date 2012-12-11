using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using System.IO;


namespace Wammer.Station
{
	class ImportDocTask : ITask
	{
		private LoginedSession session;
		private string[] paths;
		
		public ImportDocTask(LoginedSession session, string[] paths)
		{
			this.session = session;
			this.paths = paths;
		}

		public void Execute()
		{
			var user = DriverCollection.Instance.FindOneById(session.user.user_id);
			if (user == null)
				return;

			// enumerate all docs
			List<ObjectIdAndPath> allDocs = new List<ObjectIdAndPath>();
			foreach (var path in paths)
			{
				if (Directory.Exists(path))
				{
					new DirectoryInfo(path).SearchFiles(new string[] { "*.ppt", "*.pptx", "*.pdf" },
						(file) =>
						{
							var id = new ObjectIdAndPath
							{
								file_path = file,
								object_id = Guid.NewGuid().ToString()
							};

							allDocs.Add(id);

							return true;
						});
				}
				else if (File.Exists(path))
				{
					throw new NotImplementedException();
				}
			}


			// Import to stream
			Dictionary<string, FolderCollection> folders = new Dictionary<string, FolderCollection>();
			foreach (var doc in allDocs)
			{
				try
				{
					Wammer.Station.Doc.ImportDoc.Process(user, doc.object_id, doc.file_path, File.GetLastAccessTime(doc.file_path));

					var dirName = Path.GetDirectoryName(doc.file_path);
					if (folders.ContainsKey(dirName))
					{
						folders[dirName].Objects.Add(doc.object_id);
					}
					else
					{
						folders.Add(dirName, new FolderCollection(dirName, doc.object_id));
					}
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Unable to import doc: " + doc.file_path, e);
				}
			}

			TaskQueue.Enqueue(new CreatePhotoFolderCollectionTask(folders, user.session_token, Cloud.CloudServer.APIKey), TaskPriority.Medium);
		}
	}
}
