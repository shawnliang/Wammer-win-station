using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public class AutoImporter
	{
		#region Var
		private AutoImportContentProvider _contentProvider;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ content provider.
		/// </summary>
		/// <value>The m_ content provider.</value>
		public AutoImportContentProvider m_ContentProvider
		{
			get
			{
				return _contentProvider ?? (_contentProvider = new AutoImportContentProvider());
			}
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the bitmap frame.
		/// </summary>
		/// <param name="photoFile">The photo file.</param>
		/// <returns></returns>
		private BitmapFrame GetBitmapFrame(string photoFile)
		{
			DebugInfo.ShowMethod();
			try
			{
				var decoder = BitmapDecoder.Create(new Uri(photoFile), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
				return decoder.Frames.FirstOrDefault();
			}
			catch (Exception e)
			{
				return null;
			}
		}

		/// <summary>
		/// Imports the specified contents.
		/// </summary>
		/// <param name="contents">The contents.</param>
		private void Import(IEnumerable<IContent> importContents)
		{
			DebugInfo.ShowMethod();

			if (!StreamClient.Instance.IsLogined)
				return;

			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", StreamClient.Instance.LoginedUser.SessionToken));

			if (loginedSession == null)
				return;

			var systemResourcePath = StationRegistry.GetValue("ResourceFolder", "");

			var filesToImport = new List<string>();

			foreach (var content in importContents)
			{
				try
				{
					var contentPath = content.Path;
					if (content.Path == systemResourcePath)
						continue;

					var contentLength = (new FileInfo(content.FilePath)).Length;
					if (contentLength < 20 * 1024)
						continue;

					var frame = GetBitmapFrame(content.FilePath);
					if (frame == null || frame.Height < 256 || frame.Width < 256)
						continue;

					filesToImport.Add(content.FilePath);
				}
				catch (Exception)
				{
				}
			}
			StationAPI.Import(loginedSession.session_token, loginedSession.groups.First().group_id, filesToImport);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Imports this instance.
		/// </summary>
		public void Import()
		{
			DebugInfo.ShowMethod();

			Import(m_ContentProvider.GetContents());
		}

		/// <summary>
		/// Imports the specified provider type.
		/// </summary>
		/// <param name="providerType">Type of the provider.</param>
		public void Import(ContentProviderType providerType)
		{
			DebugInfo.ShowMethod();

			Import(m_ContentProvider.GetContents(providerType));
		}


		/// <summary>
		/// Imports the specified folder.
		/// </summary>
		/// <param name="folder">The folder.</param>
		public void Import(string folder)
		{
			DebugInfo.ShowMethod();

			var contents = from file in (new DirectoryInfo(folder)).EnumerateFiles()
						   let extension = Path.GetExtension(file)
						   where extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
						   extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase) ||
						   extension.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase)
						   select (new Content(file, ContentType.Photo) as IContent);
			Import(contents);
		}
		#endregion
	}
}
