
using System.IO;

namespace Waveface
{
    public class GCONST
    {
        public string TempPath;
        public string CachePath;

        public GCONST()
        {
            InitTempDir();
            InitCacheDir();
        }

        private void InitTempDir()
        {
            TempPath = System.Windows.Forms.Application.StartupPath + "\\Temp\\";
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
    }
}
