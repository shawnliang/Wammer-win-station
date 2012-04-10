
using System.IO;
using System;

namespace Waveface
{
    public class GCONST
    {
		private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";
        public static int GetPostOffset = 10;
        
        public static int OriginFileReDownloadDelayTime = 3;

        public static bool DEBUG = true;

        public string AppDataPath;
        public string TempPath;
        public string CachePath;
        public string ImageUploadCachePath;
		private RunTime _runTime;

        public GCONST(RunTime runTime)
        {
			this._runTime = runTime;
            InitAppDataPath();
            InitTempDir();
            InitCacheDir();
            InitImageUploadCacheDir();
        }

        private void InitAppDataPath()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Waveface\\";

            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
        }

        private void InitTempDir()
        {
            TempPath = AppDataPath + "TempDir\\";
            Directory.CreateDirectory(TempPath);

            string[] _filePaths = Directory.GetFiles(TempPath);

            foreach (string _filePath in _filePaths)
                File.Delete(_filePath);
        }



        private void InitCacheDir()
        {
			var user = _runTime.Login.user;

			CachePath = ((string)Microsoft.Win32.Registry.GetValue(KEY_PATH,"resourceBasePath", null)) ?? "resource";

			CachePath = Path.Combine(CachePath, "user_" + user.user_id);

            Directory.CreateDirectory(CachePath);

            string[] _filePaths = Directory.GetFiles(CachePath);

            //foreach (string _filePath in _filePaths)
            //    File.Delete(_filePath);      
        }

        private void InitImageUploadCacheDir()
        {
            ImageUploadCachePath = AppDataPath + "ImageUploadCache\\";
            Directory.CreateDirectory(ImageUploadCachePath);

            string[] _filePaths = Directory.GetFiles(ImageUploadCachePath);

            //foreach (string _filePath in _filePaths)
            //    File.Delete(_filePath);
        }
    }
}
