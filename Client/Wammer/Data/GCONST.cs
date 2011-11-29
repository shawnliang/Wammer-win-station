
using System.IO;
using System;

namespace Waveface
{
	public class GCONST
	{
		public static int GetPostOffset = 10;

		public string AppDataPath;
		public string TempPath;
		public string CachePath;
		public string ImageUploadCachePath;

		public GCONST()
		{
			InitAppDataPath();
			InitTempDir();
			InitCacheDir();
			InitImageUploadCacheDir();
		}

		private void InitAppDataPath()
		{
			AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Waveface");
			if (!Directory.Exists(AppDataPath))
				Directory.CreateDirectory(AppDataPath);
		}

		private void InitTempDir()
		{
			TempPath = Path.Combine(AppDataPath, "TempDir");
			Directory.CreateDirectory(TempPath);

			string[] _filePaths = Directory.GetFiles(TempPath);

			foreach (string _filePath in _filePaths)
				File.Delete(_filePath);
		}

		private void InitCacheDir()
		{
			CachePath = Path.Combine(AppDataPath, "Cache");
			Directory.CreateDirectory(CachePath);
		}

		private void InitImageUploadCacheDir()
		{
			ImageUploadCachePath = Path.Combine(AppDataPath, "ImageUploadCache");
			Directory.CreateDirectory(ImageUploadCachePath);
		}
	}
}
