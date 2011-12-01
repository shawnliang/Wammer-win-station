
using System.IO;

namespace Waveface
{
    public class GCONST
    {
        public static int GetPostOffset = 10;

        public string TempPath;
        public string CachePath;
        public string ImageUploadCachePath;

        public GCONST()
        {
            InitTempDir();
            InitCacheDir();
            InitImageUploadCacheDir();
        }

        private void InitTempDir()
        {
            TempPath = System.Windows.Forms.Application.StartupPath + "\\TempDir\\";
            Directory.CreateDirectory(TempPath);

            string[] _filePaths = Directory.GetFiles(TempPath);

            foreach (string _filePath in _filePaths)
                File.Delete(_filePath);
        }

        private void InitCacheDir()
        {
            CachePath = System.Windows.Forms.Application.StartupPath + "\\Cache\\";
            Directory.CreateDirectory(CachePath);
        }

        private void InitImageUploadCacheDir()
        {
            ImageUploadCachePath = System.Windows.Forms.Application.StartupPath + "\\ImageUploadCache\\";
            Directory.CreateDirectory(ImageUploadCachePath);
        }
    }
}
