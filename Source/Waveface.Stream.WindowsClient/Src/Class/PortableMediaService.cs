using System;
using System.Collections.Generic;
using System.IO;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class PortableMediaService : IPortableMediaService
	{
		public event EventHandler<FileImportedEventArgs> FileImported;

		public event EventHandler<ImportDoneEventArgs> ImportDone;

		private ImportTranscation import;

		public PortableMediaService()
		{
		}

		public IEnumerable<PortableDevice> GetPortableDevices()
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				if ((drive.DriveType == DriveType.CDRom || drive.DriveType == DriveType.Removable) &&
					drive.IsReady)
				{
					yield return new PortableDevice
					{
						DrivePath = drive.Name,
						Name = string.Format("({0}) {1}", drive.Name, drive.VolumeLabel)
					};
				}
			}
			
		}

		public IEnumerable<string> GetFileList(string path)
		{
			var jpgs = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
			foreach (var file in jpgs)
				yield return file;

			var jpegs = Directory.GetFiles(path, "*.jpeg", SearchOption.AllDirectories);
			foreach (var file in jpegs)
				yield return file;

			var pngs = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
			foreach (var file in pngs)
				yield return file;
		}

		public void ImportAsync(IEnumerable<string> files, string user_id, string session_token, string apikey)
		{
			import = new ImportTranscation(user_id, session_token, apikey, files);
			import.FileImported += import_FileImported;
			import.ImportDone += import_ImportDone;
			import.ImportFileAsync();
		}

		void import_ImportDone(object sender, ImportDoneEventArgs e)
		{
			var handler = this.ImportDone;
			if (handler != null)
				handler(this, new ImportDoneEventArgs { Error = e.Error });
		}

		void import_FileImported(object sender, FileImportedEventArgs e)
		{
			var handler = this.FileImported;
			if (handler != null)
				handler(this, new FileImportedEventArgs(e.FilePath));
		}


	}
}
