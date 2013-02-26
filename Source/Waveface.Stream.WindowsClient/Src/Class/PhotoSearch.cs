using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class PhotoSearch : IPhotoSearch
	{
		private List<string> ignorePaths = new List<string>();
		private Dictionary<string, PathAndPhotoCount> _interestedPaths;
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

				crawler.FindPhotoDirs(path, (folder, count) => { return folderFound(folder, count); });
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(typeof(PhotoSearch)).Warn("Unable to file photo dir: " + path, e);
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

		public void ImportToStationAsync(IEnumerable<string> paths, string session_token, bool copyToStation = true)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;

			StationAPI.ImportPhoto(session_token, groupID, paths, copyToStation);
		}



		public string GetUserFolder(string user_id)
		{
			var user = DriverCollection.Instance.FindOneById(user_id);

			return user.folder;
		}
	}
}
