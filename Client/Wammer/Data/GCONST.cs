
using System.IO;
using System;
using System.Reflection;

namespace Waveface
{
    public class GCONST
    {
		private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";

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
			if (Environment.GetCommandLineArgs().Length == 1)
			{
				ImageCachePath = RunTimeDataPath;
				return;
			}

			var user = _runTime.Login.user;

			ImageCachePath = ((string)Microsoft.Win32.Registry.GetValue(KEY_PATH,"resourceBasePath", null)) ?? "resource";

			ImageCachePath = Path.Combine(AssemblyPath, ImageCachePath);
			ImageCachePath = Path.Combine(ImageCachePath, "user_" + user.user_id);

            //Directory.CreateDirectory(ImageCachePath);

			//string[] _filePaths = Directory.GetFiles(ImageCachePath);

            //foreach (string _filePath in _filePaths)
            //    File.Delete(_filePath);      
        }

        private void InitImageUploadCacheDir()
        {
            ImageUploadCachePath = AppDataPath + "ImageUploadCache\\";
            Directory.CreateDirectory(ImageUploadCachePath);

			//string[] _filePaths = Directory.GetFiles(ImageUploadCachePath);

            //foreach (string _filePath in _filePaths)
            //    File.Delete(_filePath);
        }
    }
}
