using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class PhotoSearch : IPhotoSearch
	{
		private List<string> ignorePaths = new List<string>();
		private Dictionary<string, PathAndPhotoCount> _interestedPaths;
		private BackgroundWorker backgroundWorker1;
		private object cs = new object();

		public delegate void pathFoundDelegate(string path, int photoCount);

		public PhotoSearch()
		{
			var paths = new string[] {
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetEnvironmentVariable("ProgramData"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Environment.GetEnvironmentVariable("ProgramW6432"),
				@"c:\$Recycle.bin"
			};

			ignorePaths.AddRange(paths.Where(x => !string.IsNullOrEmpty(x)));
		}


		public IEnumerable<PathAndPhotoCount> InterestedPaths
		{
			get
			{
				lock (cs)
				{ 
					return m_InterestedPaths.Values.ToList(); 
				}
			}
		}

		/// <summary>
		/// Gets the m_ interested paths.
		/// </summary>
		/// <value>The m_ interested paths.</value>
		private Dictionary<string, PathAndPhotoCount> m_InterestedPaths
		{
			get
			{
				return _interestedPaths ?? (_interestedPaths = new Dictionary<string, PathAndPhotoCount>());
			}
		}

		public void StartSearchAsync()
		{
			backgroundWorker1 = new BackgroundWorker();
			backgroundWorker1.DoWork += backgroundWorker1_DoWork;
			backgroundWorker1.RunWorkerAsync();
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;

			var drives = DriveInfo.GetDrives();

			PhotoCrawler crawler = new PhotoCrawler();

			foreach (var drive in drives)
			{
				try
				{
					if (drive.DriveType == DriveType.Fixed)
						crawler.FindPhotoDirs(drive.Name, (path, count) => AddInterestedPath(path, count));
				}
				catch (Exception err)
				{
				}
			}
		}

		/// <summary>
		/// Searches valid photos with jpg/jpeg extension from a path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="folderFound">custom action when a path containing photo is found</param>
		/// <returns>photo count under path and its subdirectories</returns>
		public void Search(string path, PhotoFolderFound folderFound = null)
		{
			try
			{
				PhotoCrawler crawler = new PhotoCrawler();

				crawler.FindPhotoDirs(path, (folder, count) => folderFound(folder, count));
			}
			catch
			{
			}
		}

		private void AddInterestedPath(String path, int photoCount)
		{

			var parts = path.Split('\\');
			var abbrevatedPath = string.Join("\\", parts.Take(Math.Min(parts.Length, 4)).ToArray());

			lock (cs)
			{
				if (m_InterestedPaths.ContainsKey(abbrevatedPath))
					m_InterestedPaths[abbrevatedPath].photoCount += photoCount;
				else
					m_InterestedPaths.Add(abbrevatedPath, new PathAndPhotoCount(abbrevatedPath, photoCount));
			}
		}

		public void ImportToStationAsync(IEnumerable<string> paths, string session_token)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;
			
			StationAPI.ImportPhoto(session_token, groupID, paths);
		}

	}
}
