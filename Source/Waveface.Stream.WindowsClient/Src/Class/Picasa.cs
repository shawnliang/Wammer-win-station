using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
	public static class Picasa
	{
		#region Const
		const string PICASA_DB_RELATIVED_STORAGE_PATH = @"Google\Picasa2\db3";
		const string ALBUM_PATH_PMP_FILENAME = "albumdata_filename.pmp";
		#endregion

		#region Static Var
		private static string _picasaDBStoragePath;
		private static string _albumPathPMPFileName;
		#endregion


		#region Private Static Property
		/// <summary>
		/// Gets the m_ picasa DB storage path.
		/// </summary>
		/// <value>The m_ picasa DB storage path.</value>
		private static string m_PicasaDBStoragePath
		{
			get
			{
				return _picasaDBStoragePath ??
					(_picasaDBStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PICASA_DB_RELATIVED_STORAGE_PATH));
			}
		}

		/// <summary>
		/// Gets the name of the m_ album path PMP file.
		/// </summary>
		/// <value>The name of the m_ album path PMP file.</value>
		private static string m_AlbumPathPMPFileName
		{
			get
			{
				return _albumPathPMPFileName ??
					(_albumPathPMPFileName = Path.Combine(m_PicasaDBStoragePath, ALBUM_PATH_PMP_FILENAME));
			}
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Checks the picasa format.
		/// </summary>
		/// <param name="file">The file.</param>
		private static void CheckPicasaFormat(string file)
		{
			using (var br = new BinaryReader(File.OpenRead(file)))
			{
				CheckPicasaFormat(br);
			}
		}

		/// <summary>
		/// Checks the picasa format.
		/// </summary>
		/// <param name="br">The br.</param>
		private static void CheckPicasaFormat(BinaryReader br)
		{
			if (!IsValidPicasaFormat(br))
				throw new FileFormatException("Incorrect picasa file format.");
		}

		/// <summary>
		/// Determines whether [is valid picasa format] [the specified file].
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid picasa format] [the specified file]; otherwise, <c>false</c>.
		/// </returns>
		private static Boolean IsValidPicasaFormat(string file)
		{
			using (var br = new BinaryReader(File.OpenRead(file)))
			{
				return IsValidPicasaFormat(br);
			}
		}

		/// <summary>
		/// Determines whether [is valid picasa format] [the specified br].
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid picasa format] [the specified br]; otherwise, <c>false</c>.
		/// </returns>
		private static Boolean IsValidPicasaFormat(BinaryReader br)
		{
			if (br == null)
				throw new ArgumentNullException("br");

			var position = br.BaseStream.Position;
			try
			{
				br.BaseStream.Seek(0, SeekOrigin.Begin);
				var magic = br.ReadBytes(4);
				if (magic[0] != 0xcd ||
					magic[1] != 0xcc ||
					magic[2] != 0xcc ||
					magic[3] != 0x3f)
				{
					return false;
				}

				var type = br.ReadInt16();

				if (0x1332 != br.ReadInt16())
				{
					return false;
				}

				if (0x00000002 != br.ReadInt32())
				{
					return false;
				}

				if (type != br.ReadInt16())
				{
					return false;
				}

				if (0x1332 != br.ReadInt16())
				{
					return false;
				}
				return true;
			}
			finally
			{
				br.BaseStream.Seek(position, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Reads all string field.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private static IEnumerable<string> ReadAllStringField(string file)
		{
			using (var fs = File.OpenRead(file))
			{
				using (var br = new BinaryReader(fs))
				{
					br.BaseStream.Seek(16, SeekOrigin.Begin);
					var number = br.ReadInt32();

					for (long i = 0; i < number; i++)
					{
						yield return getString(br);
					}
					yield break;
				}
			}
		}

		/// <summary>
		/// Reads all string field.
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns></returns>
		private static IEnumerable<string> ReadAllStringField(BinaryReader br)
		{
			br.BaseStream.Seek(16, SeekOrigin.Begin);
			var number = br.ReadInt32();

			for (long i = 0; i < number; i++)
			{
				yield return getString(br);
			}
			yield break;
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns></returns>
		private static String getString(BinaryReader br)
		{
			var sb = new StringBuilder();
			int c;
			while ((c = br.Read()) != 0)
			{
				sb.Append((char)c);
			}
			return sb.ToString();
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<String> GetAlbums()
		{
			if (!Directory.Exists(m_PicasaDBStoragePath) ||
				!File.Exists(m_AlbumPathPMPFileName) ||
				!IsValidPicasaFormat(m_AlbumPathPMPFileName))
				return new string[0];

			return ReadAllStringField(m_AlbumPathPMPFileName).Where(album => !string.IsNullOrEmpty(album));
		}
		#endregion
	}
}
