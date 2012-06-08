
using System.IO;
using System;
using System.Reflection;

namespace Waveface
{
    public class GCONST
    {
		private const string KEY_PATH = @"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation";

		#region Var

		private string m_assemblyPath;
		private String m_runTimeDataPath;

		#endregion

		#region Private Property
		
        private string AssemblyPath
		{
			get
			{
				if (m_assemblyPath == null)
					m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				
                return m_assemblyPath;
			}
		}

		#endregion

		#region Public Property

		public String RunTimeDataPath
		{
			get
			{
				if (m_runTimeDataPath == null)
					InitRunTimeDataPath();

				return m_runTimeDataPath;
			}
			private set
			{
				m_runTimeDataPath = value;
			}
		}

		#endregion

		public static int GetPostOffset = 10;
        
        public static int OriginFileReDownloadDelayTime = 2;

        public static bool DEBUG = true;

        public string AppDataPath;
        public string TempPath;
        public string ImageCachePath;
        public string ImageUploadCachePath;
		
        private RunTime m_runTime;

        public GCONST(RunTime runTime)
        {
			m_runTime = runTime;
            
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

			var user = m_runTime.Login.user;

			ImageCachePath = ((string)Microsoft.Win32.Registry.GetValue(KEY_PATH,"resourceBasePath", null)) ?? "resource";

			ImageCachePath = Path.Combine(AssemblyPath, ImageCachePath);
			ImageCachePath = Path.Combine(ImageCachePath, "user_" + user.user_id);

            Directory.CreateDirectory(ImageCachePath);

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
