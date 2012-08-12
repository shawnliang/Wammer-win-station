
using System.IO;
using System;
using System.Reflection;

namespace Waveface
{
    public class GCONST
    {
        private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";
        private const string KEY_PATH_W32OW64 = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Wammer\WinStation";

        #region Var
        private string _assemblyPath;
        private String _runTimeDataPath;
        #endregion

        #region Private Property
        private string AssemblyPath
        {
            get
            {
                if (_assemblyPath == null)
                    _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return _assemblyPath;
            }
        }

        public string WammerKeyPath
        {
            get
            {
                if (IntPtr.Size == 4)
                    // Runs in 32-bit mode
                    return KEY_PATH;
                else
                    // Runs in 64-bit mode, but registry is always in 32-bit mode.
                    return KEY_PATH_W32OW64;
            }
        }
        #endregion

        #region Public Property
        public String RunTimeDataPath
        {
            get
            {
                if (_runTimeDataPath == null)
                    InitRunTimeDataPath();
                return _runTimeDataPath;
            }
            private set
            {
                _runTimeDataPath = value;
            }
        }
        #endregion

        public static int GetPostOffset = 10;
        
        public static int OriginFileReDownloadDelayTime = 3;

        public static bool DEBUG = true;

        public string AppDataPath;
        public string TempPath;
        public string ImageCachePath;
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

        #region Private Method
        private void InitRunTimeDataPath()
        {
            RunTimeDataPath = AppDataPath + "Cache\\";
            Directory.CreateDirectory(RunTimeDataPath);
        }
        #endregion

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
            /*
            if (Environment.GetCommandLineArgs().Length == 1)
            {
                ImageCachePath = RunTimeDataPath;
                return;
            }
            */

            var user = _runTime.Login.user;

            ImageCachePath = ((string)Microsoft.Win32.Registry.GetValue(WammerKeyPath, "ResourceFolder", null)) ??
                            Path.Combine(AssemblyPath, "resource");

            ImageCachePath = Path.Combine(ImageCachePath, "user_" + user.user_id);

            Directory.CreateDirectory(ImageCachePath);
        }

        private void InitImageUploadCacheDir()
        {
            ImageUploadCachePath = AppDataPath + "ImageUploadCache\\";
            Directory.CreateDirectory(ImageUploadCachePath);
        }
    }
}
