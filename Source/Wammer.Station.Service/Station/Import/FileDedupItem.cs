using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;
using System.IO;

namespace Wammer.Station.Import
{
	class FileDedupItem
	{
		private long _file_size = -1;
		private string _file_name;

		public ObjectIdAndPath file { get; set; }

		public FileDedupItem(ObjectIdAndPath file)
		{
			this.file = file;
		}

		public long file_size
		{
			get
			{
				if (_file_size < 0)
					_file_size = new FileInfo(file.file_path).Length;

				return _file_size;
			}
		}

		public override int GetHashCode()
		{
			return (int)file_size + file_name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var rhs = obj as FileDedupItem;

			if (rhs == null)
				return false;

			return file_size == rhs.file_size && file_name.Equals(rhs.file_name, StringComparison.InvariantCultureIgnoreCase);
		}

		public string file_name
		{
			get
			{
				if (_file_name == null)
					_file_name = Path.GetFileName(file.file_path);

				return _file_name;
			}
		}
	}
}
