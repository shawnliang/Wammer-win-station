
using System.IO;
using System;

namespace Waveface
{
    public class GCONST
    {
        public static int GetPostOffset = 10;
        
        public static int OriginFileReDownloadDelayTime = 3;

        public static bool ADVANCED_FEATURE = true;
        public static bool DEBUG = true;

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
            CachePath = AppDataPath + "Cache\\";
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
