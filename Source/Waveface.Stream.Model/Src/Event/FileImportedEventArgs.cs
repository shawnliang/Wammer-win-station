using System;

namespace Waveface.Stream.Model
{
	public class FileImportedEventArgs : EventArgs
	{
		public string FilePath { get; private set; }

		public FileImportedEventArgs(string file)
		{
			FilePath = file;
		}
	}
}
